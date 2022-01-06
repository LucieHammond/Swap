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

        private CharacterDescriptor m_Descriptor;

        private Transform m_CurrentCharacter;
        private CharacterController m_CharacterController;
        private RobotBody[] m_AllRobots;

        private bool m_IsGrounded = false;
        private bool m_IsOnSlope = false;
        private Vector2 m_HorizontalVelocity = Vector2.zero;
        private float m_VerticalVelocity = 0f;
        private float m_JumpWaitingTime;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<CharacterDescriptor>("Character");

            m_AllRobots = LevelRule.GetRobotBodies();
            GameObject initialCharacter = LevelRule.GetLevelState().CurrentRobotBody.gameObject;
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
                MoveCharacter();
                TurnCharacter();
            }

            for (int i = 0; i < m_AllRobots.Length; i++)
            {
                CharacterController controller = m_AllRobots[i].Controller;
                if (controller != m_CharacterController)
                {
                    controller.Move(-2f * m_Time.DeltaTime * Vector3.up);
                }
            }
        }
        #endregion

        #region ICharacterRule API
        public void EnterCharacter(GameObject character)
        {
            m_CurrentCharacter = character.transform;
            m_CharacterController = character.GetComponent<CharacterController>();

            m_IsGrounded = false;
            m_IsOnSlope = false;
            m_HorizontalVelocity = Vector2.zero;
            m_VerticalVelocity = 0f;
            m_JumpWaitingTime = m_Descriptor.JumpCooldown;
        }

        public void ExitCharacter()
        {
            m_CurrentCharacter = null;
            m_CharacterController = null;
        }
        #endregion

        #region private
        private void MoveCharacter()
        {
            m_IsGrounded = CheckIfGrounded();
            m_IsOnSlope = CheckIfSloped();

            Vector3 horizontalMove = ComputeHorizontalMovement();
            Vector3 verticalMove = ComputeVerticalMovement();

            m_CharacterController.Move(horizontalMove + verticalMove);
            m_HorizontalVelocity = new Vector2(m_CharacterController.velocity.x, m_CharacterController.velocity.z);
        }

        private void TurnCharacter()
        {
            float inputTurn = ControllerRule.GetLookValue().x;
            if (inputTurn == 0)
                return;

            Vector3 currentRotation = m_CurrentCharacter.eulerAngles;
            float targetRotation = inputTurn * m_Descriptor.RotationSpeed;
            if (Mathf.Abs(targetRotation) > m_Descriptor.RotationThreshold)
            {
                float targetYaw = currentRotation.y + targetRotation;

                if (targetYaw < -360f) targetYaw += 360f;
                if (targetYaw > 360f) targetYaw -= 360f;

                m_CurrentCharacter.rotation = Quaternion.Euler(currentRotation.x, targetYaw, 0.0f);
            }
        }

        private bool CheckIfGrounded()
        {
            Vector3 checkPosition = m_CurrentCharacter.position + m_Descriptor.GroundCheckOffset;
            return Physics.CheckSphere(checkPosition, m_Descriptor.GroundCheckRadius, m_Descriptor.GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private bool CheckIfSloped()
        {
            if (!m_IsGrounded)
                return false;

            Vector3 checkPosition = m_CurrentCharacter.position + m_Descriptor.GroundCheckOffset;
            Physics.Raycast(checkPosition, Vector3.down, out RaycastHit hit, m_Descriptor.GroundCheckRadius, m_Descriptor.GroundLayers, QueryTriggerInteraction.Ignore);
            float angle = 90f - Mathf.Acos(new Vector2(hit.normal.x, hit.normal.z).magnitude) * Mathf.Rad2Deg;
            return angle >= m_Descriptor.SlopeMinAngle && angle <= m_Descriptor.SlopeMaxAngle;
        }

        private Vector3 ComputeHorizontalMovement()
        {
            Vector2 inputMove = ControllerRule.GetMoveValue();
            bool isRunning = ControllerRule.IsRunning();
            bool isStopped = inputMove == Vector2.zero;

            // Compute position change
            Vector3 targetDirection = m_CurrentCharacter.rotation * new Vector3(inputMove.x, 0.0f, inputMove.y).normalized;
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
                float targetRotation = m_CurrentCharacter.eulerAngles.y + Mathf.Atan2(inputMove.x, inputMove.y) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(m_CurrentCharacter.eulerAngles.y, targetRotation, ref rotationSpeed, m_Descriptor.TurningTime);

                m_CurrentCharacter.rotation = Quaternion.Euler(m_CurrentCharacter.eulerAngles.x, rotation, 0.0f);
            }

            return targetDirection * actualSpeed * m_Time.DeltaTime;
        }

        private Vector3 ComputeVerticalMovement()
        {
            if (m_IsGrounded)
            {
                m_VerticalVelocity = Mathf.Max(m_VerticalVelocity, -2f);

                if (ControllerRule.AskedJump() && m_JumpWaitingTime <= 0.0)
                {
                    m_VerticalVelocity = Mathf.Sqrt(m_Descriptor.JumpHeight * -2f * m_Descriptor.FallGravity);
                }

                if (m_JumpWaitingTime >= 0.0)
                    m_JumpWaitingTime -= m_Time.DeltaTime;
            }
            else
            {
                m_VerticalVelocity = Mathf.Max(m_VerticalVelocity, -m_Descriptor.MaxFallSpeed);

                m_JumpWaitingTime = m_Descriptor.JumpCooldown;
            }

            m_VerticalVelocity += m_Descriptor.FallGravity * Time.deltaTime;

            return new Vector3(0.0f, m_VerticalVelocity, 0.0f) * m_Time.DeltaTime;
        }
        #endregion
    }
}