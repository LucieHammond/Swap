using Swap.Data.Models;
using UnityEngine;

namespace Swap.Components
{
    public class RobotBody : MonoBehaviour
    {
        public RobotType RobotType;

        public Transform SoolRoot;

        public Transform InteractRoot;

        public Transform ObjectRoot;

        public CharacterController Controller;
    }
}
