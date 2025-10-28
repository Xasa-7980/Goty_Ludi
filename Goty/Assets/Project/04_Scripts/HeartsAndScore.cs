using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class HeartsAndScore : MonoBehaviour
{
    public static HeartsAndScore Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Transform lifeParent;
    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private float fillSpeed = 3f;
    [SerializeField] private int maxHearts = 5;

    private readonly List<Image> lifes = new List<Image>();
    private int lastHealth;

    private void Awake ( )
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (scoreTxt == null)
            scoreTxt = GetComponentInChildren<TextMeshProUGUI>();

        InitLifes();

        lastHealth = PlayerPrefs.GetInt("Health", maxHearts);
        DrawCurrentLifes();
    }

    private void Update ( )
    {
        SetText(PlayerPrefs.GetInt("Score", 0));

        int currentHealth = PlayerPrefs.GetInt("Health", maxHearts);

        // Detectar cambio de vida
        if (currentHealth != lastHealth)
        {
            StartCoroutine(AnimateHealthChange(lastHealth, currentHealth));
            lastHealth = currentHealth;
        }
    }

    // =================== INIT ===================

    private void InitLifes ( )
    {
        foreach (Transform child in lifeParent)
            Destroy(child.gameObject);
        lifes.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heart = Instantiate(imagePrefab, lifeParent).transform.GetChild(1).gameObject;
            Image heartImg = heart.GetComponent<Image>();
            heartImg.type = Image.Type.Filled;
            heartImg.fillMethod = Image.FillMethod.Vertical;
            heartImg.fillOrigin = (int)Image.OriginVertical.Bottom;
            heartImg.fillAmount = 1f; // lleno por defecto
            lifes.Add(heartImg);
        }
    }

    // =================== HEALTH ===================

    public void DrawCurrentLifes ( )
    {
        int currentHealth = PlayerPrefs.GetInt("Health", maxHearts);

        for (int i = 0; i < lifes.Count; i++)
        {
            lifes[i].fillAmount = (i < currentHealth) ? 1f : 0f;
        }
    }

    private IEnumerator AnimateHealthChange ( int oldHealth, int newHealth )
    {
        if (newHealth < oldHealth)
        {
            // Perdiste vida
            for (int i = oldHealth - 1; i >= newHealth; i--)
            {
                yield return StartCoroutine(AnimateHeart(lifes[i], 1f, 0f)); // vaciar
            }
        }
        else if (newHealth > oldHealth)
        {
            // Recuperaste vida
            for (int i = oldHealth; i < newHealth; i++)
            {
                yield return StartCoroutine(AnimateHeart(lifes[i], 0f, 1f)); // llenar
            }
        }
    }

    private IEnumerator AnimateHeart ( Image heart, float from, float to )
    {
        float t = 0f;
        heart.fillAmount = from;
        while (!Mathf.Approximately(heart.fillAmount, to))
        {
            heart.fillAmount = Mathf.MoveTowards(heart.fillAmount, to, Time.deltaTime * fillSpeed);
            t += Time.deltaTime;
            yield return null;
        }
        heart.fillAmount = to;
    }

    // =================== SCORE ===================

    public void SetScore ( int _score, bool multiplier )
    {
        if (multiplier)
        {
            scoreTxt.color = Color.yellow;
            scoreTxt.fontSize = 40;
        }
        else
        {
            scoreTxt.color = Color.white;
            scoreTxt.fontSize = 36;
        }

        SetText(_score);
    }

    private void SetText ( int _score )
    {
        if (scoreTxt != null)
            scoreTxt.text = _score.ToString();
    }
}