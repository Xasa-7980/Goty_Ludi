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

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this);
        DrawLifes();
    }

    public void DrawLifes()
    {
        for (int i = 0; i < 5; i++)
        {
            Image im = Instantiate(imagePrefab, lifeParent).GetComponent<Image>();
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

    public void SetScore(int score)
    {
        scoreTxt.text = score.ToString();
    }
}
