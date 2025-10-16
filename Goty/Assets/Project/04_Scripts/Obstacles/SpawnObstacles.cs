using UnityEngine;

public class SpawnObstacles : MonoBehaviour
{
    enum SpawnMode
    {
        None,
        Square,
        Circle,
        Sphere,
        Box
    }
    [SerializeField] private float radius;
    [SerializeField] private Vector3 size = Vector3.one;
    [SerializeField] private float spawnTime = 3;
    [SerializeField] private GameObject[] prefabsToSpawn;
    TimerObject spawnTimer;
    [SerializeField] private SpawnMode spawnMode;
    [SerializeField] private Player player;
    [SerializeField] private float maxDistance = 30;
    [SerializeField] private float minDistance = 15;
    private void Start ( )
    {
        spawnTimer = new TimerObject(this);
        player = GameObject.FindAnyObjectByType<Player>();
    }
    private void Update ( )
    {
        float distanceBetweenPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceBetweenPlayer < maxDistance && distanceBetweenPlayer > minDistance)
        {
            switch (spawnMode)
            {
                case SpawnMode.None:
                    break;
                case SpawnMode.Square:

                    if (!spawnTimer.Timer_Started())
                    {
                        spawnTimer.StartTimer(spawnTime, ( ) =>
                        {
                            SpawnInsideSquare();
                        }, Action_Timing.End);
                    }
                    break;
                case SpawnMode.Circle:
                    if (!spawnTimer.Timer_Started())
                    {
                        spawnTimer.StartTimer(spawnTime, ( ) =>
                        {
                            SpawnInsideCircle();
                        }, Action_Timing.End);
                    }
                    break;
                case SpawnMode.Sphere:
                    break;
                case SpawnMode.Box:
                    break;
                default:
                    break;
            }
        }
    }
    void SpawnInsideCircle ( )
    {
        Vector2 randPosition = Random.insideUnitCircle * radius;
        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex], new Vector3(transform.position.x + randPosition.x, transform.position.y, transform.position.z + randPosition.y), Quaternion.identity);

    }
    void SpawnInsideSquare ( )
    {
        var a = GetComponent<Collider2D>().bounds;
        float randomX = Random.Range(a.min.x, a.max.x);
        float randomY = Random.Range(a.min.y, a.max.y);
        Vector3 pointInsideSquare = new Vector3(randomX, randomY, transform.position.z);
        int randIndex = Random.Range( 0, prefabsToSpawn.Length );
        Instantiate(prefabsToSpawn[randIndex], pointInsideSquare, Quaternion.identity);

    }
    void SpawnInsideSphere ( )
    {
        Vector3 randPosition = Random.insideUnitSphere * radius;
        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex], new Vector3(transform.position.x + randPosition.x, transform.position.y + randPosition.y, transform.position.z + randPosition.z), Quaternion.identity);
    }
    void SpawnInsideBox ( )
    {

    }
    private void OnDrawGizmos ( )
    {
        Color color = Color.red;
        Gizmos.color = color;
        if(spawnMode == SpawnMode.Circle)
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
        else if(spawnMode == SpawnMode.Square)
        {
            Gizmos.DrawWireCube(transform.position,size);
        }
    }
}
