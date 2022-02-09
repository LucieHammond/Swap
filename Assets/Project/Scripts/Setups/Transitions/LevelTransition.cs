using GameEngine.PMR.Process.Transitions;
using GameEngine.PMR.Unity.Transitions;
using GameEngine.PMR.Unity.Transitions.Elements;
using Swap.Data.Descriptors;
using UnityEngine.UI;

namespace Swap.Setups.Transitions
{
    public class LevelTransition : InterfaceTransition
    {
        private Text m_LevelTitle;

        public override bool UpdateDuringEntry => false;
        public override bool UpdateDuringExit => true;

        public LevelTransition()
            : base("LevelTransition", "LevelScreen")
        {

        }

        protected override void Prepare()
        {
            AddTransitionObject<Image>("Background", SetupBackgroundTransition);
            AddTransitionObject<Text>("LevelTitle", SetupLevelTitleTransition);

            base.Prepare();
        }

        protected override void Enter()
        {
            bool replay = m_ModuleConfiguration.Get<bool>("replay");
            string levelName = m_ModuleConfiguration.Get<LevelDescriptor>("level").Name;

            if (!replay)
            {
                SetTransitionTimes(2.0f, 0.4f, 1.2f);
                m_LevelTitle.enabled = true;
                m_LevelTitle.text = levelName.ToUpper();
            }
            else
            {
                SetTransitionTimes(0f, 0f, 0.8f);
                m_LevelTitle.enabled = false;
                m_LevelTitle.text = "";
            }

            base.Enter();
        }

        protected override void Cleanup()
        {
            base.Cleanup();
        }

        private ITransitionElement SetupBackgroundTransition(Image background)
        {
            return new FadingElement(background, 0f, 0.8f, 0f, 0.4f);
        }

        private ITransitionElement SetupLevelTitleTransition(Text levelTitle)
        {
            m_LevelTitle = levelTitle;
            return new FadingElement(levelTitle, 0.3f, 0.3f, 0f, 0f);
        }
    }
}
