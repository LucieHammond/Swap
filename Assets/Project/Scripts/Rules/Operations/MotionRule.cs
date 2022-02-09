using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Rules.Operations
{
    [RuleAccess(typeof(IMotionRule))]
    public class MotionRule : GameRule, IMotionRule
    {
        private class AppliedMotion
        {
            public Vector3 TMovement = Vector3.zero;
            public Vector3 TVelocity = Vector3.zero;
            public Vector3 TAcceleration = Vector3.zero;

            public float RMovement = 0f;
            public float RVelocity = 0f;
            public float RAcceleration = 0f;
        }

        private class CurrentMotion
        {
            public Vector3 Velocity = Vector3.zero;
            public bool IsGrounded = true;
            public Collider Ground = null;
            public float Slope = 0f;
        }

        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        private MotionDescriptor m_Descriptor;

        private RobotBody[] m_Robots;
        private Dictionary<RobotBody, AppliedMotion> m_AppliedMotions;
        private Dictionary<RobotBody, CurrentMotion> m_CurrentMotions;

        public MotionRule()
        {
            m_AppliedMotions = new Dictionary<RobotBody, AppliedMotion>();
            m_CurrentMotions = new Dictionary<RobotBody, CurrentMotion>();
        }

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<MotionDescriptor>("Motion");

            m_Robots = LevelRule.GetRobotBodies();
            foreach (RobotBody robot in m_Robots)
            {
                m_AppliedMotions.Add(robot, new AppliedMotion());
                m_CurrentMotions.Add(robot, new CurrentMotion());

                ResetTranslationalMotion(robot);
                ResetRotationalMotion(robot);
            }

            MarkInitialized();
        }

        protected override void Unload()
        {
            m_AppliedMotions.Clear();
            m_CurrentMotions.Clear();

            MarkUnloaded();
        }

        protected override void Update()
        {
            foreach (RobotBody robot in m_Robots)
            {
                if (robot.Active)
                {
                    CurrentMotion currentState = m_CurrentMotions[robot];

                    currentState.IsGrounded = CheckIfGrounded(robot, out currentState.Ground, out currentState.Slope);
                    AdjustSpeed(robot, currentState.IsGrounded);
                    ApplyMotion(robot, out currentState.Velocity);
                }
            }
        }
        #endregion

        #region IMotionRule API
        public void ApplyTranslationalMovement(RobotBody robot, Vector3 movement, bool additive = true)
        {
            if (additive)
                m_AppliedMotions[robot].TMovement += movement;
            else
                m_AppliedMotions[robot].TMovement = movement;
        }

        public void ApplyTranslationalVelocity(RobotBody robot, Vector3 velocity, bool additive = true)
        {
            if (additive)
                m_AppliedMotions[robot].TVelocity += velocity;
            else
                m_AppliedMotions[robot].TVelocity = velocity;
        }

        public void ApplyTranslationalAcceleration(RobotBody robot, Vector3 acceleration, bool additive = true)
        {
            if (additive)
                m_AppliedMotions[robot].TAcceleration += acceleration;
            else
                m_AppliedMotions[robot].TAcceleration = acceleration;
        }

        public void ResetTranslationalMotion(RobotBody robot)
        {
            m_AppliedMotions[robot].TAcceleration = m_Descriptor.CharacterGravity * Vector3.up;
            m_AppliedMotions[robot].TVelocity = Vector3.zero;
            m_AppliedMotions[robot].TMovement = Vector3.zero;
        }

        public void ApplyRotationalMovement(RobotBody robot, float movement, bool additive = true)
        {
            if (additive)
                m_AppliedMotions[robot].RMovement += movement;
            else
                m_AppliedMotions[robot].RMovement = movement;
        }

        public void ApplyRotationalVelocity(RobotBody robot, float velocity, bool additive = true)
        {
            if (additive)
                m_AppliedMotions[robot].RVelocity += velocity;
            else
                m_AppliedMotions[robot].RVelocity = velocity;
        }

        public void ApplyRotationalAcceleration(RobotBody robot, float acceleration, bool additive = true)
        {
            if (additive)
                m_AppliedMotions[robot].RAcceleration += acceleration;
            else
                m_AppliedMotions[robot].RAcceleration = acceleration;
        }

        public void ResetRotationalMotion(RobotBody robot)
        {
            m_AppliedMotions[robot].RAcceleration = 0f;
            m_AppliedMotions[robot].RVelocity = 0f;
            m_AppliedMotions[robot].RMovement = 0f;
        }

        public Vector3 GetCurrentVelocity(RobotBody robot)
        {
            return m_CurrentMotions[robot].Velocity;
        }

        public bool IsCurrentlyGrounded(RobotBody robot, out Collider groundCollider, out float slopeAngle)
        {
            groundCollider = m_CurrentMotions[robot].Ground;
            slopeAngle = m_CurrentMotions[robot].Slope;
            return m_CurrentMotions[robot].IsGrounded;
        }
        #endregion

        #region private
        private bool CheckIfGrounded(RobotBody robot, out Collider ground, out float slope)
        {
            ground = null;
            slope = 0f;

            Vector3 checkPosition = robot.transform.position + m_Descriptor.GroundCheckOffset;
            if (!Physics.CheckSphere(checkPosition, m_Descriptor.GroundCheckRadius, m_Descriptor.GroundLayers))
                return false;

            if (Physics.Raycast(checkPosition, Vector3.down, out RaycastHit hit, m_Descriptor.GroundCheckRadius, m_Descriptor.GroundLayers))
            {
                ground = hit.collider;

                float angle = 90f - Mathf.Acos(new Vector2(hit.normal.x, hit.normal.z).magnitude) * Mathf.Rad2Deg;
                slope = angle >= m_Descriptor.SlopeMinAngle && angle <= m_Descriptor.SlopeMaxAngle ? angle : 0f;
            }

            return true;
        }

        private void AdjustSpeed(RobotBody robot, bool grounded)
        {
            ref Vector3 velocity = ref m_AppliedMotions[robot].TVelocity;

            if (grounded)
                velocity.y = Mathf.Max(velocity.y, m_Descriptor.FallSpeedOnGround);
            else
                velocity.y = Mathf.Max(velocity.y, m_Descriptor.FallSpeedInAir);
        }

        private void ApplyMotion(RobotBody robot, out Vector3 velocity)
        {
            AppliedMotion motion = m_AppliedMotions[robot];

            motion.TVelocity += motion.TAcceleration * m_Time.DeltaTime;
            motion.RVelocity += motion.RAcceleration * m_Time.DeltaTime;
            motion.TMovement += motion.TVelocity * m_Time.DeltaTime;
            motion.RMovement += motion.RVelocity * m_Time.DeltaTime;

            robot.Controller.Move(motion.TMovement);
            robot.transform.rotation = Quaternion.Euler(0.0f, robot.transform.eulerAngles.y + motion.RMovement, 0.0f);
            velocity = robot.Controller.velocity;

            motion.TMovement = Vector3.zero;
            motion.RMovement = 0f;
        }
        #endregion
    }
}