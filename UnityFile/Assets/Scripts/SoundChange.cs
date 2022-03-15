using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundChange : MonoBehaviour
{
    Slider audioSlider;
    AudioMixer audioMixer;
    public string audioStr;
    public GameManager manager;

    public void AudioControl()
    {
        //
        if (audioSlider == null || audioMixer == null) { audioSlider = GetComponent<Slider>(); audioMixer = manager.audioMixer; }

        //
        if (audioSlider.value == -40f) audioMixer.SetFloat(audioStr, -80);
        else audioMixer.SetFloat(audioStr, audioSlider.value);
    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
}
