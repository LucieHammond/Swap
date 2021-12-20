using GameEngine.Core.Unity.System;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewChapterDescriptor", menuName = "Content/Game/Chapter Descriptor", order = 101)]
    public class ChapterDescriptor : ContentDescriptor
    {
        public string Name;

        public List<LevelDescriptor> Levels;
    }
}
