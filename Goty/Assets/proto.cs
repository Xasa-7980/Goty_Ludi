using UnityEngine;

public class proto : MonoBehaviour
{
    const int INITIALY = -5;

    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float maxVSpeed;
    [SerializeField] private float impulse;

    private float dir;
    private float verticalSpeed;

    void Start()
    {
        verticalSpeed = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        dir = Input.GetAxis("Horizontal");
        verticalSpeed -= gravity * Time.deltaTime; 

        verticalSpeed = Mathf.Clamp(verticalSpeed,-maxVSpeed,maxVSpeed);
        transform.position += new Vector3(horizontalSpeed * dir * Time.deltaTime, verticalSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8) //Cold air
        {
            verticalSpeed -= impulse * Time.deltaTime;
        }
        if (collision.gameObject.layer == 9) //Hot air
        {
            verticalSpeed += impulse * Time.deltaTime;
        }
    }
}
