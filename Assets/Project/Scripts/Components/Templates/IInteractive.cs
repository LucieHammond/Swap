using UnityEngine;

namespace Swap.Components.Template
{
    public interface IInteractive
    {
        bool IsAvailable();

        Vector3 GetInteractionCenter();

        bool ReceiveInteractionRay(RaycastHit hit);

        Vector3 GetMarkerPosition();
    }
}
