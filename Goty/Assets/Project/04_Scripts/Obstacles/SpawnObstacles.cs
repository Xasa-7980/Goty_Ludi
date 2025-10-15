using UnityEngine;

public class SpawnObstacles : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float spawnTime = 3;
    [SerializeField] private GameObject prefab;
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
                SpawnObstacle();
            }, Action_Timing.End);
        }
    }
    void SpawnObstacle ( )
    {
        Vector2 randPosition = Random.insideUnitCircle * radius;
        Instantiate(prefab, new Vector3(transform.position.x + randPosition.x, transform.position.y, transform.position.z + randPosition.y),Quaternion.identity);
    }
    private void OnDrawGizmos ( )
    {
        Color color = Color.red;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
