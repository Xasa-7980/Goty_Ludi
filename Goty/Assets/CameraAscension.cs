using UnityEngine;

public class CameraAscension : MonoBehaviour
{ 
    [SerializeField] private GameObject player;

    private float initialZ;
    private float initialX;

    private void Start()
    {
        initialZ = transform.position.z;
        initialX = transform.position.x;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 lastPos = transform.position;

        if (player.transform.position.y > lastPos.y)
            transform.position = new Vector3(0, player.transform.position.y, initialZ);
    }
}
