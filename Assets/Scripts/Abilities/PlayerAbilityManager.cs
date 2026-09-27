using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Connects the Player's progression crystals
    /// to their corresponding abilities.
    ///
    /// A crystal being collected means its ability
    /// is equipped and available.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(PlayerInventory))]
    public class PlayerAbilityManager : MonoBehaviour
    {
        #region Components

        // Player's progression inventory.
        private PlayerInventory inventory;

        #endregion


        #region Ability State

        // True once the corresponding
        // crystal has been collected.
        public bool CanAwaken =>
            inventory != null &&
            inventory.HasCrystal(
                CrystalType.Awaken
            );


        public bool CanResonate =>
            inventory != null &&
            inventory.HasCrystal(
                CrystalType.Resonate
            );


        public bool CanSever =>
            inventory != null &&
            inventory.HasCrystal(
                CrystalType.Sever
            );

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Cache the PlayerInventory component.
            inventory =
                GetComponent<PlayerInventory>();
        }

        #endregion
    }
}