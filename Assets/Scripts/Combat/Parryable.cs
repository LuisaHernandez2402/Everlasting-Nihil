using UnityEngine;

namespace EverlastingNihil
{
    public class Parryable : MonoBehaviour
    {
        #region Settings

        [Header("Parry Reaction")]

        [SerializeField] private float knockbackForce = 12f;

        #endregion


        #region Components

        private Rigidbody2D rb;

        #endregion


        #region State

        // Prevents the same object from being parried
        // repeatedly during one parry window.
        private bool hasBeenParried;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        #endregion


        #region Parry Reaction

        public void OnParried(Transform player)
        {
            if (hasBeenParried)
                return;

            hasBeenParried = true;

            Debug.Log($"{gameObject.name} was parried!");

            if (rb != null)
            {
                // Push the object away from the player.
                Vector2 direction =
                    (transform.position - player.position).normalized;

                rb.linearVelocity = direction * knockbackForce;
            }
        }

        #endregion
    }
}