using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private int health = 5;
    [SerializeField] public int multiplier;
    private bool isDeath { get { return curHealth == 0; } }
    private int curHealth; 
    private TimerObject timer;
    private float height;
    private float lastHeight;
    [SerializeField] private float invulnerableTime = 1f;
    private float score;
    private float scoreTime;
    GamePhase fase;


    private void Awake ( )
    {
        scoreTime = 0;
        height = 0;
        score = 0;
        multiplier = 1;
        anim = GetComponent<Animator>();
        if (!PlayerPrefs.HasKey("Health"))
        {
            PlayerPrefs.SetInt("Health",health);
        }
        curHealth = PlayerPrefs.GetInt("Health"); 
        timer = new TimerObject(this); 
        fase = GetComponent<Player>().phase;
    }
    private void Update ( )
    {
        if ( isDeath)
        {
            SceneController.Instance.ResetScene();
        }
        scoreTime += Time.deltaTime;
        AddScore();
    }
    public void SetHealth ( int value )
    {
        Debug.Log("a");
        if (!timer.Timer_Started())
        {
            timer.StartTimer(invulnerableTime, ( ) => { 
            anim.SetTrigger("Damaged"); 
            curHealth += value; 
            Debug.Log("b");
            HeartsAndScore.Instance.DrawCurrentLifes(curHealth);
            }, Action_Timing.Start);
        }
    }

    public void AddScore()
    {
        print(score);
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
        print(score);
        HeartsAndScore.Instance.SetScore((int)score);
    }
}
