using GameEngine.Core.System;
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
    public class VictoryRule : GameRule, ISceneGameRule
    {
        public HashSet<string> RequiredScenes => new HashSet<string>() { "VictoryUI" };

        [ObjectDependency("VictoryInterface", ObjectDependencyElement.GameObject, true)]
        public GameObject m_VictoryPanel;

        [RuleDependency(RuleDependencySource.Service, true)]
        public IProgressionService ProgressionService;

        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ICharacterRule CharacterRule;

        private VictoryDescriptor m_Descriptor;

        private GameObject m_Credits;
        private PlayerSoul m_PlayerSoul;
        private Vector3 m_VictoryPoint;

        private bool m_Victory;
        private float m_VictoryDisplayTime;
        private bool m_IsAttracted;
        private Vector3 m_LastPosition;
        private Quaternion m_LastRotation;
        private float m_AttractProgression;


        #region GameRule cycle
        protected override void Initialize()
        {
            Configuration config = m_Process.CurrentGameMode.Configuration;
            m_VictoryPoint = config.Get<LevelDescriptor>("level").EndPoint;
            m_Credits = m_VictoryPanel.transform.Find("Credits").gameObject;

            m_Descriptor = ContentService.GetContentDescriptor<VictoryDescriptor>("Victory");

            m_VictoryPanel.SetActive(false);
            m_Credits.SetActive(false);
            m_PlayerSoul = LevelRule.GetPlayerSoul();
            m_IsAttracted = false;
            m_Victory = false;

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            if (m_Victory)
            {
                m_VictoryDisplayTime -= m_Time.DeltaTime;
                if (m_VictoryDisplayTime < 0)
                {
                    bool success = ProgressionService.GoToNextLevel();
                    if (!success) m_Credits.SetActive(true);
                }
            }
            else if (m_IsAttracted)
            {
                PerformAttraction();
                if (CheckVictoryZone())
                {
                    m_Victory = true;
                    m_VictoryPanel.SetActive(true);
                    m_VictoryDisplayTime = m_Descriptor.DisplayTime;
                }
            }
            else
            {
                if (CheckAttractionZone())
                {
                    m_IsAttracted = true;
                    m_LastPosition = m_PlayerSoul.transform.position;
                    m_LastRotation = m_PlayerSoul.transform.rotation;
                    m_AttractProgression = 0f;

                    m_PlayerSoul.transform.SetParent(null, true);
                    CharacterRule.ExitCharacter();
                }
            }
        }
        #endregion

        #region private
        private bool CheckAttractionZone()
        {
            return Vector3.Distance(m_PlayerSoul.transform.position, m_VictoryPoint) < m_Descriptor.AttractionRadius;
        }

        private bool CheckVictoryZone()
        {
            return Vector3.Distance(m_PlayerSoul.transform.position, m_VictoryPoint) < m_Descriptor.VictoryRadius;
        }

        private void PerformAttraction()
        {
            m_AttractProgression = Mathf.Clamp(m_AttractProgression + m_Time.DeltaTime / m_Descriptor.AttractionTime, 0, 1);

            Vector3 baseMove = (m_VictoryPoint - m_LastPosition) * m_AttractProgression;
            Vector3 deviationMove = 4 * (m_Descriptor.AttractionDeviation * Vector3.up) * m_AttractProgression * (1 - m_AttractProgression);
            m_PlayerSoul.transform.position = m_LastPosition + baseMove + deviationMove;

            Quaternion rotation = Quaternion.Lerp(m_LastRotation, Quaternion.identity, m_AttractProgression * 2);
            m_PlayerSoul.transform.rotation = rotation;
        }
        #endregion
    }
}
