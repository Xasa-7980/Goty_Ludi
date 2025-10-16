using UnityEngine;

public class DamageDealer2D : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            // Matar al player
            SceneController.Instance.ResetScene();
        }
    }
}
