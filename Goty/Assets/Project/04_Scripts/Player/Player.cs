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
    private bool useMouse;
    [SerializeField] private float sensitivity;
    [SerializeField] private FixedJoystick joystick;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float divingSpeed = 3f;
    [SerializeField] private float forwardSpeed = 1f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float descendentFlowPower = -7f;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float checkDist = 0.3f;
    [SerializeField] private float radius = 0.5f;
    private Bounds screenBounds;

    [Header("Gameplay Settings")]
    [SerializeField] private GamePhase phase = GamePhase.RIVER;
    [SerializeField] private float phaseTransitionMaxTime = 2f;
    [SerializeField] private float timeScale = 1f;
    [SerializeField] private float shakeDurationOnHit = 0.5f;
    [SerializeField] private float shakeAmountOnHit = 0.1f;
    [SerializeField] private Collider screenCollider;

    // Components
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private CircleCollider2D circleCollider2D;
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
    }

    private void Update ( )
    {
        deltaTime = Time.deltaTime * timeScale;
        HandleMovement();
        if(Input.GetKeyDown(KeyCode.U))
        {
            SceneController.Instance.NextScene();
        }
    }

    private void InitializeComponents ( )
    {
        gameInput = new GameInput(playerInput);
        timer = new TimerObject(this);
        rb2d = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        if(screenCollider != null) screenBounds = screenCollider.bounds;
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.TryGetComponent<CameraBehaviour>(out cameraBehaviour))
        {
            Debug.Log("Added camera behaviour succefully");
        }
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // Lógica para apps móviles
            useMouse = true;
            joystick.gameObject.SetActive(true);
        }
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL: puede ser PC o móvil, hay que comprobar si tiene pantalla táctil
            if (Input.touchSupported)
            {
                useMouse = true;
                print("has touch support");
                joystick.gameObject.SetActive(true);
            }
            else
            {
                useMouse = false;
                joystick.gameObject.SetActive(false);
            }
        }
        else
        {
            // PC o editor
            useMouse = false;
            joystick.gameObject.SetActive(false);
        }
    }
    private void HandleMovement ( )
    {
        Vector2 inputDirection = GetInputDirection();
        switch (phase)
        {
            case GamePhase.RIVER:
                RiverMove(GetInputDirection());
                break;
            case GamePhase.SEA:
                SeaMove(GetInputDirection());
                break;
            case GamePhase.ASCENSION:
                AscensionMove(GetInputDirection());
                break;
            case GamePhase.SKY:
                SkyMove();
                break;
            case GamePhase.FALL:
                FallMove(GetInputDirection());
                break;
        }
    }

    private Vector2 GetInputDirection ( )
    {
        if (useMouse)
        {
            return GetTouchBasedInput(true);
        }
        return gameInput.GetKeyboardDirection();
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
        MoveHorizontallyWithCollision(direction);
    }

    private void ConfigureRigidbodyForRiver ( )
    {
        if (rb2d.gravityScale != 0)
        {
            rb2d.gravityScale = 1f;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    private void ConfigureRigidbodyForSkyFall ( )
    {
        if (rb2d.simulated)
        {
            rb2d.simulated = false;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void MoveHorizontallyWithCollision ( Vector2 direction )
    {
        float deltaX = -direction.y * speed;

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, radius + checkDist, groundMask);
        if (deltaX > 0 && hitRight.collider != null)
        {
            deltaX = Mathf.Min(deltaX, hitRight.distance - radius);
        }

        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, radius + checkDist, groundMask);
        if (deltaX < 0 && hitLeft.collider != null)
        {
            deltaX = Mathf.Max(deltaX, -(hitLeft.distance - radius));
        }

        Vector2 nextPos = Vector2.right * deltaX; 
        dirZ = Vector3.forward.z * forwardSpeed;

        rb2d.linearVelocity = new Vector2(nextPos.x, dirZ);
    }

    private void SeaMove ( Vector2 direction )
    {
        ConfigureRigidbodyForFlappy();

        float targetVelX = -direction.y * speed;


        rb2d.linearVelocity = new Vector3(targetVelX, rb2d.linearVelocity.y);

        HandleJump();
    }

    private void ConfigureRigidbodyForFlappy ( )
    {
        if (!rb2d.simulated)
        {
            rb2d.simulated = true;
            rb2d.constraints = RigidbodyConstraints2D.None;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void HandleJump ( )
    {
        bool jumpInput = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            jumpInput = touch.phase == UnityEngine.TouchPhase.Began && touch.position.x > 0;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            jumpInput = Input.mousePosition.x > Screen.width / 2;
        }

        if (jumpInput)
        {
            float velY = Mathf.Clamp(rb2d.linearVelocity.y, minGravityFall, maxGravityFall);

            rb2d.linearVelocity = new Vector3(rb2d.linearVelocity.x, velY);
            rb2d.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    public float maxGravityFall = 7;
    public float minGravityFall = -3;
    private void AscensionMove ( Vector2 direction )
    {
        ConfigureRigidbodyForAscension();
        float targetVelX = direction.x * speed;
        if (Physics2D.CircleCast(transform.position, 1, Vector2.zero, 1,5/* layermask 5*/))
        {
            minGravityFall = descendentFlowPower;
        }
        else
        {
            minGravityFall = -3;
        }
        float velY = Mathf.Clamp(rb2d.linearVelocity.y, minGravityFall, maxGravityFall);
        rb2d.linearVelocity = Vector3.up * velY + Vector3.right * targetVelX;;
    }
    float lerpSpeed = 3;
    float originalSpeed;
    private void ConfigureRigidbodyForAscension ( )
    {
        if (!rb2d != null)
        {
            if (!rb2d.simulated)
            {
                rb2d.simulated = true;
                rb2d.constraints = RigidbodyConstraints2D.None;
                rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }
    private void SkyMove ( )
    {
        ConfigureRigidbodyForFlappy();
        HandleJump();
    }
    private void FallMove ( Vector2 direction )
    {
        ConfigureRigidbodyForSkyFall();
        if (direction == Vector2.zero) { lerpSpeed = 0; }
        Vector2 velocity = direction * speed;

        rb2d.linearVelocity = velocity;
    }

    Vector3 ClampPositionInsideScreen ( )
    {
        float clampedX = Mathf.Clamp(transform.position.x, screenBounds.min.x, screenBounds.max.x);
        float clampedY = Mathf.Clamp(transform.position.y, screenBounds.min.y, screenBounds.max.y);
        return new Vector3(clampedX, clampedY, transform.position.z);
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
    bool lookRight;
    void Flip ( )
    {
        lookRight = !lookRight;
        int right = lookRight ? 1 : -1;
        transform.localScale =  new Vector2 ( right, transform.localScale.y );
    }
}