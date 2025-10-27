using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private int health = 5;
    [SerializeField] public int multiplier;
    [SerializeField] float multiplierDuration;
    [SerializeField] public bool multiplierOn;
    [SerializeField] private float invulnerableTime = 1f;

    private bool isDeath { get { return curHealth == 0; } }
    private int curHealth;
    private TimerObject timer;
    private float height;
    private float lastHeight;
    private float score;
    private float scoreTime;
    private float multiplierTime;
    private int initialMultiplier;
    GamePhase fase;


    private void Awake ( )
    {
        initialMultiplier = multiplier;
        scoreTime = 0;
        multiplierTime = 0;
        height = 0;
        score = 0;
        anim = GetComponent<Animator>();
        if (!PlayerPrefs.HasKey("Health"))
        {
            PlayerPrefs.SetInt("Health",health);
        }
        curHealth = PlayerPrefs.GetInt("Health"); 
        timer = new TimerObject(this); 
        fase = GetComponent<Player>().phase;
    }
    HeartsAndScore hearts;
    private void Start ( )
    {
    }
    private void Update ( )
    {
        if ( isDeath)
        {
            SceneController.Instance.ResetScene();
        }
        scoreTime += Time.deltaTime;

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
        AddScore();
    }
    public void SetHealth ( int value )
    {
        if (!timer.Timer_Started())
        {
            timer.StartTimer(invulnerableTime, ( ) => {

                if (value < 0)
                    anim.SetTrigger("Damaged");
                else
                    anim.SetTrigger("Boost");

                curHealth += value; 
                curHealth = Mathf.Clamp(curHealth, 0, 5);
                
                PlayerPrefs.SetInt("Health",curHealth);
                HeartsAndScore.Instance.DrawCurrentLifes();

            }, Action_Timing.Start);
        }
    }

    public void OnMultiplier()
    {
        if (!multiplierOn)
        {
            multiplier *= 2;
            multiplierOn = true;
        }
        anim.SetTrigger("Boost");
        multiplierTime = 0;
    }

    public void AddScore()
    {
        if (fase == GamePhase.ASCENSION)
        {
            if ((int)transform.position.y > (int)lastHeight)
            {
                height += multiplier;
                lastHeight = transform.position.y;
            }
            score = (int)height;
        }
        else if (scoreTime > 0.2f)
        {
            score += multiplier;
            scoreTime = 0;
        }
        PlayerPrefs.SetInt("Score", (int)score);
        HeartsAndScore.Instance.SetScore((int)score, multiplier > initialMultiplier);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
}
