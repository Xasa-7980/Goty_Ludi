using UnityEngine;

public class Waves : MonoBehaviour
{
    const int INITIAL_X = -36;
    const int INITIAL_Y = 10;
    const int INITIAL_Z = 8;
    const int END_X = 32;
    [SerializeField] private float vel;

    private Animator animator;
    private Vector3 initalPos;
    private TimerObject timer;
    private float timeToWave;

    void Start()
    {   
        timer = new TimerObject(this);
        animator = GetComponent<Animator>();
        initalPos = new Vector3(INITIAL_X, INITIAL_Y, INITIAL_Z);
        transform.position = initalPos;
        timeToWave = Random.Range(1.5f, 3f);
    }

    void Update()
    {
        transform.position += Time.deltaTime * vel * Vector3.right;
        if (transform.position.x > END_X)
            transform.position = initalPos;
        if (!timer.Timer_Started())
        {
            timer.StartTimer(timeToWave, () =>
            {
                animator.SetTrigger("Wave");
                timeToWave = Random.Range(1.5f, 3f);
            }, Action_Timing.End);
        }

    }
}
