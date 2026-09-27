using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls transitions between world scenes.
    ///
    /// Transition sequence:
    ///
    /// 1. Fade screen to black.
    /// 2. Load destination scene.
    /// 3. Find destination entrance.
    /// 4. Move Player there.
    /// 5. Fade back into gameplay.
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        #region Singleton

        // Global access to the transition manager.
        public static SceneTransitionManager Instance
        {
            get;
            private set;
        }

        #endregion


        #region Settings

        [Header("Transition Settings")]

        // Handles the visual screen fade.
        [SerializeField] private ScreenFader screenFader;

        #endregion


        #region Transition State

        // Prevents multiple exits from triggering
        // simultaneously.
        private bool isTransitioning;

        // Entrance we want to find
        // after loading the destination.
        private string destinationEntranceID;

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


            // Register this instance.
            Instance = this;


            // Keep this manager between scenes.
            DontDestroyOnLoad(gameObject);
        }


        private void OnDestroy()
        {
            // Only clear Instance if this
            // was the active manager.
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #endregion


        #region Transition Requests

        /// <summary>
        /// Requests travel to another scene.
        /// </summary>
        public void TransitionTo(
            string sceneName,
            string entranceID
        )
        {
            // Ignore additional requests while
            // a transition is already happening.
            if (isTransitioning)
                return;


            // Reject invalid scene names.
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError(
                    "Scene transition has no destination scene!"
                );

                return;
            }


            // Remember the entrance we want
            // after the scene loads.
            destinationEntranceID =
                entranceID;


            // Begin the transition.
            StartCoroutine(
                TransitionRoutine(sceneName)
            );
        }

        #endregion


        #region Transition Routine

        /// <summary>
        /// Performs the complete transition.
        /// </summary>
        private IEnumerator TransitionRoutine(
            string sceneName
        )
        {
            // Lock additional transition requests.
            isTransitioning = true;


            // -----------------------------
            // FADE OUT
            // -----------------------------

            if (screenFader != null)
            {
                yield return screenFader.FadeOut();
            }


            // -----------------------------
            // LOAD SCENE
            // -----------------------------

            AsyncOperation loading =
                SceneManager.LoadSceneAsync(
                    sceneName
                );


            // Stop safely if Unity couldn't
            // begin loading the scene.
            if (loading == null)
            {
                Debug.LogError(
                    $"Could not load scene: {sceneName}"
                );


                isTransitioning = false;

                yield break;
            }


            // Wait until loading finishes.
            while (!loading.isDone)
            {
                yield return null;
            }


            // Wait one additional frame so the
            // destination scene can initialize.
            yield return null;


            // -----------------------------
            // PLACE PLAYER
            // -----------------------------

            PlacePlayerAtEntrance();


            // -----------------------------
            // FADE IN
            // -----------------------------

            if (screenFader != null)
            {
                yield return screenFader.FadeIn();
            }


            // Transition is complete.
            isTransitioning = false;
        }

        #endregion


        #region Player Placement

        /// <summary>
        /// Finds the requested entrance in the
        /// newly loaded scene and places the Player there.
        /// </summary>
        private void PlacePlayerAtEntrance()
        {
            // Find the Player in the new scene.
            PlayerMovement player =
                FindFirstObjectByType<PlayerMovement>();


            if (player == null)
            {
                Debug.LogError(
                    "No Player exists in the destination scene!"
                );

                return;
            }


            // Find every entrance in the scene.
            SceneEntrance[] entrances =
                FindObjectsByType<SceneEntrance>(
                    FindObjectsSortMode.None
                );


            // Search for the requested entrance.
            foreach (SceneEntrance entrance in entrances)
            {
                // Skip entrances that don't match.
                if (entrance.EntranceID !=
                    destinationEntranceID)
                {
                    continue;
                }


                // Move the Player.
                player.transform.position =
                    entrance.SpawnPosition;


                // Remove leftover velocity.
                Rigidbody2D rb =
                    player.GetComponent<Rigidbody2D>();


                if (rb != null)
                {
                    rb.linearVelocity =
                        Vector2.zero;
                }


                Debug.Log(
                    $"Player entered " +
                    $"{SceneManager.GetActiveScene().name} " +
                    $"through {destinationEntranceID}."
                );


                return;
            }


            // If we reach this point, the ID
            // didn't exist in the destination scene.
            Debug.LogError(
                $"Entrance '{destinationEntranceID}' " +
                $"was not found in scene " +
                $"'{SceneManager.GetActiveScene().name}'."
            );
        }

        #endregion
    }
}