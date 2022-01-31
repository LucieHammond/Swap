using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using System.Collections.Generic;
using System.Linq;

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

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IInteractionRule InteractionRule;

        private PressDescriptor m_Descriptor;

        private Button[] m_Buttons;

        private Dictionary<Button, float> m_CurrentCoolDowns;
        private Dictionary<Button, float> m_Activations;

        public ButtonPressRule()
        {
            m_CurrentCoolDowns = new Dictionary<Button, float>();
            m_Activations = new Dictionary<Button, float>();
        }

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<PressDescriptor>("ButtonPress");

            m_Buttons = LevelRule.GetButtons();

            foreach (Button button in m_Buttons)
            {
                m_CurrentCoolDowns.Add(button, -1.0f);
                m_Activations.Add(button, -1.0f);
            }

            MarkInitialized();
        }

        protected override void Unload()
        {
            m_CurrentCoolDowns.Clear();
            m_Activations.Clear();

            MarkUnloaded();
        }

        protected override void Update()
        {
            UpdateCoolDownSchedules();
            UpdateActivationSchedules();

            bool pressPossible = InteractionRule.FindEligibleInteraction(
                m_Buttons.Where((button) => IsReady(button)), (button) => button.Interactivity, out Button interactingButton);

            if (ControllerRule.AskedInteraction() && pressPossible)
            {
                TriggerButton(interactingButton);
            }
        }
        #endregion

        #region private
        private bool IsReady(Button button)
        {
            return m_CurrentCoolDowns[button] < 0;
        }

        private void TriggerButton(Button button)
        {
            button.Animator.SetTrigger(m_Descriptor.AnimationParameter);
            m_CurrentCoolDowns[button] = m_Descriptor.CoolDownTime;
            m_Activations[button] = m_Descriptor.ActivationDelay;
        }

        private void UpdateCoolDownSchedules()
        {
            foreach (Button button in m_Buttons)
            {
                if (m_CurrentCoolDowns[button] >= 0)
                {
                    m_CurrentCoolDowns[button] -= m_Time.DeltaTime;
                }
            }
        }

        private void UpdateActivationSchedules()
        {
            foreach (Button button in m_Buttons)
            {
                if (m_Activations[button] >= 0)
                {
                    m_Activations[button] -= m_Time.DeltaTime;
                    
                    if (m_Activations[button] < 0)
                    {
                        LogicRule.TriggerInstantSignal(button.SignalToSend);
                    }
                }
            }
        }
        #endregion
    }
}
