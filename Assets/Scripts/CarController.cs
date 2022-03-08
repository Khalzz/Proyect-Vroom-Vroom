using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarController : MonoBehaviour
{
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

    public Vector3 velocity;
    public Vector3 fDrag;
    public float cDrag;
    public float fSpeed;

    public int actualGear = 1;
    public float[] gearsRatio = new float[10];

    [Header("Inputs")]
    public float steerInput;

    public float ackermannAngleLeft;
    public float ackermannAngleRight;

    void Update()
    {
        velocity = GetComponent<Rigidbody>().velocity;
        fSpeed = Mathf.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);
        fDrag.x = -cDrag * velocity.x * fSpeed;
        fDrag.z = -cDrag * velocity.z * fSpeed;



        rpm = (((GetComponent<Rigidbody>().velocity.magnitude * 0.621371f) * gearsRatio[actualGear] * 336) / 0.6f);
        speedText.text = (GetComponent<Rigidbody>().velocity.magnitude * 3.6f).ToString("F0") + "Km/h";

        if (rpm > maxRpm)
        {
            rpm = maxRpm;
            fDrive = 0;

        }
        else
        {
            fDrive = (Input.GetAxis("Throttle") * (412 * gearsRatio[actualGear]) / 0.3f);
            fBrake = (Input.GetAxis("Brake") * -(412 * gearsRatio[actualGear]) / 0.3f);
        }

        rpmText.text = rpm.ToString();

        if (actualGear >= 1 && actualGear < 4)
        {
            gearText.text = (actualGear).ToString();
        }
        else if (actualGear == 0)
        {
            gearText.text = "N";
        }



        if (Input.GetButtonDown("UpShift"))
        {
            if (actualGear >= 0 && actualGear < 4)
            {
                actualGear += 1;
                print(actualGear);
            }
        }

        if (Input.GetButtonDown("DownShift"))
        {
            if (actualGear > 0 && actualGear < 5)
            {
                actualGear -= 1;
                print(actualGear);
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            print("Restart");
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
            w.fDrive = fDrive;
            w.fDrag = fDrag;
            w.fBrake = fBrake;
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
