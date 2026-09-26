using System;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Universal health system used by the Player, enemies,
    /// bosses, and anything else that can take damage.
    ///
    /// This script ONLY manages health values.
    /// Player/enemy/boss-specific behavior belongs in other scripts.
    /// </summary>
    public class Health : MonoBehaviour
    {
        #region Health Settings

        [Header("Health Settings")]

        // Maximum amount of health this object can have.
        [SerializeField] private int maxHealth = 100;

        // Current amount of health this object has.
        private int currentHealth;

        #endregion


        #region Public Properties

        // Other scripts can read the current health,
        // but they cannot directly change it.
        public int CurrentHealth => currentHealth;

        // Gives other scripts access to our maximum health.
        public int MaxHealth => maxHealth;

        // Returns true when this object has no health remaining.
        public bool IsDead => currentHealth <= 0;

        // Controls whether this object is currently allowed
        // to receive damage.
        //
        // PlayerDamageReceiver uses this for invincibility frames.
        public bool CanTakeDamage { get; set; } = true;

        #endregion


        #region Events

        // Called whenever this object successfully takes damage.
        //
        // The int contains the amount of damage dealt.
        public event Action<int> Damaged;

        // Called whenever this object successfully heals.
        //
        // The int contains the actual amount of health restored.
        public event Action<int> Healed;

        // Called whenever health changes.
        //
        // First int  = Current Health
        // Second int = Maximum Health
        //
        // This will be useful for player and boss health bars.
        public event Action<int, int> HealthChanged;

        // Called when health reaches zero.
        public event Action Died;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Every object starts at full health.
            currentHealth = maxHealth;
        }

        #endregion


        #region Damage

        /// <summary>
        /// Removes health from this object.
        /// </summary>
        public void TakeDamage(int damageAmount)
        {
            // Dead objects cannot continue taking damage.
            if (IsDead)
                return;

            // Ignore damage while damage receiving is disabled.
            //
            // For the player, this is used during i-frames.
            if (!CanTakeDamage)
                return;

            // Ignore zero or negative damage values.
            if (damageAmount <= 0)
                return;


            // Remove health.
            currentHealth -= damageAmount;


            // Prevent health from going below zero.
            currentHealth = Mathf.Clamp(
                currentHealth,
                0,
                maxHealth
            );


            // Announce that damage occurred.
            Damaged?.Invoke(damageAmount);


            // Announce the new health values.
            HealthChanged?.Invoke(
                currentHealth,
                maxHealth
            );


            Debug.Log(
                $"{gameObject.name} took {damageAmount} damage. " +
                $"Current HP: {currentHealth}/{maxHealth}"
            );


            // Check whether this hit killed the object.
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        #endregion


        #region Healing

        /// <summary>
        /// Restores some health to this object.
        /// </summary>
        public void Heal(int healAmount)
        {
            // Dead objects cannot heal normally.
            if (IsDead)
                return;

            // Ignore invalid healing values.
            if (healAmount <= 0)
                return;


            // Remember our health before healing.
            //
            // This lets us calculate how much health
            // was actually restored.
            int healthBeforeHealing = currentHealth;


            // Add health.
            currentHealth += healAmount;


            // Prevent health from exceeding maxHealth.
            currentHealth = Mathf.Clamp(
                currentHealth,
                0,
                maxHealth
            );


            // Calculate the amount actually restored.
            int actualHealing =
                currentHealth - healthBeforeHealing;


            // Nothing happened if we were already full.
            if (actualHealing <= 0)
                return;


            // Announce that healing occurred.
            Healed?.Invoke(actualHealing);


            // Announce the new health values.
            HealthChanged?.Invoke(
                currentHealth,
                maxHealth
            );


            Debug.Log(
                $"{gameObject.name} healed {actualHealing}. " +
                $"Current HP: {currentHealth}/{maxHealth}"
            );
        }


        /// <summary>
        /// Completely restores this object to maximum health.
        ///
        /// This is mainly used when the player respawns.
        /// </summary>
        public void RestoreToFullHealth()
        {
            // Restore maximum health.
            currentHealth = maxHealth;


            // Tell UI and other systems that health changed.
            HealthChanged?.Invoke(
                currentHealth,
                maxHealth
            );


            Debug.Log(
                $"{gameObject.name} was restored to full health. " +
                $"Current HP: {currentHealth}/{maxHealth}"
            );
        }

        #endregion


        #region Death

        /// <summary>
        /// Announces that this object has died.
        ///
        /// This does NOT destroy or disable the object.
        /// Player, enemy, and boss scripts decide what
        /// death means for themselves.
        /// </summary>
        private void Die()
        {
            Debug.Log($"{gameObject.name} died!");


            // Notify anything listening for death.
            Died?.Invoke();
        }

        #endregion
    }
}