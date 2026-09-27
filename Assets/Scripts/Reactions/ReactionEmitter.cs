using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    public class ReactionEmitter : MonoBehaviour
    {
        #region Events

        [Header("Reaction Output")]

        // Anything connected here will react
        // when this object emits a reaction.
        [SerializeField] private UnityEvent onReaction;

        #endregion


        #region Reaction

        public void EmitReaction()
        {
            Debug.Log($"{gameObject.name} emitted a reaction.");

            onReaction?.Invoke();
        }

        #endregion
    }
}