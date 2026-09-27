using System;
using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    /// <summary>
    /// A puzzle switch that activates when it
    /// receives a reaction.
    ///
    /// Because this class implements IReactable,
    /// reactions can reach it through systems such
    /// as Resonate.
    ///
    /// Other systems, such as PuzzleController,
    /// can listen for this switch being activated.
    /// </summary>
    public class ReactionSwitch : MonoBehaviour, IReactable
    {
        #region Switch Settings

        [Header("Switch Settings")]

        // If true, this switch can only activate once.
        //
        // For our first puzzle we'll leave this enabled.
        [SerializeField] private bool activateOnce = true;

        #endregion


        #region Visual Settings

        [Header("Visual Settings")]

        // SpriteRenderer belonging to this switch.
        //
        // This is optional, but it gives us easy
        // visual feedback while testing.
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Color shown before the switch activates.
        [SerializeField] private Color inactiveColor = Color.white;

        // Color shown after the switch activates.
        [SerializeField] private Color activeColor = Color.green;

        #endregion


        #region Unity Events

        [Header("Switch Events")]

        // Inspector event fired when this
        // switch successfully activates.
        //
        // Later we can connect animations,
        // sounds, particles, doors, etc.
        [SerializeField] private UnityEvent onActivated;

        #endregion


        #region C# Events

        // Event used by scripts such as PuzzleController.
        //
        // We use a C# event here so other gameplay
        // systems can subscribe without requiring
        // Inspector connections.
        public event Action<ReactionSwitch> Activated;

        #endregion


        #region Switch State

        // Tracks whether this switch is currently active.
        public bool IsActive { get; private set; }

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // If no SpriteRenderer was manually assigned,
            // try finding one on this GameObject.
            if (spriteRenderer == null)
            {
                spriteRenderer =
                    GetComponent<SpriteRenderer>();
            }


            // Make sure the switch begins
            // in its inactive visual state.
            UpdateVisual();
        }

        #endregion


        #region Reaction Logic

        /// <summary>
        /// Called when this object receives a reaction.
        ///
        /// This method comes from IReactable.
        /// </summary>
        public void React()
        {
            // If this switch only activates once
            // and it has already activated,
            // don't activate it again.
            if (activateOnce && IsActive)
                return;


            // Activate the switch.
            Activate();
        }

        #endregion


        #region Switch Logic

        /// <summary>
        /// Activates this switch and notifies
        /// anything listening for activation.
        /// </summary>
        private void Activate()
        {
            // Mark the switch as active.
            IsActive = true;


            // Update its appearance.
            UpdateVisual();


            // Print useful testing information.
            Debug.Log(
                $"{gameObject.name} activated!"
            );


            // Trigger anything connected through
            // the Unity Inspector.
            onActivated?.Invoke();


            // Notify gameplay scripts such as
            // our future PuzzleController.
            Activated?.Invoke(this);
        }


        /// <summary>
        /// Resets the switch to its inactive state.
        ///
        /// This will be useful if we later create
        /// puzzles that can reset after failure.
        /// </summary>
        public void ResetSwitch()
        {
            // Return to inactive.
            IsActive = false;


            // Restore the inactive appearance.
            UpdateVisual();


            Debug.Log(
                $"{gameObject.name} reset."
            );
        }

        #endregion


        #region Visual Logic

        /// <summary>
        /// Updates the switch color based on
        /// whether it is active or inactive.
        /// </summary>
        private void UpdateVisual()
        {
            // If this switch doesn't have a
            // SpriteRenderer, there's nothing
            // visual for us to change.
            if (spriteRenderer == null)
                return;


            // Green while active.
            //
            // White while inactive.
            spriteRenderer.color =
                IsActive
                    ? activeColor
                    : inactiveColor;
        }

        #endregion
    }
}