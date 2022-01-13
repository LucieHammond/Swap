using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewPickupDescriptor", menuName = "Content/Game/Pickup Descriptor", order = 143)]
    public class PickupDescriptor : ContentDescriptor
    {
        [Header("Retrieve")]
        public float RetrieveSpeed;

        [Header("Release")]
        public float ReleaseSpeed;

        public Vector3 ReleaseOffset;

        public Vector3 ReleaseAngle;
    }
}
