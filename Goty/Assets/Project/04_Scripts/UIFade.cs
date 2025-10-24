using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public static UIFade Instance { get; private set; }

    private CanvasGroup canvasGroup;
    private CanvasGroup mainMenuGroup;
    private Image fadeImage;

    private void Awake ( )
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvasGroup = GetComponent<CanvasGroup>();
        fadeImage = GetComponentInChildren<Image>();

        // Asegurar que empieza transparente
        canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeOut ( float duration = 0.5f )
    {
        yield return Fade(0f, 1f, duration, canvasGroup);
    }

    public IEnumerator FadeIn ( float duration = 0.5f )
    {
        StartCoroutine(Fade(1f, 0f, duration,mainMenuGroup));
        yield return Fade(1f, 0f, duration,canvasGroup);
    }

    private IEnumerator Fade ( float from, float to, float duration,CanvasGroup group )
    {
        float t = 0f;
        group.blocksRaycasts = true;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
        canvasGroup.blocksRaycasts = (to > 0f);
    }
}