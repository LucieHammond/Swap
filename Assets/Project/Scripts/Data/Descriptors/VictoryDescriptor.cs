using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewVictoryDescriptor", menuName = "Content/Game/Victory Descriptor", order = 162)]
    public class VictoryDescriptor : ContentDescriptor
    {
        [Header("Attraction")]
        public float AttractionRadius;

        public float AttractionDeviation;

        public float AttractionTime;

        [Header("Condition")]
        public float VictoryRadius;

        [Header("Display")]
        public float DisplayTime;
    }
}

