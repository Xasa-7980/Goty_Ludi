using UnityEngine;

public class Cloud : MonoBehaviour
{
    const float INITIALZ = 0;
    const float INITIALX = 8;

    [SerializeField] private GameObject[] models;

    [SerializeField] private int yMax;
    [SerializeField] private int yMin;
    [SerializeField] private float vel;
    
    private float initialY;

    void Start()
    {
        GameObject cloud = Instantiate(models[Random.Range(0, models.Length)], transform);
        if (models.Length > 2)
            cloud.transform.localScale /= 20;
        else
            cloud.transform.localScale /= 10;

        initialY = UnityEngine.Random.Range(yMin, yMax);
        transform.position = new Vector3(INITIALX, initialY, INITIALZ);
    }

    void Update()
    {
        transform.position += Time.deltaTime * vel * Vector3.left;
    }
}
