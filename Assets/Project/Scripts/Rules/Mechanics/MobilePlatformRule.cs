using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using Swap.Components;
using Swap.Components.States;
using Swap.Data.Models;
using Swap.Interfaces;
using UnityEngine;

namespace Swap.Rules.Mechanics
{
    public class MobilePlatformRule : GameRule
    {
        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILogicRule LogicRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IMotionRule MotionRule;

        private MobilePlatform[] m_MobilePlatforms;
        private RobotBody[] m_Robots;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_MobilePlatforms = LevelRule.GetMobilePlatforms();
            m_Robots = LevelRule.GetRobotBodies();

            MarkInitialized();
        }

        protected override void Unload()
        {
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
            if ((platform.State == MobilePlatformState.InPositionA && signalValue)
                || platform.State == MobilePlatformState.MovingFromAToB)
            {
                platform.SetState(MovePlatformTowardsB(platform));
            }

            else if ((platform.State == MobilePlatformState.InPositionB && signalValue)
                || platform.State == MobilePlatformState.MovingFromBToA)
            {
                platform.SetState(MovePlatformTowardsA(platform));
            }
        }

        private void UpdateWithStatusMatching(MobilePlatform platform, bool signalValue)
        {
            if (platform.State != MobilePlatformState.InPositionB && signalValue)
            {
                platform.SetState(MovePlatformTowardsB(platform));
            }

            else if (platform.State != MobilePlatformState.InPositionA && !signalValue)
            {
                platform.SetState(MovePlatformTowardsA(platform));
            }
        }

        private void UpdateWithMovementBlocking(MobilePlatform platform, bool signalValue)
        {
            if (!signalValue)
                return;

            if (platform.State == MobilePlatformState.InPositionA || platform.State == MobilePlatformState.MovingFromAToB)
            {
                platform.SetState(MovePlatformTowardsB(platform));
            }

            else if (platform.State == MobilePlatformState.InPositionB || platform.State == MobilePlatformState.MovingFromBToA)
            {
                platform.SetState(MovePlatformTowardsA(platform));
            }
        }

        private void UpdateWithPermanentCompletion(MobilePlatform platform, bool signalValue)
        {
            if (platform.State == MobilePlatformState.InPositionB)
                return;

            if ((platform.State == MobilePlatformState.InPositionA && signalValue)
                || platform.State == MobilePlatformState.MovingFromAToB)
            {
                platform.SetState(MovePlatformTowardsB(platform));
            }
        }

        private MobilePlatformState MovePlatformTowardsA(MobilePlatform platform)
        {
            Vector3 positionDiff = -platform.TranslationalMotion * m_Time.DeltaTime / platform.MovementDuration;
            Vector3 rotationDiff = -platform.RotationalMotion * m_Time.DeltaTime / platform.MovementDuration;
            ApplyMoveToRobots(platform, positionDiff, rotationDiff);

            if (positionDiff.magnitude < Vector3.Distance(platform.transform.position, platform.PositionA)
                || rotationDiff.magnitude < Quaternion.Angle(platform.transform.rotation, Quaternion.Euler(platform.RotationA)))
            {
                platform.transform.position += positionDiff;
                platform.transform.eulerAngles += rotationDiff;
                return MobilePlatformState.MovingFromBToA;
            }
            else
            {
                platform.transform.position = platform.PositionA;
                platform.transform.eulerAngles = platform.RotationA;
                return MobilePlatformState.InPositionA;
            }
        }

        private MobilePlatformState MovePlatformTowardsB(MobilePlatform platform)
        {
            Vector3 positionDiff = platform.TranslationalMotion * m_Time.DeltaTime / platform.MovementDuration;
            Vector3 rotationDiff = platform.RotationalMotion * m_Time.DeltaTime / platform.MovementDuration;
            ApplyMoveToRobots(platform, positionDiff, rotationDiff);

            if (positionDiff.magnitude < Vector3.Distance(platform.transform.position, platform.PositionB)
                || rotationDiff.magnitude < Quaternion.Angle(platform.transform.rotation, Quaternion.Euler(platform.RotationB)))
            {
                platform.transform.position += positionDiff;
                platform.transform.eulerAngles += rotationDiff;
                return MobilePlatformState.MovingFromAToB;
            }
            else
            {
                platform.transform.position = platform.PositionB;
                platform.transform.eulerAngles = platform.RotationB;
                return MobilePlatformState.InPositionB;
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
