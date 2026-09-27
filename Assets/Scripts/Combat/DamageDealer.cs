using System.Collections.Generic;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Universal component used by anything capable
    /// of dealing damage.
    ///
    /// Examples:
    /// - Player attacks
    /// - Enemy attacks
    /// - Boss attacks
    /// - Projectiles
    /// - Spikes
    /// - Environmental hazards
    ///
    /// This component does NOT manage health.
    /// It finds a Health component and tells it
    /// how much damage to take.
    /// </summary>
    public class DamageDealer : MonoBehaviour
    {
        #region Damage Settings

        [Header("Damage Settings")]

        // Amount of damage this object attempts to deal
        // whenever it successfully hits a valid target.
        [SerializeField] private int damageAmount = 10;

        // If true, each target can only be damaged once
        // while it remains inside this trigger.
        //
        // Example:
        //
        // Player enters spikes:
        //      Takes damage once.
        //
        // Player remains on spikes:
        //      Does NOT repeatedly take damage.
        //
        // Player leaves and comes back:
        //      Can take damage again.
        [SerializeField] private bool damageOncePerContact = true;

        #endregion


        #region Target Settings

        [Header("Target Settings")]

        // Determines which Unity layers this DamageDealer
        // is allowed to damage.
        //
        // Examples:
        //
        // Player Attack:
        //      Enemy
        //
        // Enemy Attack:
        //      Player
        //
        // Hazard:
        //      Player + Enemy
        [SerializeField] private LayerMask targetLayers;

        #endregion


        #region Hit Tracking

        // Stores every Health component that has already
        // been damaged during the current contact.
        //
        // HashSet is useful here because it automatically
        // prevents duplicate entries.
        //
        // This means one attack can hit MULTIPLE enemies,
        // while still preventing the same enemy from being
        // damaged repeatedly by one contact.
        private readonly HashSet<Health> damagedTargets =
            new HashSet<Health>();

        #endregion


        #region Trigger Detection

        /// <summary>
        /// Called automatically when another Collider2D
        /// enters this trigger.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Attempt to damage the object that
            // entered our trigger.
            TryDealDamage(other);
        }


        /// <summary>
        /// Called by Unity while another Collider2D
        /// remains inside this trigger.
        /// </summary>
        private void OnTriggerStay2D(Collider2D other)
        {
            // If we're only damaging once per contact,
            // OnTriggerEnter2D already handled the hit.
            if (damageOncePerContact)
                return;


            // Otherwise, allow continuous damage.
            //
            // This behavior could eventually be useful for
            // things like damaging fog, fire, poison zones,
            // or other environmental hazards.
            TryDealDamage(other);
        }


        /// <summary>
        /// Called automatically when another Collider2D
        /// leaves this trigger.
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            // Find the Health component associated
            // with the collider leaving our trigger.
            Health health = FindHealth(other);


            // If there is no Health component,
            // there is nothing to remove.
            if (health == null)
                return;


            // Remove this target from our remembered
            // collection of damaged targets.
            //
            // This allows the target to take damage again
            // if it enters this trigger later.
            damagedTargets.Remove(health);
        }

        #endregion


        #region Damage Logic

        /// <summary>
        /// Attempts to damage the object represented
        /// by the supplied Collider2D.
        /// </summary>
        private void TryDealDamage(Collider2D other)
        {
            // First check whether the collider's layer
            // is one we're allowed to damage.
            if (!IsLayerDamageable(other.gameObject.layer))
                return;


            // Find the Health component associated
            // with this collider.
            Health health = FindHealth(other);


            // No Health component means this object
            // cannot receive damage.
            if (health == null)
                return;


            // Don't attempt to damage something
            // that is already dead.
            if (health.IsDead)
                return;


            // If we're using one-hit-per-contact behavior,
            // check whether this Health component has
            // already been damaged.
            if (damageOncePerContact &&
                damagedTargets.Contains(health))
            {
                return;
            }


            // Ask the Health component to process
            // the incoming damage.
            health.TakeDamage(damageAmount);


            // If we're limiting damage to once per contact,
            // remember this target.
            if (damageOncePerContact)
            {
                damagedTargets.Add(health);
            }
        }

        #endregion


        #region Health Detection

        /// <summary>
        /// Finds the Health component belonging to
        /// the supplied collider.
        ///
        /// Health may exist directly on the collider
        /// or on one of its parent objects.
        /// </summary>
        private Health FindHealth(Collider2D other)
        {
            // First look directly on the object
            // containing the collider.
            Health health =
                other.GetComponent<Health>();


            // If we found Health directly on the object,
            // return it immediately.
            if (health != null)
            {
                return health;
            }


            // Otherwise search upward through its parents.
            //
            // This is important for more complicated objects.
            //
            // Example:
            //
            // Boss
            // ├── Health
            // ├── Body
            // │   └── Collider
            // └── Head
            //     └── Collider
            //
            // Hitting Body or Head can still find the
            // Health component on the Boss root.
            return other.GetComponentInParent<Health>();
        }

        #endregion


        #region Layer Detection

        /// <summary>
        /// Checks whether a Unity layer exists inside
        /// our Target Layers LayerMask.
        /// </summary>
        private bool IsLayerDamageable(int objectLayer)
        {
            // Unity LayerMasks use individual bits
            // to represent layers.
            //
            // 1 << objectLayer creates the bit representing
            // the layer of the object we touched.
            //
            // The & operation checks whether that bit
            // exists inside targetLayers.
            return
                (targetLayers.value & (1 << objectLayer)) != 0;
        }

        #endregion


        #region Public Controls

        /// <summary>
        /// Clears all remembered targets.
        ///
        /// This will become especially useful for attacks.
        /// When a NEW sword swing begins, we can clear
        /// previous targets so that the new attack is
        /// allowed to damage them again.
        /// </summary>
        public void ResetDamagedTargets()
        {
            damagedTargets.Clear();
        }

        #endregion
    }
}