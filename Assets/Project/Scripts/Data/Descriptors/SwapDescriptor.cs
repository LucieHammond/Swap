using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewSwapDescriptor", menuName = "Content/Game/Swap Descriptor", order = 140)]
    public class SwapDescriptor : ContentDescriptor
    {
        [Header("Detection")]
        public LayerMask VisibleLayers;

        public float TargetRadius;

        public Vector3 TargetOffset;

        [Header("Movement")]
        public float SwapSpeed;

        public float PlayerDeviation;

        public float PartnerDeviation;
    }
}
