using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private Animator anim;
    [SerializeField]private int health = 1;
    private bool isDeath;
    private int curHealth; 
    private TimerObject timer;
    [SerializeField] private float invulnerableTime = 1f;

    private void Awake ( )
    {
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
        if (!timer.Timer_Started())
        {
            timer.StartTimer(invulnerableTime, ( ) => { 
            anim.SetTrigger("Damaged"); 
            curHealth = value; }, 
            Action_Timing.Start);
        }
    }
}
