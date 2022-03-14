using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarController : MonoBehaviour
{
    public Quaternion slipAngle;

    public Wheel[] wheels;
    public TextMeshProUGUI rpmText;
    public TextMeshProUGUI gearText;
    public TextMeshProUGUI speedText;

    [Header("Car Specs")]
    public float wheelBase; // in meters
    public float rearTracks; // in meters
    public float turnRadius; // in meters

    public float rpm;
    public float maxRpm;

    public float fDrive;
    public float fBrake;
    public float fDriveJustForce;


    public Vector3 velocity;
    public Vector3 fDrag;
    public float cDrag;
    public float fSpeed;

    public int actualGear = 1;
    public float[] gearsRatio = new float[10];
    public int maxGears;
    public int hp;

    // weight transfer
    public float wF;
    public float wR;

    public float disBetweenAxis;
    public float disRearCg;
    public float disFrontCg;
    public float weight;

    public GameObject frontAxis;
    public GameObject rearAxis;
    public GameObject cG;

    public Vector3 frontAxisPosition;
    public Vector3 rearAxisPosition;
    public Vector3 cgPosition;

    // acceleration
    public float acceleration;
    public float distancemoved = 0;
    public float lastdistancemoved = 0;
    public Vector3 last;

    // traction
    public float fTraction;
    public float wheelRotationRate;
    public float wheelRadius;

    // slip
    Vector3 longForce;


    [Header("Inputs")]
    public float steerInput;

    public float ackermannAngleLeft;
    public float ackermannAngleRight;

    public Rigidbody carRb;

    private void Start()
    {
        carRb = GetComponent<Rigidbody>();
        last = transform.position;
        weight = GetComponent<Rigidbody>().mass;
    }

    void Update()
    {

        // acceleration
        distancemoved = Vector3.Distance(last, transform.position);
        distancemoved *= Time.deltaTime;
        acceleration = distancemoved - lastdistancemoved;
        lastdistancemoved = distancemoved;
        last = transform.position;
        // acceleration

        frontAxisPosition = frontAxis.transform.position;
        rearAxisPosition = rearAxis.transform.position;
        cgPosition = cG.transform.position;

        disBetweenAxis = Vector3.Distance(rearAxisPosition, frontAxisPosition);
        disRearCg = Vector3.Distance(rearAxisPosition, cgPosition);
        disFrontCg = Vector3.Distance(frontAxisPosition, cgPosition);
        
        if (velocity.magnitude == 0f)
        {
            wF = (disRearCg / disBetweenAxis) * weight;
            wR = (disFrontCg / disBetweenAxis) * weight;
        }
        else
        {
            wF = (disRearCg / disBetweenAxis) * weight - (cgPosition.y - disBetweenAxis) * (weight * 9.8f) * acceleration;
            wR = (disFrontCg / disBetweenAxis) * weight + (cgPosition.y - disBetweenAxis) * (weight * 9.8f) * acceleration;
        }

        velocity = GetComponent<Rigidbody>().velocity;
        fSpeed = Mathf.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);
        fDrag.x = -cDrag * velocity.x * fSpeed;
        fDrag.z = -cDrag * velocity.z * fSpeed;

        wheelRotationRate = velocity.magnitude / wheelRadius;
        rpm = ((wheelRotationRate * gearsRatio[actualGear] * (gearsRatio[1] + 0.5f)) * 60) / 6.28318530718f;
        //rpm = (((GetComponent<Rigidbody>().velocity.magnitude * 0.621371f) * gearsRatio[actualGear] * 336) / 0.6f);
        speedText.text = (GetComponent<Rigidbody>().velocity.magnitude * 3.6f).ToString("F0") + "Km/h";

        if (rpm > maxRpm)
        {
            rpm = maxRpm;
            fDrive = 0;

        }
        else
        {
            fDrive = (((Input.GetAxis("Throttle") * (hp / rpm) * 5252) + fDrag.x + ((cDrag * 30)) + gearsRatio[actualGear]) / 0.3f) ;
            fBrake = (Input.GetAxis("Brake") * -(412 * gearsRatio[actualGear]) / 0.3f);
        }

        rpmText.text = rpm.ToString();

        if (actualGear >= 1 && actualGear < maxGears)
        {
            gearText.text = (actualGear).ToString();
        }
        else if (actualGear == 0)
        {
            gearText.text = "N";
        }

        if (Input.GetButtonDown("UpShift"))
        {
            if (actualGear >= 0 && actualGear < maxGears)
            {
                actualGear += 1;
            }
        }

        if (Input.GetButtonDown("DownShift"))
        {
            if (actualGear > 0 && actualGear < maxGears + 1)
            {
                actualGear -= 1;
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            GetComponent<Rigidbody>().AddForce(-transform.up * 100, ForceMode.Impulse);
        }

        steerInput = Input.GetAxis("Steer") * 2;

        if (steerInput > 0)
        {
            // is turning Rignt
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTracks / 2))) * steerInput;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTracks / 2))) * steerInput;
        }
        else if (steerInput < 0)
        {
            // is turning left
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTracks / 2))) * steerInput;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTracks / 2))) * steerInput;
        }
        else
        {
            // its not turning
            ackermannAngleLeft = 0;
            ackermannAngleRight = 0;
        }

        foreach (Wheel w in wheels) // we send info to the front wheels
        {
            if (w.wheelFrontLeft)
                w.steerAngle = ackermannAngleLeft;
            if (w.wheelFrontRight)
                w.steerAngle = ackermannAngleRight;
            if (w.wheelRearLeft)
                w.fDrive = fDrive;
            if (w.wheelRearRight)
                w.fDrive = fDrive;
            w.wheelRotationRate = wheelRotationRate;
            w.wheelBase = wheelBase;
            w.carLongForce = transform.localRotation;
            w.fDrag = fDrag;
            w.fBrake = fBrake;
            w.carRb = carRb;
        }
    }

    private void FixedUpdate()
    {
        if (transform.InverseTransformDirection(velocity).z > 0)
        {
            GetComponent<Rigidbody>().AddForce(-transform.forward * ((1 * 412 * gearsRatio[actualGear]) / 0.3f) + fDrag);
            // We're moving forward
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * ((1 * 412 * gearsRatio[actualGear]) / 0.3f) + fDrag);
            // We're moving backward
        }
    }
}