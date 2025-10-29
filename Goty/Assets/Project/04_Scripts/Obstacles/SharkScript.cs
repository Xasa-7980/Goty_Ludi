using System;
using UnityEngine;

public class SharkScript : MonoBehaviour
{
    const float INITIALZ = 0;
    enum State
    {
        APPEARING,
        WAITING,
        SWIMMING
    }

    private TimerObject timer;
    [SerializeField] private float yMax;
    [SerializeField] private float yMin;
    [SerializeField] private float x_left;
    [SerializeField] private float x_right;
    [SerializeField] private float appearingDistance;
    [SerializeField] private float vel;
    private float appearVel;
    private bool spawnRight;
    private readonly float timeToWait = 2.0f;
    private float initialY;
    private float initialX;
    private Animator animator;

    //public float timeToSpawn; 

    private State state;
    private Vector3 initialPosition;
    private Vector3 stopPosition;

   void Start()
    {
        animator = GetComponent<Animator>();
        appearVel = vel / 5;
        timer = new TimerObject(this);
        SetRandomSpawn();
        transform.position = initialPosition;
        state = State.APPEARING;
    }
    void Update()
    {
        switch (state)
        {
            case State.APPEARING:
                transform.position = Vector3.Lerp(transform.position, stopPosition, appearVel * Time.deltaTime);
                if (Vector3.Distance(transform.position, stopPosition) <= 0.1 && !timer.Timer_Started())
                {
                    state = State.WAITING;
                    timer.StartTimer(timeToWait, () =>
                    {
                        state = State.SWIMMING;
                        animator.SetBool("Attack", true);
                    }, Action_Timing.End);
                }
                break;
            case State.WAITING:
                break;
            case State.SWIMMING:
                if (spawnRight)
                    transform.position -= new Vector3(vel * Time.deltaTime, 0, 0);
                else
                    transform.position += new Vector3(vel * Time.deltaTime, 0, 0);
                break;
            default:
                break;
        }
        if (transform.position.x > 30 || transform.position.x < -30)
            Destroy(gameObject);
    }

    void SetRandomSpawn()
    {
        // Posicion Y aleatoria entre el primer valor y el segundo
        initialY = UnityEngine.Random.Range(yMin, yMax);

        // Posicion X aleatoria entre dos valores
        spawnRight = UnityEngine.Random.value > 0.5f;

        if (spawnRight)
        {
            initialX = x_right;
            initialPosition = new Vector3(initialX, initialY, INITIALZ); 
            stopPosition = initialPosition - new Vector3(appearingDistance, 0, 0);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            initialX = x_left;
            initialPosition = new Vector3(initialX, initialY, INITIALZ); 
            stopPosition = initialPosition + new Vector3(appearingDistance, 0, 0);
        }
    }
}
