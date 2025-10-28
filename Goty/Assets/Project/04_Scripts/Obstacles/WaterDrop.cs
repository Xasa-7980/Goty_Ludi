using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    const float INITIALY = 11;
    const float INITIALZ = 0;

    [SerializeField] private float fallingVelMax;
    [SerializeField] private float fallingVelMin;
    [SerializeField] private float xMax;
    [SerializeField] private float xMin;

    private float initialX;
    private float fallingVel;

    void Start()
    {
        initialX = Random.Range(xMax, xMin);
        transform.position = new Vector3(initialX, INITIALY, INITIALZ);
        fallingVel = Random.Range(fallingVelMin, fallingVelMax);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * fallingVel * Time.deltaTime;
    }
}
