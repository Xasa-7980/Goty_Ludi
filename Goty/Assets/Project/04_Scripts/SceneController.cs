using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController :MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    string nextSceneName;
    int nextSceneIndex = 1;
    AsyncOperation asyncNextSceneLoaded;
    bool nextScene_IsPreLoaded;

    public static Action ChangeToNextScene;
    public static Action GoMainMenu;
    public static Action GoToRiver;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);          
        }
        Instance = this;    
        DontDestroyOnLoad(gameObject);

    }
    private void Start ( )
    {
        CheckFor_NextSceneIndex();
        //Preparando las escenas
        StartCoroutine(PreloadNextScene());
        InitializeActions();
    }
    private void InitializeActions ( )
    {
        ChangeToNextScene = ( ) =>
        {
            if (nextScene_IsPreLoaded && asyncNextSceneLoaded != null)
            {
                print("ASYNCLOADED");
                StartCoroutine(PreloadNextScene());
                asyncNextSceneLoaded.allowSceneActivation = true;
                nextSceneIndex++;
                CheckFor_NextSceneIndex() ;
            }
        };
        GoMainMenu = ( ) =>
        {
            SceneManager.LoadScene("MainMenu");
            nextSceneIndex = 1;
            CheckFor_NextSceneIndex() ;
        };
        GoToRiver = ( ) =>
        {
            SceneManager.LoadScene("River");
            nextSceneIndex++;
            CheckFor_NextSceneIndex() ;
        };

    }
    public IEnumerator PreloadNextScene( )
    {
        asyncNextSceneLoaded = SceneManager.LoadSceneAsync(nextSceneIndex);
        asyncNextSceneLoaded.allowSceneActivation = false;
        while (!asyncNextSceneLoaded.isDone)
        {
            if (asyncNextSceneLoaded.progress >= 0.9f)
            {
                nextScene_IsPreLoaded = true;
                break;
            }
            yield return null;
        }
    }

    public void LoadMainMenu()
    {
        GoMainMenu?.Invoke();
    }

    public void LoadRiver()
    {
        GoToRiver?.Invoke();
    }
    private void CheckFor_NextSceneIndex ( )
    {
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
    }
    public void NextScene()
    {
        ChangeToNextScene?.Invoke();
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadPauseMenu()
    {

    }

    public void LoadOptionsMenu()
    {

    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
