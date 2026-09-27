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

        // Fired when the Jump button is pressed.
        public event Action JumpPressed;

        // Fired when the Jump button is released.
        public event Action JumpReleased;

        // Fired when Dash is pressed.
        public event Action DashPressed;

        // Fired when Parry is pressed.
        public event Action ParryPressed;

        // Fired when Awaken is pressed.
        public event Action AwakenPressed;

        // Fired when Resonate is pressed.
        //
        // PlayerResonate will listen for this event.
        public event Action ResonatePressed;

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

            //AWAKEN
            controls.Player.Awaken.started += OnAwakenStarted;

            //RESONATE
            // Listen for the Resonate input.
            //
            // "started" means this happens once when
            // the button is initially pressed.
            controls.Player.Resonate.started += OnResonateStarted;
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

            controls.Player.Awaken.started -= OnAwakenStarted;

            // Stop listening for the Resonate input
            // when this component is disabled.
            controls.Player.Resonate.started -= OnResonateStarted;
        }

        #endregion

        /// <summary>
        /// Called by Unity's Input System when
        /// the Resonate button is pressed.
        /// </summary>
        private void OnResonateStarted(
            InputAction.CallbackContext context
        )
        {
            // Notify anything listening for
            // the Player's Resonate input.
            ResonatePressed?.Invoke();
        }

        #region Parry Input

        private void OnParryStarted(InputAction.CallbackContext context)
        {
            ParryPressed?.Invoke();
        }

        #endregion

        #region Awaken Input

        private void OnAwakenStarted(InputAction.CallbackContext context)
        {
            AwakenPressed?.Invoke();
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