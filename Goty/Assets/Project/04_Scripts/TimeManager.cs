using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float timeToWin;
    public UnityEvent onTimeFinishes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SeaTime());
    }

    IEnumerator SeaTime()
    {
        yield return new WaitForSeconds(timeToWin);

        onTimeFinishes?.Invoke();

        yield return null;
    }
}
