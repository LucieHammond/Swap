using System.Collections.Generic;
using UnityEngine;

namespace Swap.Components
{
    public class PlayerSoul : MonoBehaviour
    {
        #region Component properties
        public RobotBody CurrentRobotBody { get; private set; }

        public Dictionary<RobotBody, GemStone> CurrentlyHeldGems;
        #endregion

        #region Exposed methods
        public PlayerSoul Setup()
        {
            CurrentRobotBody = GetComponentInParent<RobotBody>();
            return this;
        }

        public void SetCurrentRobot(RobotBody robot)
        {
            CurrentRobotBody = robot;
            CurrentlyHeldGems = new Dictionary<RobotBody, GemStone>();
        }
        #endregion
    }
}
