using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarHUD : MonoBehaviour {

    public Text customerText;
    public Text infoText;
    public Text durabilityText;
    public Text timerText;
    public Text speedText;
    public Text driftScoreText;
    public Slider speedSlider;
    public Slider durabilitySlider;

    public void SetCustomerInfo(string name, string info)
    {
        customerText.text = name;
        infoText.text = info;
    }

    public void SetDurability(float durability)
    {
        durabilityText.text = "Durability: " + durability.ToString();
        durabilitySlider.value = durability;
    }

    public void SetTimerText(float time)
    {
        float seconds = time % 60f;
        float minutes = time / 60;
        timerText.text = System.String.Format("{0:00}:{1:00}", (int)minutes, (int)seconds);
    }

    public void SetTimer(bool status)
    {
        timerText.gameObject.SetActive(status);
    }

    public void SetSpeed(float speed)
    {
        speedText.text = "Speed: " + speed.ToString("F0") + " km/h";
        speedSlider.value = speed;
    }

    internal void SetMaxSpeed(float maxSpeed)
    {
        speedSlider.maxValue = maxSpeed;
    }

    internal void SetMaxDurability(float maxDurability)
    {
        durabilitySlider.maxValue = maxDurability;
    }

    internal void SetDriftText(float driftScore)
    {
        driftScoreText.text = driftScore.ToString("F0");
    }

    internal void SetDriftScore(bool v)
    {
        driftScoreText.gameObject.SetActive(v);
    }
}
