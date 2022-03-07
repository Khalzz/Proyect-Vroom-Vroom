using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Wheel[] wheels;

    [Header("Car Specs")]
    public float wheelBase; // in meters
    public float rearTracks; // in meters
    public float turnRadius; // in meters
    public Vector3 dragForceVector;
    public float dragForceMagnitude;
    public float drag;
    public Vector3 velocity;
    public float velocityMagnitude;

    [Header("Inputs")]
    public float steerInput;

    public float ackermannAngleLeft;
    public float ackermannAngleRight;

    void Update()
    {
        velocity = GetComponent<Rigidbody>().velocity;
        dragForceMagnitude = Mathf.Pow(velocity.magnitude, 2) * drag;
        dragForceVector = dragForceMagnitude * -velocity.normalized;

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
            w.dragVector = dragForceVector;
            w.velocity = velocity;
        }
    }
}
