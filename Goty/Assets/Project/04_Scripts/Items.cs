using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    //SEA CONSTANTS:

    const float SEA_INITIAL_Y = 8;
    const float SEA_X_MIN = -9;
    const float SEA_X_MAX = 9;
    const float SEA_HEIGHT = 4;

    //SKY CONSTANTS:
    const float SKY_INITIAL_X = 8;
    const float SKY_Y_MIN = -12;
    const float SKY_Y_MAX = 12;

    //ASCENSION CONSTANTS:
    const float ASC_X_MIN = -12;
    const float ASC_X_MAX = 12;

    //FALL CONSTANTS:
    const float FALL_X_MIN = -12;
    const float FALL_X_MAX = 12;

    [SerializeField] private bool isMultiplier;
    [SerializeField] public Player player;

    private GamePhase fase;
    private float vel = 3;
    private float seaVel;
    private float initialY;

    private void Awake()
    {
        player = GameObject.FindFirstObjectByType<Player>();
        fase = player.phase;
        switch (fase)
        {
            case (GamePhase.RIVER):
                break;
            case (GamePhase.SEA):
                transform.position = new Vector3(Random.Range(SEA_X_MIN, SEA_X_MAX), SEA_INITIAL_Y, 0f);
                break;
            case (GamePhase.FALL):
                if (transform.position.x <= -20)
                    gameObject.SetActive(false);
                transform.position = new Vector3(Random.Range(FALL_X_MIN, FALL_X_MAX), player.transform.position.y - 15, 0f);
                break;
            default:
                break;
        }
    }

    void Start()
    {
        seaVel = vel / 10;
    }

    void Update()
    {
        switch (fase)
        {
            case (GamePhase.SEA):
                Debug.Log("sea");
                if (transform.position.y < SEA_HEIGHT)
                    transform.position += Time.deltaTime * Vector3.down * seaVel;
                else
                    transform.position += Time.deltaTime * Vector3.down * vel;
                break;

            case (GamePhase.SKY):
                transform.position += Time.deltaTime * Vector3.left * vel;
                break;

            case (GamePhase.FALL):
                transform.position += Time.deltaTime * Vector3.up * seaVel;
                break;
            default:
                break;  
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (!isMultiplier)
                collision.gameObject.GetComponent<PlayerStats>().SetHealth(1);
            else
            {
                collision.gameObject.GetComponent<PlayerStats>().OnMultiplier();
            }
            gameObject.SetActive(false);
        }
    }
}
