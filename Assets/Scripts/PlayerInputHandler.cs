using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EverlastingNihil
{
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Input Values

        // Current movement input.
        // X = horizontal movement
        // Y = vertical input
        public Vector2 MoveInput { get; private set; }

        #endregion


        #region Input Events

        // Other player scripts can subscribe to these events.
        // This keeps the input system separate from the actual abilities.
        public event Action JumpPressed;
        public event Action JumpReleased;

        public event Action DashPressed;

        public event Action ParryPressed;

        #endregion


        #region Input System

        private PlayerControls controls;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Create our generated Input Actions class.
            controls = new PlayerControls();
        }


        private void OnEnable()
        {
            // Enable the Player action map.
            controls.Player.Enable();

            // MOVEMENT
            controls.Player.Move.performed += OnMove;
            controls.Player.Move.canceled += OnMove;

            // JUMP
            controls.Player.Jump.started += OnJumpStarted;
            controls.Player.Jump.canceled += OnJumpCanceled;

            // DASH
            controls.Player.Dash.started += OnDashStarted;

            //PARRY
            controls.Player.Parry.started += OnParryStarted;
        }


        private void OnDisable()
        {
            // Always unsubscribe from events when disabled.
            // Otherwise duplicate subscriptions can happen.

            controls.Player.Move.performed -= OnMove;
            controls.Player.Move.canceled -= OnMove;

            controls.Player.Jump.started -= OnJumpStarted;
            controls.Player.Jump.canceled -= OnJumpCanceled;

            controls.Player.Dash.started -= OnDashStarted;

            controls.Player.Disable();

            controls.Player.Parry.started -= OnParryStarted;
        }

        #endregion

        #region Parry Input

        private void OnParryStarted(InputAction.CallbackContext context)
        {
            ParryPressed?.Invoke();
        }

        #endregion


        #region Movement Input

        private void OnMove(InputAction.CallbackContext context)
        {
            // Save the Vector2 supplied by the Input System.
            MoveInput = context.ReadValue<Vector2>();
        }

        #endregion


        #region Jump Input

        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            JumpPressed?.Invoke();
        }


        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            JumpReleased?.Invoke();
        }

        #endregion


        #region Dash Input

        private void OnDashStarted(InputAction.CallbackContext context)
        {
            DashPressed?.Invoke();
        }

        #endregion
    }
}