using UnityEngine;

public class Cloud : MonoBehaviour
{
    const float INITIALX = 8;

    [SerializeField] private int yMax;
    [SerializeField] private int yMin;
    [SerializeField] private float vel;
    [SerializeField] private float timeToAttack;

    private float initialY;
    private TimerObject timer;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        timer = new TimerObject(this);
        initialY = UnityEngine.Random.Range(yMin, yMax);
        transform.position = new Vector2(INITIALX, initialY);
    }

    void Update()
    {
        transform.position += Time.deltaTime * vel * Vector3.left;
        if (!timer.Timer_Started())
        {
            animator.SetBool("Attack", true);
            timer.StartTimer(timeToAttack, () =>
            {
                animator.SetBool("Attack", false);
            }, Action_Timing.End);
        }
    }
}
