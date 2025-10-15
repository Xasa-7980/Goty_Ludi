using Unity.VisualScripting;
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
    [SerializeField] private float spawnTime = 3;
    [SerializeField] private GameObject[] prefabsToSpawn;
    TimerObject spawnTimer;
    [SerializeField] private SpawnMode spawnMode;
    [SerializeField] private Player player;
    [SerializeField] private float maxDistance = 5;
    private void Start ( )
    {
        spawnTimer = new TimerObject(this);
        player = GameObject.FindAnyObjectByType( typeof( Player ) ).GetComponent<Player>();
    }
    private void Update ( )
    {
        switch (spawnMode)
        {
            case SpawnMode.None:
                break;
            case SpawnMode.Square:
                break;
            case SpawnMode.Circle:
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= maxDistance && distance >= 20 )
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
    void SpawnInsideCircle ( )
    {
        Vector2 randPosition = Random.insideUnitCircle * radius;
        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex], new Vector3(transform.position.x + randPosition.x, transform.position.y, transform.position.z + randPosition.y), Quaternion.identity);

    }
    void SpawnInsideSquare ( )
    {
        var a = GetComponent<Collider>().bounds;
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
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
