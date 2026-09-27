using System;
using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    /// <summary>
    /// Emits reaction signals from an object.
    ///
    /// UnityEvents handle permanent Inspector connections.
    /// ReactionEmitted handles runtime connections such
    /// as the Resonate mechanic.
    /// </summary>
    public class ReactionEmitter : MonoBehaviour
    {
        #region Inspector Events

        [Header("Reaction Output")]

        // Permanent reactions configured
        // through the Inspector.
        [SerializeField] private UnityEvent onReaction;

        #endregion


        #region Runtime Events

        // Runtime reaction event.
        //
        // ResonanceNode listens to this.
        public event Action ReactionEmitted;

        #endregion


        #region Reaction Logic

        /// <summary>
        /// Sends a reaction signal.
        /// </summary>
        public void EmitReaction()
        {
            Debug.Log(
                $"{gameObject.name} emitted a reaction."
            );


            // Trigger Inspector reactions.
            onReaction?.Invoke();


            // Trigger runtime reactions.
            ReactionEmitted?.Invoke();
        }

        #endregion
    }
}