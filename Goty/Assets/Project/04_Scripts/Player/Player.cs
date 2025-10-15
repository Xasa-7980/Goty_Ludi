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

    [Header("Collision Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float checkDist = 0.3f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private Collider screenCollider;
    private Bounds screenBounds;

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
        if (Application.platform == RuntimePlatform.Android)
        {
            // Lógica para Android
            useMouse = true;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Lógica para PC/Web
            useMouse = false;
            joystick.gameObject.SetActive(false);
        }
        originalSpeed = speed;
    }

    private void Update ( )
    {
        deltaTime = Time.deltaTime * timeScale;
        HandleMovement();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneController.Instance.NextScene();
        }
    }

    private void InitializeComponents ( )
    {
        gameInput = new GameInput(playerInput);
        timer = new TimerObject(this);
        screenBounds = screenCollider.bounds;
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.TryGetComponent<CameraBehaviour>(out cameraBehaviour))
        {
            Debug.Log("Added camera behaviour succefully");
        }
    }

    public void ChangePhase ( GamePhase newPhase )
    {
        PhysicsMode requiredPhysics = phase == GamePhase.ASCENSION ? PhysicsMode.Mode2D : PhysicsMode.Mode3D;

        if (requiredPhysics == PhysicsMode.Mode2D)
        {
            Add2DPhysicsSetUp();
        }
        else
        {
            Add3DPhysicsSetUp();
        }

        phase = newPhase;
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
        return new Vector3(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"),0);
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
    private void ConfigureRigidbodyForSkyFall ( )
    {
        if (rb.useGravity)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void MoveHorizontallyWithCollision ( Vector3 direction )
    {
        float deltaX = Vector3.right.x * direction.y * speed;

        RaycastHit hit;

        if (deltaX > 0 && Physics.Raycast(transform.position, Vector3.right, out hit, radius + checkDist, groundMask))
        {
            deltaX = Mathf.Min(deltaX, hit.distance - radius);

        }
        else if (deltaX < 0 && Physics.Raycast(transform.position, Vector3.left, out hit, radius + checkDist, groundMask))
        {
            deltaX = Mathf.Max(deltaX, -(hit.distance - radius));
        }
        Vector3 nextPos = new Vector3(deltaX, 0, 0);
        rb.linearVelocity = new Vector3(nextPos.x * deltaTime, rb.linearVelocity.y, rb.linearVelocity.z);
    }
    private void MoveForward ( )
    {
        dirZ = Vector3.forward.z * forwardSpeed;
        //Posible feature: añadir mas velocidad a medida del tiempo con +=
        Vector3 forwardMovement = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, dirZ);
        rb.linearVelocity = forwardMovement;
    }

    private void SeaMove ( Vector2 direction )
    {
        ConfigureRigidbodyForFlappy();

        float targetVelX = direction.x * speed;
        float velY = Mathf.Clamp(rb2d.linearVelocity.y, minGravityFall, maxGravityFall);

        rb.linearVelocity = new Vector3(targetVelX, velY, rb.linearVelocity.z);

        HandleJump();
    }

    private void ConfigureRigidbodyForFlappy ( )
    {
        if (!rb.useGravity)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
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
            float velY = Mathf.Clamp(rb.linearVelocity.y, minGravityFall, maxGravityFall);

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, velY, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public float maxGravityFall = 7;
    public float minGravityFall = -3;
    private void AscensionMove ( Vector2 direction )
    {
        ConfigureRigidbodyForAscension();
        float targetVelX = direction.x * speed;
        float velY = Mathf.Clamp(rb2d.linearVelocity.y, minGravityFall, maxGravityFall);
        rb2d.linearVelocity = Vector3.up * velY + Vector3.right * targetVelX;;
    }
    float lerpSpeed = 3;
    float originalSpeed;
    private void FallMove ( Vector2 direction )
    {
        ConfigureRigidbodyForSkyFall();
        if(direction == Vector2.zero) { lerpSpeed = 0; }
        Vector2 velocity = direction * speed;

        rb.linearVelocity = velocity;
        float clampedX = Mathf.Clamp(transform.position.x, screenBounds.min.x, screenBounds.max.x);
        float clampedY = Mathf.Clamp(transform.position.y, screenBounds.min.y, screenBounds.max.y);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, transform.position.z);

        rb.MovePosition(clampedPosition);
    }
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
            if(!TryGetComponent<Rigidbody>(out rb))
            {

                rb = gameObject.AddComponent<Rigidbody>();
            }
        }
        if (sphereCollider == null)
        {
            if (!TryGetComponent<SphereCollider>(out sphereCollider))
            {

                sphereCollider = gameObject.AddComponent<SphereCollider>();
            }
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
        if (!TryGetComponent<Rigidbody2D>(out rb2d))
        {
            print("No encuentro el rb2d");
            rb2d = gameObject.AddComponent<Rigidbody2D>();
        }
        if (!TryGetComponent<CircleCollider2D>(out circleCollider2D))
        {
            print("No encuentro el collider 2d");
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