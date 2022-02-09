using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Components.States;
using Swap.Data.Descriptors;
using Swap.Interfaces;

namespace Swap.Rules.Mechanics
{
    public class ButtonRule : GameRule
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

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<PressDescriptor>("ButtonPress");

            m_Buttons = LevelRule.GetButtons();

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            foreach (Button button in m_Buttons)
            {
                UpdateButton(button);
                if (button.IsActivated)
                    LogicRule.TriggerInstantSignal(button.SignalToSend);
            }

            bool pressPossible = InteractionRule.FindEligibleInteraction(m_Buttons, out Button interactingButton);

            if (ControllerRule.AskedInteraction() && pressPossible)
            {
                interactingButton.Press();
            }
        }
        #endregion

        #region private
        private void UpdateButton(Button button)
        {
            switch (button.State)
            {
                case ButtonState.Pressed:
                    button.UpdateSchedule(m_Time.DeltaTime);
                    if (button.ActivationProgression > m_Descriptor.ActivationDelay)
                        button.AchieveActivation();
                    break;
                case ButtonState.CoolingDown:
                    button.UpdateSchedule(m_Time.DeltaTime);
                    if (button.CoolDownProgression > m_Descriptor.CoolDownTime)
                        button.FinishCoolDown();
                    break;
            }
        }
        #endregion
    }
}
