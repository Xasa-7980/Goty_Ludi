using UnityEngine;

public class FishScript : MonoBehaviour
{
    enum State
    {
        ENTER,
        CIRCLE,
        OUT,
        SLEEP
    }

    [SerializeField] private float initialY;
    [SerializeField] private float x_left;
    [SerializeField] private float x_right;
    [SerializeField] private float vel;
    [SerializeField] private float rotationSpeed;

    private bool spawnRight;
    private bool looping;
    private short dir;
    private float initialX;

    private Vector3 screenCenter;
    private Vector3 initialPosition;

    State state;

    void Start()
    {
        spawnRight = UnityEngine.Random.value > 0.5f;

        if (spawnRight)
        {
            dir = -1;
            initialX = x_right;
            initialPosition = new Vector3(initialX, initialY, 0);
        }
        else
        {
            dir = 1;
            initialX = x_left;
            initialPosition = new Vector2(initialX, initialY);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        transform.position = initialPosition;
        state = State.ENTER;
        screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0));
        Debug.Log(screenCenter);
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.ENTER:
                if (transform.position.x < 0.1f && spawnRight || !spawnRight && transform.position.x > -0.1f)
                    state = State.CIRCLE;
                transform.position += new Vector3(vel * Time.deltaTime * dir * transform.right.x, 0, 0);
                break;
            case State.CIRCLE:
                Debug.Log("Estoy en circle");
                transform.localPosition += vel * Time.deltaTime * dir * transform.right;
                transform.Rotate(dir * rotationSpeed * Time.deltaTime * Vector3.forward);
                if (looping)
                {
                    if (transform.position.y - initialY < 0.05f)
                        state = State.OUT;
                }
                else if (transform.position.y - initialY > 0.2f)
                    looping = true;

                break;
            case State.OUT:
                transform.localPosition += vel * Time.deltaTime * dir * transform.right;
                break;
            case State.SLEEP:
                break;
            default:
                break;
        }

        if (transform.position.x > initialX + 5 || transform.position.x < -(initialX + 5))
            Destroy(gameObject);
    }
}
