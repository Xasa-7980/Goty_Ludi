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
    public int index;

    public static Action ChangeToNextScene;
    public static Action GoMainMenu;
    public static Action GoToRiver;
    private void Awake ( )
    {
        if (Instance != null && Instance != this)
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
        index = 0;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadRiver()
    {
        index++;
        SceneManager.LoadScene("Transition",LoadSceneMode.Additive);
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
    public string GetCurrentSceneName ( )
    {
        return SceneManager.GetSceneAt(index).name;
    }
    private bool isReloading = false;

    public void ResetScene ( )
    {
        if (isReloading) return; // evita recargar dos veces
        isReloading = true;

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
