using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationScript : MonoBehaviour {

    public AudioClip musicClip;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Destination");
        if (collider.CompareTag("Player"))
        {
            CarController carController = collider.GetComponent<CarController>();
            if (carController.PickUp(this))
            {
                audioManager.PlayClip(musicClip, AudioClipType.Background);
                Destroy(this.gameObject);
            }
        }
    }
}
