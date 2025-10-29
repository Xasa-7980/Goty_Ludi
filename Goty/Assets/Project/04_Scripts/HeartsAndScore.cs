using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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

    [SerializeField] private Image controlsImageParent;
    [SerializeField] private GameObject panelPauseMenu;
    [SerializeField] private GameObject scoreLabel;
    [SerializeField] private GameObject healthLabel;
    [SerializeField] private GameObject[] controlsImagePrefab;

    public static bool pause;
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

        SceneManager.sceneLoaded += OnSceneLoaded;

        InitLifes();

        lastHealth = PlayerPrefs.GetInt("Health", maxHearts);
        DrawCurrentLifes();
        if (curControlsImage == null) CreateCurrentSceneControls();

    }
    private void Start ( )
    {
        SceneManager.sceneLoaded += ( sc, md ) =>
        {
            if (sc.name == "MainMenu")
            {
                RestartValues();
            }
        };
    }
    private void OnDisable ( )
    {
        // Desuscribirse para evitar referencias duplicadas
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded ( Scene sc, LoadSceneMode mode )
    {
        if (sc.name == "MainMenu")
        {
            scoreLabel.SetActive(false);
            healthLabel.SetActive(false);  
            RestartValues();
        }
        else
        {
            healthLabel.SetActive(true);
            scoreLabel.SetActive(true);
        }
    }
    public void RestartValues ( )
    {
        PlayerPrefs.SetInt("Health", maxHearts);
        PlayerPrefs.SetInt("Score", 0);
        SetScore(0, false);
        DrawCurrentLifes();
    }
    private void Update ( )
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause = true;
            Time.timeScale = 0f;
            panelPauseMenu?.SetActive(true);
        }
        SetText(PlayerPrefs.GetInt("Score", 0));

        int currentHealth = PlayerPrefs.GetInt("Health", maxHearts);

        // Detectar cambio de vida
        if (currentHealth != lastHealth)
        {
            StartCoroutine(AnimateHealthChange(lastHealth, currentHealth));
            lastHealth = currentHealth;
        }
    }

    public void QuitPause ( )
    {
        Time.timeScale = 1f;
        pause = false;
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

    //public void RestartValues()
    //{
    //    PlayerPrefs.SetInt("Health", 5);
    //    PlayerPrefs.SetInt("Score", 0);
    //    SetScore(PlayerPrefs.GetInt("Score"), false);
    //}

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

    // =================== CONTROLS ===================
    GameObject curControlsImage;
    void CreateCurrentSceneControls ( )
    {
        int curSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        curControlsImage = Instantiate(controlsImagePrefab[curSceneIndex], controlsImageParent.transform);
        curControlsImage.SetActive(false);
    }
    public void ShowCurrentControls ( )
    {
        curControlsImage.SetActive(true);

    }
}