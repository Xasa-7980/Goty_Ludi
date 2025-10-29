using UnityEngine;
using TMPro;

public class FinalScore : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("Score").ToString();
    }
}
