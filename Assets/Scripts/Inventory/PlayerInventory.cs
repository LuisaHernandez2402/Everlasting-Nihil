using System;
using System.Collections.Generic;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Stores the Player's important progression items.
    ///
    /// For now, this inventory focuses on the three
    /// ability crystals:
    ///
    /// Awaken
    /// Resonate
    /// Sever
    ///
    /// Crystals are automatically considered equipped
    /// when collected.
    ///
    /// No Update() is required.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        #region Crystal Storage

        // Stores every crystal the Player currently owns.
        //
        // HashSet is useful here because the Player
        // should never own duplicate progression crystals.
        private readonly HashSet<CrystalType>
            collectedCrystals =
                new HashSet<CrystalType>();

        #endregion


        #region Events

        /// <summary>
        /// Fired whenever the Player obtains
        /// a new crystal.
        ///
        /// Other systems can listen to this for:
        /// - Ability unlocking
        /// - UI
        /// - Audio
        /// - Visual effects
        /// - Gate progression
        /// </summary>
        public event Action<CrystalType>
            CrystalCollected;

        #endregion


        #region Crystal Collection

        /// <summary>
        /// Gives the Player a progression crystal.
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


            // Don't trigger collection logic
            // for duplicate crystals.
            if (!wasAdded)
            {
                Debug.Log(
                    $"Player already owns the " +
                    $"{crystalType} Crystal."
                );

                return false;
            }


            Debug.Log(
                $"Collected {crystalType} Crystal."
            );


            // Notify every system listening
            // for crystal collection.
            CrystalCollected?.Invoke(
                crystalType
            );


            return true;
        }

        #endregion


        #region Crystal Queries

        /// <summary>
        /// Returns true if the Player owns
        /// the requested crystal.
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
        /// progression crystals have been collected.
        /// </summary>
        public int GetCrystalCount()
        {
            return collectedCrystals.Count;
        }


        /// <summary>
        /// Returns true when the Player owns
        /// Awaken, Resonate, AND Sever.
        /// </summary>
        public bool HasAllCrystals()
        {
            return
                HasCrystal(CrystalType.Awaken) &&
                HasCrystal(CrystalType.Resonate) &&
                HasCrystal(CrystalType.Sever);
        }

        #endregion
    }
}