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
    [SerializeField] public GameObject player;

    private GamePhase fase;
    private float vel = 10;
    private float seaVel;
    private float initialY;

    private void Awake()
    {
        fase = player.GetComponent<Player>().phase;
        switch (fase)
        {
            case (GamePhase.RIVER):
                break;
            case (GamePhase.SEA):
                transform.position = new Vector3(Random.Range(SEA_X_MIN, SEA_X_MAX), SEA_INITIAL_Y, 0f);
                break;
            case (GamePhase.ASCENSION):
                transform.position = new Vector3(Random.Range(ASC_X_MIN, ASC_X_MAX), player.transform.position.y + 15, 0f);
                break;
            case (GamePhase.SKY):
                transform.position = new Vector3(SKY_INITIAL_X, Random.Range(SKY_Y_MIN, SKY_Y_MAX), 0f);
                break;
            case (GamePhase.FALL):
                transform.position = new Vector3(Random.Range(FALL_X_MIN, FALL_X_MAX), player.transform.position.y - 15, 0f);
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
            case (GamePhase.RIVER):
                break;

            case (GamePhase.SEA):
                Debug.Log("sea");
                if (transform.position.y < SEA_HEIGHT)
                    transform.position += Time.deltaTime * Vector3.down * seaVel;
                else
                    transform.position += Time.deltaTime * Vector3.down * vel;
                break;

            case (GamePhase.ASCENSION):
                transform.position += Time.deltaTime * Vector3.up * vel;
                break;

            case (GamePhase.SKY):
                transform.position += Time.deltaTime * Vector3.left * vel;
                break;

            case (GamePhase.FALL):
                transform.position += Time.deltaTime * Vector3.up * seaVel;
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
