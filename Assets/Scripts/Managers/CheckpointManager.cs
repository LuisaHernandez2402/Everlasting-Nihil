using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EverlastingNihil
{
    /// <summary>
    /// Stores the Player's most recently
    /// activated checkpoint.
    ///
    /// Instead of storing a reference to a scene
    /// GameObject, we store:
    ///
    /// - Scene name
    /// - Checkpoint ID
    ///
    /// This makes checkpoints safer across scenes.
    /// </summary>
    public class CheckpointManager : MonoBehaviour
    {
        #region Singleton

        // Global access to the active manager.
        public static CheckpointManager Instance
        {
            get;
            private set;
        }

        #endregion


        #region Default Respawn

        [Header("Default Respawn")]

        // Used before the Player has activated
        // their first checkpoint.
        [SerializeField] private Transform defaultRespawnPoint;

        #endregion


        #region Saved Checkpoint

        // Scene containing our active checkpoint.
        private string activeCheckpointScene;

        // Unique ID of our active checkpoint.
        private string activeCheckpointID;

        // True once the Player has activated
        // at least one checkpoint.
        public bool HasCheckpoint =>
            !string.IsNullOrWhiteSpace(activeCheckpointID);

        // Read-only access for other systems.
        public string ActiveCheckpointScene =>
            activeCheckpointScene;

        public string ActiveCheckpointID =>
            activeCheckpointID;

        #endregion


        #region Events

        // Fired when a new checkpoint is activated.
        public event Action<string, string>
            CheckpointActivated;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Prevent duplicate managers.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);

                return;
            }

            // Register this manager.
            Instance = this;

            // Keep it between scenes.
            DontDestroyOnLoad(gameObject);
        }


        private void OnDestroy()
        {
            // Only clear the singleton if this
            // was the active manager.
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #endregion


        #region Checkpoint Management

        /// <summary>
        /// Saves a checkpoint using its
        /// scene name and unique ID.
        /// </summary>
        public void SetCheckpoint(
            string sceneName,
            string checkpointID
        )
        {
            // Reject invalid IDs.
            if (string.IsNullOrWhiteSpace(checkpointID))
            {
                Debug.LogWarning(
                    "Tried to save a checkpoint with no ID."
                );

                return;
            }

            // Store our new checkpoint.
            activeCheckpointScene = sceneName;
            activeCheckpointID = checkpointID;

            Debug.Log(
                $"Checkpoint saved: " +
                $"{activeCheckpointScene} / " +
                $"{activeCheckpointID}"
            );

            // Notify other systems.
            CheckpointActivated?.Invoke(
                activeCheckpointScene,
                activeCheckpointID
            );
        }

        #endregion


        #region Respawn Location

        /// <summary>
        /// Finds the active checkpoint inside
        /// the currently loaded scene.
        /// </summary>
        public Vector3 GetRespawnPosition()
        {
            // If we have a saved checkpoint AND
            // we're currently in its scene,
            // search for that checkpoint.
            if (HasCheckpoint &&
                SceneManager.GetActiveScene().name ==
                activeCheckpointScene)
            {
                // Find all checkpoints currently
                // loaded in this scene.
                Checkpoint[] checkpoints =
                    FindObjectsByType<Checkpoint>(
                        FindObjectsSortMode.None
                    );

                // Find the matching ID.
                foreach (Checkpoint checkpoint in checkpoints)
                {
                    if (checkpoint.CheckpointID ==
                        activeCheckpointID)
                    {
                        Debug.Log(
                            $"Respawning at checkpoint: " +
                            $"{activeCheckpointID}"
                        );

                        return checkpoint.RespawnPosition;
                    }
                }

                // We expected the checkpoint to exist
                // but couldn't find it.
                Debug.LogWarning(
                    $"Checkpoint '{activeCheckpointID}' " +
                    $"was not found in scene " +
                    $"'{activeCheckpointScene}'."
                );
            }

            // Use the original default position
            // if no usable checkpoint exists.
            if (defaultRespawnPoint != null)
            {
                return defaultRespawnPoint.position;
            }

            Debug.LogError(
                "No valid respawn position was found!"
            );

            return Vector3.zero;
        }

        #endregion
    }
}