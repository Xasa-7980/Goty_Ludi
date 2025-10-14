using System;
using UnityEngine;

public class SharkScript : MonoBehaviour
{
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
    private readonly float timeToStop = 1.0f;
    private readonly float timeToWait = 2.0f;
    private float initialY;
    private float initialX;
    private readonly float initialZ = 7f;


    private State state;
    private Vector3 initialPosition;
    private Vector3 stopPosition;

   void Start()
    {
        appearVel = vel / 10;
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
    }

    void SetRandomSpawn()
    {
        // Posicion Y aleatoria entre el primer valor y el segundo
        initialY = UnityEngine.Random.Range(yMin, yMax);

        // Posicion X aleatoria entre dos valores
        spawnRight = (int)UnityEngine.Random.Range(0, 2) > 0 ? true : false;

        if (spawnRight)
        {
            initialX = x_right;
            initialPosition = new Vector3(initialX, initialY, initialZ); 
            stopPosition = initialPosition - new Vector3(appearingDistance, 0, 0);
        }
        else
        {
            initialX = x_left;
            initialPosition = new Vector3(initialX, initialY, initialZ); 
            stopPosition = initialPosition + new Vector3(appearingDistance, 0, 0);
        }
    }
}
