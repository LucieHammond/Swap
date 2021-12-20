using Swap.Components;
using UnityEngine;

namespace Swap.Interfaces
{
    public interface ILevelRule
    {
        GameObject GetCamera();

        PlayerSoul GetPlayerSoul();

        NonPlayerSoul[] GetNonPlayerSouls();

        RobotBody[] GetRobotBodies();
    }
}
