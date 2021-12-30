using GameEngine.Core.System;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Rules;
using GameEngine.PMR.Unity.Rules.Dependencies;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Data.Models;
using Swap.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Swap.Rules.World
{
    [RuleAccess(typeof(ILevelRule))]
    public class LevelRule : GameRule, ISceneGameRule, ILevelRule
    {
        private const string LIGHTS_ROOT = "Lights";
        private const string ENVIRONMENT_ROOT = "Environment";
        private const string OBJECTS_ROOT = "Objects";

        private const string PLAYER_TAG = "Player";
        private const string NON_PLAYER_TAG = "NonPlayerSoul";
        private const string ROBOT_BODY_TAG = "RobotBody";
        private const string BUTTON_TAG = "Button";
        private const string GEM_STONE_TAG = "GemStone";
        private const string GEM_RECEPTACLE_TAG = "GemReceptacle";
        private const string DOOR_TAG = "Door";

        public HashSet<string> RequiredScenes
        {
            get
            {
                Configuration config = m_Process.CurrentGameMode.Configuration;
                return new HashSet<string> { config.Get<LevelDescriptor>("level").Scene };
            }
        }

        [ObjectDependency(LIGHTS_ROOT, ObjectDependencyElement.Transform, false)]
        public Transform m_LightsRoot;

        [ObjectDependency(ENVIRONMENT_ROOT, ObjectDependencyElement.Transform, false)]
        public Transform m_EnvironmentRoot;

        [ObjectDependency(OBJECTS_ROOT, ObjectDependencyElement.Transform, false)]
        public Transform m_ObjectsRoot;

        [ObjectDependency("3rdPersonCamera", ObjectDependencyElement.GameObject, true)]
        public GameObject m_Camera;

        private PlayerSoul m_PlayerSoul;
        private NonPlayerSoul[] m_NonPlayerSouls;
        private RobotBody[] m_Robots;
        private Button[] m_Buttons;
        private GemStone[] m_GemStones;
        private GemReceptacle[] m_GemReceptacles;
        private Door[] m_Doors;

        private LevelState m_CurrentState;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_PlayerSoul = GameObject.FindGameObjectWithTag(PLAYER_TAG)
                .GetComponent<PlayerSoul>();

            m_NonPlayerSouls = GameObject.FindGameObjectsWithTag(NON_PLAYER_TAG)
                .Select((gameObject) => gameObject.GetComponent<NonPlayerSoul>()).ToArray();
            
            m_Robots = GameObject.FindGameObjectsWithTag(ROBOT_BODY_TAG)
                .Select((gameObject) => gameObject.GetComponent<RobotBody>()).ToArray();

            m_Buttons = GameObject.FindGameObjectsWithTag(BUTTON_TAG)
                .Select((gameObject) => gameObject.GetComponent<Button>()).ToArray();

            m_GemStones = GameObject.FindGameObjectsWithTag(GEM_STONE_TAG)
                .Select((gameObject) => gameObject.GetComponent<GemStone>()).ToArray();

            m_GemReceptacles = GameObject.FindGameObjectsWithTag(GEM_RECEPTACLE_TAG)
                .Select((gameObject) => gameObject.GetComponent<GemReceptacle>()).ToArray();

            m_Doors = GameObject.FindGameObjectsWithTag(DOOR_TAG)
                .Select((gameObject) => gameObject.GetComponent<Door>()).ToArray();


            m_CurrentState = new LevelState()
            {
                CurrentRobotBody = m_PlayerSoul.GetComponentInParent<RobotBody>()
            };

            MarkInitialized();
        }

        protected override void Unload()
        {
            m_CurrentState = null;
            MarkUnloaded();
        }

        protected override void Update() { }
        #endregion

        #region ILevel API
        public Transform GetRootTransform(string rootName)
        {
            if (rootName == LIGHTS_ROOT) return m_LightsRoot;
            if (rootName == ENVIRONMENT_ROOT) return m_EnvironmentRoot;
            if (rootName == OBJECTS_ROOT) return m_ObjectsRoot;

            return null;
        }

        public LevelState GetLevelState() => m_CurrentState;

        public GameObject GetCamera() => m_Camera;

        public PlayerSoul GetPlayerSoul() => m_PlayerSoul;

        public NonPlayerSoul[] GetNonPlayerSouls() => m_NonPlayerSouls;

        public RobotBody[] GetRobotBodies() => m_Robots;

        public Button[] GetButtons() => m_Buttons;

        public GemStone[] GetGemStones() => m_GemStones;

        public GemReceptacle[] GetGemReceptacles() => m_GemReceptacles;

        public Door[] GetDoors() => m_Doors;
        #endregion
    }
}
