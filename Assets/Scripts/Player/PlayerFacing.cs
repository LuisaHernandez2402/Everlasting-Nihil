using System;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls which horizontal direction the Player is facing.
    ///
    /// This script:
    /// - Reads horizontal movement input.
    /// - Flips the Player sprite.
    /// - Moves the AttackPoint to the correct side.
    /// - Moves the ParryPoint to the correct side.
    /// - Provides the current facing direction to other scripts.
    ///
    /// No Update() is used.
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerFacing : MonoBehaviour
    {
        #region References

        [Header("References")]

        // SpriteRenderer containing the
        // Player's visible sprite.
        [SerializeField]
        private SpriteRenderer playerSprite;

        // Point used by PlayerAttack.
        [SerializeField]
        private Transform attackPoint;

        // Point used by PlayerParry.
        [SerializeField]
        private Transform parryPoint;

        #endregion


        #region Sprite Settings

        [Header("Sprite Settings")]

        // Turn this ON if the original
        // Player sprite naturally faces right.
        //
        // Turn this OFF if the original
        // Player sprite naturally faces left.
        [SerializeField]
        private bool spriteFacesRight = true;

        #endregion


        #region Components

        // Handles our Input System controls.
        private PlayerInputHandler inputHandler;

        #endregion


        #region Facing State

        // 1 means facing right.
        // -1 means facing left.
        private int facingDirection = 1;


        /// <summary>
        /// Current horizontal facing direction.
        ///
        /// 1 = Right
        /// -1 = Left
        /// </summary>
        public int FacingDirection
        {
            get
            {
                return facingDirection;
            }
        }


        /// <summary>
        /// Returns true when the Player
        /// is currently facing right.
        /// </summary>
        public bool IsFacingRight
        {
            get
            {
                return facingDirection > 0;
            }
        }


        // Other systems can listen for
        // changes in facing direction.
        public event Action<int> FacingChanged;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Cache our PlayerInputHandler.
            inputHandler =
                GetComponent<PlayerInputHandler>();


            // Apply the starting direction
            // immediately.
            ApplyFacing();
        }


        private void FixedUpdate()
        {
            // Stop if InputHandler is missing.
            if (inputHandler == null)
                return;


            // Read horizontal movement.
            float horizontalInput =
                inputHandler.MoveInput.x;


            // Don't change direction if the
            // Player isn't pressing left/right.
            if (Mathf.Abs(horizontalInput) <
                0.01f)
            {
                return;
            }


            // Determine which direction
            // the Player wants to face.
            int newDirection =
                horizontalInput > 0f
                    ? 1
                    : -1;


            // Don't do anything if we're
            // already facing that direction.
            if (newDirection ==
                facingDirection)
            {
                return;
            }


            // Store our new direction.
            facingDirection =
                newDirection;


            // Update sprite and interaction points.
            ApplyFacing();


            // Notify any other systems
            // listening for direction changes.
            FacingChanged?.Invoke(
                facingDirection
            );
        }

        #endregion


        #region Facing

        /// <summary>
        /// Applies the current facing direction
        /// to the Player's visual and action points.
        /// </summary>
        private void ApplyFacing()
        {
            // -----------------------------
            // FLIP PLAYER SPRITE
            // -----------------------------

            if (playerSprite != null)
            {
                // If the original sprite faces right:
                //
                // Facing Right = flipX false
                // Facing Left  = flipX true
                //
                // If the original sprite faces left,
                // the behavior is reversed.
                if (spriteFacesRight)
                {
                    playerSprite.flipX =
                        facingDirection < 0;
                }
                else
                {
                    playerSprite.flipX =
                        facingDirection > 0;
                }
            }


            // -----------------------------
            // FLIP ATTACK POINT
            // -----------------------------

            FlipPoint(
                attackPoint
            );


            // -----------------------------
            // FLIP PARRY POINT
            // -----------------------------

            FlipPoint(
                parryPoint
            );
        }


        /// <summary>
        /// Moves a child Transform to the
        /// correct horizontal side of the Player.
        ///
        /// The point keeps the same distance
        /// from the Player.
        /// </summary>
        private void FlipPoint(
            Transform point
        )
        {
            // Ignore missing points.
            if (point == null)
                return;


            // Get the point's current
            // local position.
            Vector3 position =
                point.localPosition;


            // Keep its distance from the Player
            // but move it to the correct side.
            position.x =
                Mathf.Abs(position.x) *
                facingDirection;


            // Apply the position.
            point.localPosition =
                position;
        }

        #endregion
    }
}