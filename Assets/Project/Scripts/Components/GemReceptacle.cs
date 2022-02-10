using Swap.Components.States;
using Swap.Components.Models;
using UnityEngine;

namespace Swap.Components
{
    public class GemReceptacle : MonoBehaviour
    {
        [Header("Configuration")]
        public BasicColor Color;

        public Signal SignalToSend;

        [Header("Components")]
        public Transform ReceptionPoint;

        [Header("Parameters")]
        public float ActivationRadius;


        #region Component properties
        public GemReceptacleState State { get; private set; }
        public GemStone ConnectedStone { get; private set; }
        public bool IsActivated { get; private set; }
        #endregion

        #region Component methods
        public GemReceptacle Setup()
        {
            State = GemReceptacleState.Waiting;
            ConnectedStone = null;
            IsActivated = false;
            return this;
        }

        public void AttractStone(GemStone stone)
        {
            State = GemReceptacleState.Attracting;
            ConnectedStone = stone;
        }

        public void InsertStone(GemStone stone)
        {
            State = GemReceptacleState.Inserting;
            ConnectedStone = stone;
        }

        public void HoldStone(GemStone stone)
        {
            State = GemReceptacleState.Holding;
            ConnectedStone = stone;
        }

        public void DisconnectStone()
        {
            State = GemReceptacleState.Waiting;
            ConnectedStone = null;
        }

        public void UpdateActivation()
        {
            IsActivated = ConnectedStone != null && ConnectedStone.Color == Color
                && Vector3.Distance(ReceptionPoint.position, ConnectedStone.transform.position) < ActivationRadius;
        }
        #endregion
    }
}
