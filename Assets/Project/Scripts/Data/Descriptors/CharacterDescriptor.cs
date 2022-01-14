using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewCharacterDescriptor", menuName = "Content/Game/Character Descriptor", order = 121)]
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

        public float JumpGravity;
    }
}
