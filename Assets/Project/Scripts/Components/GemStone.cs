using Swap.Components.States;
using Swap.Components.Template;
using Swap.Components.Models;
using UnityEngine;

namespace Swap.Components
{
    public class GemStone : MonoBehaviour, IInteractive, IGenerative
    {
        [Header("Configuration")]
        public BasicColor Color;

        [Header("Components")]
        public Rigidbody RigidBody;

        public Collider Collider;

        [Header("Parameters")]
        public bool InitiallyActive;

        public float MarkerDistance;

        public Vector3 BaseRotation;


        #region Component properties
        public static Transform ObjectsRoot { get; private set; }

        public GemStoneState State { get; private set; }
        public RobotBody CurrentRobotOwner { get; private set; }
        public GemReceptacle CurrentReceptacle { get; private set; }
        #endregion

        #region Component methods
        public static void SetupRoot(Transform root)
        {
            ObjectsRoot = root;
        }

        public GemStone Setup()
        {
            State = InitiallyActive ? GemStoneState.Free : GemStoneState.Inactive;
            CurrentRobotOwner = null;
            CurrentReceptacle = null;
            return this;
        }

        public void Free()
        {
            State = GemStoneState.Free;
            CurrentRobotOwner = null;
            CurrentReceptacle = null;

            transform.SetParent(ObjectsRoot, true);
            RigidBody.isKinematic = false;
            Collider.isTrigger = false;
        }

        public void RetrievedWithRobot(RobotBody robot)
        {
            State = GemStoneState.TakenByRobot;
            CurrentRobotOwner = robot;
            CurrentReceptacle = null;

            transform.SetParent(robot.ObjectPoint, true);
            RigidBody.isKinematic = true;
            Collider.isTrigger = true;
        }

        public void ConnectToReceptacle(GemReceptacle receptacle)
        {
            State = GemStoneState.InReceptacle;
            CurrentRobotOwner = null;
            CurrentReceptacle = receptacle;

            transform.SetParent(receptacle.ReceptionPoint, true);
            RigidBody.isKinematic = true;
        }

        public void SetInactive()
        {
            State = GemStoneState.Inactive;
            CurrentRobotOwner = null;
            CurrentReceptacle = null;

            transform.SetParent(ObjectsRoot, true);
            RigidBody.isKinematic = true;
            Collider.isTrigger = false;
        }
        #endregion

        #region IInteractive
        public bool IsAvailable() => State != GemStoneState.Inactive;

        public Vector3 GetInteractionCenter() => transform.position;

        public bool ReceiveInteractionRay(RaycastHit hit) => hit.collider == Collider;

        public Vector3 GetMarkerPosition() => transform.position + MarkerDistance * Vector3.up;
        #endregion

        #region IGenerative
        public void Activate() => Free();

        public void Deactivate() => SetInactive();

        public Quaternion DefaultRotation() => Quaternion.Euler(BaseRotation);
        #endregion
    }
}
