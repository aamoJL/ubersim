using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairScript : MonoBehaviour {

    public float amount;
    public AudioClip repairClip;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController carController = other.GetComponent<CarController>();
            if (carController.PickUp(this))
            {
                audioManager.PlayClip(repairClip, AudioClipType.Effect);
                Destroy(this.gameObject);
            }
        }
    }
}
