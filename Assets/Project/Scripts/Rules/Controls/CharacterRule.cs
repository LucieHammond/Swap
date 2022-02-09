using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using UnityEngine;

namespace Swap.Rules.Controls
{
    [RuleAccess(typeof(ICharacterRule))]
    public class CharacterRule : GameRule, ICharacterRule
    {
        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IControllerRule ControllerRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IMotionRule MotionRule;

        private CharacterDescriptor m_Descriptor;

        private RobotBody m_CurrentCharacter;

        private bool m_IsGrounded = false;
        private Vector3 m_HorizontalVelocity = Vector3.zero;
        private float m_JumpWaitingTime;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<CharacterDescriptor>("Character");

            RobotBody initialCharacter = LevelRule.GetPlayerSoul().CurrentRobotBody;
            EnterCharacter(initialCharacter);

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            if (m_CurrentCharacter != null)
            {
                m_IsGrounded = MotionRule.IsCurrentlyGrounded(m_CurrentCharacter, out _, out _);

                ComputeHorizontalMovement();
                ComputeVerticalMovement();
                ComputeRotationalMovement();
            }
        }
        #endregion

        #region ICharacterRule API
        public void EnterCharacter(RobotBody character)
        {
            m_CurrentCharacter = character;

            m_IsGrounded = false;
            m_HorizontalVelocity = Vector2.zero;
            m_JumpWaitingTime = m_Descriptor.JumpCooldown;
        }

        public void ExitCharacter()
        {
            m_CurrentCharacter = null;
        }
        #endregion

        #region private
        private void ComputeHorizontalMovement()
        {
            Vector2 inputMove = ControllerRule.GetMoveValue();
            bool isRunning = ControllerRule.IsRunning() && m_IsGrounded;
            bool isStopped = inputMove == Vector2.zero;

            // Compute position change
            Vector3 targetDirection = m_CurrentCharacter.transform.rotation * new Vector3(inputMove.x, 0.0f, inputMove.y).normalized;
            float targetSpeed = isStopped ? 0.0f : isRunning ? m_Descriptor.RunSpeed : m_Descriptor.WalkSpeed;
            float currentSpeed = m_HorizontalVelocity.magnitude;

            float actualSpeed;
            if (currentSpeed < targetSpeed - m_Descriptor.TargetSpeedDelta || currentSpeed > targetSpeed + m_Descriptor.TargetSpeedDelta)
                actualSpeed = Mathf.Lerp(currentSpeed, targetSpeed, m_Time.DeltaTime * m_Descriptor.SpeedChangeRate);
            else
                actualSpeed = targetSpeed;

            // Compute rotation change
            if (m_Descriptor.ActivateTurn && inputMove.x != 0)
            {
                float rotationSpeed = 0f;
                float targetRotation = Mathf.Atan2(inputMove.x, inputMove.y) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(0f, targetRotation, ref rotationSpeed, m_Descriptor.TurningTime);

                MotionRule.ApplyRotationalMovement(m_CurrentCharacter, rotation);
            }

            m_HorizontalVelocity = targetDirection * actualSpeed;
            MotionRule.ApplyTranslationalMovement(m_CurrentCharacter, targetDirection * actualSpeed * m_Time.DeltaTime);
        }

        private void ComputeVerticalMovement()
        {
            if (m_IsGrounded)
            {
                if (ControllerRule.AskedJump() && m_JumpWaitingTime <= 0.0)
                {
                    float verticalSpeed = Mathf.Sqrt(m_Descriptor.JumpHeight * -2f * m_Descriptor.JumpGravity);
                    MotionRule.ApplyTranslationalVelocity(m_CurrentCharacter, verticalSpeed * Vector3.up, false);
                }

                if (m_JumpWaitingTime >= 0.0)
                    m_JumpWaitingTime -= m_Time.DeltaTime;
            }
            else
            {
                m_JumpWaitingTime = m_Descriptor.JumpCooldown;
            }
        }

        private void ComputeRotationalMovement()
        {
            float inputTurn = ControllerRule.GetLookValue().x;
            if (inputTurn == 0)
                return;

            float targetRotation = inputTurn * m_Descriptor.RotationSpeed;
            if (Mathf.Abs(targetRotation) > m_Descriptor.RotationThreshold)
            {
                MotionRule.ApplyRotationalMovement(m_CurrentCharacter, targetRotation);
            }
        }
        #endregion
    }
}