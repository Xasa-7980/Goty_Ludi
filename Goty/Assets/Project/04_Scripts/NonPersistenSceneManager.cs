using TransitionsPlus;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NonPersistentSceneManager : MonoBehaviour
{
    [SerializeField] private TransitionAnimator transitionAnimator;

    public void SetLoadSceneName ( )
    {
        transitionAnimator.sceneNameToLoad = SceneController.Instance.GetCurrentSceneName ();
    }
}