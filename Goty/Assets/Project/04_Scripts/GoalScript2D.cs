using UnityEngine;

public class GoalScript2D : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            SceneController.Instance.NextScene();
        }      
    }
}
