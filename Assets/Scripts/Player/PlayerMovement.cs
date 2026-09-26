using UnityEngine;

namespace EverlastingNihil
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Inspector Settings

        [Header("Horizontal Movement")]
        [SerializeField] private float maxMoveSpeed = 7f;
        [SerializeField] private float acceleration = 45f;
        [SerializeField] private float deceleration = 55f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 12f;

        // Lets the player still jump for a tiny moment after
        // walking off a platform.
        [SerializeField] private float coyoteTime = 0.12f;

        // Remembers a jump press slightly before landing.
        [SerializeField] private float jumpBufferTime = 0.12f;

        [Header("Gravity")]
        [SerializeField] private float normalGravity = 3f;

        // Makes falling feel faster and less floaty.
        [SerializeField] private float fallGravityMultiplier = 1.8f;

        // Used when the player releases Jump early.
        [SerializeField] private float jumpCutMultiplier = 0.5f;

        [SerializeField] private float maxFallSpeed = 20f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        #endregion


        #region Components

        private Rigidbody2D rb;
        private PlayerInputHandler input;

        #endregion


        #region State

        public bool IsGrounded { get; private set; }

        // When true, another movement ability such as Dash
        // has temporary control of the Rigidbody.
        public bool MovementLocked { get; set; }

        private float coyoteTimer;
        private float jumpBufferTimer;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInputHandler>();

            rb.gravityScale = normalGravity;
        }


        private void OnEnable()
        {
            input.JumpPressed += HandleJumpPressed;
            input.JumpReleased += HandleJumpReleased;
        }


        private void OnDisable()
        {
            input.JumpPressed -= HandleJumpPressed;
            input.JumpReleased -= HandleJumpReleased;
        }


        private void FixedUpdate()
        {
            CheckGround();

            UpdateTimers();

            // Another ability currently controls movement.
            if (MovementLocked)
                return;

            HandleHorizontalMovement();
            HandleJump();
            HandleGravity();
        }

        #endregion


        #region Horizontal Movement

        private void HandleHorizontalMovement()
        {
            float inputDirection = input.MoveInput.x;

            // The speed we WANT to reach.
            float targetSpeed = inputDirection * maxMoveSpeed;

            // Accelerate when receiving movement input.
            // Decelerate faster when the player lets go.
            float speedChangeRate =
                Mathf.Abs(inputDirection) > 0.01f
                ? acceleration
                : deceleration;

            float newHorizontalSpeed = Mathf.MoveTowards(
                rb.linearVelocity.x,
                targetSpeed,
                speedChangeRate * Time.fixedDeltaTime
            );

            rb.linearVelocity = new Vector2(
                newHorizontalSpeed,
                rb.linearVelocity.y
            );
        }

        #endregion


        #region Jump Input

        private void HandleJumpPressed()
        {
            // Don't immediately decide whether we're allowed
            // to jump.
            //
            // Instead, remember that the player pressed Jump.
            jumpBufferTimer = jumpBufferTime;
        }


        private void HandleJumpReleased()
        {
            // VARIABLE JUMP HEIGHT
            //
            // If we're still traveling upward when Jump is
            // released, reduce that upward velocity.
            //
            // Tapping Jump = short jump.
            // Holding Jump = full jump.

            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * jumpCutMultiplier
                );
            }
        }

        #endregion


        #region Jump Logic

        private void HandleJump()
        {
            // We can jump when:
            //
            // 1. Jump was pressed recently.
            // 2. We are grounded OR within coyote time.

            if (jumpBufferTimer > 0f &&
                coyoteTimer > 0f)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    jumpForce
                );

                // Consume both timers.
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }
        }

        #endregion


        #region Timers

        private void UpdateTimers()
        {
            // COYOTE TIME
            if (IsGrounded)
            {
                coyoteTimer = coyoteTime;
            }
            else
            {
                coyoteTimer -= Time.fixedDeltaTime;
            }


            // JUMP BUFFER
            if (jumpBufferTimer > 0f)
            {
                jumpBufferTimer -= Time.fixedDeltaTime;
            }
        }

        #endregion


        #region Gravity

        private void HandleGravity()
        {
            // Falling
            if (rb.linearVelocity.y < 0f)
            {
                rb.gravityScale =
                    normalGravity * fallGravityMultiplier;
            }
            else
            {
                rb.gravityScale = normalGravity;
            }


            // Terminal velocity
            if (rb.linearVelocity.y < -maxFallSpeed)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    -maxFallSpeed
                );
            }
        }

        #endregion


        #region Ground Detection

        private void CheckGround()
        {
            if (groundCheck == null)
                return;

            IsGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        #endregion


        #region Debug

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null)
                return;

            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }

        #endregion
    }
}