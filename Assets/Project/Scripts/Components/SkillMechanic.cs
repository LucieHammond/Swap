using Swap.Components.Models;
using System.Linq;
using UnityEngine;

namespace Swap.Components
{
    public class SkillMechanic : MonoBehaviour
    {
        [Header("Configuration")]
        public RobotType RobotRequired;

        public SkillType SkillType;

        [SerializeField, HideInInspector]
        public SkillSettings Settings;

        [Header("Components")]
        public Transform TriggerPoint;

        [Header("Parameters")]
        public float TriggerRadius;

        public float TriggerHeight;


        #region Component Properties
        public bool IsActivated { get; private set; }

        public RobotBody ActiveRobot { get; private set; }
        #endregion

        #region Component methods
        public SkillMechanic Setup()
        {
            IsActivated = false;
            ActiveRobot = null;
            return this;
        }

        public void UpdateActivation(RobotBody[] robots)
        {
            foreach (RobotBody robot in robots.Where(robot => robot.RobotType == RobotRequired))
            {
                Vector3 relativePosition = robot.transform.position - TriggerPoint.position;
                if (relativePosition.magnitude < TriggerRadius && Mathf.Abs(relativePosition.y) < TriggerHeight)
                {
                    IsActivated = true;
                    ActiveRobot = robot;
                    return;
                }
            }

            IsActivated = false;
            ActiveRobot = null;
        }
        #endregion
    }
}
