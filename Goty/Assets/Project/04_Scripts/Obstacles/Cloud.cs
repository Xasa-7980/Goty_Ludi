using UnityEngine;

public class Cloud : MonoBehaviour
{
    const float INITIALX = 8;

   // [SerializeField] private Sprite[] sprites;

    [SerializeField] private int yMax;
    [SerializeField] private int yMin;
    [SerializeField] private float vel;

    private SpriteRenderer sr;
    private float initialY;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        initialY = UnityEngine.Random.Range(yMin, yMax);
        transform.position = new Vector2(INITIALX, initialY);
    }

    void Update()
    {
        transform.position += Time.deltaTime * vel * Vector3.left;
    }
}
