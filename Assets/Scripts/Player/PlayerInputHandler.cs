using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EverlastingNihil
{
    /// <summary>
    /// Central input handler for the Player.
    ///
    /// This script reads Unity's New Input System
    /// and sends input to the Player's other systems
    /// through events.
    ///
    /// Individual abilities therefore do not need
    /// to directly check keyboard/controller input.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Movement Input

        // Stores the Player's current movement direction.
        //
        // PlayerMovement and other movement-related
        // systems can safely read this value.
        public Vector2 MoveInput { get; private set; }

        #endregion


        #region Input Events

        // Fired when Jump is pressed.
        public event Action JumpPressed;

        // Fired when Jump is released.
        public event Action JumpReleased;

        // Fired when Dash is pressed.
        public event Action DashPressed;

        // Fired when Parry is pressed.
        public event Action ParryPressed;

        // Fired when Awaken is pressed.
        public event Action AwakenPressed;

        // Fired when Resonate is pressed.
        public event Action ResonatePressed;

        // Fired when Sever is pressed.
        //
        // PlayerSever will listen for this event.
        public event Action SeverPressed;

        #endregion


        #region Input System

        // Generated class created from
        // PlayerControls.inputactions.
        //
        // Do NOT manually edit PlayerControls.cs.
        private PlayerControls controls;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Create our generated input controls.
            controls = new PlayerControls();
        }


        private void OnEnable()
        {
            // Enable the Player action map.
            controls.Player.Enable();


            // -----------------------------
            // MOVEMENT
            // -----------------------------

            controls.Player.Move.performed += OnMove;
            controls.Player.Move.canceled += OnMove;


            // -----------------------------
            // JUMP
            // -----------------------------

            controls.Player.Jump.started += OnJumpStarted;
            controls.Player.Jump.canceled += OnJumpCanceled;


            // -----------------------------
            // DASH
            // -----------------------------

            controls.Player.Dash.started += OnDashStarted;


            // -----------------------------
            // PARRY
            // -----------------------------

            controls.Player.Parry.started += OnParryStarted;


            // -----------------------------
            // AWAKEN
            // -----------------------------

            controls.Player.Awaken.started += OnAwakenStarted;


            // -----------------------------
            // RESONATE
            // -----------------------------

            controls.Player.Resonate.started += OnResonateStarted;


            // -----------------------------
            // SEVER
            // -----------------------------

            controls.Player.Sever.started += OnSeverStarted;
        }


        private void OnDisable()
        {
            // Always unsubscribe from our Input System
            // callbacks when this component is disabled.


            // MOVEMENT
            controls.Player.Move.performed -= OnMove;
            controls.Player.Move.canceled -= OnMove;


            // JUMP
            controls.Player.Jump.started -= OnJumpStarted;
            controls.Player.Jump.canceled -= OnJumpCanceled;


            // DASH
            controls.Player.Dash.started -= OnDashStarted;


            // PARRY
            controls.Player.Parry.started -= OnParryStarted;


            // AWAKEN
            controls.Player.Awaken.started -= OnAwakenStarted;


            // RESONATE
            controls.Player.Resonate.started -= OnResonateStarted;


            // SEVER
            controls.Player.Sever.started -= OnSeverStarted;


            // Disable the Player action map.
            controls.Player.Disable();
        }

        #endregion


        #region Movement Callbacks

        /// <summary>
        /// Reads the current movement Vector2.
        /// </summary>
        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        #endregion


        #region Jump Callbacks

        /// <summary>
        /// Called when Jump is first pressed.
        /// </summary>
        private void OnJumpStarted(
            InputAction.CallbackContext context
        )
        {
            JumpPressed?.Invoke();
        }


        /// <summary>
        /// Called when Jump is released.
        /// </summary>
        private void OnJumpCanceled(
            InputAction.CallbackContext context
        )
        {
            JumpReleased?.Invoke();
        }

        #endregion


        #region Ability Callbacks

        /// <summary>
        /// Announces a Dash request.
        /// </summary>
        private void OnDashStarted(
            InputAction.CallbackContext context
        )
        {
            DashPressed?.Invoke();
        }


        /// <summary>
        /// Announces a Parry request.
        /// </summary>
        private void OnParryStarted(
            InputAction.CallbackContext context
        )
        {
            ParryPressed?.Invoke();
        }


        /// <summary>
        /// Announces an Awaken request.
        /// </summary>
        private void OnAwakenStarted(
            InputAction.CallbackContext context
        )
        {
            AwakenPressed?.Invoke();
        }


        /// <summary>
        /// Announces a Resonate request.
        /// </summary>
        private void OnResonateStarted(
            InputAction.CallbackContext context
        )
        {
            ResonatePressed?.Invoke();
        }


        /// <summary>
        /// Announces a Sever request.
        /// </summary>
        private void OnSeverStarted(
            InputAction.CallbackContext context
        )
        {
            SeverPressed?.Invoke();
        }

        #endregion
    }
}