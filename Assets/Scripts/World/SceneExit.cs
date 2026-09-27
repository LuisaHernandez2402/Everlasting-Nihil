
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Loads another scene when the Player
    /// enters this object's trigger.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SceneExit : MonoBehaviour
    {
        #region Destination Settings

        [Header("Destination")]

        // Name of the scene we want to load.
        [SerializeField] private string destinationScene;

        // Entrance ID inside the destination scene.
        [SerializeField] private string destinationEntranceID;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // Ensure the exit collider is a trigger.
            GetComponent<Collider2D>().isTrigger = true;
        }

        #endregion

        #region Trigger Detection

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Confirm that the entering collider
            // belongs to the Player.
            PlayerMovement player =
                other.GetComponentInParent<PlayerMovement>();

            if (player == null)
                return;

            // Make sure our transition manager exists.
            if (SceneTransitionManager.Instance == null)
            {
                Debug.LogError(
                    "SceneTransitionManager is missing!"
                );
                return;
            }

            // Request the destination scene
            // and the entrance where we should appear.
            SceneTransitionManager.Instance.TransitionTo(
                destinationScene,
                destinationEntranceID
            );
        }

        #endregion
    }
}
