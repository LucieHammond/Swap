using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewInteractDescriptor", menuName = "Content/Game/Interact Descriptor", order = 141)]
    public class InteractDescriptor : ContentDescriptor
    {
        [Header("Check")]
        public float CheckHeight;

        public float CheckRadius;

        public LayerMask CheckRaycastLayers;
    }
}
