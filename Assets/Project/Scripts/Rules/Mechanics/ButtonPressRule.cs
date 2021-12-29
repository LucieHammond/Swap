using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Data.Models;
using Swap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Rules.Mechanics
{
    public class ButtonPressRule : GameRule
    {
        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILogicRule LogicRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IControllerRule ControllerRule;

        private PressDescriptor m_Descriptor;

        private LevelState m_LevelState;
        private Button[] m_Buttons;

        private Dictionary<int, float> m_Triggers;

        public ButtonPressRule()
        {
            m_Triggers = new Dictionary<int, float>();
        }

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<PressDescriptor>("ButtonPress");

            m_LevelState = LevelRule.GetLevelState();
            m_Buttons = LevelRule.GetButtons();

            for (int i = 0; i < m_Buttons.Length; i++)
            {
                m_Triggers.Add(i, -1.0f);
            }

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            UpdateActivationSchedules();

            if (ControllerRule.AskedInteraction() && m_LevelState.CurrentRobotBody != null)
            {
                if (GetEligibleButton(m_LevelState.CurrentRobotBody.InteractRoot, out int buttonIndex) && IsReady(buttonIndex))
                {
                    TriggerButton(buttonIndex);

                    ControllerRule.ResetInteraction();
                }
            }
        }
        #endregion

        #region private
        private bool GetEligibleButton(Transform interactRoot, out int buttonIndex)
        {
            buttonIndex = -1;
            float minDistance = float.MaxValue;

            for (int i = 0; i < m_Buttons.Length; i++)
            {
                Vector3 triggerCenter = m_Buttons[i].transform.position + m_Buttons[i].transform.rotation * m_Descriptor.TriggerOffset;
                float distance = Vector3.Distance(interactRoot.position, triggerCenter);

                if (distance > m_Descriptor.TriggerRadius)
                    continue;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    buttonIndex = i;
                }
            }

            return buttonIndex >= 0;
        }

        private bool IsReady(int buttonIndex)
        {
            return m_Triggers[buttonIndex] < 0;
        }

        private void TriggerButton(int buttonIndex)
        {
            m_Buttons[buttonIndex].Animator.SetTrigger("Press");
            m_Triggers[buttonIndex] = m_Descriptor.ActivationDelay;
        }

        private void UpdateActivationSchedules()
        {
            for (int i = 0; i < m_Buttons.Length; i++)
            {
                if (m_Triggers[i] >= 0)
                {
                    m_Triggers[i] -= m_Time.DeltaTime;
                    
                    if (m_Triggers[i] < 0)
                    {
                        LogicRule.TriggerInstantSignal(m_Buttons[i].SignalToSend);
                    }
                }
            }
        }
        #endregion
    }
}
