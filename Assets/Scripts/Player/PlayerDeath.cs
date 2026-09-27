using System.Collections;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Handles the Player's death and respawn sequence.
    ///
    /// When the Player dies:
    /// - Movement is locked.
    /// - Damage is disabled.
    /// - The Player waits briefly.
    /// - The Player moves to the active checkpoint.
    /// - Health is restored.
    /// - Gameplay resumes.
    /// </summary>
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerDeath : MonoBehaviour
    {
        #region Respawn Settings

        [Header("Respawn Settings")]

        // How long we wait after death
        // before respawning the Player.
        [SerializeField] private float respawnDelay = 1f;

        // Backup respawn point.
        //
        // Normally we use CheckpointManager.
        // This exists in case the manager
        // isn't available for some reason.
        [SerializeField] private Transform fallbackRespawnPoint;

        #endregion


        #region Components

        // Player's Health component.
        private Health health;

        // Player's Rigidbody2D.
        private Rigidbody2D rb;

        // Player movement controller.
        private PlayerMovement movement;

        #endregion


        #region State

        // Prevents multiple respawn sequences
        // from running at the same time.
        private bool isRespawning;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get required components.
            health =
                GetComponent<Health>();

            rb =
                GetComponent<Rigidbody2D>();


            // PlayerMovement is optional here,
            // although the Player should normally have it.
            movement =
                GetComponent<PlayerMovement>();
        }


        private void OnEnable()
        {
            // Listen for the Player dying.
            health.Died +=
                HandleDeath;
        }


        private void OnDisable()
        {
            // Stop listening when disabled.
            health.Died -=
                HandleDeath;
        }

        #endregion


        #region Death

        /// <summary>
        /// Called when Health announces that
        /// the Player has died.
        /// </summary>
        private void HandleDeath()
        {
            // Don't start another respawn
            // if one is already happening.
            if (isRespawning)
                return;


            // Start the respawn sequence.
            StartCoroutine(
                RespawnRoutine()
            );
        }

        #endregion


        #region Respawn

        /// <summary>
        /// Handles the complete respawn sequence.
        /// </summary>
        private IEnumerator RespawnRoutine()
        {
            // Mark the Player as respawning.
            isRespawning = true;


            // Stop any existing movement.
            rb.linearVelocity = Vector2.zero;


            // Lock normal Player movement.
            if (movement != null)
            {
                movement.MovementLocked = true;
            }


            // Prevent additional damage while dead.
            health.CanTakeDamage = false;


            // Wait before respawning.
            yield return new WaitForSeconds(
                respawnDelay
            );


            // Ask the CheckpointManager for
            // our current respawn position.
            Vector3 respawnPosition =
                GetRespawnPosition();


            // Move the Player to that position.
            transform.position =
                respawnPosition;


            // Make sure no previous physics velocity
            // survives the respawn.
            rb.linearVelocity =
                Vector2.zero;


            // Restore the Player's health.
            health.RestoreToFullHealth();


            // Allow damage again.
            health.CanTakeDamage = true;


            // Unlock movement.
            if (movement != null)
            {
                movement.MovementLocked = false;
            }


            // Respawn sequence is finished.
            isRespawning = false;
        }


        /// <summary>
        /// Determines where the Player
        /// should respawn.
        /// </summary>
        private Vector3 GetRespawnPosition()
        {
            // Prefer our checkpoint system.
            if (CheckpointManager.Instance != null)
            {
                return CheckpointManager.Instance
                    .GetRespawnPosition();
            }


            // Use the Player's fallback point
            // if the manager doesn't exist.
            if (fallbackRespawnPoint != null)
            {
                return fallbackRespawnPoint.position;
            }


            // Emergency fallback.
            //
            // Ideally this should never happen.
            Debug.LogWarning(
                "No valid Player respawn position was found!"
            );


            return transform.position;
        }

        #endregion
    }
}