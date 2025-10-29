using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start ( )
    {
        SyncWithAudioManager();

        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    private void SyncWithAudioManager ( )
    {
        if (AudioManager.instance != null)
        {
            //musicSlider.value = AudioManager.instance.GetMusicVolume();
            //sfxSlider.value = AudioManager.instance.GetSFXVolume();
        }
    }

    private void OnMusicSliderChanged ( float value )
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicVolume(value);
    }

    private void OnSFXSliderChanged ( float value )
    {
        //if (AudioManager.instance != null)
        //    AudioManager.instance.SetSFXVolume(value);
    }
}