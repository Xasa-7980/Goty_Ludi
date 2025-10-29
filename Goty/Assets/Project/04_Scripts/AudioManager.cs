using UnityEngine.Audio;
using System;
using UnityEngine;
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

    void Awake ( )
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumes();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable ( )
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable ( )
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded ( Scene scene, LoadSceneMode mode )
    {
        Debug.Log($"[AudioManager] Escena cargada: {scene.name}");

        sourceMusic.Stop();
        sourceEffects.Stop();

        switch (scene.name)
        {
            case "MainMenu":
                PlayMusic("Background");
                break;

            case "River":
                PlayMusic("Background"); 
                PlayEffect("River");
                break;

            case "Sea":
                PlayMusic("Background"); 
                PlayEffect("Sea");
                break;

            case "Ascension":
                PlayMusic("Background");
                PlayEffect("Ascension");
                break;

            case "Sky":
                PlayMusic("Background");
                PlayEffect("Sky");
                break;

            case "SkyFall":
                PlayMusic("Background");
                PlayEffect("Fall");
                break;

            case "GameOver":
                PlaySfx("GameOver");
                break;

            default:
                break;
        }
    }
    public void PlayMusic ( string name )
    {
        if (sourceMusic == null)
        {
            Debug.LogError("[AudioManager] ❌ sourceMusic no está asignado en el Inspector!");
            return;
        }

        Sound s = Array.Find(sounds, x => x.name == name);
        if (s == null || s.clip == null)
        {
            Debug.LogError($"[AudioManager] ❌ No se encontró el sonido '{name}' o no tiene clip asignado.");
            return;
        }

        Debug.Log($"[AudioManager] ▶️ Reproduciendo música: {name}");

        sourceMusic.Stop();
        sourceMusic.clip = s.clip;
        sourceMusic.time = 0f;
        sourceMusic.loop = true;
        sourceMusic.Play();
    }

    public void PlaySfx ( string name )
    {
        if (sourceSfx == null)
        {
            Debug.LogError("[AudioManager] ❌ sourceSfx no está asignado en el Inspector!");
            return;
        }

        Sound s = Array.Find(sounds, x => x.name == name);
        if (s == null || s.clip == null)
        {
            Debug.LogWarning($"[AudioManager] SFX '{name}' no encontrado o sin clip.");
            return;
        }

        sourceSfx.PlayOneShot(s.clip);
    }

    public void PlayEffect ( string name )
    {
        if (sourceEffects == null)
        {
            Debug.LogError("[AudioManager] ❌ sourceEffects no está asignado en el Inspector!");
            return;
        }

        Sound s = Array.Find(sounds, x => x.name == name);
        if (s == null || s.clip == null)
        {
            Debug.LogWarning($"[AudioManager] Efecto '{name}' no encontrado o sin clip.");
            return;
        }

        Debug.Log($"[AudioManager] 🌊 Reproduciendo efecto: {name}");

        sourceEffects.Stop();
        sourceEffects.clip = s.clip;
        sourceEffects.time = 0f;
        sourceEffects.loop = true;
        sourceEffects.Play();
    }

    private void LoadVolumes ( )
    {
        float musicVol = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
    }

    public void SetMusicVolume ( float volume )
    {
        if (sourceMusic != null)
            sourceMusic.volume = volume;
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }

    public void SetSFXVolume ( float volume )
    {
        if (sourceSfx != null)
            sourceSfx.volume = volume;
        if (sourceEffects != null)
            sourceEffects.volume = volume;
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }

    public float GetMusicVolume ( ) => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
    public float GetSFXVolume ( ) => PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
}