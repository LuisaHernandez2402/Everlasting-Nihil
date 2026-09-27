using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    /// <summary>
    /// Represents one of the three collectible
    /// progression crystals.
    ///
    /// When the Player touches the crystal:
    ///
    /// 1. Find PlayerInventory.
    /// 2. Add the crystal.
    /// 3. Trigger optional effects.
    /// 4. Remove the world pickup.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CrystalPickup : MonoBehaviour
    {
        #region Crystal Settings

        [Header("Crystal Settings")]

        // Determines which progression crystal
        // this pickup represents.
        [SerializeField] private CrystalType crystalType;

        #endregion


        #region Collection Events

        [Header("Collection Events")]

        // Optional Inspector event.
        //
        // Later this can trigger:
        // - Collection animation
        // - Sound
        // - Particle effect
        // - UI notification
        [SerializeField] private UnityEvent onCollected;

        #endregion


        #region State

        // Prevents the pickup from being
        // collected multiple times.
        private bool hasBeenCollected;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get the pickup's collider.
            Collider2D pickupCollider =
                GetComponent<Collider2D>();


            // The Player should pass through
            // the crystal rather than collide with it.
            pickupCollider.isTrigger = true;
        }

        #endregion


        #region Collection Detection

        /// <summary>
        /// Detects when the Player touches
        /// the crystal.
        /// </summary>
        private void OnTriggerEnter2D(
            Collider2D other
        )
        {
            // Don't process collection twice.
            if (hasBeenCollected)
                return;


            // Search the entering object and its
            // parents for PlayerInventory.
            PlayerInventory inventory =
                other.GetComponentInParent<PlayerInventory>();


            // Ignore anything that isn't the Player.
            if (inventory == null)
                return;


            // Try adding the crystal.
            bool collected =
                inventory.AddCrystal(
                    crystalType
                );


            // Stop if the Player already owns it.
            if (!collected)
                return;


            // Mark this pickup as collected.
            hasBeenCollected = true;


            // Trigger optional Inspector effects.
            onCollected?.Invoke();


            // Remove the crystal from the world.
            Destroy(gameObject);
        }

        #endregion
    }
}