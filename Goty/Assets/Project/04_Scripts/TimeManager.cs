using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float timeToWin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SeaTime());
    }

    IEnumerator SeaTime()
    {
        yield return new WaitForSeconds(timeToWin);

        SceneController.Instance.NextScene();

        yield return null;
    }
}
