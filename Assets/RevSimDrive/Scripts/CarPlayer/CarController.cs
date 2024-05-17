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

    public float StandardWheelDampeningRate;

    public bool usingWheel = false;
    public float maxRpm = 300;

    [Header("Force controls")]
    [SerializeField] public float motorForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] public float Speed;
    [SerializeField] public float Braking;

    [SerializeField] public Transform SteeringWheel;

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
        
        for(int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<WheelCollider>().wheelDampingRate = StandardWheelDampeningRate;
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
            
            if(Input.GetKey(KeyCode.UpArrow) == true)
            {
                verticalInput = 1;
            }
            else
            {
                verticalInput = 0;
            }

            if (Input.GetKey(KeyCode.Space) == true)
            {
                brakeInput = 1000;
            }
            else
            {
                brakeInput = 0;
            }
        }




        
    }


    private void HandleMotor()
    {
        float SpeedOfWheels = transform.GetComponent<Rigidbody>().velocity.sqrMagnitude;

        if (SpeedOfWheels < maxRpm)
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;
            rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
            rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        }
        else if (SpeedOfWheels < maxRpm + (maxRpm * 1 / 4))
        {
            verticalInput = 0;
            rearRightWheelCollider.motorTorque = 0;
            rearLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            frontLeftWheelCollider.motorTorque = 0;
        }
        else
        {
            ApplyBreaking();
        }

        if(usingWheel == true)
        {
            currentbreakForce = brakeInput * brakeForce * 10;
        }
        else
        {
            currentbreakForce = brakeInput;
        }
        ApplyBreaking();
        Speed = SpeedOfWheels;
    }

    private void ApplyBreaking()
    {
        if (currentbreakForce <= 900)
        {
            frontRightWheelCollider.wheelDampingRate = StandardWheelDampeningRate + currentbreakForce;
            frontLeftWheelCollider.wheelDampingRate = StandardWheelDampeningRate +  currentbreakForce;
            rearLeftWheelCollider.wheelDampingRate = StandardWheelDampeningRate + currentbreakForce;
            rearRightWheelCollider.wheelDampingRate = StandardWheelDampeningRate + currentbreakForce;

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

        float steeringVisual = horizontalInput * 450;
        Quaternion rot = Quaternion.Euler(-90 + -steeringVisual, -90, -90);
        SteeringWheel.localRotation = rot;

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