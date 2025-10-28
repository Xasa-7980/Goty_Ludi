using UnityEngine;

public class DescentBackGround : MonoBehaviour
{
    [SerializeField] private GameObject[] backgrounds;
    [SerializeField] private float vel;
    void Start()
    {
        backgrounds[0].SetActive(true);
        backgrounds[1].SetActive(false);
        backgrounds[2].SetActive(true);
    }

    void Update()
    {
        transform.position += Time.deltaTime * Vector3.up * vel;
        if (transform.position.y > 35)
            backgrounds[0].SetActive(false);
    }
}
