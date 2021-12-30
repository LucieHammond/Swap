using GameEngine.Core.System;
using GameEngine.Core.Unity.Rendering;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using GameEngine.PMR.Unity.Rules;
using GameEngine.PMR.Unity.Rules.Dependencies;
using Swap.Data.Descriptors;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Swap.Rules.World
{
    public class StartRule : GameRule, ISceneGameRule
    {
        public HashSet<string> RequiredScenes => new HashSet<string>() { "StartUI" };

        [ObjectDependency("StartInterface", ObjectDependencyElement.GameObject, true)]
        public GameObject m_StartPanel;

        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        private StartDescriptor m_Descriptor;

        private Image m_Background;
        private Text m_TitleText;
        private FadeRenderer m_BackgroundFade;
        
        private bool m_IsStarting;
        private float m_TitleDisplayTime;
        private float m_PannelDisplayTime;

        #region GameRule cycle
        protected override void Initialize()
        {
            Configuration config = m_Process.CurrentGameMode.Configuration;
            bool replay = config.Get<bool>("replay");
            string levelName = config.Get<LevelDescriptor>("level").Name;

            m_Descriptor = ContentService.GetContentDescriptor<StartDescriptor>("Start");

            m_Background = m_StartPanel.GetComponentInChildren<Image>();
            m_TitleText = m_StartPanel.GetComponentInChildren<Text>();
            m_BackgroundFade = new FadeRenderer(m_Background, m_Descriptor.PanelFadeTime, true);
            
            m_IsStarting = true;
            m_TitleDisplayTime = replay ? 0f : m_Descriptor.TitleDisplayTime;
            m_PannelDisplayTime = m_Descriptor.PanelDisplayTime;

            if (!replay)
                m_TitleText.text = levelName.ToUpper();

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            if (m_IsStarting)
            {
                ManageTitleDisplay();
                ManagePanelDisplay();
                PerformFade();
            }
        }
        #endregion

        #region private
        private void ManageTitleDisplay()
        {
            if (m_TitleDisplayTime >= 0)
            {
                m_TitleDisplayTime -= m_Time.DeltaTime;
                if (m_TitleDisplayTime < 0)
                    m_TitleText.gameObject.SetActive(false);
            }
        }

        private void ManagePanelDisplay()
        {
            if (m_TitleDisplayTime < 0 && m_PannelDisplayTime >= 0)
            {
                m_PannelDisplayTime -= m_Time.DeltaTime;
                if (m_PannelDisplayTime < 0)
                {
                    m_BackgroundFade.StartFadeOut();
                }
            }
        }

        private void PerformFade()
        {
            if (m_TitleDisplayTime < 0 && m_PannelDisplayTime < 0)
            {
                m_BackgroundFade.Update();
                if (m_BackgroundFade.IsFullyOut)
                {
                    m_IsStarting = false;
                    m_StartPanel.SetActive(false);
                }
            }
        }
        #endregion
    }
}