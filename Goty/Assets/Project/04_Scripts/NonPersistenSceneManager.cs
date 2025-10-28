using TransitionsPlus;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NonPersistentSceneManager : MonoBehaviour
{
    [SerializeField] private TransitionAnimator transitionAnimator;
    private int index;

    private void Awake ( )
    {
        index = SceneController.Instance.index;

        // Evita errores si index-1 < 0 o fuera de rango
        if (index > 0 && index < SceneManager.sceneCountInBuildSettings)
        {
            string prevSceneName = SceneManager.GetSceneByBuildIndex(index - 1).name;
            Debug.Log("Unloading previous scene: " + prevSceneName);
            LoadNextScene(prevSceneName);
        }
    }

    public void LoadNextScene ( string prevSceneName )
    {
        // Obtén el número total de escenas en build settings
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        // Si ya estamos en la última, ir a "River"
        if (index >= totalScenes - 1)
        {
            index = SceneManager.GetSceneByName("River").buildIndex;
        }

        // Obtener el nombre de la siguiente escena por índice
        string nextSceneName = System.IO.Path.GetFileNameWithoutExtension(
            SceneUtility.GetScenePathByBuildIndex(index)
        );

        Debug.Log("Loading next scene: " + nextSceneName);

        // Configura el TransitionAnimator para cargar esa escena
        transitionAnimator.loadSceneAtEnd = true;
        transitionAnimator.sceneNameToLoad = nextSceneName;
        transitionAnimator.sceneLoadMode = LoadSceneMode.Single;

        // Inicia la transición (TransitionAnimator se encargará de cargar la escena)
        transitionAnimator.Play();
        transitionAnimator.onTransitionEnd.AddListener( () => {
            SceneManager.LoadScene(nextSceneName);
            SceneManager.UnloadSceneAsync(prevSceneName);
        });
    }
}