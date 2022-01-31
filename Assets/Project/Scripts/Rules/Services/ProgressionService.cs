using GameEngine.Core.Logger;
using GameEngine.Core.System;
using GameEngine.PMR.Modules;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using Swap.Setups.Modes;

namespace Swap.Rules.Services
{
    [RuleAccess(typeof(IProgressionService))]
    public class ProgressionService : GameRule, IProgressionService
    {
        private const string TAG = "ProgressionService";

        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        private QuestDescriptor m_MainQuest;
        private ChapterDescriptor m_CurrentChapter;
        private LevelDescriptor m_CurrentLevel;
        private int m_ChapterNumber;
        private int m_LevelNumber;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_MainQuest = ContentService.GetContentDescriptor<QuestDescriptor>("MainQuest");
            m_ChapterNumber = -1;
            m_LevelNumber = -1;

            if (!TryIncrementLevel())
            {
                Log.Error(TAG, "The main quest does not contain any levels");
                MarkError();
            }
                
            SetPlaymodeConfiguration(false);
            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update() { }
        #endregion

        #region IProgressionService API
        public void RestartLevel()
        {
            SetPlaymodeConfiguration(true);
            m_Process.CurrentGameMode.Reload();
        }

        public bool GoToNextLevel()
        {
            if (!TryIncrementLevel())
                return false;

            SetPlaymodeConfiguration(false);
            m_Process.CurrentGameMode.Reload();
            return true;
        }
        #endregion

        #region private
        private bool TryIncrementLevel()
        {
            if (m_ChapterNumber < 0) m_ChapterNumber = 0;
            if (m_LevelNumber < 0) m_LevelNumber = 0;
            else m_LevelNumber++;

            while (m_ChapterNumber < m_MainQuest.Chapters.Count)
            {
                m_CurrentChapter = m_MainQuest.Chapters[m_ChapterNumber];
                if (m_LevelNumber < m_CurrentChapter.Levels.Count)
                {
                    m_CurrentLevel = m_CurrentChapter.Levels[m_LevelNumber];
                    return true;
                }

                m_ChapterNumber++;
                m_LevelNumber = 0;
            }

            return false;
        }

        private void SetPlaymodeConfiguration(bool replay)
        {
            if (m_Process.CurrentGameMode == null)
            {
                Configuration playmodeConfig = new Configuration();
                playmodeConfig.Add("chapter", m_CurrentChapter);
                playmodeConfig.Add("level", m_CurrentLevel);
                playmodeConfig.Add("replay", replay);

                m_Process.SetModuleConfiguration(typeof(PlayModeSetup), playmodeConfig);
            }
            else
            {
                m_Process.CurrentGameMode.Configuration["chapter"] = m_CurrentChapter;
                m_Process.CurrentGameMode.Configuration["level"] = m_CurrentLevel;
                m_Process.CurrentGameMode.Configuration["replay"] = replay;
            }
        }
        #endregion
    }
}
