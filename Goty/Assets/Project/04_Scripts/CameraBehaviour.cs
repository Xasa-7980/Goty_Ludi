using UnityEngine;

public enum CameraFollowMode
{
    Vertical,
    Horizontal,
    Full
}
public class CameraBehaviour : MonoBehaviour
{
    private TimerObject timer;
    private Vector3 originalPos;
    [SerializeField] private Player player;
    public CameraFollowMode followMode;
    private void Start ( )
    {
        originalPos = transform.localPosition;
        timer = new TimerObject(this);
    }
    private void Update ( )
    {
        if(followMode == CameraFollowMode.Full) transform.position = new Vector3(player.transform.position.x, player.transform.position.y,transform.position.z);
        else if(followMode == CameraFollowMode.Vertical) transform.position = new Vector3(transform.position.x, player.transform.position.y,transform.position.z);
        else if(followMode == CameraFollowMode.Horizontal) transform.position = new Vector3(player.transform.position.x, transform.position.y,transform.position.z);
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

