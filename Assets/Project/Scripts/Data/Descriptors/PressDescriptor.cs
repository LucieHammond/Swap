using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewPressDescriptor", menuName = "Content/Game/Press Descriptor", order = 160)]
    public class PressDescriptor : ContentDescriptor
    {
        [Header("Activation")]
        public float ActivationDelay;

        [Header("CoolDown")]
        public float CoolDownTime;
    }
}
