using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float vel;
    private float offset;
    private SpriteRenderer sr;

    private void Start()
    {
        vel /= 100;
        sr = GetComponent<SpriteRenderer>();
        offset = 0;
    }
    // Update is called once per frame
    void Update()
    {
        offset += Time.deltaTime * vel;
        sr.material.SetTextureOffset("_MainTex", offset * Vector2.right);
    }
}
