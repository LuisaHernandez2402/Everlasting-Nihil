using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls the Main Gate that blocks access
    /// to the final area of the game.
    ///
    /// The gate opens only when the Player owns:
    ///
    /// - Awaken Crystal
    /// - Resonate Crystal
    /// - Sever Crystal
    ///
    /// The crystals are NOT consumed.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MainGate : MonoBehaviour
    {
        #region Gate Settings

        [Header("Gate Settings")]

        // The physical collider blocking the Player.
        //
        // This can be the collider on this GameObject
        // or a separate child object representing
        // the solid portion of the gate.
        [SerializeField] private Collider2D blockingCollider;

        // Optional visual object representing
        // the closed gate.
        //
        // When the gate opens, this object
        // can be disabled.
        [SerializeField] private GameObject gateVisual;

        #endregion


        #region Gate Events

        [Header("Gate Events")]

        // Fired when the Player successfully
        // opens the Main Gate.
        //
        // Later this can trigger:
        // - Animation
        // - Sound
        // - Particles
        // - Camera effects
        // - Story events
        [SerializeField] private UnityEvent onGateOpened;

        // Fired when the Player reaches the gate
        // without owning all three crystals.
        //
        // Later this could display something like:
        //
        // "Three fragments are required."
        [SerializeField] private UnityEvent onGateLocked;

        #endregion


        #region Gate State

        // Prevents the gate from opening
        // more than once.
        public bool IsOpen
        {
            get;
            private set;
        }

        #endregion


        #region Components

        // Trigger used to detect the Player.
        private Collider2D detectionTrigger;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get the collider attached directly
            // to this MainGate object.
            detectionTrigger =
                GetComponent<Collider2D>();

            // This collider detects the Player
            // approaching the gate.
            detectionTrigger.isTrigger = true;


            // Warn us if we forgot to assign
            // the physical blocking collider.
            if (blockingCollider == null)
            {
                Debug.LogWarning(
                    $"{gameObject.name} has no " +
                    "Blocking Collider assigned."
                );
            }
        }

        #endregion


        #region Player Detection

        /// <summary>
        /// Checks the Player's inventory when
        /// they enter the gate's detection area.
        /// </summary>
        private void OnTriggerEnter2D(
            Collider2D other
        )
        {
            // The gate is already open,
            // so nothing else needs to happen.
            if (IsOpen)
                return;


            // Find PlayerInventory on the object
            // entering the trigger or its parents.
            PlayerInventory inventory =
                other.GetComponentInParent<PlayerInventory>();


            // Ignore anything that isn't the Player.
            if (inventory == null)
                return;


            // Check the Player's crystal progression.
            CheckCrystalRequirement(
                inventory
            );
        }

        #endregion


        #region Crystal Requirement

        /// <summary>
        /// Determines whether the Player owns
        /// all three progression crystals.
        /// </summary>
        private void CheckCrystalRequirement(
            PlayerInventory inventory
        )
        {
            // Ask the inventory whether all three
            // crystals have been collected.
            if (inventory.HasAllCrystals())
            {
                // Requirement completed.
                OpenGate();

                return;
            }


            // --------------------------------
            // GATE IS STILL LOCKED
            // --------------------------------

            Debug.Log(
                "The Main Gate is locked. " +
                $"Crystals collected: " +
                $"{inventory.GetCrystalCount()}/3"
            );


            // Trigger optional locked effects.
            onGateLocked?.Invoke();
        }

        #endregion


        #region Gate Opening

        /// <summary>
        /// Permanently opens the Main Gate
        /// for the current game session.
        /// </summary>
        private void OpenGate()
        {
            // Don't open more than once.
            if (IsOpen)
                return;


            // Remember that the gate has opened.
            IsOpen = true;


            Debug.Log(
                "All three crystals detected. " +
                "The Main Gate has opened!"
            );


            // Disable the physical wall so
            // the Player can walk through.
            if (blockingCollider != null)
            {
                blockingCollider.enabled =
                    false;
            }


            // For our temporary version,
            // hide the closed gate visual.
            //
            // Later we'll replace this with
            // an actual opening animation.
            if (gateVisual != null)
            {
                gateVisual.SetActive(
                    false
                );
            }


            // Trigger optional Inspector effects.
            onGateOpened?.Invoke();
        }

        #endregion
    }
}