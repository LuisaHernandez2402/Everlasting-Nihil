using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Keeps the Player alive when changing scenes.
    ///
    /// This allows things stored on the Player,
    /// such as health, inventory, and unlocked
    /// abilities, to survive scene transitions.
    /// </summary>
    public class PersistentPlayer : MonoBehaviour
    {
        #region Singleton

        // Keeps track of the one Player
        // that should exist in the game.
        public static PersistentPlayer Instance
        {
            get;
            private set;
        }

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // If another persistent Player
            // already exists, this Player
            // is a duplicate.
            if (Instance != null &&
                Instance != this)
            {
                // Destroy the duplicate.
                Destroy(gameObject);

                return;
            }


            // Register this Player as
            // the persistent Player.
            Instance = this;


            // Keep the Player when Unity
            // loads another scene.
            DontDestroyOnLoad(gameObject);
        }


        private void OnDestroy()
        {
            // Only clear the singleton if
            // this object was the active Player.
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #endregion
    }
}