using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiStuckSystem : MonoBehaviour
{
    public bool wheelOnFloor;
    public Transform wheelPoint;
    public int raysNumber = 36;
    public float raysMaxAngle = 180f;

    public float wheelWidth = 0.15f;

    private Wheel _wheel;
    private float orgRadius;

    private CarController car;

    void Awake()
    {
        car = GetComponentInParent<CarController>();
        _wheel = GetComponent<Wheel>();
        orgRadius = _wheel.wheelRadius;
    }

    void Update()
    {
        float radiusOffset = 0;

        wheelPoint.position = new Vector3(transform.position.x, transform.position.y - _wheel.springLengthGetter, transform.position.z);

        for (int i = 0; i <= raysNumber; i++)
        {
            Vector3 rayDirection = Quaternion.AngleAxis(_wheel.steerAngle, transform.up) * Quaternion.AngleAxis(i * (raysMaxAngle / raysNumber) + ((180f - raysMaxAngle) / 2), transform.right) * transform.forward;
            if (Physics.Raycast(wheelPoint.transform.position, rayDirection, out RaycastHit hit, _wheel.wheelRadius))
            {
                if (!hit.transform.IsChildOf(car.transform))
                {
                    Debug.DrawLine(wheelPoint.transform.position, hit.point, Color.yellow);
                    radiusOffset = Mathf.Max(radiusOffset, _wheel.wheelRadius - hit.distance);
                }
            }
            Debug.DrawRay(wheelPoint.transform.position + wheelPoint.transform.right * wheelWidth * 0.5f, rayDirection * orgRadius, Color.green);

            if (Physics.Raycast(wheelPoint.transform.position + wheelPoint.transform.right * wheelWidth * 0.5f, rayDirection, out RaycastHit rightHit, _wheel.wheelRadius))
            {
                if (!rightHit.transform.IsChildOf(car.transform))
                {
                    Debug.DrawLine(wheelPoint.transform.position + wheelPoint.transform.right * wheelWidth * 0.5f, rightHit.point, Color.yellow);
                    radiusOffset = Mathf.Max(radiusOffset, _wheel.wheelRadius - rightHit.distance);
                }
            }
            Debug.DrawRay(wheelPoint.transform.position + wheelPoint.transform.right * wheelWidth * 0.5f, rayDirection * orgRadius, Color.green);

            if (Physics.Raycast(wheelPoint.transform.position - wheelPoint.transform.right * wheelWidth * 0.5f, rayDirection, out RaycastHit leftHit, _wheel.wheelRadius))
            {
                if (!leftHit.transform.IsChildOf(car.transform))
                {
                    Debug.DrawLine(wheelPoint.transform.position - wheelPoint.transform.right * wheelWidth * 0.5f, leftHit.point, Color.yellow);
                    radiusOffset = Mathf.Max(radiusOffset, _wheel.wheelRadius - leftHit.distance);
                }
            }
            Debug.DrawRay(wheelPoint.transform.position - wheelPoint.transform.right * wheelWidth * 0.5f, rayDirection * orgRadius, Color.green);

            _wheel.wheelRadius = Mathf.LerpUnclamped(_wheel.wheelRadius, orgRadius + radiusOffset, Time.deltaTime * 2f);
        }
    }
}
