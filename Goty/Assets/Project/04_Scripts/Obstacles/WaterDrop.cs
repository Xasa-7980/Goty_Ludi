using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    const float INITIALY = 1;
    const float INITIALZ = 7;

    [SerializeField] private float fallingVelMax;
    [SerializeField] private float fallingVelMin;
    [SerializeField] private float xMax;
    [SerializeField] private float xMin;
    [SerializeField] private float probabilityToBeClean;
    private float initialX;
    private float fallingVel;

    void Start()
    {
        GetComponent<DamageDealer>().enabled = Random.value < probabilityToBeClean;
        initialX = Random.Range(xMax, xMin);
        transform.position = new Vector3(initialX, INITIALY, INITIALZ);
        fallingVel = Random.Range(fallingVelMin, fallingVelMax);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * fallingVel * Time.deltaTime;
    }
}
