using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Data.Models;
using Swap.Interfaces;
using UnityEngine;

namespace Swap.Rules.Operations
{
    public class RobotsRepulsionRule : GameRule
    {
        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IMotionRule MotionRule;

        private MotionDescriptor m_Descriptor;

        private RobotBody[] m_Robots;
        private LevelState m_LevelState;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<MotionDescriptor>("Motion");

            m_Robots = LevelRule.GetRobotBodies();
            m_LevelState = LevelRule.GetLevelState();

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            for (int i = 0; i < m_Robots.Length; i++)
            {
                for (int j = i + 1; j < m_Robots.Length; j++)
                {
                    if (CheckRepulsion(m_Robots[i], m_Robots[j], out RobotBody repulsedRobot, out Vector3 direction, out float distance))
                    {
                        ApplyRepulsion(repulsedRobot, direction, distance);
                    }
                }
            }
        }
        #endregion

        #region private
        private bool CheckRepulsion(RobotBody robot1, RobotBody robot2, out RobotBody repulsedRobot, out Vector3 direction, out float distance)
        {
            CharacterController controller1 = robot1.Controller;
            Bounds bounds1 = new Bounds(controller1.bounds.center, controller1.bounds.size + m_Descriptor.RepulsionMargin * Vector3.one);
            
            CharacterController controller2 = robot2.Controller;
            Bounds bounds2 = new Bounds(controller2.bounds.center, controller2.bounds.size + m_Descriptor.RepulsionMargin * Vector3.one);

            if (bounds1.Intersects(bounds2))
            {
                Vector3 relativePosition = robot1.transform.position - robot2.transform.position;
                Vector3 horizontalDistance = new Vector3(relativePosition.x, 0f, relativePosition.z);
                float verticalDistance = relativePosition.y;

                distance = controller1.radius + controller2.radius + m_Descriptor.RepulsionMargin - horizontalDistance.magnitude;
                if (distance > 0)
                {
                    if (verticalDistance >= m_Descriptor.RepulsionMinHeight || robot1 == m_LevelState.CurrentRobotBody)
                    {
                        repulsedRobot = robot1;
                        direction = horizontalDistance.magnitude > 0 ? horizontalDistance.normalized : Vector3.forward;
                        return true;
                    }
                    else if (verticalDistance <= -m_Descriptor.RepulsionMinHeight || robot2 == m_LevelState.CurrentRobotBody)
                    {
                        repulsedRobot = robot2;
                        direction = horizontalDistance.magnitude > 0 ? -horizontalDistance.normalized : Vector3.forward;
                        return true;
                    }
                }
            }

            repulsedRobot = null;
            direction = Vector3.zero;
            distance = 0f;
            return false;
        }

        private void ApplyRepulsion(RobotBody robot, Vector3 repulsionDirection, float repulsionDistance)
        {
            float repulsionSpeed = m_Descriptor.RepulsionSpeed * (1 + m_Descriptor.RepulsionLinearFactor * repulsionDistance);
            Vector3 repulsionMove = Mathf.Min(repulsionSpeed * m_Time.DeltaTime, repulsionDistance) * repulsionDirection;

            MotionRule.ApplyTranslationalMovement(robot, repulsionMove);
        }
        #endregion
    }
}
