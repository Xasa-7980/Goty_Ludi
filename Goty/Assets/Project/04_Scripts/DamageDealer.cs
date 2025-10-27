using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private bool isSky;
    [SerializeField] private bool isSea;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            // Matar al player
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (isSky)
            {
                collision.gameObject.transform.position = new Vector3(-5, 0, 0);
                collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            }
            if (isSea)
            {
                collision.gameObject.transform.position = new Vector3(0, 3, 0);
                collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            }
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
