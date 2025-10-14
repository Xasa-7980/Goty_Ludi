using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            // Matar al player
            SceneController.Instance.ResetScene();
        }
    }
}
