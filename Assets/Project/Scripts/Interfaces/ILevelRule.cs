using Swap.Components;
using UnityEngine;

namespace Swap.Interfaces
{
    public interface ILevelRule
    {
        Transform GetRootTransform(string rootName);

        GameObject GetCamera();

        PlayerSoul GetPlayerSoul();

        NonPlayerSoul[] GetNonPlayerSouls();

        RobotBody[] GetRobotBodies();

        Button[] GetButtons();

        GemStone[] GetGemStones();

        GemReceptacle[] GetGemReceptacles();

        Door[] GetDoors();

        MobilePlatform[] GetMobilePlatforms();
    }
}
