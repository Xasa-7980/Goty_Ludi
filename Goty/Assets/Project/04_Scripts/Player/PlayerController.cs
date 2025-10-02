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
            moveActionK = playerControllerMap.actions["Move"];
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
    private Vector2 lastPointerPos;
    private Timer timer;
    private float targetX;
    public float distX;
    private float newDeltaTime;
    private void Start ( )
    {
        gameInput = new GameInput(playerInput);
        rb = GetComponent<Rigidbody>();
        timer = new Timer( this );
    }
    private void Update ( )
    {
        newDeltaTime = Time.deltaTime * timeScale;
        PlayerMove(phase);
    }
    float dirZ;
    void PlayerMove(GamePhase phase)
    {
        Vector2 keyDirection = gameInput.GetKeyboardDirection( );
        Vector2 touchDirection = gameInput.GetTouchDirection( );

        Vector2 direction = useMouse ? touchDirection : keyDirection; 
        if (phase == GamePhase.RIVER)
        {
            float maxX = 0;
            if (Physics.Raycast(transform.position, Vector3.right, out RaycastHit hitRight, 100, groundMask))
            {
                maxX = hitRight.point.x;
            }
            float minX = 0;
            if (Physics.Raycast(transform.position, Vector3.left, out RaycastHit hitLeft, 100, groundMask))
            {
                minX = hitLeft.point.x;
            }
            GetRiverMovement(direction, minX, maxX);
        }
        else if(phase == GamePhase.SEA)
        {
            Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1,1, Camera.main.nearClipPlane));
            Vector3 size = topRight - bottomLeft;
            Vector3 center = bottomLeft + size / 2;
            Bounds bound = new Bounds(center, size);

            Debug.Log(bound);
            GetSeaMovement(bound, direction);
        }
    }
    void GetRiverMovement ( Vector2 direction, float minX, float maxX )
    {
        float deltaX = 0f;

        if (direction.x != 0 && !useMouse) // Using keyboard
        {
            if (Keyboard.current.aKey.isPressed) // Left
            {
                deltaX = -speed * newDeltaTime;
            }
            else if (Keyboard.current.dKey.isPressed) // Right
            {
                deltaX = speed * newDeltaTime;
            }
        }
        else if (direction != Vector2.zero && (Mouse.current != null || Touchscreen.current != null) && useMouse) // Mouse / touch screen
        {
            deltaX = (direction.x - lastPointerPos.x) * sensitivity;
            lastPointerPos = direction;
        } 
        float nextX = Mathf.Clamp(transform.position.x + deltaX, minX - distX, maxX + distX); 
        dirZ += forwardSpeed * speed * newDeltaTime; 
        Vector3 newPos = new Vector3(nextX, transform.position.y, dirZ * Vector3.back.z); 
        rb.MovePosition(newPos);
    }
    void GetSeaMovement(Bounds bound, Vector2 direction)
    {
        direction.y += divingSpeed * newDeltaTime;
        float deltaX = (direction.x - lastPointerPos.x) * speed * newDeltaTime;
        float deltaY = (direction.y - lastPointerPos.y) * speed * newDeltaTime;
        Vector3 newDir = transform.position + Vector3.right * deltaX + Vector3.up * deltaY;
        Vector3 newPos = new Vector3(newDir.x, newDir.y, transform.position.z);
        direction.x = Mathf.Clamp(direction.x, bound.min.x, bound.max.x);
        direction.y = Mathf.Clamp(direction.y, bound.min.y, bound.max.y);
        rb.MovePosition(newPos);
    }
    void GetSkyMovement ( )
    {

    }
    void GetFallMovement ( )
    {

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
}
