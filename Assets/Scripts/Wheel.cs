using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Wheel : MonoBehaviour
{
    public TextMeshProUGUI slipRatioText;

    private Rigidbody rb;

    public GameObject wheelAxis;

    public bool wheelFrontLeft;
    public bool wheelFrontRight;
    public bool wheelRearLeft;
    public bool wheelRearRight;

    [Header("Anti Roll Bar")]
    public float antiRollBar = 1;

    [Header("Suspension")]
    public float restLength; // the length of the suspension on rest state
    public float springTravel; // the travel that a spring can make
    public float springStiffness; // how hard is the spring to be pressed or stretched
    public float damperStiffness; // how hard the spring can go to its original position


    private float minLength;
    public float maxLength;
    private float lastLength;
    private float springLength;
    public float springLengthGetter;
    private float springForce;
    private float springVelocity;
    private float damperForce;

    [Header("Wheel")]
    public float steerAngle;
    public float steerTime; 

    private Vector3 suspensionForce; // the force that keeps the car up (thanks to the suspension)
    private Vector3 wheelVelocityLS; // ls stands for local space

    private float fX;
    private float fY;

    public Vector3 fDrag;

    public float wheelAngle;

    public bool isGrounded;
    public RaycastHit wheelHit;
    public RaycastHit wheelPosition;


    [Header("Wheels")]
    public float wheelRadius;

    private bool suspensionOnFloor;
    private bool wheelPoint;

    public LayerMask wheels;
    public Vector3 wheelPositionVector;

    public Quaternion carLongForce;

    [Header("Anti Stuck System")]
    public int raysNumber = 36;
    public float raysMaxAngle = 180f;

    private float orgRadius;

    private CarController car;

    public float fDrive;
    public float fBrake;

    public Rigidbody carRb;

    public float slipAngle;
    public float slipRatio; // change to float

    public float wheelBase;
    private float circleRadius;
    public float wheelRotationRate;

    // longitudinal calculations
    public Vector3 vLong;
    public float wheelAngularVelocity;
    public float engineTorque;

    public float slip;
    public float wheelRpm;
    public float engineRpm;
    public float gearRatio;

    /*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(wheelPositionVector, wheelRadius);
    }
    */

    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();
        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
    }

    void Update()
    {
        AngularVelocityCalculations();
        slipAngle = ((Mathf.Atan(transform.localRotation.y / Mathf.Max(Mathf.Abs(carRb.transform.localRotation.x), 3.0f))) * Mathf.Rad2Deg);
        springLengthGetter = springLength;
        wheelPositionVector = new Vector3(transform.position.x, transform.position.y - springLength, transform.position.z);
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);

        Debug.DrawRay(transform.position, -transform.up * (springLength), Color.green); // spring length

        isGrounded = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius);

        if (Input.GetAxis("Throttle") > 0)
        {
            fX = fDrive;
        }

        if (Input.GetAxis("Brake") > 0)
        {
            fX = fBrake;
        }

        Debug.DrawRay(carRb.transform.position, (Quaternion.AngleAxis(slipAngle, transform.up) * transform.localRotation) * transform.forward * 100, Color.red); // spring length
        Debug.DrawRay(carRb.transform.position, carRb.transform.forward * 100, Color.blue);
        Debug.DrawRay(transform.position, transform.forward * 100, Color.green);
        circleRadius = wheelBase / Mathf.Sin(Vector3.Angle(transform.right, carRb.transform.right));
        if (wheelFrontLeft || wheelFrontRight)
        {
            //print("slip Ratio: " + slipRatio + " wheel angular velocity: " + fY + " slip angle(rads): " + slipAngle + " Angular Speed: " + (fX));
        }
        wheelRpm = engineRpm / gearRatio;
        print(carRb.velocity / wheelRadius);
        //print((transform.forward * carRb.velocity.magnitude).magnitude + " // " + wheelRotationRate / 3.6 + " // " + ((transform.forward*wheelRotationRate) * 3.6f).magnitude) ;
        slipRatioText.text = slipRatio.ToString();

        //print(((wheelRadius * (2 * Mathf.PI))/360)/Time.deltaTime);
    }

    void FixedUpdate()
    {
        suspensionOnFloor = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius, ~wheels);
        wheelPoint = Physics.Raycast(wheelPositionVector, -transform.up, out RaycastHit hitWheel, maxLength, ~wheels);
        wheelPosition = hitWheel;
        //wheelOnFloor = Physics.SphereCast(wheelPoint.transform.position, wheelRadius, -transform.up, out hit);
        if (suspensionOnFloor)
        {
            wheelHit = hit;
            lastLength = springLength;
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);
            springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
            springForce = springStiffness * (restLength - springLength);
            damperForce = damperStiffness * springVelocity;

            suspensionForce = (springForce + damperForce) * transform.up;

            /*
                the GetPointVelocity works on the global space,
                to work with the local one we use inverse transfrom direction 
            */
            wheelVelocityLS = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));

            fY = wheelVelocityLS.x * springForce;

            rb.AddForceAtPosition((suspensionForce + (fX * transform.forward) + ((fY * -transform.right))), hit.point); // we apply the force to the model of the car itself
        }
    }

    float AngularVelocity(float linearVelocity, float wheelRadius)
    {
        return linearVelocity/wheelRadius;
    }

    void AngularVelocityCalculations()
    {
        float torque = 0;
        vLong = wheelVelocityLS;
        torque += engineTorque;
        //print(torque);

        // i need to see how can i get a better value for the "angular mass" on the angular velocity value
        wheelAngularVelocity = torque / (3 * wheelRadius * (fX)) * Mathf.Pow(wheelRadius, 2) / 2 * Time.deltaTime;
        slipRatio = (((wheelAngularVelocity * wheelRadius)/ (vLong.x)));
        //print(slipRatio);
    }
}
