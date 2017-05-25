using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainmenuScript : MonoBehaviour {

    public AudioMixer mixer;
    public Slider musicVolumeSlider;

    public Text namesText;
    public Text scoresText;

    private void Awake()
    {
        mixer.FindSnapshot("unpaused").TransitionTo(0f);
    }

    private void Start()
    {
        scoresText.text = Scores.GetScores();
        namesText.text = Scores.GetNames();
        float value;
        if(mixer.GetFloat("MusicVol", out value))
        {
            musicVolumeSlider.value = value;
        }
    }

	public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeMusicVolume()
    {
        mixer.SetFloat("MusicVol", musicVolumeSlider.value);
    }
}
