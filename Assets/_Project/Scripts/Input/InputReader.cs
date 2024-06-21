using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace CarRace
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Plataformer/Input/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<bool> Run = delegate { };
        public event UnityAction<bool> TurnRight = delegate { };
        public event UnityAction<bool> TurnLeft = delegate { };

        PlayerInputActions inputActions;

        public Vector3 Direction => (Vector3)inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }
            inputActions.Enable();
        }

        public void EnablePlayerActions()
        {
            inputActions.Enable();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            // npp
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // npp
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            // npp
        }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            // npp
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            // npp
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Run.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Run.Invoke(false);
                    break;
            }
        }

        public void OnMoveRight(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    TurnRight.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    TurnRight.Invoke(false);
                    break;
            }
        }

        public void OnMoveLeft(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    TurnLeft.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    TurnLeft.Invoke(false);
                    break;
            }
        }
    }
}
