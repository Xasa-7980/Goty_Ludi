using UnityEngine;
using UnityEngine.Events;

public class GoalScript : MonoBehaviour
{
    public UnityEvent onPlayerTouch;
    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if(collision.gameObject.layer == 7)
        {
            onPlayerTouch?.Invoke ();
        }
    }
}
