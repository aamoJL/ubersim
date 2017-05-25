using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    public AudioMixer mixer;
    public Slider musicVolumeSlider;
    public Slider effectVolumeSlider;

    [Header("Sources")]
    public AudioSource backgroundSource;
    public AudioSource speechSource;
    public AudioSource effectSource;
    
    private void Start()
    {
        float value;
        if(mixer.GetFloat("MusicVol", out value))
        {
            musicVolumeSlider.value = value;
        }
        if(mixer.GetFloat("EffectVol",out value))
        {
            effectVolumeSlider.value = value;
        }
    }

    public void PlayClip(AudioClip clip, AudioClipType clipType)
    {
        AudioSource source = null;
        switch (clipType)
        {
            case AudioClipType.Background:
                source = backgroundSource;
                if (source.clip == clip) { return; }
                break;
            case AudioClipType.Speech:
                source = speechSource;
                break;
            case AudioClipType.Effect:
                source = effectSource;
                break;
            default:
                break;
        }
        source.clip = clip;
        source.Play();
    }

    public void ChangeBackgroundMusicVolume()
    {
        mixer.SetFloat("MusicVol", musicVolumeSlider.value);
    }

    public void ChangeEffectVolume()
    {
        mixer.SetFloat("EffectVol", effectVolumeSlider.value);
    }

    public void paused(bool paused)
    {
        if (paused)
        {
            mixer.FindSnapshot("paused").TransitionTo(0);
        }
        else
        {
            mixer.FindSnapshot("unpaused").TransitionTo(0);
        }
    }
}

public enum AudioClipType
{
    Background,
    Speech,
    Effect
}
