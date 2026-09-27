using System.Collections;
using TMPro;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Displays a short message whenever the Player
    /// collects one of the three main ability crystals.
    ///
    /// Examples:
    ///
    /// AWAKEN UNLOCKED
    /// RESONATE UNLOCKED
    /// SEVER UNLOCKED
    ///
    /// No Update() is required.
    /// </summary>
    public class AbilityUnlockUI : MonoBehaviour
    {
        #region References

        [Header("References")]

        // Player inventory that tells us when
        // a main crystal has been collected.
        [SerializeField]
        private PlayerInventory playerInventory;

        // Parent object containing the
        // unlock message UI.
        [SerializeField]
        private GameObject unlockPanel;

        // Large ability-name text.
        [SerializeField]
        private TMP_Text abilityNameText;

        // Smaller text underneath.
        [SerializeField]
        private TMP_Text descriptionText;

        #endregion


        #region Settings

        [Header("Settings")]

        // Amount of time the unlock message
        // remains visible.
        [SerializeField]
        private float displayDuration = 2f;

        #endregion


        #region State

        // Keeps track of the currently
        // running UI coroutine.
        private Coroutine displayRoutine;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Start with the unlock UI hidden.
            if (unlockPanel != null)
            {
                unlockPanel.SetActive(false);
            }
        }


        private void OnEnable()
        {
            // Listen for main crystal collection.
            if (playerInventory != null)
            {
                playerInventory.CrystalCollected +=
                    HandleCrystalCollected;
            }
        }


        private void OnDisable()
        {
            // Remove our event subscription.
            if (playerInventory != null)
            {
                playerInventory.CrystalCollected -=
                    HandleCrystalCollected;
            }
        }

        #endregion


        #region Crystal Event

        /// <summary>
        /// Called automatically whenever the Player
        /// collects a new main crystal.
        /// </summary>
        private void HandleCrystalCollected(
            CrystalType crystalType
        )
        {
            // Stop an older message if one
            // somehow happens to still be running.
            if (displayRoutine != null)
            {
                StopCoroutine(
                    displayRoutine
                );
            }


            // Start the new unlock message.
            displayRoutine =
                StartCoroutine(
                    ShowUnlockRoutine(
                        crystalType
                    )
                );
        }

        #endregion


        #region Unlock Display

        /// <summary>
        /// Shows the appropriate message for
        /// the newly unlocked ability.
        /// </summary>
        private IEnumerator ShowUnlockRoutine(
            CrystalType crystalType
        )
        {
            // -----------------------------
            // SET ABILITY NAME
            // -----------------------------

            if (abilityNameText != null)
            {
                abilityNameText.text =
                    $"{crystalType.ToString().ToUpper()} UNLOCKED";
            }


            // -----------------------------
            // SET DESCRIPTION
            // -----------------------------

            if (descriptionText != null)
            {
                descriptionText.text =
                    GetAbilityDescription(
                        crystalType
                    );
            }


            // -----------------------------
            // SHOW PANEL
            // -----------------------------

            if (unlockPanel != null)
            {
                unlockPanel.SetActive(true);
            }


            // Keep it visible briefly.
            yield return
                new WaitForSeconds(
                    displayDuration
                );


            // -----------------------------
            // HIDE PANEL
            // -----------------------------

            if (unlockPanel != null)
            {
                unlockPanel.SetActive(false);
            }


            // Clear our coroutine reference.
            displayRoutine = null;
        }

        #endregion


        #region Ability Descriptions

        /// <summary>
        /// Returns a short explanation of
        /// each unlocked ability.
        /// </summary>
        private string GetAbilityDescription(
            CrystalType crystalType
        )
        {
            switch (crystalType)
            {
                case CrystalType.Awaken:

                    return
                        "Awaken dormant objects.";


                case CrystalType.Resonate:

                    return
                        "Create connections between resonance points.";


                case CrystalType.Sever:

                    return
                        "Break existing resonance connections.";


                default:

                    return
                        "A new ability has awakened.";
            }
        }

        #endregion
    }
}