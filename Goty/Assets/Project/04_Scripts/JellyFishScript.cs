using UnityEngine;
using System.Collections;

public class JellyFishScript : MonoBehaviour
{
    //[SerializeField] private float initialX;
    //[SerializeField] private float initialY;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private uint flapTime;
    [SerializeField] private uint flapImpulse;
    [SerializeField] private short direction;
    [SerializeField] private float gravity;
    private Rigidbody rb;
    private bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isDead = false;
        //transform.position = new Vector3(initialX,initialY);
        StartCoroutine(TimeToFlap());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position + new Vector3 (horizontalSpeed * direction * Time.deltaTime, -gravity * Time.deltaTime, 0);
        rb.MovePosition(newPos);

        if (transform.position.y > 5)
            isDead = true;
    }

    IEnumerator TimeToFlap()
    {
        while(!isDead)
        {
            Debug.Log("I'm flapping");
            rb.AddForce(Vector3.up * flapImpulse, ForceMode.Impulse);
            new WaitForSeconds(flapTime);
        }
        yield return null;
    }
}
