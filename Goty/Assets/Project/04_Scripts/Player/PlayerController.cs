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
    [SerializeField] private float sensitivity; //Sensibilidad de pantalla con movimiento de dedo o ratón
    [SerializeField] private bool useMouse; //Sensibilidad de pantalla con movimiento de dedo o ratón
    private Rigidbody rb;
    private GameInput gameInput;
    private Vector2 lastPointerPos;
    private float targetX;
    private void Start ( )
    {
        gameInput = new GameInput(playerInput);
        rb = GetComponent<Rigidbody>();
    }
    private void Update ( )
    {
        PlayerMove();
    }
    void PlayerMove()
    {
        Vector2 direction = gameInput.GetKeyboardDirection( ).normalized;
        Vector2 direction2 = gameInput.GetTouchDirection( ).normalized;
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
        if(direction.x != 0 && !useMouse)
        {
            float delta = direction.x * speed * Time.deltaTime;

            if (Keyboard.current.aKey.isPressed) //Go left ( KeyBoard )
            {
                targetX = Mathf.Clamp(transform.position.x - delta, minX, maxX);
            }
            else if(Keyboard.current.dKey.isPressed) //Go right ( KeyBoard )
            {
                targetX = Mathf.Clamp(transform.position.x + delta, minX, maxX);
            }
        }
        else if(direction != Vector2.zero && (Mouse.current != null || Touchscreen.current != null) && useMouse)
        {
            float delta = (direction.x - lastPointerPos.x) * sensitivity;
            targetX = Mathf.Clamp(transform.position.x + delta, minX, maxX);
            lastPointerPos = direction;
        }
        Vector3 newPos = new Vector3(targetX, transform.position.y, transform.position.z);
        rb.MovePosition(newPos);
    }
}
