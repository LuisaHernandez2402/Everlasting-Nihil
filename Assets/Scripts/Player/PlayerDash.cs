using System.Collections;
using UnityEngine;

namespace EverlastingNihil
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerDash : MonoBehaviour
    {
        #region Inspector Settings

        [Header("Dash")]
        
        // How fast the player travels during a dash.
        [SerializeField] private float dashSpeed = 16f;

        // How long the actual dash lasts.
        [SerializeField] private float dashDuration = 0.15f;

        // Small delay before another grounded dash can occur.
        [SerializeField] private float dashCooldown = 0.25f;

        #endregion


        #region Components

        private Rigidbody2D rb;
        private PlayerInputHandler input;
        private PlayerMovement movement;

        #endregion


        #region State

        public bool IsDashing { get; private set; }

        private bool canDash = true;

        // Prevents repeatedly dashing in the air.
        private bool airDashUsed;

        // Remembers the direction the player last moved.
        // Start facing right.
        private float facingDirection = 1f;

        private float gravityBeforeDash;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInputHandler>();
            movement = GetComponent<PlayerMovement>();
        }


        private void OnEnable()
        {
            // PlayerInputHandler tells us when Dash is pressed.
            input.DashPressed += HandleDashPressed;
        }


        private void OnDisable()
        {
            input.DashPressed -= HandleDashPressed;
        }


        private void FixedUpdate()
        {
            UpdateFacingDirection();
            CheckDashReset();
        }

        #endregion


        #region Direction

        private void UpdateFacingDirection()
        {
            float horizontalInput = input.MoveInput.x;

            // Only change facing direction when the player
            // actually gives horizontal input.
            if (horizontalInput > 0.01f)
            {
                facingDirection = 1f;
            }
            else if (horizontalInput < -0.01f)
            {
                facingDirection = -1f;
            }
        }

        #endregion


        #region Dash Input

        private void HandleDashPressed()
        {
            if (!CanStartDash())
                return;

            StartCoroutine(DashRoutine());
        }


        private bool CanStartDash()
        {
            // Can't start another dash while already dashing.
            if (IsDashing)
                return false;

            // Cooldown hasn't finished.
            if (!canDash)
                return false;

            // If we're in the air and already used our air dash,
            // don't allow another one.
            if (!movement.IsGrounded && airDashUsed)
                return false;

            return true;
        }

        #endregion


        #region Dash

        private IEnumerator DashRoutine()
        {
            IsDashing = true;
            canDash = false;

            // If we're airborne, consume our one air dash.
            if (!movement.IsGrounded)
            {
                airDashUsed = true;
            }

            // Normal movement temporarily stops controlling
            // the Rigidbody.
            movement.MovementLocked = true;

            // Save whatever gravity we currently have.
            gravityBeforeDash = rb.gravityScale;

            // Turn gravity off during the dash.
            rb.gravityScale = 0f;

            // Remove vertical velocity and launch horizontally.
            rb.linearVelocity = new Vector2(
                facingDirection * dashSpeed,
                0f
            );

            // Keep this velocity for the dash duration.
            yield return new WaitForSeconds(dashDuration);

            FinishDash();

            // Cooldown occurs AFTER the dash ends.
            yield return new WaitForSeconds(dashCooldown);

            canDash = true;
        }


        private void FinishDash()
        {
            // Restore gravity.
            rb.gravityScale = gravityBeforeDash;

            // Reduce the huge dash velocity before giving
            // control back to normal movement.
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            movement.MovementLocked = false;

            IsDashing = false;
        }

        #endregion


        #region Dash Reset

        private void CheckDashReset()
        {
            // Touching the ground restores the player's
            // air dash.
            if (movement.IsGrounded)
            {
                airDashUsed = false;
            }
        }

        #endregion
    }
}