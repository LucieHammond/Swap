using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewStartDescriptor", menuName = "Content/Game/Start Descriptor", order = 180)]
    public class StartDescriptor : ContentDescriptor
    {
        [Header("Start Screen")]
        public float TitleDisplayTime;

        public float PanelDisplayTime;

        public float PanelFadeTime;
    }
}
