using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewSwapDescriptor", menuName = "Content/Game/Swap Descriptor", order = 140)]
    public class SwapDescriptor : ContentDescriptor
    {
        public float TargetRadius;

        public float SwapSpeed;

        public float PlayerDeviation;

        public float PartnerDeviation;
    }
}
