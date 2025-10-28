using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacles : MonoBehaviour
{
    private enum SpawnMode
    {
        None,
        Square,
        Circle,
        Sphere,
        Box
    }

    [Header("Spawn Settings")]
    [SerializeField] private float radius = 10f;
    [SerializeField] private Vector3 size = Vector3.one * 10f;
    [SerializeField] private float spawnTime = 3f;
    [SerializeField] private GameObject[] prefabsToSpawn;
    [SerializeField] private SpawnMode spawnMode = SpawnMode.Circle;

    [Header("Player Distance Check")]
    [SerializeField] private Player player;
    [SerializeField] private float maxDistance = 30f;
    [SerializeField] private float minDistance = 15f;

    [Header("Spawn Spacing Control")]
    [SerializeField] private float minSpawnDistance = 1.5f;
    [SerializeField] private int maxAttempts = 100;

    [SerializeField, Range(-1,1)] private float objectsGravityScale = 0.5f;
    private TimerObject spawnTimer;
    private List<Vector3> occupiedPositions = new List<Vector3>();

    private void Start ( )
    {
        spawnTimer = new TimerObject(this);
        player = GameObject.FindAnyObjectByType<Player>();
        if(spawnMode == SpawnMode.Circle)
        {
            if(TryGetComponent<Collider2D>(out Collider2D col))
            {
                col.enabled = false;
            }
        }
    }

    private void Update ( )
    {
        if (player == null) return;

        float distanceY = Mathf.Abs(transform.position.y - player.transform.position.y);

        if (distanceY <= minDistance)
        {
            if (spawnTimer.Timer_Started())
                spawnTimer.PauseTimer();
            return;
        }

        if (distanceY < maxDistance && distanceY > minDistance)
        {
            if (!spawnTimer.Timer_Started())
            {
                spawnTimer.StartTimer(spawnTime, ( ) =>
                {
                    switch (spawnMode)
                    {
                        case SpawnMode.Square:
                            SpawnInsideSquare();
                            break;

                        case SpawnMode.Circle:
                            SpawnInsideCircle();
                            break;

                        case SpawnMode.Sphere:
                            SpawnInsideSphere();
                            break;

                        case SpawnMode.Box:
                            SpawnInsideBox();
                            break;
                    }
                }, Action_Timing.End);
            }
        }
    }

    // ========================= CIRCLE SPAWN =========================
    private void SpawnInsideCircle ( )
    {
        int attempts = 0;
        Vector3 newPos;

        do
        {
            Vector2 randPos = Random.insideUnitCircle * radius;
            newPos = transform.position + new Vector3(randPos.x, randPos.y, 0);
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogWarning("No se encontró posición válida para spawn (Círculo).");
                return;
            }

        } while (!IsPositionValid(newPos));

        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex], newPos, Quaternion.identity);
        occupiedPositions.Add(newPos);
    }

    // ========================= SQUARE SPAWN =========================
    private void SpawnInsideSquare ( )
    {
        int attempts = 0;
        Vector3 newPos;

        var a = GetComponent<Collider2D>().bounds;

        do
        {
            float randomX = Random.Range(a.min.x + 0.5f, a.max.x - 0.5f);
            float randomY = Random.Range(a.min.y + 0.5f, a.max.y - 0.5f);
            newPos = new Vector3(randomX, randomY, transform.position.z);
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogWarning("No se encontró posición válida para spawn (Cuadrado).");
                return;
            }

        } while (!IsPositionValid(newPos));

        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject tempOb = Instantiate(prefabsToSpawn[randIndex], newPos, Quaternion.identity);
        occupiedPositions.Add(newPos);
        if(tempOb.TryGetComponent<Rigidbody2D> (out Rigidbody2D rb2d))
        {
            rb2d.gravityScale = objectsGravityScale;
        }
    }

    // ========================= SPHERE SPAWN =========================
    private void SpawnInsideSphere ( )
    {
        int attempts = 0;
        Vector3 newPos;

        do
        {
            Vector3 randPos = Random.insideUnitSphere * radius;
            newPos = transform.position + randPos;
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogWarning("No se encontró posición válida para spawn (Esfera).");
                return;
            }

        } while (!IsPositionValid(newPos));

        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex], newPos, Quaternion.identity);
        occupiedPositions.Add(newPos);
    }

    // ========================= BOX SPAWN =========================
    private void SpawnInsideBox ( )
    {
        int attempts = 0;
        Vector3 newPos;

        do
        {
            Vector3 randPos = new Vector3(
                Random.Range(-size.x / 2f, size.x / 2f),
                Random.Range(-size.y / 2f, size.y / 2f),
                Random.Range(-size.z / 2f, size.z / 2f)
            );

            newPos = transform.position + randPos;
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogWarning("No se encontró posición válida para spawn (Caja).");
                return;
            }

        } while (!IsPositionValid(newPos));

        int randIndex = Random.Range(0, prefabsToSpawn.Length);
        Instantiate(prefabsToSpawn[randIndex], newPos, Quaternion.identity);
        occupiedPositions.Add(newPos);
    }

    // ========================= UTILS =========================
    private bool IsPositionValid ( Vector3 pos )
    {
        foreach (var occupied in occupiedPositions)
        {
            if (Vector3.Distance(occupied, pos) < minSpawnDistance)
                return false;
        }
        return true;
    }

    // ========================= GIZMOS =========================
    private void OnDrawGizmos ( )
    {
        Gizmos.color = Color.red;

        switch (spawnMode)
        {
            case SpawnMode.Circle:
                Gizmos.DrawWireSphere(transform.position, radius);
                break;

            case SpawnMode.Square:
                Gizmos.DrawWireCube(transform.position, size);
                break;

            case SpawnMode.Sphere:
                Gizmos.DrawWireSphere(transform.position, radius);
                break;

            case SpawnMode.Box:
                Gizmos.DrawWireCube(transform.position, size);
                break;
        }
    }
}