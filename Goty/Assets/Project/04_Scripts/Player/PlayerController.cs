using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GamePhase
{
    RIVER,
    SEA,
    SKY,
    FALL
}
public class PlayerController : MonoBehaviour
{
    [Serializable]
    private class GameInput
    {
        public PlayerInput playerControllerMap;
        private InputAction moveAction;
        private InputAction moveActionK;
        public GameInput( PlayerInput input )
        {
            playerControllerMap = input;
            moveAction = playerControllerMap.actions["Move"];
            moveActionK = playerControllerMap.actions["MoveK"];
        }
        public Vector2 GetKeyboardDirection ( )
        {
            return moveActionK.ReadValue<Vector2>();
        }
        public Vector2 GetTouchDirection ( )
        {
            return moveAction.ReadValue<Vector2>();
        }
    }

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float speed; //Velocidad de movimiento
    [SerializeField] private float divingSpeed = 3;
    [SerializeField] private float sensitivity; //Sensibilidad de pantalla con movimiento de dedo o ratón
    [SerializeField] private bool useMouse; //Sensibilidad de pantalla con movimiento de dedo o ratón
    [SerializeField] private float phaseTransitionMaxTime;
    [SerializeField] private float timeScale = 1;
    [SerializeField] private float forwardSpeed = 1;
    
    
    public GamePhase phase = GamePhase.RIVER; //quitar public 
    private Rigidbody rb;
    private GameInput gameInput;
    private CameraBehaviour cameraBehaviour;
    private Vector2 lastPointerPos;
    private Timer timer;
    private float targetX;
    public float distX;
    public float radius;
    public float checkDist = 0.3f;
    private float deltaTime;
    private float shakeDurationOnHit;
    private float shakeAmountOnHit;
    private void Start ( )
    {
        gameInput = new GameInput(playerInput);
        rb = GetComponent<Rigidbody>();
        timer = new Timer( this );
        cameraBehaviour = Camera.main.gameObject.GetComponent<CameraBehaviour>();
    }
    private void Update ( )
    {
        deltaTime = Time.deltaTime * timeScale;
        PlayerMove(phase);
    }
    float dirZ;
    void PlayerMove(GamePhase phase)
    {
        Vector2 keyDirection = gameInput.GetKeyboardDirection( ).normalized;
        Vector2 touchDirection = gameInput.GetTouchDirection( ).normalized;

        Vector2 direction = useMouse ? touchDirection : keyDirection; 
        if (phase == GamePhase.RIVER)
        {
            float maxX = 0;
            RiverMove(direction);
        }
        else if(phase == GamePhase.SEA)
        {
            Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1,1, Camera.main.nearClipPlane));
            Vector3 size = topRight - bottomLeft;
            Vector3 center = bottomLeft + size / 2;
            Bounds bound = new Bounds(center, size);

            Debug.Log(bound);
            SeaMove(bound, direction);
        }
    }
    private void OnDrawGizmos ( )
    {

        // Gizmo derecha
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.right * checkDist, radius);

        // Gizmo izquierda
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.left * checkDist, radius);
    }
    void RiverMove ( Vector2 direction )
    {
        float deltaX = direction.x * speed * Time.deltaTime;

        RaycastHit hit;

        if (deltaX > 0 && Physics.Raycast(transform.position, Vector3.right, out hit, radius + checkDist, groundMask))
        {
            deltaX = Mathf.Min(deltaX, hit.distance - radius);
        }
        else if (deltaX < 0 && Physics.Raycast(transform.position, Vector3.left, out hit, radius + checkDist, groundMask))
        {
            deltaX = Mathf.Max(deltaX, -(hit.distance - radius));
        }

        dirZ += forwardSpeed * speed * deltaTime;
        Vector3 nextPos = transform.position + new Vector3(deltaX, 0, dirZ * 0.1f * Vector3.back.z);
        rb.MovePosition(nextPos);
    }
    void SeaMove(Bounds bound, Vector2 direction)
    {
        direction.y += divingSpeed * deltaTime;
        float deltaX = (direction.x - lastPointerPos.x) * speed * deltaTime;
        float deltaY = (direction.y - lastPointerPos.y) * speed * deltaTime;
        Vector3 newDir = transform.position + Vector3.right * deltaX + Vector3.up * deltaY;
        Vector3 newPos = new Vector3(newDir.x, newDir.y, transform.position.z);
        direction.x = Mathf.Clamp(direction.x, bound.min.x, bound.max.x);
        direction.y = Mathf.Clamp(direction.y, bound.min.y, bound.max.y);
        rb.MovePosition(newPos);
    }
    void AscensionMove ( Vector2 direction )
    {

        float deltaX = direction.x * speed * Time.deltaTime;

        RaycastHit hit;

        if (deltaX > 0 && Physics.Raycast(transform.position, Vector3.right, out hit, radius + checkDist, groundMask))
        {
            deltaX = Mathf.Min(deltaX, hit.distance - radius);
        }
        else if (deltaX < 0 && Physics.Raycast(transform.position, Vector3.left, out hit, radius + checkDist, groundMask))
        {
            deltaX = Mathf.Max(deltaX, -(hit.distance - radius));
        }

        dirZ += forwardSpeed * speed * deltaTime;
        Vector3 nextPos = transform.position + new Vector3(deltaX, dirZ * Vector3.down.y, transform.position.z);
        rb.MovePosition(nextPos);
    }
    void SkyMove ( )
    {

    }
    void FallMove ( Vector2 direction )
    {

        float deltaX = direction.x * speed * Time.deltaTime;

        RaycastHit hit;

        if (deltaX > 0 && Physics.Raycast(transform.position, Vector3.right, out hit, radius + checkDist, groundMask))
        {
            deltaX = Mathf.Min(deltaX, hit.distance - radius);
        }
        else if (deltaX < 0 && Physics.Raycast(transform.position, Vector3.left, out hit, radius + checkDist, groundMask))
        {
            deltaX = Mathf.Max(deltaX, -(hit.distance - radius));
        }

        dirZ += forwardSpeed * speed * deltaTime;
        Vector3 nextPos = transform.position + new Vector3(deltaX, dirZ * Vector3.down.y, transform.position.z);
        rb.MovePosition(nextPos);
    }
    void PhaseTransition()
    {
        if(!timer.Timer_Started())
        {
            timer.StartTimer(phaseTransitionMaxTime, ( ) =>
            {
                Debug.Log("Transicionando");
            }, Action_Timing.Start);
        }
    }
    private void OnCollisionEnter ( Collision collision )
    {
        if(collision.gameObject.layer == groundMask)
        {
            cameraBehaviour.CameraShake(shakeDurationOnHit, shakeAmountOnHit);
        }
    }
}
