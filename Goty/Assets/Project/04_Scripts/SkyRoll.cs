using UnityEngine;

public class SkyRoll : MonoBehaviour
{
    [SerializeField] private float vel;

    void Update()
    { 
        transform.Rotate(Time.deltaTime * Vector3.up * vel);    
    }
}
