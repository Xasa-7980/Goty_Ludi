using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [HideInInspector]
    public AudioSource[] source;

    public Sound[] soundsMusic; 
    public Sound[] soundsSfx; 

    public static AudioManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        for (int i = 0; i < soundsMusic.Length; i++)
        {
            source[i] = gameObject.AddComponent<AudioSource>();
            soundsMusic[i].source = source[i];
            soundsMusic[i].source.clip = source[i].clip;
            soundsMusic[i].source.volume = source[i].volume;
            soundsMusic[i].source.pitch = source[i].pitch;
            soundsMusic[i].source.loop = source[i].loop;
        }
        for (int i = 0; i < soundsSfx.Length; i++)
        {
            source[i] = gameObject.AddComponent<AudioSource>();
            soundsSfx[i].source = source[i];
            soundsSfx[i].source.clip = source[i].clip;
            soundsSfx[i].source.volume = source[i].volume;
            soundsSfx[i].source.pitch = source[i].pitch;
            soundsSfx[i].source.loop = source[i].loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(soundsMusic, x => x.name == name);
        s.source.PlayOneShot(s.clip);
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(soundsMusic, x => x.name == name);
        s.source.Stop();
    }

    public void SetSfxVolume(float volume)
    {
        for (int i = 0; i < soundsSfx.Length; i++)
        {
            soundsSfx[i].source.volume = volume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        for (int i = 0; i < soundsSfx.Length; i++)
        {
            soundsMusic[i].source.volume = volume;
        }
    }
}
