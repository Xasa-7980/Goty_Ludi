using UnityEngine;

public class TerrainPart : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToDeactivate;
    private bool isActived;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool IsActived()
    {
        return isActived;
    }
    public void Show()
    {
        for (int i = 0; i < objectsToDeactivate.Length; i++)
        {
            objectsToDeactivate[i].SetActive(true);
        }
        isActived = true;
    }
    public void Hide ()
    {
        for (int i = 0; i < objectsToDeactivate.Length; i++)
        {
            objectsToDeactivate[i].SetActive(false);
        }
        isActived = false;
    }
}
