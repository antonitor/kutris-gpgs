using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioManager audioManager;

    public Text highScore;


    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        highScore.text = PlayerPrefs.GetString(CONTRACT.HIGH_SCORE);
    }

    private void Start()
    {
        float musicVolume = PlayerPrefs.GetFloat(CONTRACT.MUSIC_VOLUME, 0f);
        float sfxVolume = PlayerPrefs.GetFloat(CONTRACT.SFX_VOLUME, 0f);

        audioMixer.SetFloat(CONTRACT.MUSIC_VOLUME, musicVolume);
        musicSlider.value = musicVolume;
        audioMixer.SetFloat(CONTRACT.SFX_VOLUME, sfxVolume);
        sfxSlider.value = sfxVolume;

        audioManager.StopSound("theme");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(CONTRACT.MUSIC_VOLUME, volume);
        PlayerPrefs.SetFloat(CONTRACT.MUSIC_VOLUME, volume);
    }

    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat(CONTRACT.SFX_VOLUME, volume);
        PlayerPrefs.SetFloat(CONTRACT.SFX_VOLUME, volume);
    }

    
    public void PlayBeep()
    {
        Debug.Log(audioManager);
        audioManager.Play("beep");
    }
 }
