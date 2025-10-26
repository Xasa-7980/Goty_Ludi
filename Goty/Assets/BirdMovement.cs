using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    [SerializeField] private float yOffset; 
    [SerializeField] private float vel;

    private int dir;
    private Vector3 lowestPos;
    // Update is called once per frame
    private void Start()
    {
        lowestPos = transform.position - new Vector3(0, - yOffset / 2, 0); 
    }
    void Update()
    {
        if (transform.position.y > lowestPos.y + yOffset)
            dir = -1;
        else if (transform.position.y < lowestPos.y)
            dir = 1;

        transform.position += new Vector3(0, vel * dir * Time.deltaTime, 0);
    }
}
