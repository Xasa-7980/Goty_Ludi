using UnityEngine;
using System.Collections;

public class JellyFishScript : MonoBehaviour
{
    const int INITIALY = -7;

    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float flapTime;
    [SerializeField] private float flapImpulse;
    [SerializeField] private float gravity;
    [SerializeField] private float xMax;
    [SerializeField] private float xMin;
    private float initialX;
    private short dir;
    private bool goingRight;
    private float verticalSpeed;
    private Animator animator;

    private Vector3 initialPosition;

    private TimerObject timer;

    void Start()
    {
        verticalSpeed = 0f;
        timer = new TimerObject(this);
        animator = GetComponent<Animator>();

        SetRandomSpawn();
        transform.position = initialPosition;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3 (horizontalSpeed * dir * Time.deltaTime, verticalSpeed * Time.deltaTime, 0);
        verticalSpeed -= gravity * Time.deltaTime;


        if (!timer.Timer_Started())
        {
            timer.StartTimer(flapTime, () =>
            {
                verticalSpeed = flapImpulse;
                animator.SetBool("Impulse", true);
            }, Action_Timing.Start);
        }

        if (verticalSpeed <= 0)
            animator.SetBool("Impulse", false);

        if (transform.position.x > 12 || transform.position.x < -12)
           gameObject.SetActive(false);
    }
    void SetRandomSpawn()
    {
        initialX = UnityEngine.Random.Range(xMin, xMax);

        float directionProbability = initialX > 0 ? 0.8f : 0.2f;
       
        goingRight = Random.value > directionProbability;

        if (goingRight)
            dir = 1;
        else
            dir = -1;

        initialPosition = new Vector2(initialX, INITIALY);
    }
}
