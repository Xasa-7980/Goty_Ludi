using UnityEngine;

public class Thunder : MonoBehaviour
{
    const float INITIALY = 0;

    TimerObject timer;
    [SerializeField] float warningTime;
    [SerializeField] float strikeTime;
    [SerializeField] float waitTime;
    [SerializeField] float xMax;
    [SerializeField] float xMin;
    [SerializeField] int parpadeosTotal;

    private float initialX;
    private State state;
    private int parpadeos;
    private SpriteRenderer sr;

    private Color transparentYellow = new(1,1,0,0.25f);
    private Color yellow = new Color(1, 1, 0, 0.25f);
    enum State
    {
        WARNING,
        WAIT,
        STRIKE
    }

    void Start()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        parpadeos = 0;
        state = State.WARNING;
        sr = GetComponent<SpriteRenderer>();
        sr.color = transparentYellow;
        timer = new TimerObject(this);
        initialX = UnityEngine.Random.Range(xMin, xMax);
        transform.position = new Vector2(initialX, INITIALY);
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.WARNING:
                if (!timer.Timer_Started())
                {
                    timer.StartTimer(warningTime, () =>
                    {
                        if (sr.enabled)
                        {
                            sr.enabled = false;

                            parpadeos++;

                            if (parpadeos >= parpadeosTotal)
                            {
                                state = State.WAIT;
                            }
                        }
                        else
                        {
                            sr.enabled = true;
                        }
                    }, Action_Timing.End);
                }
                break;
            case State.WAIT:
                if (!timer.Timer_Started())
                {
                    timer.StartTimer(waitTime, () =>
                    {
                        GetComponent<BoxCollider2D>().enabled = true;
                        sr.enabled = true;
                        sr.color = yellow;
                        state = State.STRIKE;
                    }, Action_Timing.End);
                }
                break;
            case State.STRIKE: 
                if(!timer.Timer_Started())
                {
                    timer.StartTimer(strikeTime, () =>
                    {
                        Destroy(gameObject);
                    }, Action_Timing.End);
                }
                break;
        }
    }
}
