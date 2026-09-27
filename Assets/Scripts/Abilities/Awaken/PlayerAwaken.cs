using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Handles the Player's Awaken ability.
    ///
    /// Awaken searches for nearby Awakenable objects
    /// and activates them.
    ///
    /// IMPORTANT:
    /// The Player must own the Awaken Crystal before
    /// this ability can be used.
    ///
    /// This system is event-driven and does not
    /// require Update().
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerAbilityManager))]
    public class PlayerAwaken : MonoBehaviour
    {
        #region Awaken Settings

        [Header("Awaken Settings")]

        // The point that acts as the center
        // of the Awaken detection area.
        //
        // Normally this should be a child
        // GameObject of the Player.
        [SerializeField] private Transform awakenPoint;

        // Determines how far Awaken can reach.
        [SerializeField] private float awakenRadius = 1.5f;

        // Determines which layers Awaken
        // is allowed to detect.
        //
        // Set this to AbilityTarget
        // in the Inspector.
        [SerializeField] private LayerMask abilityTargetLayer;

        #endregion


        #region Components

        // Handles Player input events.
        private PlayerInputHandler inputHandler;

        // Determines which abilities the Player
        // has unlocked through their crystals.
        private PlayerAbilityManager abilityManager;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Cache our input handler.
            inputHandler =
                GetComponent<PlayerInputHandler>();

            // Cache our ability progression manager.
            abilityManager =
                GetComponent<PlayerAbilityManager>();
        }


        private void OnEnable()
        {
            // Make sure the input handler exists
            // before subscribing to its event.
            if (inputHandler != null)
            {
                // Listen for the Awaken button.
                inputHandler.AwakenPressed +=
                    HandleAwakenPressed;
            }
        }


        private void OnDisable()
        {
            // Remove our event subscription when
            // this component becomes disabled.
            if (inputHandler != null)
            {
                inputHandler.AwakenPressed -=
                    HandleAwakenPressed;
            }
        }

        #endregion


        #region Awaken Input

        /// <summary>
        /// Called when the Player presses
        /// the Awaken input.
        /// </summary>
        private void HandleAwakenPressed()
        {
            // --------------------------------
            // ABILITY UNLOCK CHECK
            // --------------------------------

            // The Player cannot use Awaken until
            // the Awaken Crystal has been collected.
            if (abilityManager == null ||
                !abilityManager.CanAwaken)
            {
                Debug.Log(
                    "Awaken is locked. " +
                    "Find the Awaken Crystal first."
                );

                return;
            }


            // --------------------------------
            // AWAKEN POINT CHECK
            // --------------------------------

            // Stop safely if the AwakenPoint
            // wasn't assigned in the Inspector.
            if (awakenPoint == null)
            {
                Debug.LogWarning(
                    "PlayerAwaken has no AwakenPoint assigned."
                );

                return;
            }


            // Use the ability.
            PerformAwaken();
        }

        #endregion


        #region Awaken Ability

        /// <summary>
        /// Searches the Awaken radius for
        /// objects containing Awakenable.
        /// </summary>
        private void PerformAwaken()
        {
            // Find every collider inside
            // the Awaken detection radius.
            Collider2D[] targets =
                Physics2D.OverlapCircleAll(
                    awakenPoint.position,
                    awakenRadius,
                    abilityTargetLayer
                );


            // Check every detected target.
            foreach (Collider2D target in targets)
            {
                // Try finding an Awakenable
                // component on this object.
                Awakenable awakenable =
                    target.GetComponent<Awakenable>();


                // If this object cannot be Awakened,
                // simply continue to the next target.
                if (awakenable == null)
                    continue;


                // Activate the target.
                awakenable.Awaken();
            }
        }

        #endregion


        #region Editor Visualization

        /// <summary>
        /// Shows the Awaken detection radius
        /// while editing the game.
        ///
        /// This is editor visualization only
        /// and is not gameplay logic.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // We can't draw the radius without
            // an AwakenPoint.
            if (awakenPoint == null)
                return;


            // Draw the ability's detection range.
            Gizmos.DrawWireSphere(
                awakenPoint.position,
                awakenRadius
            );
        }

        #endregion
    }
}