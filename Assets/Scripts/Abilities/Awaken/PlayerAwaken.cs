using UnityEngine;

namespace EverlastingNihil
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerAwaken : MonoBehaviour
    {
        #region Settings

        [Header("Awaken")]

        // Where the Awaken detection originates.
        [SerializeField] private Transform awakenPoint;

        // How far Awaken reaches.
        [SerializeField] private float awakenRadius = 1.5f;

        // Only objects on this layer can be detected.
        [SerializeField] private LayerMask awakenableLayer;

        #endregion


        #region Components

        private PlayerInputHandler input;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            input = GetComponent<PlayerInputHandler>();
        }


        private void OnEnable()
        {
            input.AwakenPressed += HandleAwaken;
        }


        private void OnDisable()
        {
            input.AwakenPressed -= HandleAwaken;
        }

        #endregion


        #region Awaken

        private void HandleAwaken()
        {
            if (awakenPoint == null)
                return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                awakenPoint.position,
                awakenRadius,
                awakenableLayer
            );

            foreach (Collider2D hit in hits)
            {
                Awakenable awakenable =
                    hit.GetComponent<Awakenable>();

                if (awakenable != null)
                {
                    awakenable.Awaken();
                }
            }
        }

        #endregion


        #region Debug

        private void OnDrawGizmosSelected()
        {
            if (awakenPoint == null)
                return;

            Gizmos.DrawWireSphere(
                awakenPoint.position,
                awakenRadius
            );
        }

        #endregion
    }
}