using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Rules;
using GameEngine.PMR.Unity.Rules.Dependencies;
using Swap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Rules.World
{
    [RuleAccess(typeof(ILevelRule))]
    public class LevelRule : GameRule, ISceneGameRule, ILevelRule
    {
        public HashSet<string> RequiredScenes => new HashSet<string>() { "Level_1.1" };

        [ObjectDependency("InitialRobot", ObjectDependencyElement.GameObject, false)]
        private GameObject m_InitialCharacter;

        [ObjectDependency("3rdPersonCamera", ObjectDependencyElement.GameObject, false)]
        private GameObject m_Camera;

        #region GameRule cycle
        protected override void Initialize()
        {
            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update() { }
        #endregion

        #region ILevel API
        public GameObject GetInitialCharacter()
        {
            return m_InitialCharacter;
        }

        public GameObject GetCamera()
        {
            return m_Camera;
        }
        #endregion
    }
}
