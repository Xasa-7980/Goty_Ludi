using UnityEngine;

public class fallSimulationMovemen : MonoBehaviour
{
    [SerializeField] float vel;

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * vel * Vector3.up;

        if (transform.position.y > 25)
            transform.position -= Vector3.up * 50;
    }
}
