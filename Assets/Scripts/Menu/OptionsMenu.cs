using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;


    private void Start()
    {
        float musicVolume = PlayerPrefs.GetFloat("musicvolume", 0f);
        float sfxVolume = PlayerPrefs.GetFloat("sfxvolume", 0f);

        audioMixer.SetFloat("musicvolume", musicVolume);
        musicSlider.value = musicVolume;
        audioMixer.SetFloat("sfxvolume", sfxVolume);
        sfxSlider.value = sfxVolume;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicvolume", volume);
        PlayerPrefs.SetFloat("musicvolume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("sfxvolume", volume);
        PlayerPrefs.SetFloat("sfxvolume", volume);
    }
}
