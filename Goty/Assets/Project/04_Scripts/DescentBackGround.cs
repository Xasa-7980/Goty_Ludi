using UnityEngine;

public class DescentBackGround : MonoBehaviour
{
    [SerializeField] private GameObject[] backgrounds;
    [SerializeField] private float vel;
    [SerializeField] private float timeToEnd;
    private float time;

    void Start()
    {
        time = 0;
        backgrounds[0].SetActive(true);
        backgrounds[1].SetActive(false);
        backgrounds[2].SetActive(true);
    }

    void Update()
    {
        transform.position += Time.deltaTime * Vector3.up * vel;
        if (transform.position.y > 35)
            backgrounds[0].SetActive(false);
        
        if (time >= timeToEnd)
            backgrounds[1].SetActive(true);
        else if(transform.position.y >= 95)
            transform.position = new Vector3(0, 35, 0);

        time += Time.deltaTime;
    }
}
