using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerScript : MonoBehaviour {

    public int spawnIndex;
    public GameObject marker;
    public CustomerType type;
    public string[] names;
    public string customerName;
    public string[] infos;
    public string info;
    public AudioClip[] speechClips;
    public AudioClip[] musicClips;
    public float timeLimit;

    private int typeInt;
    private float speed = 5f;
    private bool dead = false;
    private CarController carController;
    private Rigidbody otherRb;
    private MeshRenderer markerRenderer;
    private AudioManager audioManager;
    private Rigidbody rb;

    private void Awake()
    {
        markerRenderer = marker.GetComponent<MeshRenderer>();
        markerRenderer.material.color = Color.red;
        carController = GameObject.FindWithTag("Player").GetComponent<CarController>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        rb = GetComponent<Rigidbody>();
    }

    public void InitCustomer(CustomerType type, int spawnIndex)
    {
        this.spawnIndex = spawnIndex;
        this.type = type;
        typeInt = (int)type;
        timeLimit = UnityEngine.Random.Range(30, 120);
        info = infos[typeInt];
        customerName = names[typeInt];
    }

    // Collision - small collider
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (collision.collider.GetComponent<Rigidbody>().velocity.magnitude > 15f && !dead && carController.PickUp(this, true)) //if drive too fast
            {
                //customer die
                dead = true;
                rb.constraints = 0;
                transform.GetChild(0).gameObject.SetActive(false);
                Destroy(this.gameObject, 20f);
                Debug.Log("customer is kill :(");
            }
            else if(!dead && carController.PickUp(this, false))
            {
                // customer to car
                audioManager.PlayClip(speechClips[typeInt], AudioClipType.Speech);
                if(musicClips[typeInt] != null)
                {
                    audioManager.PlayClip(musicClips[typeInt], AudioClipType.Background);
                }
                Destroy(this.gameObject);
            }
        }
        else if (collision.collider.CompareTag("AI"))
        {
            //customer die
            dead = true;
            rb.constraints = 0;
            transform.GetChild(0).gameObject.SetActive(false);
            Destroy(this.gameObject, 20f);
        }
    }

    // Trigger enter - Big collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            otherRb = other.GetComponent<Rigidbody>();
        }
    }

    // Trigger stay - Big collider
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (otherRb.velocity.magnitude < 15f && carController.TriggerStay(this))
            {
                transform.position = Vector3.MoveTowards(transform.position, other.transform.position, speed * Time.deltaTime); // walk towards player
                markerRenderer.material.color = Color.blue;
            }
            // add lookat player here
        }
    }

    // Trigger exit - Big collider
    private void OnTriggerExit(Collider other)
    {
       markerRenderer.material.color = Color.red;
    }
}


public enum CustomerType
{
    Fast,
    Drift,
    Poliisileijona,
    Clown
}