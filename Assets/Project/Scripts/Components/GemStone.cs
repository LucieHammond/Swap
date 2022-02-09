using Swap.Components.Template;
using Swap.Data.Models;
using UnityEngine;

namespace Swap.Components
{
    public class GemStone : MonoBehaviour, IInteractive
    {
        public BasicColor Color;

        public Rigidbody RigidBody;

        public Collider Collider;

        public float MarkerDistance;


        #region IInteractive
        public bool IsAvailable() => true;

        public Vector3 GetInteractionCenter() => transform.position;

        public bool ReceiveInteractionRay(RaycastHit hit) => hit.collider == Collider;

        public Vector3 GetMarkerPosition() => transform.position + MarkerDistance * Vector3.up;
        #endregion
    }
}
