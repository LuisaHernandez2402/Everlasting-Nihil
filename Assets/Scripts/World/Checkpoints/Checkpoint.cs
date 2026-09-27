using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EverlastingNihil
{
    /// <summary>
    /// Represents a checkpoint inside a game area.
    ///
    /// Every checkpoint has a unique ID that allows
    /// the CheckpointManager to identify it even
    /// after changing scenes.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Checkpoint : MonoBehaviour
    {
        #region Settings

        [Header("Checkpoint Settings")]

        // Unique identifier for this checkpoint.
        //
        // Examples:
        // Omnia_Start
        // Omnia_CrystalRoom
        // Arbora_Entrance
        [SerializeField] private string checkpointID;

        // Exact position where the Player should
        // appear after respawning.
        [SerializeField] private Transform respawnPoint;

        #endregion


        #region Events

        [Header("Checkpoint Events")]

        // Optional effects triggered the first
        // time this checkpoint activates.
        [SerializeField] private UnityEvent onActivated;

        #endregion


        #region State

        // Tracks whether this checkpoint has
        // already been activated during this scene.
        public bool IsActivated
        {
            get;
            private set;
        }

        // Allows other scripts to read this ID.
        public string CheckpointID =>
            checkpointID;

        // Returns the scene containing
        // this checkpoint.
        public string SceneName =>
            gameObject.scene.name;

        // Returns the Player's actual
        // respawn position.
        public Vector3 RespawnPosition =>
            respawnPoint != null
                ? respawnPoint.position
                : transform.position;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get this checkpoint's collider.
            Collider2D checkpointCollider =
                GetComponent<Collider2D>();

            // Checkpoints should always
            // behave as triggers.
            checkpointCollider.isTrigger = true;

            // Warn us about missing IDs.
            if (string.IsNullOrWhiteSpace(checkpointID))
            {
                Debug.LogWarning(
                    $"{gameObject.name} has no Checkpoint ID!"
                );
            }
        }

        #endregion


        #region Player Detection

        /// <summary>
        /// Detects when the Player enters
        /// this checkpoint.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Find PlayerMovement on the entering
            // object or one of its parents.
            PlayerMovement player =
                other.GetComponentInParent<PlayerMovement>();

            // Ignore non-player objects.
            if (player == null)
                return;

            // Activate this checkpoint.
            ActivateCheckpoint();
        }

        #endregion


        #region Activation

        /// <summary>
        /// Sends this checkpoint's information
        /// to the CheckpointManager.
        /// </summary>
        private void ActivateCheckpoint()
        {
            // Make sure our manager exists.
            if (CheckpointManager.Instance == null)
            {
                Debug.LogError(
                    "CheckpointManager does not exist!"
                );

                return;
            }

            // Store this checkpoint's scene and ID.
            CheckpointManager.Instance.SetCheckpoint(
                SceneName,
                checkpointID
            );

            // Don't replay activation effects
            // every time the Player walks through.
            if (IsActivated)
                return;

            IsActivated = true;

            Debug.Log(
                $"Checkpoint activated: {checkpointID}"
            );

            // Play optional Inspector effects.
            onActivated?.Invoke();
        }

        #endregion


        #region Debug Visualization

        private void OnDrawGizmosSelected()
        {
            // Only draw the respawn marker
            // when one exists.
            if (respawnPoint == null)
                return;

            // Draw a line from the checkpoint
            // to its respawn location.
            Gizmos.DrawLine(
                transform.position,
                respawnPoint.position
            );

            // Show the actual respawn location.
            Gizmos.DrawWireSphere(
                respawnPoint.position,
                0.2f
            );
        }

        #endregion
    }
}