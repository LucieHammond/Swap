using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Components.States;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using UnityEngine;

namespace Swap.Rules.Mechanics
{
    public class GeneratorRule : GameRule
    {
        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILogicRule LogicRule;

        private SpawnDescriptor m_Descriptor;

        private Generator[] m_Generators;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<SpawnDescriptor>("GeneratorSpawn");

            m_Generators = LevelRule.GetGenerators();

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            foreach (Generator generator in m_Generators)
            {
                switch (generator.State)
                {
                    case (GeneratorState.Inactive):
                        bool activate = LogicRule.IsActiveAll(generator.SignalsToListen);
                        if (activate)
                            InitiateDespawn(generator);
                        break;

                    case (GeneratorState.Despawning):
                        bool despawned = PerformDespawn(generator);
                        if (despawned)
                            InitiateSpawn(generator);
                        break;

                    case (GeneratorState.Spawning):
                        bool spawned = PerformSpawn(generator);
                        if (spawned)
                            EndSpawning(generator);
                        break;
                }
            }
        }
        #endregion

        #region private
        private void InitiateDespawn(Generator generator)
        {
            generator.ObjectComponent.Deactivate();
            generator.LinkedObject.SetActive(false);

            generator.DespawnObject();
        }

        private bool PerformDespawn(Generator generator)
        {
            generator.UpdateSpawning(m_Time.DeltaTime);
            return generator.DespawnProgression > m_Descriptor.DespawnTime;
        }

        private void InitiateSpawn(Generator generator)
        {
            generator.LinkedObject.transform.position = generator.SpawnPoint.position;
            generator.LinkedObject.transform.localRotation = generator.SpawnPoint.rotation * generator.ObjectComponent.DefaultRotation();
            generator.LinkedObject.SetActive(true);

            generator.SpawnObject();
        }

        private bool PerformSpawn(Generator generator)
        {
            generator.UpdateSpawning(m_Time.DeltaTime);
            float progress = m_Descriptor.SpawnTime > 0 ? Mathf.Clamp(generator.SpawnProgression / m_Descriptor.SpawnTime, 0, 1) : 1f;

            Vector3 position = generator.SpawnPoint.position * (1f - progress) + generator.FinalPoint.position * progress;
            generator.LinkedObject.transform.position = position;
            return generator.SpawnProgression > m_Descriptor.SpawnTime;
        }

        private void EndSpawning(Generator generator)
        {
            generator.ObjectComponent.Activate();
            generator.DropObject();
        }
        #endregion
    }
}
