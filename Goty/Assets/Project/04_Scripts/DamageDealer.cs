using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            // Matar al player
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            playerStats.SetHealth(-1);

        }
    }
    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if (collision.gameObject.layer == 7)
        {
            // Matar al player
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            playerStats.SetHealth(-1);
        }
    }
}
