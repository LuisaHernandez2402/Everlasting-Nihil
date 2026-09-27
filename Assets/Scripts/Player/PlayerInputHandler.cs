using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EverlastingNihil
{
    /// <summary>
    /// Central input handler for the Player.
    ///
    /// This class reads the Unity Input System
    /// and converts inputs into values and events
    /// that the Player's other systems can use.
    ///
    /// This keeps movement, combat, and abilities
    /// separated from the actual input system.
    ///
    /// No Update() is required.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Input Controls

        // Generated input class created from
        // PlayerControls.inputactions.
        private PlayerControls controls;

        #endregion


        #region Movement Input

        // Current movement direction.
        //
        // PlayerMovement reads this value
        // during FixedUpdate().
        public Vector2 MoveInput
        {
            get;
            private set;
        }

        #endregion


        #region Input Events

        // Jump events.
        public event Action JumpPressed;
        public event Action JumpReleased;

        // Movement ability events.
        public event Action DashPressed;
        public event Action ParryPressed;

        // Combat event.
        public event Action AttackPressed;

        // Main power events.
        public event Action AwakenPressed;
        public event Action ResonatePressed;
        public event Action SeverPressed;

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

            controls.Player.Move.performed +=
                HandleMovePerformed;

            controls.Player.Move.canceled +=
                HandleMoveCanceled;


            // -----------------------------
            // JUMP
            // -----------------------------

            controls.Player.Jump.started +=
                HandleJumpStarted;

            controls.Player.Jump.canceled +=
                HandleJumpCanceled;


            // -----------------------------
            // DASH
            // -----------------------------

            controls.Player.Dash.started +=
                HandleDashStarted;


            // -----------------------------
            // PARRY
            // -----------------------------

            controls.Player.Parry.started +=
                HandleParryStarted;


            // -----------------------------
            // ATTACK
            // -----------------------------

            controls.Player.Attack.started +=
                HandleAttackStarted;


            // -----------------------------
            // AWAKEN
            // -----------------------------

            controls.Player.Awaken.started +=
                HandleAwakenStarted;


            // -----------------------------
            // RESONATE
            // -----------------------------

            controls.Player.Resonate.started +=
                HandleResonateStarted;


            // -----------------------------
            // SEVER
            // -----------------------------

            controls.Player.Sever.started +=
                HandleSeverStarted;
        }


        private void OnDisable()
        {
            // -----------------------------
            // MOVEMENT
            // -----------------------------

            controls.Player.Move.performed -=
                HandleMovePerformed;

            controls.Player.Move.canceled -=
                HandleMoveCanceled;


            // -----------------------------
            // JUMP
            // -----------------------------

            controls.Player.Jump.started -=
                HandleJumpStarted;

            controls.Player.Jump.canceled -=
                HandleJumpCanceled;


            // -----------------------------
            // DASH
            // -----------------------------

            controls.Player.Dash.started -=
                HandleDashStarted;


            // -----------------------------
            // PARRY
            // -----------------------------

            controls.Player.Parry.started -=
                HandleParryStarted;


            // -----------------------------
            // ATTACK
            // -----------------------------

            controls.Player.Attack.started -=
                HandleAttackStarted;


            // -----------------------------
            // AWAKEN
            // -----------------------------

            controls.Player.Awaken.started -=
                HandleAwakenStarted;


            // -----------------------------
            // RESONATE
            // -----------------------------

            controls.Player.Resonate.started -=
                HandleResonateStarted;


            // -----------------------------
            // SEVER
            // -----------------------------

            controls.Player.Sever.started -=
                HandleSeverStarted;


            // Disable the Player action map.
            controls.Player.Disable();
        }

        #endregion


        #region Movement Callbacks

        /// <summary>
        /// Called whenever the movement
        /// input changes.
        /// </summary>
        private void HandleMovePerformed(
            InputAction.CallbackContext context
        )
        {
            // Read the current Vector2 movement input.
            MoveInput =
                context.ReadValue<Vector2>();
        }


        /// <summary>
        /// Called when movement input is released.
        /// </summary>
        private void HandleMoveCanceled(
            InputAction.CallbackContext context
        )
        {
            // Reset movement to zero.
            MoveInput =
                Vector2.zero;
        }

        #endregion


        #region Jump Callbacks

        private void HandleJumpStarted(
            InputAction.CallbackContext context
        )
        {
            // Notify PlayerMovement that
            // jump was pressed.
            JumpPressed?.Invoke();
        }


        private void HandleJumpCanceled(
            InputAction.CallbackContext context
        )
        {
            // Notify PlayerMovement that
            // jump was released.
            JumpReleased?.Invoke();
        }

        #endregion


        #region Combat Callbacks

        private void HandleDashStarted(
            InputAction.CallbackContext context
        )
        {
            // Notify PlayerDash.
            DashPressed?.Invoke();
        }


        private void HandleParryStarted(
            InputAction.CallbackContext context
        )
        {
            // Notify PlayerParry.
            ParryPressed?.Invoke();
        }


        private void HandleAttackStarted(
            InputAction.CallbackContext context
        )
        {
            // Notify PlayerAttack.
            AttackPressed?.Invoke();
        }

        #endregion


        #region Ability Callbacks

        private void HandleAwakenStarted(
            InputAction.CallbackContext context
        )
        {
            // Notify PlayerAwaken.
            AwakenPressed?.Invoke();
        }


        private void HandleResonateStarted(
            InputAction.CallbackContext context
        )
        {
            // Notify PlayerResonate.
            ResonatePressed?.Invoke();
        }


        private void HandleSeverStarted(
            InputAction.CallbackContext context
        )
        {
            // Notify PlayerSever.
            SeverPressed?.Invoke();
        }

        #endregion
    }
}