using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public AudioClip clip;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            //AudioManager.PlayOnce(clip);

            // Matar al player
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            PlayerResizer playerResizer = collision.gameObject.GetComponent<PlayerResizer>();
            Player p = collision.gameObject.GetComponent<Player>();
            switch (p.phase)
            {
                case GamePhase.SEA:
                    collision.gameObject.transform.position = new Vector3(0, 3, 0);
                    collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
                    break;
                case GamePhase.ASCENSION:
                    print("found collision at ascension");
                    collision.gameObject.transform.position = new Vector3(0, transform.position.y + 5, 0);
                    collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
                    break;
                case GamePhase.SKY:
                    collision.gameObject.transform.position = new Vector3(-5, 0, 0);
                    collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
                    playerResizer.OnPlayerAbsorbs?.Invoke(false);
                    break;
                default:
                    break;
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
            Player p = collision.gameObject.GetComponent<Player>();
            if (p.phase == GamePhase.ASCENSION)
            {
                collision.gameObject.transform.position = new Vector3(0, transform.position.y + 5, 0);
                collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            }
            playerStats.SetHealth(-1);
        }
    }
}
