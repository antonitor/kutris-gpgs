using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup sfxAudioMixer;
    public AudioMixerGroup musicAudioMixer;
    public static AudioManager instance;
    public Sound[] sounds;
    // Start is called before the first frame update
    void Awake()
    {

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            if (s.isMusic) s.source.outputAudioMixerGroup = musicAudioMixer;
            else s.source.outputAudioMixerGroup = sfxAudioMixer;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.source!=null)
            s.source.Play();
    }

    public void StopSound(string name) 
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.source != null)
            s.source.Stop();
    }



}
