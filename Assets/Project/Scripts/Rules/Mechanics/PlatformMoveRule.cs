using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using Swap.Components;
using Swap.Data.Models;
using Swap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Rules.Mechanics
{
    public class PlatformMoveRule : GameRule
    {
        private class PlatformInfo
        {
            public Vector3 PositionA;
            public Vector3 RotationA;

            public Vector3 PositionB;
            public Vector3 RotationB;
        }

        private enum PlatformState
        {
            InPositionA,
            InPositionB,
            MovingFromAToB,
            MovingFromBToA
        }

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILogicRule LogicRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IMotionRule MotionRule;

        private MobilePlatform[] m_MobilePlatforms;
        private RobotBody[] m_Robots;

        private Dictionary<MobilePlatform, PlatformInfo> m_InitialInfos;
        private Dictionary<MobilePlatform, PlatformState> m_CurrentStates;

        public PlatformMoveRule()
        {
            m_InitialInfos = new Dictionary<MobilePlatform, PlatformInfo>();
            m_CurrentStates = new Dictionary<MobilePlatform, PlatformState>();
        }

        #region GameRule cycle
        protected override void Initialize()
        {
            m_MobilePlatforms = LevelRule.GetMobilePlatforms();
            m_Robots = LevelRule.GetRobotBodies();

            foreach (MobilePlatform platform in m_MobilePlatforms)
            {
                PlatformInfo initialInfo = new PlatformInfo()
                {
                    PositionA = platform.transform.position,
                    RotationA = platform.transform.eulerAngles,
                    PositionB = platform.transform.position + platform.TranslationalMotion,
                    RotationB = platform.transform.eulerAngles + platform.RotationalMotion
                };

                m_InitialInfos.Add(platform, initialInfo);
                m_CurrentStates.Add(platform, PlatformState.InPositionA);
            }

            MarkInitialized();
        }

        protected override void Unload()
        {
            m_InitialInfos.Clear();
            m_CurrentStates.Clear();

            MarkUnloaded();
        }

        protected override void Update()
        {
            foreach (MobilePlatform platform in m_MobilePlatforms)
            {
                bool signalValue = LogicRule.IsActiveAll(platform.SignalsToListen);

                switch (platform.ActivationType)
                {
                    case (ActivationType.TriggerSwitch):
                        UpdateWithInstantSwitch(platform, signalValue);
                        break;
                    case (ActivationType.MatchPosition):
                        UpdateWithStatusMatching(platform, signalValue);
                        break;
                    case (ActivationType.ControlMotion):
                        UpdateWithMovementBlocking(platform, signalValue);
                        break;
                    case (ActivationType.LaunchOneWayChange):
                        UpdateWithPermanentCompletion(platform, signalValue);
                        break;
                }
            }
        }
        #endregion

        #region private
        private void UpdateWithInstantSwitch(MobilePlatform platform, bool signalValue)
        {
            if ((m_CurrentStates[platform] == PlatformState.InPositionA && signalValue) 
                || m_CurrentStates[platform] == PlatformState.MovingFromAToB)
            {
                m_CurrentStates[platform] = MovePlatformTowardsB(platform, m_InitialInfos[platform]);
            }

            else if ((m_CurrentStates[platform] == PlatformState.InPositionB && signalValue)
                || m_CurrentStates[platform] == PlatformState.MovingFromBToA)
            {
                m_CurrentStates[platform] = MovePlatformTowardsA(platform, m_InitialInfos[platform]);
            }
        }

        private void UpdateWithStatusMatching(MobilePlatform platform, bool signalValue)
        {
            if (m_CurrentStates[platform] != PlatformState.InPositionB && signalValue)
            {
                m_CurrentStates[platform] = MovePlatformTowardsB(platform, m_InitialInfos[platform]);
            }

            else if (m_CurrentStates[platform] != PlatformState.InPositionA && !signalValue)
            {
                m_CurrentStates[platform] = MovePlatformTowardsA(platform, m_InitialInfos[platform]);
            }
        }

        private void UpdateWithMovementBlocking(MobilePlatform platform, bool signalValue)
        {
            if (!signalValue)
                return;

            if (m_CurrentStates[platform] == PlatformState.InPositionA || m_CurrentStates[platform] == PlatformState.MovingFromAToB)
            {
                m_CurrentStates[platform] = MovePlatformTowardsB(platform, m_InitialInfos[platform]);
            }

            else if (m_CurrentStates[platform] == PlatformState.InPositionB || m_CurrentStates[platform] == PlatformState.MovingFromBToA)
            {
                m_CurrentStates[platform] = MovePlatformTowardsA(platform, m_InitialInfos[platform]);
            }
        }

        private void UpdateWithPermanentCompletion(MobilePlatform platform, bool signalValue)
        {
            if (m_CurrentStates[platform] == PlatformState.InPositionB)
                return;

            if ((m_CurrentStates[platform] == PlatformState.InPositionA && signalValue)
                || m_CurrentStates[platform] == PlatformState.MovingFromAToB)
            {
                m_CurrentStates[platform] = MovePlatformTowardsB(platform, m_InitialInfos[platform]);
            }
        }

        private PlatformState MovePlatformTowardsA(MobilePlatform platform, PlatformInfo info)
        {
            Vector3 positionDiff = -platform.TranslationalMotion * m_Time.DeltaTime / platform.MovementDuration;
            Vector3 rotationDiff = -platform.RotationalMotion * m_Time.DeltaTime / platform.MovementDuration;
            ApplyMoveToRobots(platform, positionDiff, rotationDiff);

            if (positionDiff.magnitude < Vector3.Distance(platform.transform.position, info.PositionA)
                || rotationDiff.magnitude < Quaternion.Angle(platform.transform.rotation, Quaternion.Euler(info.RotationA)))
            {
                platform.transform.position += positionDiff;
                platform.transform.eulerAngles += rotationDiff;
                return PlatformState.MovingFromBToA;
            }
            else
            {
                platform.transform.position = info.PositionA;
                platform.transform.eulerAngles = info.RotationA;
                return PlatformState.InPositionA;
            }
        }

        private PlatformState MovePlatformTowardsB(MobilePlatform platform, PlatformInfo info)
        {
            Vector3 positionDiff = platform.TranslationalMotion * m_Time.DeltaTime / platform.MovementDuration;
            Vector3 rotationDiff = platform.RotationalMotion * m_Time.DeltaTime / platform.MovementDuration;
            ApplyMoveToRobots(platform, positionDiff, rotationDiff);

            if (positionDiff.magnitude < Vector3.Distance(platform.transform.position, info.PositionB)
                || rotationDiff.magnitude < Quaternion.Angle(platform.transform.rotation, Quaternion.Euler(info.RotationB)))
            {
                platform.transform.position += positionDiff;
                platform.transform.eulerAngles += rotationDiff;
                return PlatformState.MovingFromAToB;
            }
            else
            {
                platform.transform.position = info.PositionB;
                platform.transform.eulerAngles = info.RotationB;
                return PlatformState.InPositionB;
            }
        }

        private void ApplyMoveToRobots(MobilePlatform platform, Vector3 positionMove, Vector3 rotationMove)
        {
            foreach (RobotBody robot in m_Robots)
            {
                if (MotionRule.IsCurrentlyGrounded(robot, out Collider groundCollider, out _) && groundCollider == platform.Collider)
                {
                    Vector3 relativePosition = robot.transform.position - platform.transform.position;
                    Vector3 translation = positionMove - relativePosition + Quaternion.Euler(rotationMove) * relativePosition;
                    MotionRule.ApplyTranslationalMovement(robot, translation);

                    float rotation = rotationMove.y;
                    MotionRule.ApplyRotationalMovement(robot, rotation);
                }
            }
        }
        #endregion
    }
}
