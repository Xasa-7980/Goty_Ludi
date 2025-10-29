using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource sourceSfx;
    public AudioSource sourceMusic;
    public AudioSource sourceEffects;

    public Sound[] sounds;

    public static AudioManager instance;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

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

        LoadVolumes();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayMusic("Background");
                break;
            case "River":
                PlayEffect("River");
                break;
            case "Sea":
                PlayEffect("Sea");
                break;
            case "Ascension":
                PlayEffect("Ascension");
                break;
            case "Sky":
                PlayEffect("Sky");
                break;
            case "SkyFall":
                PlayEffect("Fall");
                break;
            case "GameOver":
                sourceMusic.Stop();
                sourceEffects.Stop();
                PlaySfx("GameOver");
                break;
            default:
                break;
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(sounds, x => x.name == name);
        sourceMusic.clip = s.clip;
        sourceMusic.Play();
    }

    public void PlaySfx(string name)
    {
        Sound s = Array.Find(sounds, x => x.name == name);
        sourceSfx.PlayOneShot(s.clip);
    }

    public void PlayEffect(string name)
    {
        Sound s = Array.Find(sounds, x => x.name == name);
        sourceEffects.clip = s.clip;
        sourceEffects.Play();
    }

    private void LoadVolumes()
    {
        float musicVol = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
    }

    public void SetMusicVolume(float volume)
    {
        if (sourceMusic != null)
            sourceMusic.volume = volume;

        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sourceSfx != null)
            sourceSfx.volume = volume;
        if (sourceEffects != null)
            sourceEffects.volume = volume;
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
    }
}
