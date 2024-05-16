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

    [Header("Car Front (Transform)")]// Assign a Gameobject representing the front of the car
    public Transform carFront;

    public float maxSteerAngle = 90f;
    public int MaxRPM = 300;
    private int localMaxRpm;
    public float speed = 0;
    private bool turning = false;

    private float MovementTorque = 1;
    private int count = 0;
    private float distanceThreshold = 3;
    private float steeringSpeed = 15f;

    private List<Transform> waypoints = new List<Transform>();

    void Start()
    {
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints.Add(waypointParent.GetChild(i));
        }

        localMaxRpm = MaxRPM;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, waypoints[count].position) < distanceThreshold && count < waypoints.Count)
        {
            if (waypoints[count].tag == "DriveTurnPoint")
            {
                localMaxRpm = 100;


            }
            else if (waypoints[count].tag == "DriveTurnEndPoint")
            {
                localMaxRpm = MaxRPM;

            }

            count++;
        }
        else if (count >= waypoints.Count)
        {
            Destroy(gameObject);
        }



        UpdateWheels();
        ApplySteering(waypoints[count]);
        Movement();
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
        frontLeft.brakeTorque = 0;
        frontRight.brakeTorque = 0;
        backLeft.brakeTorque = 0;
        backRight.brakeTorque = 0;

        int SpeedOfWheels = (int)((frontLeft.rpm + frontRight.rpm + backLeft.rpm + backRight.rpm) / 4);

        if (SpeedOfWheels < localMaxRpm)
        {
            backRight.motorTorque = 400 * MovementTorque;
            backLeft.motorTorque = 400 * MovementTorque;
            frontRight.motorTorque = 400 * MovementTorque;
            frontLeft.motorTorque = 400 * MovementTorque;
        }
        else if (SpeedOfWheels < localMaxRpm + (localMaxRpm * 1 / 4))
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
