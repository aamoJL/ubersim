using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // publics
    public float durability = 100;
    public float aiCollisionDamageBig = 20;
    public float aiCollisionDamageSmall = 5;
    public float collisionImpulseBig;
    public float collisionImpulseSmall;

    // properties
    private float Durability { get { return durability; } set { if (value < 0) { durability = 0; } else if (value > maxDurability) { durability = maxDurability; } else durability = value; } }
    private bool IsDead { get { if (durability <= 0) { return true; } else return false; } }
    private bool IsDamaged { get { if (durability < maxDurability) { return true; } else return false; } }

    // game components
    private IGameController gameController;

    // Car components
    private CarEngine carEngine;
    private CarNavigator carNavigator;
    private CarSuspensions carSuspensions;
    private CarHUD carHUD;
    private CarCustomerScore carCustomerScore;

    //privates
    private float h, v; // inputs
    private Rigidbody rb;
    private float maxDurability;
    private float velocity;
    private bool disabled = false;

    private void Awake()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<IGameController>();
        carEngine = GetComponent<CarEngine>();
        carNavigator = GetComponent<CarNavigator>();
        carHUD = GetComponent<CarHUD>();
        carCustomerScore = GetComponent<CarCustomerScore>();
        carSuspensions = GetComponent<CarSuspensions>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        carNavigator.SetDestinations(gameController.DestinationObjects);
        maxDurability = durability;
        carHUD.SetMaxDurability(durability);
        carHUD.SetDurability(Durability);
        disabled = false;
    }

    private void Update()
    {
        velocity = rb.velocity.magnitude;
        if (!disabled)
        {
            if (carCustomerScore.switchInputs) // clown inputs
            {
                h = -Input.GetAxisRaw("Vertical");
                v = Input.GetAxisRaw("Horizontal");
            }
            else
            {
                h = Input.GetAxisRaw("Horizontal");
                v = Input.GetAxisRaw("Vertical");
            }
        }
        else
        {
            h = 0; v = 0;
        }

        // if dead
        if (IsDead)
        {
            disabled = true;
            gameController.EndGame();
        }
        else if(gameController.GameTimeLimit <= 0)
        {
            disabled = true;
        }

        // id customer in car
        if (!carCustomerScore.IsFree)
        {
            carCustomerScore.UpdateScore(carEngine.Drifting);
        }
    }

    private void LateUpdate()
    {
        //navigator
        if (carCustomerScore.IsFree)
        {
            carNavigator.UpdateCustomerNavigation();
        }
        else if (!carCustomerScore.IsFree)
        {
            carNavigator.UpdateDestinationNavigation();
        }
    }

    private void FixedUpdate()
    {

        carSuspensions.Inputs(h, v);
        carEngine.Inputs(h, v);
    }

    public bool PickUp(CustomerScript customer, bool tooFast)
    {
        if (tooFast) // customer die
        {
            gameController.AddPoints(-gameController.KillPoints);
            gameController.SpawnCustomer(customer.spawnIndex);
            Debug.Log(customer.spawnIndex);
            TakeDamage(5f); //debug damage
            return true;
        }
        else
        {
            if (carCustomerScore.IsFree) // customer to the car
            {
                carCustomerScore.CustomerToCar(customer);
                gameController.spawnDestination();
                gameController.SpawnCustomer(customer.spawnIndex);
                return true;
            }
        }
        return false;
    }

    public bool PickUp(RepairScript repair)
    {
        if (IsDamaged)
        {
            Durability += repair.amount;
            carHUD.SetDurability(Durability);
            return true;
        }
        return false;
    }

    public bool PickUp(DestinationScript destination)
    {
        if (!carCustomerScore.IsFree)
        {
            gameController.AddPoints(carCustomerScore.GetCustomerScore());
            carCustomerScore.CustomerInDestination();
            return true;
        }
        return false;
    }

    internal void PickUp(AIController aIController, Vector3 forceDirection, float collisionVelocity)
    {
        if(collisionVelocity > 6)
        {
            TakeDamage(aiCollisionDamageBig);
            rb.AddForce(forceDirection * collisionImpulseBig, ForceMode.VelocityChange);
        }
        else
        {
            TakeDamage(aiCollisionDamageSmall);
            rb.AddForce(forceDirection * collisionImpulseSmall, ForceMode.VelocityChange);
        }
    }

    internal void PickUp(PoliisileijonaScript poliisileijonaScript, Vector3 forceDirection)
    {
        TakeDamage(aiCollisionDamageBig);
        rb.AddForce(forceDirection * collisionImpulseBig, ForceMode.VelocityChange);
        gameController.AddPoints(-50);
    }

    internal bool TriggerStay(CustomerScript customerScript)
    {
        if (carCustomerScore.IsFree) { return true; }
        return false;
    }

    public void TakeDamage(float damage)
    {
        Durability -= damage;
        carHUD.SetDurability(Durability);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9)
        {
            if (velocity < 15f) // under 20km/h
            {
                return;
            }
            else if (velocity < 30f) // under 60km/h
            {
                TakeDamage(10);
            }
            else // over 60km/h
            {
                TakeDamage(30);
            }
        }
    }
}

    
