using UnityEngine;
using TMPro;

public class FinalScore : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshPro>().text = PlayerPrefs.GetInt("Score").ToString();
    }
}
