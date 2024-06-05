using UnityEngine;
using TMPro;
using System.Collections;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";

    private float horizontalInput;
    private float verticalInput;
    private float brakeInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private bool driveBackwards;
    private float backwardsValue = 1;

    public float StandardWheelDampeningRate;
    public bool usingWheel = false;
    public float maxKmPH = 60;

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

    [Header("Speedometer")]
    public TMP_Text speedTextMesh;
    public bool canDrive = true;
    private Rigidbody car;

    [Header("Audio")]
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float pitchTransitionSpeed = 2.0f;
    [SerializeField] private AudioSource accelerationAudioSource;
    [SerializeField] private AudioSource idleAudioSource;
    [SerializeField] private AudioSource decelerationAudioSource;

    [SerializeField]
    public Transform playerCam;
    public float yPosCam = 0.1f;
    public float zPosCam = 0.1f;

    private void Start()
    {
        yPosCam = playerCam.localPosition.y;
        zPosCam = playerCam.localPosition.z;

        Debug.Log("Y = " + yPosCam + " And Z = " + zPosCam);

        idleAudioSource.Play(); 
        car = GetComponent<Rigidbody>();
        if (car != null && centerOfMass != null)
        {
            car.centerOfMass = centerOfMass.localPosition;
        }

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<WheelCollider>().wheelDampingRate = StandardWheelDampeningRate;
        }
    }

    private void FixedUpdate()
    {
        if (canDrive == true)
        {
            GetInput();
            HandleMotor();
            HandleSteering();
            UpdateWheels();

            for (int joystick = 1; joystick < 5; joystick++)
            {
                for (int button = 0; button < 20; button++)
                {
                    if (Input.GetKey("joystick " + joystick + " button " + button))
                    {

                        Debug.Log("joystick = " + joystick + "  button = " + button);
                    }
                }
            }
        }

        
    }

    public void MoveCamera(string Axis, float Amount)
    {
        if (Axis == "Y")
        {
            Vector3 newPosition = playerCam.transform.localPosition;
            newPosition.y = Amount;
            playerCam.localPosition = newPosition;
            Debug.Log("Y changed to = " + newPosition.y);
        }

        if (Axis == "Z")
        {
            Vector3 newPosition = playerCam.transform.localPosition;
            newPosition.z = Amount;
            playerCam.localPosition = newPosition;
            Debug.Log("Z changed to = " + newPosition.z);
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);

        if (usingWheel)
        {
            verticalInput = (Input.GetAxis("VerticalB") + 1) * 0.5f;
            brakeInput = (Input.GetAxis("Brake") + 1) * 0.5f;




            if (Input.GetKey("joystick 1 button 5") == true && driveBackwards == false)
            {
                driveBackwards = true;
                backwardsValue = -1;
            }
            else if (Input.GetKey("joystick 1 button 4") == true && driveBackwards == true)
            {
                driveBackwards = false;
                backwardsValue = 1;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                verticalInput = 1;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                verticalInput = -1;
            }
            else verticalInput = 0;

            brakeInput = Input.GetKey(KeyCode.Space) ? 1000 : 0;
        }
    }

    private void HandleMotor()
    {
        Vector3 velocity = car.velocity;
        float speedInMetersPerSecond = velocity.magnitude;
        float speedInKilometersPerHour = speedInMetersPerSecond * 3.6f;

        int roundedSpeed = Mathf.RoundToInt(speedInKilometersPerHour);
        speedTextMesh.text = roundedSpeed.ToString();

        if (speedInKilometersPerHour < maxKmPH)
        {
            SetMotorTorque(verticalInput * motorForce);
        }
        else if (speedInKilometersPerHour < maxKmPH + (maxKmPH * 1 / 4))
        {
            verticalInput = 0;
            SetMotorTorque(0);
        }
        else
        {
            ApplyBraking();
        }

        currentBrakeForce = usingWheel ? brakeInput * brakeForce * 10 : brakeInput;
        ApplyBraking();

        HandleEngineSound();
    }

    private void SetMotorTorque(float torque)
    {
        torque = torque * backwardsValue;
        frontLeftWheelCollider.motorTorque = torque;
        frontRightWheelCollider.motorTorque = torque;
        rearLeftWheelCollider.motorTorque = torque;
        rearRightWheelCollider.motorTorque = torque;
    }

    private void ApplyBraking()
    {
        Braking = currentBrakeForce;
        if (currentBrakeForce <= 900)
        {
            SetWheelDampingRate(StandardWheelDampeningRate + currentBrakeForce);
            SetBrakeTorque(0);
        }
        else
        {
            SetBrakeTorque(100);
        }

        AdjustFrictionStiffness();

    }

    private void SetWheelDampingRate(float rate)
    {
        frontRightWheelCollider.wheelDampingRate = rate;
        frontLeftWheelCollider.wheelDampingRate = rate;
        rearLeftWheelCollider.wheelDampingRate = rate;
        rearRightWheelCollider.wheelDampingRate = rate;
    }

    private void SetBrakeTorque(float torque)
    {
        frontRightWheelCollider.brakeTorque = torque;
        frontLeftWheelCollider.brakeTorque = torque;
        rearLeftWheelCollider.brakeTorque = torque;
        rearRightWheelCollider.brakeTorque = torque;
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

    private void HandleEngineSound()
    {
        float carVelocityRatio = car.velocity.magnitude / maxKmPH;
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, carVelocityRatio);

        accelerationAudioSource.pitch = targetPitch;
        decelerationAudioSource.pitch = targetPitch;

        if (verticalInput > 0)
        {
            StartCoroutine(TransitionAudio(accelerationAudioSource, true));
            StartCoroutine(TransitionAudio(decelerationAudioSource, false));
            StartCoroutine(TransitionAudio(idleAudioSource, false));
        }
        else if (verticalInput == 0 && car.velocity.sqrMagnitude > 0.1)
        {
            StartCoroutine(TransitionAudio(accelerationAudioSource, false));
            StartCoroutine(TransitionAudio(decelerationAudioSource, true));
            StartCoroutine(TransitionAudio(idleAudioSource, false));
        }
        else
        {
            StartCoroutine(TransitionAudio(accelerationAudioSource, false));
            StartCoroutine(TransitionAudio(decelerationAudioSource, false));
            StartCoroutine(TransitionAudio(idleAudioSource, true));
        }
    }

    private IEnumerator TransitionAudio(AudioSource audioSource, bool shouldPlay)
    {
        if (shouldPlay)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.loop = true;
                audioSource.Play();
            }

            while (audioSource.volume < 1f)
            {
                audioSource.volume += Time.deltaTime * pitchTransitionSpeed;
                yield return null;
            }
        }
        else
        {
            while (audioSource.volume > 0f)
            {
                audioSource.volume -= Time.deltaTime * pitchTransitionSpeed;
                yield return null;
            }

            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void AdjustFrictionStiffness()
    {
        WheelFrictionCurve forwardFrictionBase = frontLeftWheelCollider.forwardFriction;
        forwardFrictionBase.stiffness = 1f;

        WheelFrictionCurve forwardFrictionBrake = frontLeftWheelCollider.forwardFriction;
        forwardFrictionBrake.stiffness = 6f;

        if (Braking >= 900)
        {

            rearLeftWheelCollider.forwardFriction = forwardFrictionBrake;
            rearRightWheelCollider.forwardFriction = forwardFrictionBrake;
        }
        else
        {

            rearLeftWheelCollider.forwardFriction = forwardFrictionBase;
            rearRightWheelCollider.forwardFriction = forwardFrictionBase;
        }

    }

    public void StopCar()
    {
        SetMotorTorque(0);
        SetBrakeTorque(0);
        SetWheelDampingRate(StandardWheelDampeningRate);

        car.velocity = Vector3.zero;
        car.angularVelocity = Vector3.zero;
    }
}
