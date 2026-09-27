using System;
using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    /// <summary>
    /// Sends reaction signals to other systems.
    ///
    /// This supports:
    /// 1. UnityEvents for connections made in the Inspector.
    /// 2. C# events for connections made during gameplay,
    ///    such as the Resonate ability.
    /// </summary>
    public class ReactionEmitter : MonoBehaviour
    {
        #region Inspector Events

        [Header("Reaction Output")]

        // This event can be configured directly
        // through the Unity Inspector.
        //
        // We can use this for permanent reactions
        // that are already known when designing a puzzle.
        [SerializeField] private UnityEvent onReaction;

        #endregion


        #region Runtime Events

        // This event allows other scripts to listen
        // for reactions during gameplay.
        //
        // ResonanceNode uses this to detect when
        // its object has emitted a reaction.
        public event Action ReactionEmitted;

        #endregion


        #region Reaction Logic

        /// <summary>
        /// Emits a reaction from this object.
        /// </summary>
        public void EmitReaction()
        {
            // Print the reaction for testing/debugging.
            Debug.Log(
                $"{gameObject.name} emitted a reaction."
            );


            // Trigger anything connected through
            // the Unity Inspector.
            onReaction?.Invoke();


            // Notify scripts listening through code.
            //
            // ResonanceNode listens here.
            ReactionEmitted?.Invoke();
        }

        #endregion
    }
}