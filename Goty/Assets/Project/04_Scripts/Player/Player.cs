using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum GamePhase
{
    RIVER = 1,
    SEA = 2,
    ASCENSION = 3,
    SKY = 4,
    FALL = 5
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
    private Bounds screenBounds;

    [Header("Gameplay Settings")]
    [SerializeField] public GamePhase phase = GamePhase.RIVER;
    [SerializeField] private float phaseTransitionMaxTime = 2f;
    [SerializeField] private float timeScale = 1f;
    [SerializeField] private float shakeDurationOnHit = 0.5f;
    [SerializeField] private float shakeAmountOnHit = 0.1f;
    [SerializeField] private Collider screenCollider;

    [Header("Visual Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite dropSprite;
    [SerializeField] private Sprite cloudSprite;
    [SerializeField] private Sprite vaporSprite;
    [SerializeField] private Animator anim;

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
    bool blockMovement;

    private void Start ( )
    {
        phase = (GamePhase)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        InitializeComponents();
    }

    private void Update ( )
    {
        if(blockMovement)
        {
            blockMovement = false;
            rb2d.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
        else
        {
            if (timeScale < 0) timeScale = 1;
            if(rb2d.gravityScale < 0) rb2d.gravityScale = 1;
                
        }
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
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        if(screenCollider != null) screenBounds = screenCollider.bounds;
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.TryGetComponent<CameraBehaviour>(out cameraBehaviour))
        {
            Debug.Log("Added camera behaviour succefully");
        }
        
        
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {  // Lógica para apps móviles 
            useMouse = true;
            joystick.gameObject.SetActive(true);
        }
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL: puede ser PC o móvil, hay que comprobar si tiene pantalla táctil con:
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
        blockMovement = phase == GamePhase.SEA || phase == GamePhase.SKY;
    }
    private void HandleMovement ( )
    {
        Vector2 inputDirection = GetInputDirection();
        switch (phase)
        {
            case GamePhase.RIVER:
                spriteRenderer.sprite = dropSprite;
                anim.SetBool("IsCloud", false);
                RiverMove(GetInputDirection());
                break;
            case GamePhase.SEA:
                spriteRenderer.sprite = dropSprite;
                anim.SetBool("IsCloud", false);
                SeaMove(GetInputDirection());
                break;
            case GamePhase.ASCENSION:
                spriteRenderer.sprite = vaporSprite;
                anim.SetBool("IsCloud", false);
                anim.SetBool("IsVapor", true);
                AscensionMove(GetInputDirection());
                break;
            case GamePhase.SKY:
                spriteRenderer.sprite = cloudSprite;
                anim.SetBool("IsCloud", true);
                SkyMove();
                break;
            case GamePhase.FALL:
                spriteRenderer.sprite = dropSprite;
                anim.SetBool("IsCloud", false);
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
        do
        {
            CameraBehaviour.followMode = CameraFollowMode.Full;
            spriteRenderer.sprite = dropSprite;
            rb2d.simulated = true;
            rb2d.gravityScale = 1f;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        } while (!rb2d.simulated);
    }
    private void ConfigureRigidbodyForSkyFall ( )
    {
        do
        {
            CameraBehaviour.followMode = CameraFollowMode.None;
            CameraBehaviour.cinemachineCameraStatic.enabled = false;
            spriteRenderer.sprite = dropSprite;
            rb2d.simulated = true;
            rb2d.gravityScale = 0f;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        } while (!rb2d.simulated);
    }
    float rotZ;
    [SerializeField] private float rotSpeed;
    private void MoveHorizontallyWithCollision ( Vector2 direction )
    {
        float deltaX = direction.x * speed;

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
        rotZ += Vector2.right.x * deltaX * deltaTime * rotSpeed;
        if(deltaX == 0)
        {
            rotZ = Mathf.MoveTowards(rotZ, 0, deltaTime * rotSpeed * rotSpeed);
        }
        rotZ = Mathf.Clamp(rotZ, -30, 30);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotZ));
        rb2d.linearVelocity = new Vector2(nextPos.x, dirZ);
    }

    private void SeaMove ( Vector2 direction )
    {
        ConfigureRigidbodyForFlappy();

        float targetVelX = direction.x * speed;


        rb2d.linearVelocity = new Vector3(targetVelX, rb2d.linearVelocity.y);

        HandleJump();
    }

    private void ConfigureRigidbodyForFlappy ( )
    {
        do
        {
            CameraBehaviour.followMode = CameraFollowMode.None;
            CameraBehaviour.cinemachineCameraStatic.enabled = false;
            spriteRenderer.sprite = dropSprite;
            rb2d.simulated = true;
            rb2d.gravityScale = 1f;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        } while (!rb2d.simulated);
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
            jumpInput = Input.mousePosition.x > 0;
        }

        if (jumpInput)
        {
            rb2d.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            float velY = Mathf.Clamp(rb2d.linearVelocity.y, minGravityFall, maxGravityFall);
            rb2d.linearVelocity = velY * Vector3.up ;
        }
    }
    [SerializeField] float maxGravityFall = 7;
    [SerializeField] private float descendentFlowPower = -3f;
    [SerializeField] float minGravityFall = -1;
    private void AscensionMove ( Vector2 direction )
    {
        ConfigureRigidbodyForAscension();
        float deltaX = direction.x * speed;

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
        if (Physics2D.OverlapCircle(transform.position, 1,1 << 8))
        {
            anim.SetBool("ColdFlow", true);
            anim.SetBool("HotFlow", false);
            minGravityFall = descendentFlowPower;
        }
        else if(Physics2D.OverlapCircle(transform.position, 1,1 << 9))
        {
            anim.SetBool("ColdFlow", false);
            anim.SetBool("HotFlow", true);
            minGravityFall = -1;
        }
        else
        {
            anim.SetBool("ColdFlow", false);
            anim.SetBool("HotFlow", false);
            minGravityFall = -1;
        }
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, radius + checkDist, groundMask);

        if (rb2d.linearVelocity.y <= 0 && hitDown.collider != null)
        {
            transform.position = new Vector2(transform.position.x,
                                                 transform.position.y - (hitDown.distance - radius));
            Vector2 vel = rb2d.linearVelocity;
            vel.y = 0;
            rb2d.linearVelocity = vel;
        }
        float velY = Mathf.Clamp(rb2d.linearVelocity.y, minGravityFall, maxGravityFall);
        rb2d.linearVelocity = rb2d.linearVelocity.y * Vector3.up + Vector3.right * deltaX; ;
    }
    float lerpSpeed = 3;
    float originalSpeed;
    private void ConfigureRigidbodyForAscension ( )
    {
        do
        {
            print(rb2d.simulated);
            CameraBehaviour.followMode = CameraFollowMode.FollowOnlyUp;
            CameraBehaviour.cinemachineCameraStatic.enabled = false;
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            spriteRenderer.sprite = dropSprite;
            rb2d.simulated = true;
            rb2d.gravityScale = 0.5f;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        } while (!rb2d.simulated);
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
        rotZ += Vector2.right.x * velocity.x * deltaTime * rotSpeed;
        if (velocity.x == 0)
        {
            rotZ = Mathf.MoveTowards(rotZ, 0, deltaTime * rotSpeed * rotSpeed);
        }
        rotZ = Mathf.Clamp(rotZ, -30, 30);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotZ));
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
    private void OnTriggerEnter2D ( Collider2D collision )
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