using Cinemachine;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Data.Descriptors;
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
            m_Camera.enabled = true;

            MarkInitialized();
        }

        protected override void Unload()
        {
            m_Camera.enabled = false;

            MarkUnloaded();
        }

        protected override void Update() 
        {
            RotateCamera();
            ZoomCamera();
        }
        #endregion

        #region private
        private void RotateCamera()
        {
            float inputRotation = ControllerRule.GetLookValue().y;
            if (inputRotation == 0)
                return;

            float targetRotation = inputRotation * m_Descriptor.AngularSpeed * m_Time.DeltaTime;
            float targetPitch = m_CameraTarget.eulerAngles.x + targetRotation;
            
            if (targetPitch < -180f) targetPitch += 360f;
            if (targetPitch > 180f) targetPitch -= 360f;
            targetPitch = Mathf.Clamp(targetPitch, m_Descriptor.MinDownAngle, m_Descriptor.MaxUpAngle);

            m_CameraTarget.rotation = Quaternion.Euler(targetPitch, m_CameraTarget.eulerAngles.y, 0.0f);
        }

        private void ZoomCamera()
        {
            float inputZoom = ControllerRule.GetZoomValue();
            if (inputZoom == 0)
                return;

            float targetZoom = inputZoom * m_Descriptor.ZoomSpeed * m_Time.DeltaTime;
            float targetFieldOfView = m_Camera.m_Lens.FieldOfView - targetZoom;
            targetFieldOfView = Mathf.Clamp(targetFieldOfView, m_Descriptor.MinFieldOfView, m_Descriptor.MaxFieldOfView);

            m_Camera.m_Lens.FieldOfView = targetFieldOfView;
        }
        #endregion
    }
}
