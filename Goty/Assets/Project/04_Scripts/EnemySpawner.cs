using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject Shark;
    [SerializeField] private GameObject Jellyfish;
    [SerializeField] private GameObject Fish;
    [SerializeField] private GameObject WaterDrop;
    private TimerObject timer;
    private int i = 0;

    [SerializeField] private float[] times;
    [SerializeField] private EnemyType[] enemies;

    enum EnemyType
    {
        SHARK,
        JELLYFISH,
        FISH,
        WATERDROP,
        NULL
    }

    void Start()
    {
        timer = new TimerObject(this);
    }

    private void Update()
    {
        if (i < enemies.Length)
        {
            if (!timer.Timer_Started())
            {
                GameObject enemy;
                timer.StartTimer(times[i], () =>
                {
                    switch (enemies[i])
                    {
                        case EnemyType.SHARK:
                            enemy = Instantiate(Shark);
                            break;

                        case EnemyType.JELLYFISH:
                            enemy = Instantiate(Jellyfish);
                            break;

                        case EnemyType.FISH:
                            enemy = Instantiate(Fish);
                            break;
                        case EnemyType.WATERDROP:
                            enemy = Instantiate(WaterDrop);
                            break;

                        default:
                            break;
                    }
                    i++;
                }, Action_Timing.End);
            }
        }
    }
}
