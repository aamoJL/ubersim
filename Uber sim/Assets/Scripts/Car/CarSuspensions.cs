using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSuspensions : MonoBehaviour {

    [Header("Suspensions")]
    public GameObject rayPositions;         // Renkaiden paikat; eturenkaista taaksepäin
    public float RestLength = 0.5f;         // Renkaiden korkeus
    public float suspensionForce = 0.8f;    // Jousitusvoima
    public float springConstant = 40000;    // Jousitusvoima
    public float damperConstant = 2000;     // Jousitusvoima
    [Header("Forces")]
    public float accelerationForce = 50;    // Kuinka paljon auto kallistuu taakse kiihdytyksessä
    public float brakingForce = 50;         // Kuinka paljon auto kallistuu eteen jarrutuksessa
    public float driftForce = 50;           // Kuinka paljon auto kallistuu sivulle kun driftaa
    public float turnForce = 20;            // Kuinka paljon auto kallistuu sivulle kun kääntyy

    //suspension privates
    private float[] previousLengths;
    private float[] currentLengths;
    private bool[] wheelOnGround;
    private RaycastHit wheelRayHit;
    private float springForce;
    private float damperForce;
    private float springVelocity;
    private Transform[] rayPos;
    private Ray wheelRay;
    private Rigidbody rb;
    private bool onGround;

    private void Awake()
    {
        wheelRay = new Ray();
        previousLengths = new float[4];
        currentLengths = new float[4];
        wheelOnGround = new bool[4];
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //get wheelrays
        int wheelCount = rayPositions.transform.childCount;
        rayPos = new Transform[wheelCount];
        for (int i = 0; i < wheelCount; i++)
        {
            rayPos[i] = rayPositions.transform.GetChild(i);
        }
    }

    public void Inputs(float h, float v)
    {
        Suspension();
        AccelerationSuspension(v);
        BrakeSuspension(v);
        TurningSuspension();
    }

    private void Suspension()
    {
        for (int i = 0; i < 4; i++)
        {
            wheelRay.origin = rayPos[i].position;
            wheelRay.direction = rayPos[i].TransformDirection(Vector3.down);
            if (Physics.Raycast(wheelRay, out wheelRayHit, RestLength))
            {
                wheelOnGround[i] = true;
                previousLengths[i] = currentLengths[i];
                currentLengths[i] = RestLength - (wheelRayHit.distance);

                springVelocity = (currentLengths[i] - previousLengths[i]) / Time.fixedDeltaTime;
                springForce = springConstant * currentLengths[i];
                damperForce = damperConstant * springVelocity;

                rb.AddForceAtPosition(wheelRayHit.normal * (springForce + damperForce), rayPos[i].position);
                Debug.DrawLine(wheelRay.origin, wheelRayHit.point, Color.green);
            }
            else
            {
                wheelOnGround[i] = false;
                Debug.DrawLine(rayPos[i].position, rayPos[i].position + rayPos[i].TransformDirection(Vector3.down) * (RestLength), Color.red);
            }
        }
    }

    public bool GroundCheck()
    {
        onGround = false;
        for (int i = 0; i < 4; i++)
        {
            if (wheelOnGround[i]) { onGround = true; }
        }
        return onGround;
    }

    private void AccelerationSuspension(float input)
    {
        if (rb.velocity.magnitude > 0.1f && input > 0) // if moving
        {
            rb.AddForceAtPosition(Vector3.down * accelerationForce * Mathf.Clamp(((150 - rb.velocity.magnitude * 3.6f * 2) / 150), 0, 1), rayPos[2].position, ForceMode.Impulse);
            rb.AddForceAtPosition(Vector3.down * accelerationForce * Mathf.Clamp(((150 - rb.velocity.magnitude * 3.6f * 2) / 150), 0, 1), rayPos[3].position, ForceMode.Impulse);
        }
    }

    private void BrakeSuspension(float input)
    {
        if (input < 0)
        {
            float brakeSuspension = Mathf.Clamp((brakingForce * (rb.velocity.magnitude / 10f)), 0, brakingForce);
            rb.AddForceAtPosition(Vector3.down * brakeSuspension, rayPos[0].position, ForceMode.Impulse);
            rb.AddForceAtPosition(Vector3.down * brakeSuspension, rayPos[1].position, ForceMode.Impulse);
        }
    }

    private void TurningSuspension()
    {
        if(Vector3.Dot(rb.angularVelocity, transform.forward) > 0) //right
        {
            rb.AddForceAtPosition(Vector3.down * turnForce * (rb.angularVelocity.magnitude / 3), rayPos[0].position, ForceMode.Impulse);
            rb.AddForceAtPosition(Vector3.down * turnForce * (rb.angularVelocity.magnitude / 3), rayPos[3].position, ForceMode.Impulse);
        }
        else // left
        {
            rb.AddForceAtPosition(Vector3.down * turnForce * (rb.angularVelocity.magnitude / 3), rayPos[1].position, ForceMode.Impulse);
            rb.AddForceAtPosition(Vector3.down * turnForce * (rb.angularVelocity.magnitude / 3), rayPos[2].position, ForceMode.Impulse);
        }
    }
}
