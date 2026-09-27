using System;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Tracks which horizontal direction
    /// the Player is currently facing.
    ///
    /// Other systems such as:
    /// - PlayerAttack
    /// - PlayerParry
    /// - PlayerDash
    ///
    /// can use this component instead of
    /// calculating facing separately.
    ///
    /// Facing is checked during FixedUpdate()
    /// because Player movement already works
    /// with the physics timestep.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerFacing : MonoBehaviour
    {
        #region Facing Settings

        [Header("Facing Settings")]

        // Optional SpriteRenderer for the Player.
        //
        // If assigned, the sprite will automatically
        // flip when the Player changes direction.
        [SerializeField]
        private SpriteRenderer playerSprite;

        // Determines whether this particular sprite
        // naturally faces right in the artwork.
        //
        // Leave this enabled if the sprite's
        // default direction is right.
        [SerializeField]
        private bool spriteFacesRight = true;

        #endregion


        #region Facing Points

        [Header("Facing Points")]

        // Attack detection point.
        [SerializeField]
        private Transform attackPoint;

        // Parry detection point.
        [SerializeField]
        private Transform parryPoint;

        #endregion


        #region Components

        // Provides the current movement input.
        private PlayerInputHandler inputHandler;

        #endregion


        #region Facing State

        // 1 means right.
        // -1 means left.
        private int facingDirection = 1;

        // Public access for systems such
        // as PlayerDash.
        public int FacingDirection
        {
            get
            {
                return facingDirection;
            }
        }

        // Convenience property for checking
        // whether the Player faces right.
        public bool IsFacingRight
        {
            get
            {
                return facingDirection > 0;
            }
        }

        // Fired whenever the Player
        // actually changes direction.
        public event Action<int> FacingChanged;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Cache our input handler.
            inputHandler =
                GetComponent<PlayerInputHandler>();


            // Apply our starting direction
            // immediately.
            ApplyFacing();
        }


        private void FixedUpdate()
        {
            // Stop safely if the input
            // handler wasn't found.
            if (inputHandler == null)
                return;


            // Read horizontal movement.
            float horizontalInput =
                inputHandler.MoveInput.x;


            // Facing should NOT change while
            // the Player isn't moving horizontally.
            if (Mathf.Abs(horizontalInput) < 0.01f)
                return;


            // Convert movement into either
            // right (+1) or left (-1).
            int newDirection =
                horizontalInput > 0f
                    ? 1
                    : -1;


            // Nothing changed.
            if (newDirection == facingDirection)
                return;


            // Store the new direction.
            facingDirection =
                newDirection;


            // Apply it to our sprite
            // and detection points.
            ApplyFacing();


            // Notify other systems.
            FacingChanged?.Invoke(
                facingDirection
            );
        }

        #endregion


        #region Facing Logic

        /// <summary>
        /// Updates everything that visually or
        /// physically depends on facing direction.
        /// </summary>
        private void ApplyFacing()
        {
            // -----------------------------
            // PLAYER SPRITE
            // -----------------------------

            if (playerSprite != null)
            {
                // If the artwork naturally faces
                // right, flip it when facing left.
                if (spriteFacesRight)
                {
                    playerSprite.flipX =
                        facingDirection < 0;
                }

                // If the artwork naturally faces
                // left, flip it when facing right.
                else
                {
                    playerSprite.flipX =
                        facingDirection > 0;
                }
            }


            // -----------------------------
            // ATTACK POINT
            // -----------------------------

            FlipPoint(
                attackPoint
            );


            // -----------------------------
            // PARRY POINT
            // -----------------------------

            FlipPoint(
                parryPoint
            );
        }


        /// <summary>
        /// Places a child detection point
        /// on the correct side of the Player.
        ///
        /// The absolute X distance is preserved,
        /// while facingDirection determines
        /// whether that distance is left or right.
        /// </summary>
        private void FlipPoint(
            Transform point
        )
        {
            // This point is optional.
            if (point == null)
                return;


            // Read its current local position.
            Vector3 position =
                point.localPosition;


            // Preserve the distance from the Player,
            // but move it to the correct side.
            position.x =
                Mathf.Abs(position.x) *
                facingDirection;


            // Apply the new position.
            point.localPosition =
                position;
        }

        #endregion
    }
}