using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private int health = 1;
    [SerializeField] public int multiplier;
    private bool isDeath;
    private int curHealth; 
    private TimerObject timer;
    [SerializeField] private float invulnerableTime = 1f;
    private int score;


    private void Awake ( )
    {
        score = 0;
        multiplier = 1;
        anim = GetComponent<Animator>();
        if (!PlayerPrefs.HasKey("Health"))
        {
            PlayerPrefs.SetInt("Health",health);
        }
        curHealth = PlayerPrefs.GetInt("Health"); 
        timer = new TimerObject(this); 
    }
    private void Update ( )
    {
        isDeath = curHealth == 0;
        if ( isDeath)
        {
            SceneController.Instance.ResetScene();
        }
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
        score += (int)Time.unscaledDeltaTime * multiplier; 
        HeartsAndScore.Instance.SetScore(score);
    }
}
