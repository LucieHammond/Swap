using Swap.Components;
using UnityEngine;

namespace Swap.Interfaces
{
    public interface IMotionRule
    {
        void ApplyTranslationalMovement(RobotBody robot, Vector3 movement, bool additive = true);

        void ApplyTranslationalVelocity(RobotBody robot, Vector3 velocity, bool additive = true);

        void ApplyTranslationalAcceleration(RobotBody robot, Vector3 acceleration, bool additive = true);

        void ResetTranslationalMotion(RobotBody robot);


        void ApplyRotationalMovement(RobotBody robot, float movement, bool additive = true);

        void ApplyRotationalVelocity(RobotBody robot, float velocity, bool additive = true);

        void ApplyRotationalAcceleration(RobotBody robot, float acceleration, bool additive = true);

        void ResetRotationalMotion(RobotBody robot);


        Vector3 GetCurrentVelocity(RobotBody robot);

        bool IsCurrentlyGrounded(RobotBody robot, out Collider groundCollider, out float slopeAngle);
    }
}
