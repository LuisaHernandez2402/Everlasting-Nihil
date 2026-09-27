using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    /// <summary>
    /// Collectible used for the Mole Guardian
    /// side quest.
    ///
    /// When touched by the Player, this adds
    /// one Burrow Crystal to PlayerInventory.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class BurrowCrystalPickup : MonoBehaviour
    {
        #region Events

        [Header("Events")]

        // Optional event for particles,
        // sounds, or other collection effects.
        [SerializeField]
        private UnityEvent onCollected;

        #endregion


        #region State

        // Prevents the same pickup from
        // being collected more than once.
        private bool hasBeenCollected;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get the pickup's collider.
            Collider2D pickupCollider =
                GetComponent<Collider2D>();


            // This should detect the Player
            // instead of physically blocking them.
            pickupCollider.isTrigger = true;
        }


        private void OnTriggerEnter2D(
            Collider2D other
        )
        {
            // Stop duplicate collection.
            if (hasBeenCollected)
                return;


            // Search the entering object and
            // its parents for PlayerInventory.
            PlayerInventory inventory =
                other.GetComponentInParent<PlayerInventory>();


            // Ignore anything that isn't
            // part of the Player.
            if (inventory == null)
                return;


            // Mark this pickup as collected.
            hasBeenCollected = true;


            // Add one crystal.
            inventory.AddBurrowCrystal();


            // Trigger optional collection effects.
            onCollected?.Invoke();


            // Remove the crystal from the world.
            Destroy(
                gameObject
            );
        }

        #endregion
    }
}