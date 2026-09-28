using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EverlastingNihil
{
    /// <summary>
    /// Central input handler for the Player.
    ///
    /// This script reads the generated PlayerControls
    /// Input System class and sends events to the
    /// Player's other gameplay scripts.
    ///
    /// Other scripts should listen to these events
    /// instead of directly reading keyboard/controller input.
    ///
    /// No Update() is required.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Input Controls

        // Generated automatically from
        // PlayerControls.inputactions.
        private PlayerControls controls;

        #endregion


        #region Movement Input

        /// <summary>
        /// Current movement direction.
        ///
        /// X:
        /// -1 = Left
        ///  1 = Right
        ///
        /// Y can also be used later if needed.
        /// </summary>
        public Vector2 MoveInput
        {
            get;
            private set;
        }

        #endregion


        #region Input Events

        // -----------------------------
        // JUMP
        // -----------------------------

        // Fired when Jump is initially pressed.
        public event Action JumpPressed;

        // Fired when Jump is released.
        //
        // PlayerMovement uses this for
        // variable jump height / jump cutting.
        public event Action JumpReleased;


        // -----------------------------
        // MOVEMENT ABILITIES
        // -----------------------------

        // Fired when Dash is pressed.
        public event Action DashPressed;


        // -----------------------------
        // COMBAT
        // -----------------------------

        // Fired when Parry is pressed.
        public event Action ParryPressed;

        // Fired when the basic sword
        // attack is pressed.
        public event Action AttackPressed;


        // -----------------------------
        // CRYSTAL ABILITIES
        // -----------------------------

        // Fired when Awaken is pressed.
        public event Action AwakenPressed;

        // Fired when Resonate is pressed.
        public event Action ResonatePressed;

        // Fired when Sever is pressed.
        public event Action SeverPressed;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Create our generated Input System
            // controls object.
            controls =
                new PlayerControls();
        }


        private void OnEnable()
        {
            // -----------------------------
            // ENABLE PLAYER INPUT
            // -----------------------------

            controls.Player.Enable();


            // -----------------------------
            // MOVEMENT
            // -----------------------------

            // Called while movement input
            // is being performed.
            controls.Player.Move.performed +=
                HandleMovePerformed;

            // Called when movement input
            // returns to zero.
            controls.Player.Move.canceled +=
                HandleMoveCanceled;


            // -----------------------------
            // JUMP
            // -----------------------------

            // "started" happens when the
            // Jump button is initially pressed.
            controls.Player.Jump.started +=
                HandleJumpStarted;

            // "canceled" happens when the
            // Jump button is released.
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


            // Disable the entire Player
            // action map.
            controls.Player.Disable();


            // Reset movement so the Player
            // doesn't continue moving if this
            // component gets disabled while
            // a direction is held.
            MoveInput =
                Vector2.zero;
        }

        #endregion


        #region Movement Callbacks

        /// <summary>
        /// Called whenever movement input changes
        /// while being performed.
        /// </summary>
        private void HandleMovePerformed(
            InputAction.CallbackContext context
        )
        {
            // Read our Vector2 movement input.
            MoveInput =
                context.ReadValue<Vector2>();
        }


        /// <summary>
        /// Called when movement input
        /// returns to zero.
        /// </summary>
        private void HandleMoveCanceled(
            InputAction.CallbackContext context
        )
        {
            MoveInput =
                Vector2.zero;
        }

        #endregion


        #region Jump Callbacks

        /// <summary>
        /// Called when Jump is pressed.
        /// </summary>
        private void HandleJumpStarted(
            InputAction.CallbackContext context
        )
        {
            JumpPressed?.Invoke();
        }


        /// <summary>
        /// Called when Jump is released.
        /// </summary>
        private void HandleJumpCanceled(
            InputAction.CallbackContext context
        )
        {
            JumpReleased?.Invoke();
        }

        #endregion


        #region Dash Callback

        /// <summary>
        /// Called when Dash is pressed.
        /// </summary>
        private void HandleDashStarted(
            InputAction.CallbackContext context
        )
        {
            DashPressed?.Invoke();
        }

        #endregion


        #region Parry Callback

        /// <summary>
        /// Called when Parry is pressed.
        /// </summary>
        private void HandleParryStarted(
            InputAction.CallbackContext context
        )
        {
            ParryPressed?.Invoke();
        }

        #endregion


        #region Attack Callback

        /// <summary>
        /// Called when the basic sword
        /// attack is pressed.
        /// </summary>
        private void HandleAttackStarted(
            InputAction.CallbackContext context
        )
        {
            AttackPressed?.Invoke();
        }

        #endregion


        #region Awaken Callback

        /// <summary>
        /// Called when Awaken is pressed.
        /// </summary>
        private void HandleAwakenStarted(
            InputAction.CallbackContext context
        )
        {
            AwakenPressed?.Invoke();
        }

        #endregion


        #region Resonate Callback

        /// <summary>
        /// Called when Resonate is pressed.
        /// </summary>
        private void HandleResonateStarted(
            InputAction.CallbackContext context
        )
        {
            ResonatePressed?.Invoke();
        }

        #endregion


        #region Sever Callback

        /// <summary>
        /// Called when Sever is pressed.
        /// </summary>
        private void HandleSeverStarted(
            InputAction.CallbackContext context
        )
        {
            SeverPressed?.Invoke();
        }

        #endregion
    }
}