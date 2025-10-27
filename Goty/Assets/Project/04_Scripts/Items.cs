using UnityEngine;

public class Items : MonoBehaviour
{
    private TimerObject timer;
    [SerializeField] private bool isMultiplier;
    [SerializeField] private int multiplierTime;

    void Start()
    {
        timer = new TimerObject(this);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 2)
        {
            if (!isMultiplier)
                collision.gameObject.GetComponent<PlayerStats>().SetHealth(1);
            else
            {
                if (!timer.Timer_Started())
                {
                    collision.gameObject.GetComponent<PlayerStats>().multiplier = 2;
                    timer.StartTimer(multiplierTime, () =>
                    {
                        collision.gameObject.GetComponent<PlayerStats>().multiplier = 1;
                    }, Action_Timing.End);
                }
            }
            Destroy(gameObject);
        }
    }
}
