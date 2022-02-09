using Swap.Components.States;
using Swap.Components.Template;
using Swap.Data.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Components
{
    public class Generator : MonoBehaviour
    {
        [Header("Configuration")]
        public List<Signal> SignalsToListen;

        public GameObject LinkedObject;

        [Header("Components")]
        public Transform SpawnPoint;

        public Transform FinalPoint;

        #region Component properties
        public GeneratorState State { get; private set; }
        public IGenerative ObjectComponent { get; private set; }
        public float DespawnProgression { get; private set; }
        public float SpawnProgression { get; private set; }
        #endregion

        #region Component methods
        public Generator Setup()
        {
            State = GeneratorState.Inactive;
            ObjectComponent = LinkedObject.GetComponent<IGenerative>();
            DespawnProgression = 0f;
            SpawnProgression = 0f;
            return this;
        }

        public void DespawnObject()
        {
            State = GeneratorState.Despawning;
            DespawnProgression = 0f;
        }

        public void SpawnObject()
        {
            State = GeneratorState.Spawning;
            SpawnProgression = 0f;
        }

        public void DropObject()
        {
            State = GeneratorState.Inactive;
            DespawnProgression = 0f;
            SpawnProgression = 0f;
        }

        public void UpdateSpawning(float deltaTime)
        {
            switch (State)
            {
                case GeneratorState.Despawning:
                    DespawnProgression += deltaTime;
                    break;
                case GeneratorState.Spawning:
                    SpawnProgression += deltaTime;
                    break;
            }
        }
        #endregion
    }
}
