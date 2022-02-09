using GameEngine.Core.Unity.System;
using UnityEngine;

namespace Swap.Data.Descriptors
{
    [CreateAssetMenu(fileName = "NewSpawnDescriptor", menuName = "Content/Game/Spawn Descriptor", order = 163)]
    public class SpawnDescriptor : ContentDescriptor
    {
        [Header("Despawn")]
        public float DespawnTime;

        [Header("Respawn")]
        public float SpawnTime;
    }
}
