
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Marks a location where the Player
    /// can appear after a scene transition.
    /// </summary>
    public class SceneEntrance : MonoBehaviour
    {
        #region Entrance Settings

        [Header("Entrance Settings")]

        // Unique identifier for this entrance.
        // Must match the destination ID
        // configured on a SceneExit.
        [SerializeField] private string entranceID;

        // Exact position where the Player appears.
        [SerializeField] private Transform spawnPoint;

        #endregion

        #region Public Properties

        // Other scripts can read this entrance's ID.
        public string EntranceID => entranceID;

        // Return the assigned spawn position,
        // or this object's position as a fallback.
        public Vector3 SpawnPosition =>
            spawnPoint != null
                ? spawnPoint.position
                : transform.position;

        #endregion

        #region Editor Visualization

        private void OnDrawGizmosSelected()
        {
            // Show where the Player will appear
            // when this entrance is selected.
            Gizmos.DrawWireSphere(
                SpawnPosition,
                0.3f
            );
        }

        #endregion
    }
}
