using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.DataModels.Descriptors
{
    [CreateAssetMenu(fileName = "NewCharacterDescriptor", menuName = "Content/Game/Character Descriptor", order = 120)]
    public class CharacterDescriptor : ContentDescriptor
    {
        [Header("Movement")]
        public float WalkSpeed;

        public float RunSpeed;

        public float SpeedChangeRate;

        public float TargetSpeedDelta;

        [Header("Turn")]
        public bool ActivateTurn;

        public float TurningTime;

        [Header("Rotation")]
        public float RotationSpeed;

        public float RotationThreshold;

        [Header("Jump")]
        public float JumpHeight;

        public float JumpCooldown;

        [Header("Fall")]
        public float FallGravity;

        public float MaxFallSpeed;

        [Header("Ground")]
        public Vector3 GroundCheckOffset;

        public float GroundCheckRadius;

        public LayerMask GroundLayers;

        [Header("Slope")]
        public float SlopeMinAngle;

        public float SlopeMaxAngle;
    }
}
