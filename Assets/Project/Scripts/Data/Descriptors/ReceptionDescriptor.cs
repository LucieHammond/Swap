using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewReceptionDescriptor", menuName = "Content/Game/Reception Descriptor", order = 162)]
    public class ReceptionDescriptor : ContentDescriptor
    {
        [Header("Connection")]
        public Vector3 ConnectionCenter;

        public float ConnectionRadius;

        [Header("Movement")]
        public Vector3 AttractionCenter;

        public float AttractionSpeed;

        public float InsertionSpeed;

        [Header("Activation")]
        public float ActivationRadius;
    }
}
