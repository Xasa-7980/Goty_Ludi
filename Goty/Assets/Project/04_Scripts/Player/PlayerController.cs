using System;
using UnityEngine;
using UnityEngine.InputSystem;
//public enum GamePhase
//{
//    RIVER,
//    SEA,
//    ASCENSION,
//    SKY,
//    FALL
//}

public class PlayerController : MonoBehaviour
{
    [Serializable]
    public class GameInput
    {
        public PlayerInput playerControllerMap;
        private InputAction moveAction;
        private InputAction moveActionK;
        public GameInput ( PlayerInput input )
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

    [SerializeField] protected GameInput gameInput;
    [SerializeField] protected float speed;
    [SerializeField] protected GamePhase gamePhase;
    [SerializeField] protected FixedJoystick fixedJoystick;
    [SerializeField] protected float deltaTime;
    [SerializeField] protected float jumpForce;

    //public PlayerC

}
