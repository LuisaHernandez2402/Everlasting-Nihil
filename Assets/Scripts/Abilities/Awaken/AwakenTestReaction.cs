using UnityEngine;

namespace EverlastingNihil
{
    public class AwakenTestReaction : MonoBehaviour, IReactable
    {
        #region Settings

        [Header("Reaction")]
        [SerializeField] private float awakenedScale = 1.5f;

        #endregion


        #region State

        private bool hasReacted;

        #endregion


        #region Reaction

        public void React()
        {
            // Prevent this test object from reacting repeatedly.
            if (hasReacted)
                return;

            hasReacted = true;

            transform.localScale *= awakenedScale;

            Debug.Log($"{gameObject.name} reacted!");
        }

        #endregion
    }
}