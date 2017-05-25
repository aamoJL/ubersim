using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

    public GameObject minimapMarker;
    public float playerFollowDistance = 30;

    private NavMeshAgent agent;
    private IGameController gameController;
    private Transform player;
    private Transform destination;
    private bool followingPlayer;
    private bool followingCustomer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        gameController = GameObject.FindWithTag("GameController").GetComponent<IGameController>();
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        minimapMarker.SetActive(true);
    }

    private void FixedUpdate()
    {
        if(Vector3.Distance(transform.position,player.position) < playerFollowDistance)
        {
            destination = player;
            followingPlayer = true;
            followingCustomer = false;
        }
        else
        {
            followingPlayer = false;
        }
        if (!followingPlayer && !followingCustomer)
        {
            int random;
            random = Random.Range(0, 2);
            destination = gameController.DestinationObjects[random];
            followingCustomer = true;
        }
        if(destination != null)
        {
            ChangeDestination(destination.position);
        }
        else { followingCustomer = false; }
    }

    private void ChangeDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 direction = collision.transform.position - transform.position;
            collision.gameObject.GetComponent<CarController>().PickUp(this, direction, collision.relativeVelocity.magnitude);
        }
        else if (collision.gameObject.CompareTag("Customer"))
        {
            gameController.SpawnCustomer(collision.gameObject.GetComponent<CustomerScript>().spawnIndex);
            followingCustomer = false;
        }
    }
}
