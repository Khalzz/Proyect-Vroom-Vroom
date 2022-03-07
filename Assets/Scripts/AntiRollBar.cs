using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    public float antiRollBar;

    [SerializeField] Wheel leftWheel;
    public bool lIsGrounded;

    [SerializeField] Wheel rightWheel;
    public bool rIsGrounded;

    public float antiRollForce;

    private Rigidbody car;

    void Start()
    {
        car = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        RaycastHit leftHit;
        RaycastHit rightHit;

        float travelL = 1.0f;
        float travelR = 1.0f;

        
        leftHit = leftWheel.wheelHit;
        if (leftWheel.isGrounded)
            travelL = (-leftWheel.transform.InverseTransformPoint(leftHit.point).y - leftWheel.wheelRadius / leftWheel.maxLength);
        rightHit = rightWheel.wheelHit;
        if (rightWheel.isGrounded)
            travelR = (-rightWheel.transform.InverseTransformPoint(rightHit.point).y - rightWheel.wheelRadius / rightWheel.maxLength);
        
        antiRollForce = (travelL - travelR) * antiRollBar;

        if (leftWheel.isGrounded)
            car.AddForceAtPosition(leftWheel.transform.up * -antiRollForce, leftWheel.transform.position);
        if (rightWheel.isGrounded)
            car.AddForceAtPosition(rightWheel.transform.up * antiRollForce, rightWheel.transform.position);
    }


}
