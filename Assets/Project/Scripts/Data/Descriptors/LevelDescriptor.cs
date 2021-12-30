using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewLevelDescriptor", menuName = "Content/Game/Level Descriptor", order = 102)]
    public class LevelDescriptor : ContentDescriptor
    {
        public string Name;

        public string Scene;

        public Vector3 EndPoint;
    }
}

