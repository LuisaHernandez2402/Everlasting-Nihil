using System.Collections;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Handles PLAYER-SPECIFIC death and respawning.
    ///
    /// Health.cs announces that the Player died.
    /// This script decides what happens afterward.
    /// </summary>
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerDeath : MonoBehaviour
    {
        #region Respawn Settings

        [Header("Respawn Settings")]

        // Transform representing the position where
        // the Player should return after dying.
        [SerializeField] private Transform respawnPoint;

        // How long the game waits before respawning.
        [SerializeField] private float respawnDelay = 1f;

        #endregion


        #region Components

        // Player's universal Health component.
        private Health health;

        // Player's Rigidbody2D.
        private Rigidbody2D rb;

        // Player's movement controller.
        private PlayerMovement movement;

        #endregion


        #region State

        // Prevents multiple respawn sequences from
        // starting at the same time.
        private bool isRespawning;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get our required components.
            health = GetComponent<Health>();
            rb = GetComponent<Rigidbody2D>();


            // Get the Player's movement script.
            movement = GetComponent<PlayerMovement>();
        }


        private void OnEnable()
        {
            // Listen for the Health death event.
            health.Died += HandleDeath;
        }


        private void OnDisable()
        {
            // Stop listening when this component
            // becomes disabled.
            health.Died -= HandleDeath;
        }

        #endregion


        #region Death

        /// <summary>
        /// Called when the Player's Health reaches zero.
        /// </summary>
        private void HandleDeath()
        {
            // Don't start another death sequence if
            // we're already respawning.
            if (isRespawning)
                return;


            // Start our respawn sequence.
            StartCoroutine(RespawnRoutine());
        }

        #endregion


        #region Respawning

        /// <summary>
        /// Handles the complete Player respawn sequence.
        /// </summary>
        private IEnumerator RespawnRoutine()
        {
            // Mark the Player as respawning.
            isRespawning = true;


            // Immediately stop any current velocity.
            rb.linearVelocity = Vector2.zero;


            // Lock normal Player movement.
            if (movement != null)
            {
                movement.MovementLocked = true;
            }


            // Prevent any additional damage during
            // the death/respawn sequence.
            health.CanTakeDamage = false;


            Debug.Log("Player died. Respawning...");


            // Wait before respawning.
            yield return new WaitForSeconds(
                respawnDelay
            );


            // Move the Player to the assigned
            // respawn location.
            if (respawnPoint != null)
            {
                transform.position =
                    respawnPoint.position;
            }
            else
            {
                // Warn us if we forgot to assign
                // the respawn point in the Inspector.
                Debug.LogWarning(
                    "PlayerDeath has no Respawn Point assigned!"
                );
            }


            // Remove any leftover physics movement.
            rb.linearVelocity = Vector2.zero;


            // Give the Player full health again.
            health.RestoreToFullHealth();


            // Allow damage again.
            health.CanTakeDamage = true;


            // Give movement control back.
            if (movement != null)
            {
                movement.MovementLocked = false;
            }


            // Respawning has finished.
            isRespawning = false;


            Debug.Log("Player respawned.");
        }

        #endregion
    }
}