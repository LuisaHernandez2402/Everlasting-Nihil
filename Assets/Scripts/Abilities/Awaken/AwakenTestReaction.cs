using UnityEngine;

namespace EverlastingNihil
{
    public class AwakenTestReaction : MonoBehaviour
    {
        #region Settings

        [SerializeField] private float awakenedScale = 1.5f;

        #endregion


        #region Reaction

        public void React()
        {
            transform.localScale *= awakenedScale;

            Debug.Log($"{gameObject.name} reacted to Awaken!");
        }

        #endregion
    }
}