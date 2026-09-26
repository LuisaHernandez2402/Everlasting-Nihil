using System.Collections;
using UnityEngine;

namespace EverlastingNihil
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerParry : MonoBehaviour
    {
        #region Parry Settings

        [Header("Parry")]
        
        // How long the player can successfully parry
        // after pressing the button.
        [SerializeField] private float parryWindow = 0.18f;

        // Prevents parry spamming.
        [SerializeField] private float parryCooldown = 0.45f;

        [Header("Detection")]

        // Position where we check for incoming attacks.
        [SerializeField] private Transform parryPoint;

        // Radius around ParryPoint that can catch attacks.
        [SerializeField] private float parryRadius = 0.8f;

        // Only objects on these layers can be parried.
        [SerializeField] private LayerMask parryableLayer;

        #endregion


        #region Components

        private PlayerInputHandler input;

        #endregion


        #region State

        public bool IsParrying { get; private set; }

        private bool canParry = true;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            input = GetComponent<PlayerInputHandler>();
        }


        private void OnEnable()
        {
            input.ParryPressed += HandleParryPressed;
        }


        private void OnDisable()
        {
            input.ParryPressed -= HandleParryPressed;
        }

        #endregion


        #region Parry

        private void HandleParryPressed()
        {
            if (!canParry || IsParrying)
                return;

            StartCoroutine(ParryRoutine());
        }


        private IEnumerator ParryRoutine()
        {
            canParry = false;
            IsParrying = true;

            // Check immediately when the parry starts.
            CheckForParry();

            // Keep the parry window active.
            float elapsedTime = 0f;

            while (elapsedTime < parryWindow)
            {
                CheckForParry();

                elapsedTime += Time.fixedDeltaTime;

                // Wait for the next physics step rather than
                // relying on Update().
                yield return new WaitForFixedUpdate();
            }

            IsParrying = false;

            yield return new WaitForSeconds(parryCooldown);

            canParry = true;
        }


        private void CheckForParry()
        {
            if (parryPoint == null)
                return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                parryPoint.position,
                parryRadius,
                parryableLayer
            );

            foreach (Collider2D hit in hits)
            {
                Parryable parryable = hit.GetComponent<Parryable>();

                if (parryable != null)
                {
                    parryable.OnParried(transform);
                }
            }
        }

        #endregion


        #region Debug

        private void OnDrawGizmosSelected()
        {
            if (parryPoint == null)
                return;

            Gizmos.DrawWireSphere(
                parryPoint.position,
                parryRadius
            );
        }

        #endregion
    }
}