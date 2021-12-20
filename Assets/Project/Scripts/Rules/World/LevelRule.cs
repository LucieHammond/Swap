using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Rules;
using GameEngine.PMR.Unity.Rules.Dependencies;
using Swap.Components;
using Swap.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Swap.Rules.World
{
    [RuleAccess(typeof(ILevelRule))]
    public class LevelRule : GameRule, ISceneGameRule, ILevelRule
    {
        private const string PLAYER_TAG = "Player";
        private const string NON_PLAYER_TAG = "NonPlayerSoul";
        private const string ROBOT_BODY_TAG = "RobotBody";

        public HashSet<string> RequiredScenes => new HashSet<string>() { "Level_1.1" };

        [ObjectDependency("3rdPersonCamera", ObjectDependencyElement.GameObject, true)]
        public GameObject m_Camera;

        private PlayerSoul m_PlayerSoul;
        private NonPlayerSoul[] m_NonPlayerSouls;
        private RobotBody[] m_Robots;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_PlayerSoul = GameObject.FindGameObjectWithTag(PLAYER_TAG)
                .GetComponent<PlayerSoul>();

            m_NonPlayerSouls = GameObject.FindGameObjectsWithTag(NON_PLAYER_TAG)
                .Select((gameObject) => gameObject.GetComponent<NonPlayerSoul>()).ToArray();
            
            m_Robots = GameObject.FindGameObjectsWithTag(ROBOT_BODY_TAG)
                .Select((gameObject) => gameObject.GetComponent<RobotBody>()).ToArray();

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update() { }
        #endregion

        #region ILevel API
        public GameObject GetCamera() => m_Camera;

        public PlayerSoul GetPlayerSoul() => m_PlayerSoul;

        public NonPlayerSoul[] GetNonPlayerSouls() => m_NonPlayerSouls;

        public RobotBody[] GetRobotBodies() => m_Robots;
        #endregion
    }
}
