using TMPro;
using UnityEngine;

public class SettingScoreUI : MonoBehaviour
{
    public TextMeshProUGUI score;
    private void Awake ( )
    {
        score.text = PlayerPrefs.GetInt("Score").ToString();
    }
}
