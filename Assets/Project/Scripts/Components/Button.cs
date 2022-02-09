using Swap.Components.Template;
using Swap.Data.Models;
using UnityEngine;

namespace Swap.Components
{
    public class Button : MonoBehaviour, IInteractive
    {
        public Signal SignalToSend;

        public Animator Animator;

        public Collider Collider;

        public Vector3 CenterOffset;

        public float MarkerDistance;


        #region IInteractive
        public bool IsAvailable() => true;

        public Vector3 GetInteractionCenter() => transform.position + transform.rotation * CenterOffset;

        public bool ReceiveInteractionRay(RaycastHit hit) => hit.collider == Collider;

        public Vector3 GetMarkerPosition() => GetInteractionCenter() + MarkerDistance * Vector3.up;
        #endregion
    }
}
