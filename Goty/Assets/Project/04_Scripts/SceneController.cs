using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController :MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    AsyncOperation async;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);          
        }
        Instance = this;    
        DontDestroyOnLoad(Instance);
    }
    IEnumerator LoadSceneAsync()
    {
        async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        while (async.progress < 0.9)
        {
            yield return null;
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadRiver()
    {
        SceneManager.LoadScene("River");
    }

    public void NextScene()
    {
        if (SceneManager.GetActiveScene().name != "SkyFall")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }
        LoadRiver();
    }

    public void LoadPauseMenu()
    {

    }

    public void LoadOptionsMenu()
    {

    }
}
