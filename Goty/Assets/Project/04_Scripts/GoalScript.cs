using UnityEngine;

public class GoalScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("hola");
        if (other.gameObject.layer == 7)
        {
            SceneController.Instance.NextScene();
        }
    }
}
