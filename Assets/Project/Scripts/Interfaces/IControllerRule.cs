using UnityEngine;

namespace Swap.Interfaces
{
    public interface IControllerRule
    {
        Vector2 GetMoveValue();

        Vector2 GetLookValue();

        bool IsRunning();

        bool AskedJump();

        bool AskedInteraction();

        bool AskedSwap();

        float GetZoomValue();

        bool AskedPause();
    }
}
