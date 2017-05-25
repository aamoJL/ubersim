using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoliisileijonaScript : MonoBehaviour {

    public GameObject minimapMarker;

    private NavMeshAgent agent;
    private Transform player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        minimapMarker.SetActive(true);
    }

    private void Update()
    {
        agent.SetDestination(player.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 direction = collision.transform.position - transform.position;
        collision.gameObject.GetComponent<CarController>().PickUp(this, direction);
    }
}
