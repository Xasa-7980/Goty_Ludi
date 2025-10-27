using UnityEngine;

public class Items : MonoBehaviour
{
    [SerializeField] private bool isMultiplier;

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
