using UnityEngine;

public class AbsorbableCloud : MonoBehaviour
{
    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if(collision.gameObject.layer == 7)
        {
            PlayerResizer playerResizer = collision.GetComponent<PlayerResizer>();
            playerResizer.OnPlayerAbsorbs?.Invoke(true);
        }
    }
}
