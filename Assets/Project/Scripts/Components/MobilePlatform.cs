using Swap.Components.States;
using Swap.Data.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Components
{
    public class MobilePlatform : MonoBehaviour
    {
        [Header("Configuration")]
        public ActivationType ActivationType;

        public List<Signal> SignalsToListen;

        public Vector3 TranslationalMotion;

        public Vector3 RotationalMotion;

        public float MovementDuration;

        [Header("Components")]
        public Collider Collider;


        #region Component properties
        public MobilePlatformState State { get; private set; }
        public Vector3 PositionA { get; private set; }
        public Vector3 RotationA { get; private set; }
        public Vector3 PositionB { get; private set; }
        public Vector3 RotationB { get; private set; }
        #endregion

        #region Component methods
        public MobilePlatform Setup()
        {
            State = MobilePlatformState.InPositionA;
            PositionA = transform.position;
            RotationA = transform.eulerAngles;
            PositionB = transform.position + TranslationalMotion;
            RotationB = transform.eulerAngles + RotationalMotion;
            return this;
        }

        public void SetState(MobilePlatformState state)
        {
            State = state;
        }
        #endregion
    }
}
