using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewPressDescriptor", menuName = "Content/Game/Press Descriptor", order = 142)]
    public class PressDescriptor : ContentDescriptor
    {
        [Header("Trigger")]
        public string AnimationParameter;

        public float CoolDownTime;

        [Header("Activation")]
        public float ActivationDelay;
    }
}
