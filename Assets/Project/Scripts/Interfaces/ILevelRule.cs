﻿using Swap.Components;
using Swap.Data.Models;
using UnityEngine;

namespace Swap.Interfaces
{
    public interface ILevelRule
    {
        Transform GetRootTransform(string rootName);

        LevelState GetLevelState();

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
