using UnityEngine;
using TMPro;

public class FinalScore : MonoBehaviour
{
    int score;
    void Start()
    {
        score = PlayerPrefs.GetInt("Score");
        GetComponent<TextMeshProUGUI>().text = score.ToString();
    }

    public void ResetScore()
    {
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Health", 5);
    }
}
