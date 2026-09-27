using System;
using System.Collections.Generic;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Stores the Player's important progression items.
    ///
    /// This currently tracks:
    ///
    /// 1. The three major ability crystals:
    ///    - Awaken
    ///    - Resonate
    ///    - Sever
    ///
    /// 2. Minor Burrow Crystals used for
    ///    the Mole Guardian side quest.
    ///
    /// No Update() is required.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        #region Main Crystals

        // Stores the major progression crystals
        // the Player has already collected.
        private readonly HashSet<CrystalType>
            collectedCrystals =
                new HashSet<CrystalType>();

        // Fired whenever a NEW major crystal
        // is successfully collected.
        public event Action<CrystalType>
            CrystalCollected;

        #endregion


        #region Burrow Crystals

        // Number of optional/minor crystals
        // collected for the Mole quest.
        private int burrowCrystalCount;

        // Public read-only access to the amount.
        public int BurrowCrystalCount
        {
            get
            {
                return burrowCrystalCount;
            }
        }

        // Fired whenever the Player collects
        // another Burrow Crystal.
        //
        // The integer contains the NEW total.
        public event Action<int>
            BurrowCrystalCountChanged;

        #endregion


        #region Main Crystal Methods

        /// <summary>
        /// Adds one of the three major
        /// progression crystals.
        ///
        /// Returns true if the crystal
        /// was newly collected.
        /// </summary>
        public bool AddCrystal(
            CrystalType crystalType
        )
        {
            // HashSet.Add returns false if
            // this crystal already exists.
            bool wasAdded =
                collectedCrystals.Add(
                    crystalType
                );


            // Don't collect duplicates.
            if (!wasAdded)
            {
                Debug.Log(
                    $"{crystalType} Crystal " +
                    "has already been collected."
                );

                return false;
            }


            Debug.Log(
                $"Collected {crystalType} Crystal."
            );


            // Notify the rest of the game.
            CrystalCollected?.Invoke(
                crystalType
            );


            return true;
        }


        /// <summary>
        /// Checks whether the Player owns
        /// a specific major crystal.
        /// </summary>
        public bool HasCrystal(
            CrystalType crystalType
        )
        {
            return collectedCrystals.Contains(
                crystalType
            );
        }


        /// <summary>
        /// Returns how many of the three
        /// major crystals have been collected.
        /// </summary>
        public int GetCrystalCount()
        {
            return collectedCrystals.Count;
        }


        /// <summary>
        /// Checks whether all three major
        /// progression crystals are owned.
        /// </summary>
        public bool HasAllCrystals()
        {
            return
                HasCrystal(CrystalType.Awaken) &&
                HasCrystal(CrystalType.Resonate) &&
                HasCrystal(CrystalType.Sever);
        }

        #endregion


        #region Burrow Crystal Methods

        /// <summary>
        /// Adds one Burrow Crystal
        /// to the Player's inventory.
        /// </summary>
        public void AddBurrowCrystal()
        {
            // Increase our side-quest currency.
            burrowCrystalCount++;


            Debug.Log(
                $"Burrow Crystal collected. " +
                $"Total: {burrowCrystalCount}"
            );


            // Notify quest systems and UI.
            BurrowCrystalCountChanged?.Invoke(
                burrowCrystalCount
            );
        }


        /// <summary>
        /// Checks whether the Player has
        /// collected the requested number
        /// of Burrow Crystals.
        /// </summary>
        public bool HasBurrowCrystals(
            int requiredAmount
        )
        {
            return
                burrowCrystalCount >=
                requiredAmount;
        }

        #endregion
    }
}