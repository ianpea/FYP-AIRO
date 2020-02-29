using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerModel
{
    // Player properties
    public PLAYER_STATE playerState;
    public LOCOMOTION_STATE locomotionState;
    public LOCOMOTION_STATE locomotionState1;
    public Collider currentTriggerCollider;

    // Misc
    public bool isDebug = true;

    /* 
     * Locomotion step registration.
     * Each step is stored into a List<Char>, and enqueue dequeue each step.
     * Only consecutive Left and Right step generates movement *subject to change*.
     */
    public  List<string> StepQueue;

    // Unity Components
    public GameObject go;
    public Rigidbody rb;
    public Collider coll;
    public RaycastHit hit;

    // Movement
    public float inputX;
    public float inputY;
    public Vector3 movement;
    public bool inAir = false;

    // Camera
    public Transform cameraPivot;
    public bool invertY = true;
    public Quaternion targetPivotRotation;
    public float lookSpeed = 90f;

    // Acceleration or deceleration
    public float acceleration;

    // Keep track of falling
    public bool falling;
    public float fallSpeed;

    // Average normal of the ground i'm standing on
    public Vector3 groundNormal;

    // If we're touching a dynamic object, don't prevent idle sliding
    public bool touchingDynamic;

    // Was i grounded last frame? used for fudging
    public bool groundedLastFrame;

    // The objects i'm colliding with
    public List<GameObject> collisions;

    // All of the collision contact points
    public Dictionary<int, ContactPoint[]> contactPoints;

    // Temporary calculations
    public float halfPlayerHeight;
    public float bottomCapsuleSphereOrigin; // transform.position.y - this variable = the y coord for the origin of the capsule's bottom sphere
    public float capsuleRadius;
    public bool isPushed;
}

public class PlayerStats
{
    public static float WalkSpeed = 10.0f;
    public static float RunSpeed = 20.0f;
    public static float AccelRate = 10.0f;
    public static float DecelRate = 30.0f;
    public static float AirborneAccel = -10.0f;
    public static float JumpSpeed = 22.0f;
    public static float MaximumSlope = 70.0f;
    public static float JumpFactor = 1.0f;
    public static float DistanceToBelow;
    public static string LastContactTag;
    public static Vector3 LastContactNormal;
}

public static class StepType
{
    public const string
    None = "N",
    Left = "L",
    Right = "R",
    LeftRun = "LR",
    RightRun = "RR";
}

public static class JumpType
{
    public const string
    None = "N",
    Jump = "J";
}

public enum PLAYER_STATE
{
    None = 0,
    Grounded = 1,
    Air = 2,
}

public enum LOCOMOTION_STATE
{
    None = 0,
    Left = 1,
    Right = 2
}
