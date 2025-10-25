using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private int health = 1;
    private bool isDeath;
    private int curHealth;
    private void Awake ( )
    {
        if (!PlayerPrefs.HasKey("Health"))
        {
            PlayerPrefs.SetInt("Health",health);
        }
        curHealth = PlayerPrefs.GetInt("Health");
    }
    private void Update ( )
    {
        isDeath = curHealth == 0;
        if ( isDeath)
        {
            SceneController.Instance.ResetScene();
        }
    }
    public void SetHealth(int value) => curHealth = value;
}
