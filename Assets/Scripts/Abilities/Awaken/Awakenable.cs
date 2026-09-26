using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    public class Awakenable : MonoBehaviour
    {
        #region Settings

        [Header("Awaken Settings")]

        // Determines whether this object can only
        // be Awakened once.
        [SerializeField] private bool awakenOnce = true;

        [Header("Awaken Events")]

        // Allows us to connect reactions directly
        // through the Unity Inspector.
        [SerializeField] private UnityEvent onAwaken;

        #endregion


        #region State

        public bool IsAwakened { get; private set; }

        #endregion


        #region Public Methods

        public void Awaken()
        {
            // If this object has already been Awakened
            // and is only allowed to activate once,
            // don't activate again.
            if (awakenOnce && IsAwakened)
                return;

            IsAwakened = true;

            Debug.Log($"{gameObject.name} has awakened.");

            // Trigger whatever reaction this particular
            // object has been assigned.
            onAwaken?.Invoke();
        }

        #endregion
    }
}