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
    private void Start ( )
    {
        spawnTimer = new TimerObject(this);
    }
    private void Update ( )
    {
        if (!spawnTimer.Timer_Started())
        {
            spawnTimer.StartTimer(spawnTime, ( ) =>
            {
                Spawn();
            }, Action_Timing.End);
        }
        SpawnInsideSquare();
    }
    void Spawn ( )
    {
        Vector2 randPosition = Random.insideUnitCircle * radius;
        int randIndex = Random.Range( 0, prefabsToSpawn.Length );
        Instantiate(prefabsToSpawn[randIndex], new Vector3(transform.position.x + randPosition.x, transform.position.y, transform.position.z + randPosition.y),Quaternion.identity);
        
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
    void SpawnInsideCircle ( )
    {
        Vector2 randPosition = Random.insideUnitCircle * radius;
        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex], new Vector3(transform.position.x + randPosition.x, transform.position.y, transform.position.z + randPosition.y), Quaternion.identity);
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
