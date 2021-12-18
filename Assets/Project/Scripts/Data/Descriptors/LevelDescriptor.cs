using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.DataModels.Descriptors
{
    [CreateAssetMenu(fileName = "NewLevelDescriptor", menuName = "Content/Game/Level Descriptor", order = 102)]
    public class LevelDescriptor : ContentDescriptor
    {
        public string Name;

        public string Scene;
    }
}

