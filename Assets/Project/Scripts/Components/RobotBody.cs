using Swap.Components.Template;
using Swap.Components.Models;
using UnityEngine;

namespace Swap.Components
{
    public class RobotBody : MonoBehaviour, IGenerative
    {
        [Header("Configuration")]
        public RobotType RobotType;

        [Header("Components")]
        public Transform SoulPoint;

        public Transform InteractPoint;

        public Transform ObjectPoint;

        public CharacterController Controller;

        public Collider Collider;

        [Header("Parameters")]
        public bool InitiallyActive;


        #region Component properties
        public GemStone CurrentGemStone { get; private set; }

        public bool Active { get; private set; }
        #endregion

        #region Component methods
        public RobotBody Setup()
        {
            CurrentGemStone = null;
            Active = InitiallyActive;
            return this;
        }

        public void RetrieveStone(GemStone stone)
        {
            CurrentGemStone = stone;
        }

        public void ReleaseStone()
        {
            CurrentGemStone = null;
        }

        public void SetActive(bool activated)
        {
            Active = activated;
            Controller.enabled = activated;

            if (!activated)
                ReleaseStone();
        }
        #endregion

        #region IGenerative
        public void Activate() => SetActive(true);

        public void Deactivate() => SetActive(false);

        public Quaternion DefaultRotation() => Quaternion.identity;
        #endregion
    }
}
