using Swap.Components.States;
using Swap.Components.Template;
using Swap.Data.Models;
using UnityEngine;

namespace Swap.Components
{
    public class Button : MonoBehaviour, IInteractive
    {
        [Header("Configuration")]
        public Signal SignalToSend;

        [Header("Components")]
        public Animator Animator;

        public Collider Collider;

        [Header("Parameters")]
        public string PressAnimTrigger;

        public Vector3 CenterOffset;

        public float MarkerDistance;


        #region Component properties
        public ButtonState State { get; private set; }
        public bool IsActivated { get; private set; }
        public float ActivationProgression { get; private set; }
        public float CoolDownProgression { get; private set; }
        #endregion

        #region Component methods
        public Button Setup()
        {
            State = ButtonState.Available;
            IsActivated = false;
            CoolDownProgression = 0f;
            ActivationProgression = 0f;
            return this;
        }

        public void Press()
        {
            Animator.SetTrigger(PressAnimTrigger);

            State = ButtonState.Pressed;
            CoolDownProgression = 0f;
            ActivationProgression = 0f;
        }

        public void AchieveActivation()
        {
            State = ButtonState.CoolingDown;
            IsActivated = true;
            ActivationProgression = 0f;
        }

        public void FinishCoolDown()
        {
            State = ButtonState.Available;
            CoolDownProgression = 0f;
        }

        public void UpdateSchedule(float deltaTime)
        {
            switch (State)
            {
                case ButtonState.Pressed:
                    ActivationProgression += deltaTime;
                    break;
                case ButtonState.CoolingDown:
                    IsActivated = false;
                    CoolDownProgression += deltaTime;
                    break;
            }
        }
        #endregion

        #region IInteractive
        public bool IsAvailable() => State == ButtonState.Available;

        public Vector3 GetInteractionCenter() => transform.position + transform.rotation * CenterOffset;

        public bool ReceiveInteractionRay(RaycastHit hit) => hit.collider == Collider;

        public Vector3 GetMarkerPosition() => GetInteractionCenter() + MarkerDistance * Vector3.up;
        #endregion
    }
}
