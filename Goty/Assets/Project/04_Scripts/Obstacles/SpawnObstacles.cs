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
    [SerializeField] private SpawnMode spawnMode;
    [SerializeField] private Player player;
    [SerializeField] private float maxDistance = 30;
    [SerializeField] private float minDistance = 15;

    private TimerObject spawnTimer;

    private void Start ( )
    {
        spawnTimer = new TimerObject(this);
        player = GameObject.FindAnyObjectByType<Player>();
    }

    private void Update ( )
    {
        if (player == null) return;

        float distanceY = Mathf.Abs(transform.position.y - player.transform.position.y);

        if (distanceY <= minDistance)
        {
            if (spawnTimer.Timer_Started())
                spawnTimer.StopTimer(); 
            return;
        }

        if (distanceY < maxDistance && distanceY > minDistance)
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
                    if (!spawnTimer.Timer_Started())
                    {
                        spawnTimer.StartTimer(spawnTime, ( ) =>
                        {
                            SpawnInsideSphere();
                        }, Action_Timing.End);
                    }
                    break;

                case SpawnMode.Box:
                    if (!spawnTimer.Timer_Started())
                    {
                        spawnTimer.StartTimer(spawnTime, ( ) =>
                        {
                            SpawnInsideBox();
                        }, Action_Timing.End);
                    }
                    break;
            }
        }
    }

    void SpawnInsideCircle ( )
    {
        Vector2 randPosition = Random.insideUnitCircle * radius;
        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex],
            new Vector3(transform.position.x + randPosition.x, transform.position.y, transform.position.z + randPosition.y),
            Quaternion.identity);
    }

    void SpawnInsideSquare ( )
    {
        var a = GetComponent<Collider2D>().bounds;
        float randomX = Random.Range(a.min.x + 0.5f, a.max.x -0.5f);
        float randomY = Random.Range(a.min.y, a.max.y);
        Vector3 pointInsideSquare = new Vector3(randomX, randomY, transform.position.z);
        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex], pointInsideSquare, Quaternion.identity);
    }

    void SpawnInsideSphere ( )
    {
        Vector3 randPosition = Random.insideUnitSphere * radius;
        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex],
            transform.position + randPosition,
            Quaternion.identity);
    }

    void SpawnInsideBox ( )
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            Random.Range(-size.y / 2, size.y / 2),
            Random.Range(-size.z / 2, size.z / 2)
        );
        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex], transform.position + randomPoint, Quaternion.identity);
    }

    private void OnDrawGizmos ( )
    {
        Gizmos.color = Color.red;

        if (spawnMode == SpawnMode.Circle)
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
        else if (spawnMode == SpawnMode.Square)
        {
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}