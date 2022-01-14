using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using GameEngine.PMR.Unity.Rules;
using GameEngine.PMR.Unity.Rules.Dependencies;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Rules.Events
{
    public class DefeatRule : GameRule, ISceneGameRule
    {
        public HashSet<string> RequiredScenes => new HashSet<string>() { "DefeatUI" };

        [ObjectDependency("DefeatInterface", ObjectDependencyElement.GameObject, true)]
        public GameObject m_GameOverPanel;

        [RuleDependency(RuleDependencySource.Service, true)]
        public IProgressionService ProgressionService;

        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        private DefeatDescriptor m_Descriptor;

        private PlayerSoul m_PlayerSoul;
        public bool m_GameOver;
        private float m_DefeatDisplayTime;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<DefeatDescriptor>("Defeat");

            m_GameOverPanel.SetActive(false);
            m_PlayerSoul = LevelRule.GetPlayerSoul();
            m_GameOver = false;
            m_DefeatDisplayTime = m_Descriptor.DisplayTime;

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            if (!m_GameOver)
            {
                m_GameOver = CheckFallIntoVoid();
                if (m_GameOver)
                    m_GameOverPanel.SetActive(true);
            }
            else
            {
                m_DefeatDisplayTime -= m_Time.DeltaTime;
                if (m_DefeatDisplayTime < 0)
                    ProgressionService.RestartLevel();
            }
        }
        #endregion

        #region private
        private bool CheckFallIntoVoid()
        {
            return m_PlayerSoul.transform.position.y < m_Descriptor.MaxDepthThreshold;
        }
        #endregion
    }
}
