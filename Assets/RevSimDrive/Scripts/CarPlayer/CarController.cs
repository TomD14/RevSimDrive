using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float brakeInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;
    public bool usingWheel = false;

    [Header("Force controls")]
    [SerializeField] public float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] public float Speed;
    [SerializeField] public float Braking;

    [Header("Wheels")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [Header("Mass")]
    [SerializeField] Transform centerOfMass;

    private void Start()
    {
        if (GetComponent<Rigidbody>() != null && centerOfMass != null)
        {
            GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        }

    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }


    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);

        if (usingWheel == true)
        {
            verticalInput = (Input.GetAxis("VerticalB") + 1) * 0.5f;
            brakeInput = (Input.GetAxis("Brake") + 1) * 0.5f;

        }
        else
        {
            verticalInput = Input.GetAxis(VERTICAL);
            isBreaking = Input.GetKey(KeyCode.Space);
        }

        Speed = verticalInput;



        
    }


    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce = brakeInput * brakeForce * 10;
        Braking = currentbreakForce;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        if (currentbreakForce <= 900)
        {
            frontRightWheelCollider.wheelDampingRate = currentbreakForce;
            frontLeftWheelCollider.wheelDampingRate = currentbreakForce;
            rearLeftWheelCollider.wheelDampingRate = currentbreakForce;
            rearRightWheelCollider.wheelDampingRate = currentbreakForce;

            frontRightWheelCollider.brakeTorque = 0;
            frontLeftWheelCollider.brakeTorque = 0;
            rearLeftWheelCollider.brakeTorque = 0;
            rearRightWheelCollider.brakeTorque = 0;
        }
        else
        {
            frontRightWheelCollider.brakeTorque = 100;
            frontLeftWheelCollider.brakeTorque = 100;
            rearLeftWheelCollider.brakeTorque = 100;
            rearRightWheelCollider.brakeTorque = 100;
        }

    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;       
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}