using Cinemachine;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.DataModels.Descriptors;
using Swap.Interfaces;
using UnityEngine;

namespace Swap.Rules.Controls
{
    public class CameraRule : GameRule
    {
        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IControllerRule ControllerRule;

        private CameraDescriptor m_Descriptor;

        private CinemachineVirtualCamera m_Camera;
        private Transform m_CameraTarget;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<CameraDescriptor>("Camera");

            m_Camera = LevelRule.GetCamera().GetComponent<CinemachineVirtualCamera>();
            m_CameraTarget = m_Camera.Follow.parent;

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update() { }

        protected override void LateUpdate()
        {
            RotateCamera();
        }
        #endregion

        #region private
        private void RotateCamera()
        {
            float inputRotation = ControllerRule.GetLookValue().y;
            if (inputRotation == 0)
                return;

            Vector3 currentRotation = m_CameraTarget.rotation.eulerAngles;
            float targetRotation = inputRotation * m_Descriptor.AngularSpeed * m_Time.DeltaTime;

            float targetPitch = currentRotation.x + targetRotation;
            if (targetPitch < -180f) targetPitch += 360f;
            if (targetPitch > 180f) targetPitch -= 360f;
            targetPitch = Mathf.Clamp(targetPitch, m_Descriptor.MinDownAngle, m_Descriptor.MaxUpAngle);

            m_CameraTarget.rotation = Quaternion.Euler(targetPitch, currentRotation.y, 0.0f);
        }
        #endregion
    }
}
