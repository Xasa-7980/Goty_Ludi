using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class HeartsAndScore : MonoBehaviour
{
    public static HeartsAndScore Instance { get; private set; }

    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Transform lifeParent;
    [SerializeField] public TextMeshProUGUI scoreTxt;
    public List<Image> lifes = new List<Image>();
    private int score;

    private void Awake()
    {
        print("here");
        score = 0;
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        scoreTxt = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    private void Update ( )
    {
        SetText(PlayerPrefs.GetInt("Score"));
        DrawCurrentLifes();
    }

    public void DrawCurrentLifes()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i > PlayerPrefs.GetInt("Health"))
                lifes[i].gameObject.SetActive(false);
            else
                lifes[i].gameObject.SetActive(true);
        }
    }
    public void SetScore(int _score, bool multiplier)
    {

        if (multiplier)
        {
            scoreTxt.color = Color.yellow;
            scoreTxt.fontSize = 40;
        }
        else
        {
            scoreTxt.color = Color.white;
            scoreTxt.fontSize = 36;
        }
        SetText(_score);
    }
    public void SetText(int _score )
    {
        scoreTxt.text = _score.ToString();
    }
}
