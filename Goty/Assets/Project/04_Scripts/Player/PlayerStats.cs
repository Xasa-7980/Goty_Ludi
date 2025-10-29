using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int health = 5;
    [SerializeField] private int multiplier = 1;
    [SerializeField] private float multiplierDuration = 5f;
    [SerializeField] private float invulnerableTime = 1f;

    [Header("Runtime")]
    private int currentHealth;
    private float score;
    private float height;
    private float lastHeight = 0;
    private float scoreTime;
    private float multiplierTime;
    private int initialMultiplier;
    private bool multiplierOn;
    private bool isDead => currentHealth <= 0;

    private Animator anim;
    private TimerObject timer;
    private GamePhase fase;
    public UnityEvent onDie;
    private void Awake ( )
    {
        anim = GetComponent<Animator>();
        timer = new TimerObject(this);
        fase = GetComponent<Player>().phase;

        // Cargar o inicializar valores persistentes
        initialMultiplier = multiplier;
        LoadPersistentStats();
    }

    private bool hasDied = false;

    private void Update ( )
    {
        if (isDead && !hasDied)
        {
            hasDied = true;
            StartCoroutine(RestartSceneAfterDelay(0.25f)); 
        }

        // Control de multiplicador
        if (multiplierOn)
        {
            multiplierTime += Time.deltaTime;
            if (multiplierTime >= multiplierDuration)
            {
                multiplier = initialMultiplier;
                multiplierTime = 0;
                multiplierOn = false;
            }
        }

        // Actualizar puntuación
        scoreTime += Time.deltaTime;
        AddScore();
    }
    private IEnumerator RestartSceneAfterDelay ( float delay )
    {
        yield return new WaitForSeconds(delay);

        // restaurar salud antes de recargar
        PlayerPrefs.SetInt("Health", maxHealth);
        PlayerPrefs.Save();

        hasDied = false;
        onDie?.Invoke();
    }
    // ===================== HEALTH =====================

    public void SetHealth ( int value )
    {
        if (timer.Timer_Started()) return;

        timer.StartTimer(invulnerableTime, ( ) =>
        {
            if (value < 0)
                anim.SetTrigger("Damaged");
            else
                anim.SetTrigger("Boost");

            currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
            // Guardar salud persistente
            PlayerPrefs.SetInt("Health", currentHealth);
            PlayerPrefs.Save();

            HeartsAndScore.Instance?.DrawCurrentLifes();

        }, Action_Timing.Start);
    }

    // ===================== MULTIPLIER =====================

    public void OnMultiplier ( )
    {
        if (!multiplierOn)
        {
            multiplier *= 2;
            multiplierOn = true;
            multiplierTime = 0;
        }
        anim.SetTrigger("Boost");
    }

    // ===================== SCORE =====================

    public void AddScore ( )
    {
        if (fase == GamePhase.ASCENSION)
        {
            if ((int)transform.position.y > (int)lastHeight)
            {
                height += multiplier;
                lastHeight = transform.position.y;
            }
        }
        else if (scoreTime > 0.2f)
        {
            score += multiplier;
            scoreTime = 0;
            height = lastHeight;
        }

        // Guardar puntuación persistente
        PlayerPrefs.SetInt("Score", (int)score + (int)height);
        PlayerPrefs.Save();
        HeartsAndScore.Instance?.SetScore((int)score, multiplier > initialMultiplier);

    }

    // ===================== PERSISTENCIA =====================

    private void LoadPersistentStats ( )
    {
        // Si existen, cargar los valores previos
        if (PlayerPrefs.HasKey("Health"))
            if(PlayerPrefs.GetInt("Health") == 0) currentHealth = maxHealth;
            else currentHealth = PlayerPrefs.GetInt("Health");
        else
            currentHealth = maxHealth;

        if (PlayerPrefs.HasKey("Score"))
            score = PlayerPrefs.GetInt("Score");
        else
            score = 0;


        HeartsAndScore.Instance?.DrawCurrentLifes();
        HeartsAndScore.Instance?.SetScore((int)score, multiplier > initialMultiplier);

    }

    public void ResetPersistentStats ( )
    {
        PlayerPrefs.DeleteKey("Health");
        PlayerPrefs.DeleteKey("Score");
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit ( )
    {
        ResetPersistentStats();
    }
}