using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCustomerScore : MonoBehaviour {

    public Transform PoliisileijonaPrefab;
    public float driftRayDistance;
    internal bool switchInputs = false;

    private Transform[] rayPositions;
    public GameObject rayPosition;
    private float timeScore;
    private float driftScore;
    private CustomerType customerType;
    private CarHUD carHUD;
    private Rigidbody rb;
    private Ray driftRay;
    private int layerMask = 1 << 9;
    private Transform poliisileijona = null;

    //properties
    private bool customerInCar = false;
    public bool IsFree { get { return !customerInCar; } }

    [Header("Customer in destination")]
    public Transform throwCustomer;
    public float throwForce = 1000f;

    private void Awake()
    {
        carHUD = GetComponent<CarHUD>();
        rb = GetComponent<Rigidbody>();
        rayPositions = new Transform[4];
        for(int i = 0; i < 4; i++)
        {
            rayPositions[i] = rayPosition.transform.GetChild(i);
        }
    }

    private void Start()
    {
        carHUD.SetCustomerInfo("No customers", "Find a customer");
        carHUD.SetTimer(false);
        carHUD.SetDriftScore(false);
    }

    public void CustomerToCar(CustomerScript customer)
    {
        Debug.Log("My name is Jeff");
        customerInCar = true;
        customerType = customer.type;
        carHUD.SetCustomerInfo(customer.customerName, customer.info);
        InitCustomer(customer);
    }

    public void CustomerInDestination()
    {
        customerInCar = false;
        int right = 1;
        if (rb.angularVelocity.y > 0)
        {
            right = -1;
        }
        ThrowCustomer(transform.position + transform.forward * 3 + transform.right * right + transform.up, transform.rotation, right);
        carHUD.SetCustomerInfo("No customers", "Find a customer");
    }

    public void ThrowCustomer(Vector3 position, Quaternion rotation, int right)
    {
        Vector3 tempRotation = rotation.eulerAngles;
        tempRotation.x += 70;
        tempRotation.y += 30 * right;
        Quaternion newRotation = Quaternion.Euler(tempRotation);
        Transform tCustomer = Instantiate(throwCustomer, position, newRotation);
        tCustomer.GetComponent<Rigidbody>().AddForce(tCustomer.transform.up * throwForce);
        Destroy(tCustomer.gameObject, 20f);
    }

    

    private void InitCustomer(CustomerScript customer)
    {
        switch (customer.type)
        {
            case CustomerType.Fast:
                timeScore = customer.timeLimit;
                carHUD.SetTimer(true);
                break;
            case CustomerType.Drift:
                driftScore = 0;
                timeScore = customer.timeLimit;
                carHUD.SetTimer(true);
                carHUD.SetDriftScore(true);
                break;
            case CustomerType.Poliisileijona:
                timeScore = customer.timeLimit;
                carHUD.SetTimer(true);
                StartCoroutine(SpawnPoliisileijona(customer.transform.position));
                break;
            case CustomerType.Clown:
                timeScore = customer.timeLimit;
                carHUD.SetTimer(true);
                switchInputs = true;
                break;
            default:
                break;
        }
    }

    IEnumerator SpawnPoliisileijona(Vector3 position)
    {
        yield return new WaitForSeconds(3);
        poliisileijona = Instantiate(PoliisileijonaPrefab, position, Quaternion.identity);
    }

    internal void UpdateScore(bool drifting)
    {
        switch (customerType)
        {
            case CustomerType.Clown:
            case CustomerType.Fast:
                timeScore -= Time.deltaTime;
                carHUD.SetTimerText(timeScore);
                break;
            case CustomerType.Drift:
                timeScore -= Time.deltaTime;
                carHUD.SetTimerText(timeScore);
                DriftingScore(drifting);
                carHUD.SetDriftText(driftScore);
                break;
            case CustomerType.Poliisileijona:
                timeScore -= Time.deltaTime;
                carHUD.SetTimerText(timeScore);
                break;
            default:
                break;
        }
    }

    private void DriftingScore(bool drifting)
    {
        for (int i = 0; i < 4; i++)
        {
            if (rb.velocity.magnitude > 8.3f)
            {
                if (Physics.Raycast(rayPositions[i].position, rayPositions[i].forward, driftRayDistance, layerMask))
                {
                    driftScore += Time.deltaTime * 3; // drift score 4x if close to wall
                }
                else if (drifting)
                {
                    driftScore += Time.deltaTime;
                }
                Debug.DrawRay(rayPositions[i].position, rayPositions[i].forward * driftRayDistance, Color.red);
            }
        }
    }

    public float GetCustomerScore()
    {
        float score = 0;
        switch (customerType)
        {
            case CustomerType.Fast:
                score = timeScore;
                carHUD.SetTimer(false);
                break;
            case CustomerType.Drift:
                if(timeScore < 0)
                {
                    score += timeScore * 10;
                }
                score += driftScore;
                carHUD.SetTimer(false);
                carHUD.SetDriftScore(false);
                break;
            case CustomerType.Poliisileijona:
                score = timeScore;
                carHUD.SetTimer(false);
                Destroy(poliisileijona.gameObject);
                break;
            case CustomerType.Clown:
                score = timeScore;
                carHUD.SetTimer(false);
                switchInputs = false;
                break;
            default:
                break;
        }
        return score;
    }
}
