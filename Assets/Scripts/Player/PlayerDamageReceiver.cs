using System.Collections;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Handles PLAYER-SPECIFIC reactions to taking damage.
    ///
    /// Health.cs changes the actual health value.
    /// This script handles temporary invincibility and
    /// visual damage feedback.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class PlayerDamageReceiver : MonoBehaviour
    {
        #region Damage Settings

        [Header("Damage Response")]

        // How long the player is protected from additional
        // damage after being hit.
        [SerializeField] private float invincibilityDuration = 1f;

        #endregion


        #region Visual Settings

        [Header("Damage Visuals")]

        // Player's SpriteRenderer.
        //
        // We disable/enable this during i-frames
        // to create a flashing effect.
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Time between each flash.
        [SerializeField] private float flashInterval = 0.1f;

        #endregion


        #region Components

        // Universal Health component attached to the Player.
        private Health health;

        #endregion


        #region State

        // Lets other systems check whether the player
        // is currently invincible.
        public bool IsInvincible { get; private set; }

        // Stores our running invincibility coroutine.
        private Coroutine invincibilityCoroutine;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Find Health on the Player.
            health = GetComponent<Health>();


            // Automatically search for the player's
            // SpriteRenderer if one wasn't assigned manually.
            if (spriteRenderer == null)
            {
                spriteRenderer =
                    GetComponentInChildren<SpriteRenderer>();
            }
        }


        private void OnEnable()
        {
            // Listen for successful damage from Health.
            health.Damaged += HandleDamaged;
        }


        private void OnDisable()
        {
            // Stop listening when this component is disabled.
            health.Damaged -= HandleDamaged;
        }

        #endregion


        #region Damage Response

        /// <summary>
        /// Called whenever Health successfully takes damage.
        /// </summary>
        private void HandleDamaged(int damageAmount)
        {
            Debug.Log(
                $"Player damage response triggered for " +
                $"{damageAmount} damage."
            );


            // Stop an existing invincibility routine
            // before starting another one.
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
            }


            // Start our temporary invincibility.
            invincibilityCoroutine =
                StartCoroutine(InvincibilityRoutine());
        }

        #endregion


        #region Invincibility

        /// <summary>
        /// Temporarily prevents the Player from receiving
        /// additional damage while creating a flashing effect.
        /// </summary>
        private IEnumerator InvincibilityRoutine()
        {
            // Mark the player as invincible.
            IsInvincible = true;


            // Tell Health to reject incoming damage.
            health.CanTakeDamage = false;


            // Track how much time has passed.
            float elapsedTime = 0f;


            // Continue flashing until our i-frames expire.
            while (elapsedTime < invincibilityDuration)
            {
                // Hide the sprite.
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = false;
                }


                // Wait before showing it again.
                yield return new WaitForSeconds(
                    flashInterval
                );


                // Show the sprite.
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true;
                }


                // Wait before beginning another flash.
                yield return new WaitForSeconds(
                    flashInterval
                );


                // Add the duration of this complete
                // flash cycle.
                elapsedTime += flashInterval * 2f;
            }


            // Always make sure the player is visible
            // when the effect ends.
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }


            // Allow Health to receive damage again.
            health.CanTakeDamage = true;


            // Player is no longer invincible.
            IsInvincible = false;


            // Clear our coroutine reference.
            invincibilityCoroutine = null;
        }

        #endregion
    }
}