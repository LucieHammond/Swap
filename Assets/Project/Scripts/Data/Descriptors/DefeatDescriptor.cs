using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewDefeatDescriptor", menuName = "Content/Game/Defeat Descriptor", order = 180)]
    public class DefeatDescriptor : ContentDescriptor
    {
        [Header("Conditions")]
        public float MaxDepthThreshold;

        [Header("Display")]
        public float DisplayTime;
    }
}
