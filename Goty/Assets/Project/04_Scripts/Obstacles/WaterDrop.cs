using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    const float INITIALY = 11;
    const float INITIALZ = 7;

    [SerializeField] private float fallingVelMax;
    [SerializeField] private float fallingVelMin;
    [SerializeField] private float xMax;
    [SerializeField] private float xMin;
    [SerializeField] private float probabilityToBeClean;
    [SerializeField] private Sprite[] sprites;
    private float initialX;
    private float fallingVel;

    void Start()
    {
        if (Random.value < probabilityToBeClean)
            CleanDropSet();
        else
            GetComponent<SpriteRenderer>().sprite = sprites[0];

        initialX = Random.Range(xMax, xMin);
        transform.position = new Vector3(initialX, INITIALY, INITIALZ);
        fallingVel = Random.Range(fallingVelMin, fallingVelMax);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * fallingVel * Time.deltaTime;
    }

    private void CleanDropSet()
    {
        GetComponent<DamageDealer>().enabled = false;
        GetComponent<Items>().enabled = true;
        GetComponent<SpriteRenderer>().sprite = sprites[1];
    }

}
