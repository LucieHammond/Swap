using UnityEngine;

namespace Swap.Interfaces
{
    public interface ILevelRule
    {
        GameObject GetInitialCharacter();

        GameObject GetCamera();
    }
}
