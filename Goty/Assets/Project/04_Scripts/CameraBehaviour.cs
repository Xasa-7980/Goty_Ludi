using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private TimerObject timer;
    private Vector3 originalPos;
    [SerializeField] private Player player;
    private void Start ( )
    {
        originalPos = transform.localPosition;
        timer = new TimerObject(this);
    }
    private void Update ( )
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y,transform.position.z);
    }
    public void CameraShake ( float duration, float shakeAmount )
    {
        if ( timer == null )
        {
            return;
        }
        if(!timer.Timer_Started())
        {
            originalPos = transform.localPosition;
            timer.StartTimer(duration, ( ) => {
                transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
                timer.StartTimer(duration, ( ) => transform.localPosition = originalPos, Action_Timing.End);
                }, Action_Timing.Start);
        }
        
    }
    void FollowPlayer ( )
    {
        
    }
}

