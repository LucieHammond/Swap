using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewMotionDescriptor", menuName = "Content/Game/Motion Descriptor", order = 122)]
    public class MotionDescriptor : ContentDescriptor
    {
        [Header("Gravity")]
        public float CharacterGravity;

        public float FallSpeedOnGround;

        public float FallSpeedInAir;

        [Header("Ground")]
        public Vector3 GroundCheckOffset;

        public float GroundCheckRadius;

        public LayerMask GroundLayers;

        [Header("Slope")]
        public float SlopeMinAngle;

        public float SlopeMaxAngle;

        [Header("Repulsion")]
        public float RepulsionMargin;

        public float RepulsionMinHeight;

        public float RepulsionSpeed;

        public float RepulsionLinearFactor;
    }
}
