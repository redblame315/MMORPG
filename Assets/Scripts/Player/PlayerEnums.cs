using UnityEngine;
using System.Collections;

public static class PlayerEnums
{
	public enum MovementState
    {
        Idle,
        Walking,
        WalkingLeft,
        WalkingRight,
        WalkingBack,
        WalkingBackRight,
        WalkingBackLeft,
        Running,
        RunningLeft,
        RunningRight,
        RunningBack,
        SwimIdle,
        Swimming,
        SwimmingLeft,
        SwimmingRight,
        SwimmingBack,
        SwimmingFast,
        SwimmingFastRight,
        SwimmingFastLeft,
        SwimmingBackFast,
        SwimmingUp,
        Jumping,
        Falling,
        Crouching,
    }
}
