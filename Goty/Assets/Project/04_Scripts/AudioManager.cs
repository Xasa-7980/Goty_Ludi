using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [HideInInspector]
    public AudioSource source;

    public Sound[] sounds; 

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

        foreach (Sound s in sounds)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.clip = s.clip;

            source.volume = s.volume;
            source.pitch = s.pitch;
            source.loop = s.loop; 
        }
    }

    void Start()
    {
        PlayOnce("Theme");
    }

    public static void PlayOnce(AudioClip clip)
    {
        source.PlayOneShot(clip);
        if(s == null)
        {
            return;
        }
    }

   
}
