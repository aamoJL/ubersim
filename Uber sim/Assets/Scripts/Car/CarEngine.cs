using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarEngine : MonoBehaviour
{
    [Tooltip("m/s^2")]
    public float acceleration = 20f;        // Kuinka nopeasti auto kiihtyy
    [Tooltip("m/s^2")]
    public float reverseAcceleration = 10f; // Kuinka nopeasti auto kiihtyy peruutuksessa
    [Tooltip("km/h")]
    public float maxSpeed = 150f;           // Maksimi nopeus
    [Tooltip("km/h")]
    public float maxReverseSpeed = 50f;     // Kuinka nopeasti auto voi peruuttaa
    public float brakeForce = 5000;         // S-jarrun voima
    public float rotationTorque = 5;        // Kuinka nopeasti auto kääntyy

    [Header("Tractions")]
    public float rotationTraction = 2500;       // Kuinka paljon vastustaa liukumista

    [Header("Drift")]
    public float driftTraction = 1000;          // Kuinka paljon auto vastustaa liukumista
    public float rotationDriftDrag = 5;         // Kuinka paljon driftatessa kääntyy vastaan  
    public float driftRayDist = 10f;            // Etu- ja perä-viivat pisteytykseen
    public float driftAngle = 20;               // Driftin aloituskulma

    //drift privates
    private RaycastHit driftRayHit;

    private Rigidbody rb;
    private CarHUD carHUD;
    private bool drift;

    public bool Drifting { get { return drift; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        carHUD = GetComponent<CarHUD>();
        carHUD.SetMaxSpeed(maxSpeed);
    }

    private void FixedUpdate()
    {
        DriftCheck();
        RotationTraction();
        carHUD.SetSpeed(rb.velocity.magnitude * 3.6f);
    }

    public void Inputs(float h, float v)
    {
        Accelerate(v);
        Turn(h);
        Brake(v);
    }

    private void RotationTraction()
    {
        if (drift)
        {
            rb.AddForce(-(Vector3.Project(rb.velocity, transform.right)) * driftTraction);
        }
        else
        {
            rb.AddForce(-(Vector3.Project(rb.velocity, transform.right)) * rotationTraction);
        }
    }

    private void DriftCheck()
    {
        if (Vector3.Angle(transform.forward, rb.velocity) > driftAngle)
        {
            rb.AddTorque(rb.angularVelocity.normalized * rotationDriftDrag * Mathf.Clamp((float)Math.Round((double)rb.angularVelocity.magnitude, 1), 0, 1), ForceMode.Acceleration);
            drift = true;
        }
        else
        {
            drift = false;
        }
    }

    private void Accelerate(float input)
    {
        if (input > 0)
        {
            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration); // meters/time^2

            if (rb.velocity.magnitude * 3.6f > maxSpeed) // speed limit
            {
                rb.velocity = rb.velocity.normalized * (maxSpeed / 3.6f);
            }
        }
    }

    private void Reverse()
    {
        rb.AddForce(-transform.forward * reverseAcceleration, ForceMode.Acceleration); // meters/time^2
        if (rb.velocity.magnitude * 3.6f > maxReverseSpeed) // speed limit
        {
            Debug.Log("max reverse speed");
            rb.velocity = rb.velocity.normalized * (maxReverseSpeed / 3.6f);
        }
    }

    private void Brake(float input)
    {
        if (input < 0)
        {
            if (rb.velocity.magnitude < 2f && Vector3.Dot(rb.velocity, -transform.forward) < 0)
            {
                Reverse();
            }
            else if (Vector3.Dot(rb.velocity, transform.forward) < 0)
            {
                Reverse();
            }
            else
            {
                rb.AddForce(brakeForce * -rb.velocity.normalized);
            }
        }
    }

    private void Turn(float input)
    {
        if (input != 0)
        {
            if (Vector3.Dot(rb.velocity, -transform.forward) < 0)
            {
                rb.AddRelativeTorque(Vector3.up * input * rotationTorque, ForceMode.Acceleration);
            }
            else // reverse turning
            {
                rb.AddRelativeTorque(Vector3.up * -input * rotationTorque, ForceMode.Acceleration);
            }
        }
    }
}