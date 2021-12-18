using GameEngine.Core.Unity.System;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.DataModels.Descriptors
{
    [CreateAssetMenu(fileName = "NewQuestDescriptor", menuName = "Content/Game/Quest Descriptor", order = 100)]
    public class QuestDescriptor : ContentDescriptor
    {
        public List<ChapterDescriptor> Chapters;
    }
}
