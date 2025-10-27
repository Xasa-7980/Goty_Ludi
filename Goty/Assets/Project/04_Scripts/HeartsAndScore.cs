using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class HeartsAndScore : MonoBehaviour
{
    public static HeartsAndScore Instance { get; private set; }

    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Transform lifeParent;
    [SerializeField] private TextMeshProUGUI scoreTxt;
    private List<Image> lifes = new List<Image>();
    private int score;

    private void Awake()
    {
        score = 0;
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this);
        InitLifes();
    }

    public void InitLifes()
    {
        for (int i = 0; i < 5; i++)
        {
            Image im = Instantiate(imagePrefab, lifeParent).GetComponent<Image>();
            im.gameObject.SetActive(false);
            lifes.Add(im);
        }
    }

    public void DrawCurrentLifes(int health)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i > health)
                lifes[i].gameObject.SetActive(false);
            else
                lifes[i].gameObject.SetActive(true);
        }
    }

    public void SetScore(int score, bool multiplier)
    {
        if(multiplier)
        {
            scoreTxt.color = Color.yellow;
            scoreTxt.fontSize = 40;
        }
        else
        {
            scoreTxt.color = Color.white;
            scoreTxt.fontSize = 36;
        }
            scoreTxt.text = score.ToString();
    }
}
