using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewPressDescriptor", menuName = "Content/Game/Press Descriptor", order = 143)]
    public class PressDescriptor : ContentDescriptor
    {
        [Header("Trigger")]
        public Vector3 TriggerOffset;

        public float TriggerRadius;

        [Header("Activation")]
        public float ActivationDelay;
    }
}
