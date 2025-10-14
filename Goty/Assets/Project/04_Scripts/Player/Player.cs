using System;
using Unity.Cinemachine;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum GamePhase
{
    RIVER,
    SEA,
    ASCENSION,
    SKY,
    FALL
}

public enum PhysicsMode
{
    Mode2D,
    Mode3D
}

public class Player : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private bool useMouse;
    [SerializeField] private float sensitivity;
    [SerializeField] private FixedJoystick joystick;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float divingSpeed = 3f;
    [SerializeField] private float forwardSpeed = 1f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float checkDist = 0.3f;
    [SerializeField] private float radius = 0.5f;

    [Header("Gameplay Settings")]
    [SerializeField] private GamePhase phase = GamePhase.RIVER;
    [SerializeField] private float phaseTransitionMaxTime = 2f;
    [SerializeField] private float timeScale = 1f;
    [SerializeField] private float shakeDurationOnHit = 0.5f;
    [SerializeField] private float shakeAmountOnHit = 0.1f;

    // Components
    [HideIfNoComponent(typeof(Rigidbody))] private Rigidbody rb;
    [HideIfNoComponent(typeof(SphereCollider))] private SphereCollider sphereCollider;
    [HideIfNoComponent(typeof(Rigidbody2D))] private Rigidbody2D rb2d;
    [HideIfNoComponent(typeof(CircleCollider2D))] private CircleCollider2D circleCollider2D;
    private CameraBehaviour cameraBehaviour;
    private GameInput gameInput;
    private TimerObject timer;

    // State variables
    private Vector2 lastPointerPos;
    private float dirZ;
    private float deltaTime;
    private int health = 0;
    private PhysicsMode currentPhysicsMode;

    // Events
    public UnityEvent OnDeath;

    private void Start ( )
    {
        InitializeComponents();
        ChangePhase(phase);
    }

    private void Update ( )
    {
        deltaTime = Time.deltaTime * timeScale;
        HandleMovement();
    }

    private void InitializeComponents ( )
    {
        gameInput = new GameInput(playerInput);
        timer = new TimerObject(this);

        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.TryGetComponent<CameraBehaviour>(out cameraBehaviour))
        {
            Debug.Log("Added camera behaviour succefully");
        }
    }

    public void ChangePhase ( GamePhase newPhase )
    {
        PhysicsMode requiredPhysics = newPhase == GamePhase.ASCENSION ? PhysicsMode.Mode2D : PhysicsMode.Mode3D;

        if (currentPhysicsMode != requiredPhysics)
        {
            if (requiredPhysics == PhysicsMode.Mode2D)
            {
                Add2DPhysicsSetUp();
            }
            else
            {
                Add3DPhysicsSetUp();
            }
            currentPhysicsMode = requiredPhysics;
        }

        phase = newPhase;
    }

    private void HandleMovement ( )
    {
        Vector2 inputDirection = GetInputDirection();

        switch (phase)
        {
            case GamePhase.RIVER:
                RiverMove(inputDirection);
                break;
            case GamePhase.SEA:
                SeaMove(joystick.Direction);
                break;
            case GamePhase.ASCENSION:
                AscensionMove(joystick.Direction);
                break;
            case GamePhase.SKY:
                SkyMove();
                break;
            case GamePhase.FALL:
                RiverMove(joystick.Direction);
                break;
        }
    }

    private Vector2 GetInputDirection ( )
    {
        if (useMouse)
        {
            return GetTouchBasedInput(true);
        }
        return gameInput.GetKeyboardDirection().normalized;
    }

    private Vector2 GetTouchBasedInput ( bool useJoystick )
    {
        if (useJoystick)
        {
            return joystick.Direction;
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.position.x < Screen.width / 2)
            {
                float moveDir = touch.deltaPosition.x > 0 ? 1 : -1;
                return new Vector2(moveDir, 0);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x < Screen.width / 2)
            {
                if (lastPointerPos == Vector2.zero)
                    lastPointerPos = Input.mousePosition;

                Vector2 delta = (Vector2)Input.mousePosition - lastPointerPos;
                lastPointerPos = Input.mousePosition;
                return new Vector2(delta.x * sensitivity, 0);
            }
        }
        lastPointerPos = Vector2.zero;

        return Vector2.zero;
    }

    private void RiverMove ( Vector2 direction )
    {
        ConfigureRigidbodyForRiver();
        MoveHorizontallyWithCollision(direction.x);
        MoveForward();
    }

    private void ConfigureRigidbodyForRiver ( )
    {
        if (rb.useGravity)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        }
    }

    private void MoveHorizontallyWithCollision ( float inputX )
    {
        float deltaX = inputX * speed * Time.deltaTime;
        Vector3 direction = deltaX > 0 ? Vector3.right : Vector3.left;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit,
            radius + checkDist, groundMask))
        {
            float maxDistance = hit.distance - radius;
            deltaX = deltaX > 0 ? Mathf.Min(deltaX, maxDistance) : Mathf.Max(deltaX, -maxDistance);
        }

        Vector3 horizontalMovement = new Vector3(deltaX, 0, 0);
        rb.MovePosition(transform.position + horizontalMovement);
    }

    private void MoveForward ( )
    {
        dirZ += forwardSpeed * speed * deltaTime;
        Vector3 forwardMovement = new Vector3(0, 0, dirZ * 0.1f * Vector3.back.z);
        rb.MovePosition(transform.position + forwardMovement);
    }

    private void SeaMove ( Vector2 direction )
    {
        ConfigureRigidbodyForSea();

        float targetVelX = direction.x * speed;
        rb.linearVelocity = new Vector3(targetVelX, rb.linearVelocity.y, rb.linearVelocity.z);

        HandleJump();
    }

    private void ConfigureRigidbodyForSea ( )
    {
        if (!rb.useGravity)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void HandleJump ( )
    {
        bool jumpInput = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            jumpInput = touch.phase == UnityEngine.TouchPhase.Began && touch.position.x > Screen.width / 2;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            jumpInput = Input.mousePosition.x > Screen.width / 2;
        }

        if (jumpInput)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void AscensionMove ( Vector2 direction )
    {
        float targetVelX = direction.x * speed;
        rb2d.linearVelocity = new Vector2(targetVelX, rb2d.linearVelocity.y);
    }

    private void SkyMove ( )
    {
        HandleJump();
    }

    private void OnDrawGizmos ( )
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.right * checkDist, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.left * checkDist, radius);
    }

    private void PhaseTransition ( )
    {
        if (!timer.Timer_Started())
        {
            timer.StartTimer(phaseTransitionMaxTime, ( ) =>
            {
                Debug.Log("Transicionando");
            }, Action_Timing.Start);
        }
    }

    private void Add3DPhysicsSetUp ( )
    {
        if (rb2d != null)
        {
            Destroy(rb2d);
        }
        if (circleCollider2D != null)
        {
            Destroy(circleCollider2D);
        }

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
    }

    private void Add2DPhysicsSetUp ( )
    {
        if (rb != null)
        {
            Destroy(rb);
        }
        if (sphereCollider != null)
        {
            Destroy(sphereCollider);
        }

        if (rb2d == null)
        {
            rb2d = gameObject.AddComponent<Rigidbody2D>();
        }
        if (circleCollider2D == null)
        {
            circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
        }
    }

    private void OnCollisionEnter ( Collision collision )
    {
        if (collision.gameObject.layer == groundMask)
        {
            cameraBehaviour?.CameraShake(shakeDurationOnHit, shakeAmountOnHit);
        }
    }

    private void OnCollisionEnter2D ( Collision2D collision )
    {
        if (collision.gameObject.layer == groundMask)
        {
            cameraBehaviour?.CameraShake(shakeDurationOnHit, shakeAmountOnHit);
        }
    }

    public void ReceiveDamage ( int damage )
    {
        health -= damage;
        if (health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    private System.Collections.IEnumerator Fade ( )
    {
        Debug.Log("Start");
        yield return new WaitUntil(( ) => Input.GetKeyDown(KeyCode.A));
        Debug.Log("End");
    }

    [Serializable]
    private class GameInput
    {
        private PlayerInput playerControllerMap;
        private InputAction moveAction;
        private InputAction moveActionK;

        public GameInput ( PlayerInput input )
        {
            playerControllerMap = input;
            moveAction = playerControllerMap.actions["Move"];
            moveActionK = playerControllerMap.actions["MoveK"];
        }

        public Vector2 GetKeyboardDirection ( ) => moveActionK.ReadValue<Vector2>();
        public Vector2 GetTouchDirection ( ) => moveAction.ReadValue<Vector2>();
    }
}