using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    private Rigidbody rb;

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

    [Header("Anti Stuck System")]
    public int raysNumber = 36;
    public float raysMaxAngle = 180f;

    private float orgRadius;

    private CarController car;

    public float fDrive;
    public float fBrake;

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

        print(suspensionForce);

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

            rb.AddForceAtPosition((suspensionForce + (fX * transform.forward) + (fY * -transform.right)), hit.point); // we apply the force to the model of the car itself
        }
    }
}
