using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverMovement : MonoBehaviour
{
    public Transform waypointParent;

    [Header("Car Wheels (Wheel Collider)")]// Assign wheel Colliders through the inspector
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider backLeft;
    public WheelCollider backRight;

    [Header("Car Wheels (Transform)")]// Assign wheel Transform(Mesh render) through the inspector
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelBL;
    public Transform wheelBR;

    [Header("Car Checks (Transform)")]// Assign a Gameobject representing the front of the car and the rightLane checkbox
    public Transform rightCheck;
    public Transform frontCheck;
    public float distanceUntillBraking;

    [Header("Mass")]
    [SerializeField] Transform centerOfMass;

    [Header("Motor Variables")]
    public float maxSteerAngle = 90f;
    public int MaxRPM = 300;
    private int localMaxRpm;
    public float speed = 0;
    private bool turning = false;
    private bool stop = false;

    private float MovementTorque = 1;
    private int count = 0;
    private float distanceThreshold = 3;
    private float steeringSpeed = 15f;
    private float motorMult = 1f;
    private DriverChecks driverChecks;

    private List<Transform> waypoints = new List<Transform>();

    void Start()
    {
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints.Add(waypointParent.GetChild(i));
        }

        localMaxRpm = MaxRPM;

        if (GetComponent<Rigidbody>() != null && centerOfMass != null)
        {
            GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        }

        if (rightCheck != null)
        {
            driverChecks = transform.GetChild(0).GetComponent<DriverChecks>();
        }
    }

    void Update()
    {
        RaycastHit hit;
        // Use the forward direction of the frontCheck transform
        if (Physics.Raycast(frontCheck.position, frontCheck.forward, out hit, distanceUntillBraking))
        {
            motorMult = (hit.distance / distanceUntillBraking) * 0.5f;

            if (motorMult <= 0.3f)
            {
                ApplyBrakes();
                stop = true;
            }
            else
            {
                motorMult = 1f;
                frontLeft.brakeTorque = 0;
                frontRight.brakeTorque = 0;
                backLeft.brakeTorque = 0;
                backRight.brakeTorque = 0;
            }
        }
        else
        {
            motorMult = 1f;
            frontLeft.brakeTorque = 0;
            frontRight.brakeTorque = 0;
            backLeft.brakeTorque = 0;
            backRight.brakeTorque = 0;
            stop = false;
        }

        // Visualize the raycast in the Scene view during gameplay
        Debug.DrawRay(frontCheck.position, frontCheck.forward * distanceUntillBraking, Color.red);


        if (Vector3.Distance(transform.position, waypoints[count].position) < distanceThreshold && count < waypoints.Count)
        {
            if (waypoints[count].tag == "DriveTurnPoint")
            {
                localMaxRpm = 100;
                turning = true;
            }
            else if (waypoints[count].tag == "DriveTurnEndPoint")
            {
                localMaxRpm = MaxRPM;
                turning = false;
            }

            count++;

            if (count >= waypoints.Count)
            {
                count = 0;
                transform.position = waypoints[0].position;
                transform.LookAt(waypoints[1]);
            }
        }

        if (turning == true)
        {
            if (driverChecks.IsTriggerBoxContainingObjects() == true)
            {
                ApplyBrakes();
                stop = true;
            }
            else
            {
                frontLeft.brakeTorque = 0;
                frontRight.brakeTorque = 0;
                backLeft.brakeTorque = 0;
                backRight.brakeTorque = 0;
                stop = false;
            }
        }


        UpdateWheels();
        ApplySteering(waypoints[count]);
        if (!stop) 
        {
            Movement();
        }
    }


    private void ApplyBrakes() // Apply brake torque 
    {
        frontLeft.brakeTorque = 5000;
        frontRight.brakeTorque = 5000;
        backLeft.brakeTorque = 5000;
        backRight.brakeTorque = 5000;
    }


    private void UpdateWheels() // Updates the wheel's postion and rotation
    {
        ApplyRotationAndPostion(frontLeft, wheelFL);
        ApplyRotationAndPostion(frontRight, wheelFR);
        ApplyRotationAndPostion(backLeft, wheelBL);
        ApplyRotationAndPostion(backRight, wheelBR);
    }

    private void ApplyRotationAndPostion(WheelCollider targetWheel, Transform wheel) // Updates the wheel's postion and rotation
    {
        targetWheel.ConfigureVehicleSubsteps(5, 12, 15);

        Vector3 pos;
        Quaternion rot;
        targetWheel.GetWorldPose(out pos, out rot);
        wheel.position = pos;
        wheel.rotation = rot;
    }

    void ApplySteering(Transform nextDest) // Applies steering to the Current waypoint
    {
        Vector3 targetPosition = nextDest.position;
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Keep the car on the same plane as the target

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate the car towards the target
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * steeringSpeed);

        // Calculate the angle difference between current forward direction and target direction
        float angle = Vector3.SignedAngle(transform.forward, direction.normalized, Vector3.up);

        // Clamp the angle within the maximum steer angle
        angle = Mathf.Clamp(angle, -maxSteerAngle, maxSteerAngle);

        // Apply the angle to the front wheels
        frontLeft.steerAngle = angle;
        frontRight.steerAngle = angle;
    }

    void Movement() // moves the car forward and backward depending on the input
    {
        int SpeedOfWheels = (int)((frontLeft.rpm + frontRight.rpm + backLeft.rpm + backRight.rpm) / 4);

        if (SpeedOfWheels < localMaxRpm * motorMult)
        {
            backRight.motorTorque = 400 * MovementTorque;
            backLeft.motorTorque = 400 * MovementTorque;
            frontRight.motorTorque = 400 * MovementTorque;
            frontLeft.motorTorque = 400 * MovementTorque;
        }
        else if (SpeedOfWheels < (localMaxRpm + (localMaxRpm * 1 / 4) * motorMult))
        {
            backRight.motorTorque = 0;
            backLeft.motorTorque = 0;
            frontRight.motorTorque = 0;
            frontLeft.motorTorque = 0;
        }
        else
            ApplyBrakes();

        speed = SpeedOfWheels;

    }
}
