using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Serializable]
    internal class GameInput
    {
        public PlayerInput playerControllerMap;
        private InputAction moveAction;
        public GameInput()
        {
            moveAction = playerControllerMap.actions["Move"];
        }
        public Vector2 GetDirection ( )
        {
            return moveAction.ReadValue<Vector2>();
        }
    }
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float speed; //Velocidad de movimiento
    [SerializeField] private float sensitivity; //Sensibilidad de pantalla con movimiento de dedo o ratón
    private Rigidbody rb;
    private GameInput gameInput;
    private Vector2 lastPointerPos;
    private void Start ( )
    {
        gameInput = new GameInput();
    }
    private void Update ( )
    {
        PlayerMove();
    }
    void PlayerMove()
    {
        Vector2 direction = gameInput.GetDirection( );
        float targetX = 0;
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
        if(direction.x != 0 && (Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed))
        {
            float delta = direction.x * speed * Time.deltaTime;
            targetX = Mathf.Clamp(transform.position.x + delta, minX, maxX);
        }
        else if(direction != Vector2.zero && Mouse.current != null && Touchscreen.current != null)
        {
            float delta = (direction.x - lastPointerPos.x) * sensitivity;
            targetX = Mathf.Clamp(transform.position.x + delta, minX, maxX);
            lastPointerPos = direction;
        }
        Vector3 newPos = new Vector3(targetX, transform.position.y, transform.position.z);
        rb.velocity = newPos;
    }
}
