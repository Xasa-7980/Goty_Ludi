using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController :MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    string nextSceneName;
    int nextSceneIndex = 1;
    bool nextScene_IsPreLoaded;

    public static Action ChangeToNextScene;
    public static Action GoMainMenu;
    public static Action GoToRiver;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); 
            return;
        }
        Instance = this;    
        DontDestroyOnLoad(gameObject);

    }

    //private void InitializeActions ( )
    //{
    //    ChangeToNextScene = ( ) =>
    //    {
    //        if (nextScene_IsPreLoaded && asyncNextSceneLoaded != null)
    //        {
    //            print("ASYNCLOADED");
    //            StartCoroutine(PreloadNextScene());
    //            asyncNextSceneLoaded.allowSceneActivation = true;
    //            nextSceneIndex++;
    //            CheckFor_NextSceneIndex() ;
    //        }
    //    };

    //}
    //public IEnumerator PreloadNextScene( )
    //{
    //    asyncNextSceneLoaded = SceneManager.LoadSceneAsync(nextSceneIndex);
    //    asyncNextSceneLoaded.allowSceneActivation = false;
    //    while (!asyncNextSceneLoaded.isDone)
    //    {
    //        if (asyncNextSceneLoaded.progress >= 0.9f)
    //        {
    //            nextScene_IsPreLoaded = true;
    //            break;
    //        }
    //        yield return null;
    //    }
    //}

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadRiver()
    {
        SceneManager.LoadScene("River");
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
        //ChangeToNextScene?.Invoke();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
