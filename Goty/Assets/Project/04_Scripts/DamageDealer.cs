using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            // Matar al player
            print("die player");
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            playerStats.SetHealth(-1);

        }
    }
}
