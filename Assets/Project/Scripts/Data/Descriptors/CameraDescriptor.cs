using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewCameraDescriptor", menuName = "Content/Game/Camera Descriptor", order = 120)]
    public class CameraDescriptor : ContentDescriptor
    {
        [Header("Vertical Angle")]
        public float AngularSpeed;

        public float MinDownAngle;

        public float MaxUpAngle;

        [Header("Zoom")]
        public float ZoomSpeed;

        public float MinFieldOfView;

        public float MaxFieldOfView;
    }
}
