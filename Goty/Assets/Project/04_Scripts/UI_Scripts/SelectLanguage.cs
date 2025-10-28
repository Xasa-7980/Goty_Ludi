using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SelectLanguage : MonoBehaviour
{
    private bool isChanging;
    private int currentIndex;
    private string[] localeCodes = new string[3] { "ca", "en", "es" };
    public void ChangeLanguage_ToNext()
    {
        currentIndex++;
        currentIndex = Mathf.Clamp(currentIndex, 0, 2);
        if (isChanging) return; 
        StartCoroutine(SetLocale(localeCodes[currentIndex]));
    }
    public void ChangeLanguage_ToPrev()
    {
        currentIndex--;
        currentIndex = Mathf.Clamp(currentIndex, 0, 2);
        if (isChanging) return;

        StartCoroutine(SetLocale(localeCodes[currentIndex]));
    }
    private IEnumerator SetLocale(string localeCode )
    {
        isChanging = true;

        yield return LocalizationSettings.InitializationOperation;
        var selectedLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (selectedLocale != null)
        {
            LocalizationSettings.SelectedLocale = selectedLocale;
            Debug.Log($"Idioma cambiado a: {selectedLocale.Identifier.Code}");
        }
        else
        {
            Debug.LogWarning($"No se encontró el idioma: {localeCode}");
        }

        isChanging = false;
    }




}
