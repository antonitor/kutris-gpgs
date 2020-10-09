using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public static bool GamePause = false;
    public GameObject pauseMenuUI;
    public GameObject GUI;
    AudioManager audioManager;
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        float musicVolume = PlayerPrefs.GetFloat("musicvolume", 0f);
        float sfxVolume = PlayerPrefs.GetFloat("sfxvolume", 0f);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        GUI.SetActive(false);
        Time.timeScale = 0f;
        GamePause = true;
        audioManager.musicAudioMixer.audioMixer.SetFloat("musicvolume", -80f);
        audioManager.Play("pause_in");
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GUI.SetActive(true);
        Time.timeScale = 1f;
        GamePause = false;
        audioManager.musicAudioMixer.audioMixer.SetFloat("musicvolume", PlayerPrefs.GetFloat("musicvolume", 0f));
        audioManager.Play("pause_out");
    }

    public void GoMenu()
    {
        audioManager.StopSound("theme");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");              
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("musicvolume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("sfxvolume", volume);
        PlayerPrefs.SetFloat("sfxvolume", volume);
    }

    public void PlayBeep()
    {
        audioManager.Play("beep");
    }
}
