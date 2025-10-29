using UnityEngine;

public class RiverAudio : MonoBehaviour
{
    [SerializeField] private string[] audios;
    [SerializeField] private bool[] play;
    void Start()
    {
        for (int i = 0; i < audios.Length; i++)
        {
            if (play[i])
                AudioManager.instance.Play(audios[i]);
            else
                AudioManager.instance.Stop(audios[i]);
        }
    }
}
