using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverMovement : MonoBehaviour
{
    public Transform waypointParent;

    [Header("Car Wheels (Wheel Collider)")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider backLeft;
    public WheelCollider backRight;

    [Header("Car Wheels (Transform)")]
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelBL;
    public Transform wheelBR;

    [Header("Car Checks (Transform)")]
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
    public DriverChecks driverChecks;

    private List<Transform> waypoints = new List<Transform>();
    private Rigidbody driver;

    private bool accelerating;

    private float maxKmPH = 60f;

    [Header("Audio")]
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float pitchTransitionSpeed = 2.0f;
    [SerializeField] private AudioSource accelerationAudioSource;
    [SerializeField] private AudioSource idleAudioSource;
    [SerializeField] private AudioSource decelerationAudioSource;

    void Start()
    {
        driver = transform.GetComponent<Rigidbody>();
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints.Add(waypointParent.GetChild(i));
        }

        localMaxRpm = MaxRPM;

        if (GetComponent<Rigidbody>() != null && centerOfMass != null)
        {
            GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        }
    }

    void Update()
    {
        RaycastHit hit;
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
        HandleEngineSound();
    }


    private void ApplyBrakes() 
    {
        frontLeft.brakeTorque = 5000;
        frontRight.brakeTorque = 5000;
        backLeft.brakeTorque = 5000;
        backRight.brakeTorque = 5000;
    }


    private void UpdateWheels() 
    {
        ApplyRotationAndPostion(frontLeft, wheelFL);
        ApplyRotationAndPostion(frontRight, wheelFR);
        ApplyRotationAndPostion(backLeft, wheelBL);
        ApplyRotationAndPostion(backRight, wheelBR);
    }

    private void ApplyRotationAndPostion(WheelCollider targetWheel, Transform wheel) 
    {
        targetWheel.ConfigureVehicleSubsteps(5, 12, 15);

        Vector3 pos;
        Quaternion rot;
        targetWheel.GetWorldPose(out pos, out rot);
        wheel.position = pos;
        wheel.rotation = rot;
    }

    void ApplySteering(Transform nextDest) 
    {
        Vector3 targetPosition = nextDest.position;
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; 

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * steeringSpeed);

        float angle = Vector3.SignedAngle(transform.forward, direction.normalized, Vector3.up);

        angle = Mathf.Clamp(angle, -maxSteerAngle, maxSteerAngle);

        frontLeft.steerAngle = angle;
        frontRight.steerAngle = angle;
    }

    void Movement() 
    {
        int SpeedOfWheels = (int)((frontLeft.rpm + frontRight.rpm + backLeft.rpm + backRight.rpm) / 4);

        if (SpeedOfWheels < localMaxRpm * motorMult)
        {
            backRight.motorTorque = 400 * MovementTorque;
            backLeft.motorTorque = 400 * MovementTorque;
            frontRight.motorTorque = 400 * MovementTorque;
            frontLeft.motorTorque = 400 * MovementTorque;
            accelerating = true;
        }
        else if (SpeedOfWheels < (localMaxRpm + (localMaxRpm * 1 / 4) * motorMult))
        {
            backRight.motorTorque = 0;
            backLeft.motorTorque = 0;
            frontRight.motorTorque = 0;
            frontLeft.motorTorque = 0;
            accelerating = false;
        }
        else
            ApplyBrakes();

        speed = SpeedOfWheels;


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

    private void HandleEngineSound()
    {
        float carVelocityRatio = driver.velocity.magnitude / maxKmPH;
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, carVelocityRatio);

        accelerationAudioSource.pitch = targetPitch;
        decelerationAudioSource.pitch = targetPitch;

        if (accelerating)
        {
            StartCoroutine(TransitionAudio(accelerationAudioSource, true));
            StartCoroutine(TransitionAudio(decelerationAudioSource, false));
            StartCoroutine(TransitionAudio(idleAudioSource, false));
        }
        else if (!accelerating && driver.velocity.magnitude >= 0.1)
        {
            StartCoroutine(TransitionAudio(accelerationAudioSource, false));
            StartCoroutine(TransitionAudio(decelerationAudioSource, true));
            StartCoroutine(TransitionAudio(idleAudioSource, false));
        }
        else if (driver.velocity.magnitude < 0.1)
        {
            StartCoroutine(TransitionAudio(accelerationAudioSource, false));
            StartCoroutine(TransitionAudio(decelerationAudioSource, false));
            StartCoroutine(TransitionAudio(idleAudioSource, true));
        }
    }
}
