using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.DataModels.Descriptors
{
    [CreateAssetMenu(fileName = "NewCameraDescriptor", menuName = "Content/Game/Camera Descriptor", order = 121)]
    public class CameraDescriptor : ContentDescriptor
    {
        public float AngularSpeed;

        public float MaxUpAngle;

        public float MinDownAngle;
    }
}
