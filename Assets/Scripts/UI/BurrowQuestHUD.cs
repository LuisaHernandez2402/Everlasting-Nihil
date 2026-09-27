using TMPro;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls the UI for the Mole Guardian
    /// side quest.
    ///
    /// The HUD:
    ///
    /// - Starts hidden.
    /// - Appears when the Mole gives the quest.
    /// - Updates whenever a Burrow Crystal is collected.
    /// - Disappears when the Mole quest is completed.
    ///
    /// No Update() is required.
    /// </summary>
    public class BurrowQuestHUD : MonoBehaviour
    {
        #region References

        [Header("References")]

        // The Player's inventory.
        //
        // This allows us to read the current
        // number of Burrow Crystals.
        [SerializeField]
        private PlayerInventory playerInventory;

        // Parent UI object containing
        // the quest tracker.
        [SerializeField]
        private GameObject questPanel;

        // Text displaying the crystal count.
        [SerializeField]
        private TMP_Text crystalText;

        #endregion


        #region Quest Settings

        [Header("Quest Settings")]

        // This should match the amount required
        // by MoleGuardian.
        [SerializeField]
        private int requiredCrystals = 5;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Start with the quest HUD hidden.
            //
            // The Mole will tell us when
            // it should become visible.
            if (questPanel != null)
            {
                questPanel.SetActive(false);
            }


            // Set the starting text anyway
            // so it's ready when shown.
            RefreshText();
        }


        private void OnEnable()
        {
            // Listen for Burrow Crystal
            // inventory changes.
            if (playerInventory != null)
            {
                playerInventory.BurrowCrystalCountChanged +=
                    HandleCrystalCountChanged;
            }
        }


        private void OnDisable()
        {
            // Always unsubscribe when
            // this component is disabled.
            if (playerInventory != null)
            {
                playerInventory.BurrowCrystalCountChanged -=
                    HandleCrystalCountChanged;
            }
        }

        #endregion


        #region Public Quest Methods

        /// <summary>
        /// Shows the quest tracker.
        ///
        /// MoleGuardian can call this through
        /// its UnityEvent in the Inspector.
        /// </summary>
        public void ShowQuest()
        {
            // Refresh first in case the Player
            // already collected some crystals.
            RefreshText();


            // Show the UI.
            if (questPanel != null)
            {
                questPanel.SetActive(true);
            }
        }


        /// <summary>
        /// Hides the quest tracker.
        ///
        /// MoleGuardian calls this when
        /// its quest has been completed.
        /// </summary>
        public void HideQuest()
        {
            if (questPanel != null)
            {
                questPanel.SetActive(false);
            }
        }

        #endregion


        #region Inventory Events

        /// <summary>
        /// Called whenever PlayerInventory reports
        /// that the Burrow Crystal count changed.
        /// </summary>
        private void HandleCrystalCountChanged(
            int newAmount
        )
        {
            // Update the UI using the new count.
            UpdateText(
                newAmount
            );
        }

        #endregion


        #region UI

        /// <summary>
        /// Reads the current inventory count
        /// and refreshes the HUD.
        /// </summary>
        private void RefreshText()
        {
            // If we don't have an inventory yet,
            // display zero.
            if (playerInventory == null)
            {
                UpdateText(
                    0
                );

                return;
            }


            // Display the Player's current count.
            UpdateText(
                playerInventory.BurrowCrystalCount
            );
        }


        /// <summary>
        /// Updates the actual TextMeshPro text.
        /// </summary>
        private void UpdateText(
            int currentAmount
        )
        {
            // Stop safely if the text
            // wasn't assigned.
            if (crystalText == null)
                return;


            // Prevent the UI from showing
            // something strange like 7 / 5.
            //
            // The Player can still collect more
            // than five internally if we placed
            // extra crystals in the level.
            int displayedAmount =
                Mathf.Min(
                    currentAmount,
                    requiredCrystals
                );


            // Update the displayed quest text.
            crystalText.text =
                $"BURROW CRYSTALS  {displayedAmount} / {requiredCrystals}";
        }

        #endregion
    }
}