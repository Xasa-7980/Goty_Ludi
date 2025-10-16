using UnityEngine;

public class GoalScript : MonoBehaviour
{
    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if(collision.gameObject.layer == 7)
        {
            SceneController.Instance.NextScene();
        }
    }
}
