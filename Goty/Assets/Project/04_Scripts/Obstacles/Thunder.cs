using UnityEngine;

public class Thunder : MonoBehaviour
{
    const float INITIALY = 6;
    const float INITIALZ = 6;

    TimerObject timer;
    [SerializeField] float warningTime;
    [SerializeField] float strikeTime;
    [SerializeField] float waitTime;
    [SerializeField] float xMax;
    [SerializeField] float xMin;
    [SerializeField] Material warnMaterial;
    [SerializeField] Material strikeMaterial;
    [SerializeField] int parpadeosTotal;

    private float initialX;
    private State state;
    private int parpadeos;
    private MeshRenderer meshRenderer;

    enum State
    {
        WARNING,
        WAIT,
        STRIKE
    }

    void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
        parpadeos = 0;
        state = State.WARNING;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material= warnMaterial;
        timer = new TimerObject(this);
        initialX = UnityEngine.Random.Range(xMin, xMax);
        transform.position = new Vector3(initialX, INITIALY, INITIALZ);
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
                        if (meshRenderer.enabled)
                        {
                            meshRenderer.enabled = false;

                            parpadeos++;

                            if (parpadeos >= parpadeosTotal)
                            {
                                state = State.WAIT;
                            }
                        }
                        else
                        {
                            meshRenderer.enabled = true;
                        }
                    }, Action_Timing.End);
                }
                break;
            case State.WAIT:
                if (!timer.Timer_Started())
                {
                    timer.StartTimer(waitTime, () =>
                    {
                        GetComponent<BoxCollider>().enabled = true;
                        meshRenderer.enabled = true;
                        meshRenderer.material = strikeMaterial; 
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
