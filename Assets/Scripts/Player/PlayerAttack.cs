using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Handles the Player's basic melee attack.
    ///
    /// When AttackPressed is received:
    /// - Checks an area around AttackPoint.
    /// - Finds objects with Health.
    /// - Deals damage.
    /// - Prevents attacking again until cooldown finishes.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerAttack : MonoBehaviour
    {
        #region Attack Settings

        [Header("Attack Settings")]

        // Point in front of the Player where
        // the attack detection happens.
        [SerializeField]
        private Transform attackPoint;

        // Size of the circular attack area.
        [SerializeField]
        private float attackRadius = 0.8f;

        // Damage dealt by one sword attack.
        [SerializeField]
        private int attackDamage = 20;

        // Time before another attack
        // can be performed.
        [SerializeField]
        private float attackCooldown = 0.35f;

        // Layers that can be damaged
        // by the Player's sword.
        [SerializeField]
        private LayerMask damageableLayers;

        #endregion


        #region Components

        // Handles Input System events.
        private PlayerInputHandler inputHandler;

        #endregion


        #region Attack State

        // Prevents attacking while
        // the cooldown is active.
        private bool canAttack = true;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Cache PlayerInputHandler.
            inputHandler =
                GetComponent<PlayerInputHandler>();
        }


        private void OnEnable()
        {
            // Listen for the Attack input.
            if (inputHandler != null)
            {
                inputHandler.AttackPressed +=
                    HandleAttackPressed;
            }
        }


        private void OnDisable()
        {
            // Remove the event subscription.
            if (inputHandler != null)
            {
                inputHandler.AttackPressed -=
                    HandleAttackPressed;
            }
        }


        private void OnDrawGizmosSelected()
        {
            // Don't draw anything if
            // AttackPoint isn't assigned.
            if (attackPoint == null)
                return;


            // Show the attack radius in
            // the Scene view.
            Gizmos.DrawWireSphere(
                attackPoint.position,
                attackRadius
            );
        }

        #endregion


        #region Attack Input

        /// <summary>
        /// Called whenever PlayerInputHandler
        /// receives the Attack input.
        /// </summary>
        private void HandleAttackPressed()
        {
            // Don't attack during cooldown.
            if (!canAttack)
                return;


            // Don't attack without
            // an AttackPoint.
            if (attackPoint == null)
                return;


            // Perform the actual attack.
            PerformAttack();


            // Begin cooldown.
            StartCoroutine(
                AttackCooldownRoutine()
            );
        }

        #endregion


        #region Attack

        /// <summary>
        /// Searches the attack area for
        /// damageable objects and damages them.
        /// </summary>
        private void PerformAttack()
        {
            // Find every collider inside
            // the sword's attack radius.
            Collider2D[] hitColliders =
                Physics2D.OverlapCircleAll(
                    attackPoint.position,
                    attackRadius,
                    damageableLayers
                );


            // Keeps track of Health components
            // we've already damaged during
            // this specific attack.
            //
            // This prevents a Boss with multiple
            // colliders from taking damage twice.
            HashSet<Health> damagedTargets =
                new HashSet<Health>();


            // Check everything we hit.
            foreach (Collider2D hitCollider
                     in hitColliders)
            {
                // First try finding Health
                // directly on the object.
                Health targetHealth =
                    hitCollider.GetComponent<Health>();


                // If Health isn't directly on
                // the collider, search its parent.
                if (targetHealth == null)
                {
                    targetHealth =
                        hitCollider
                            .GetComponentInParent<Health>();
                }


                // Ignore objects without Health.
                if (targetHealth == null)
                    continue;


                // Don't damage the same Health
                // component multiple times during
                // one sword swing.
                if (damagedTargets.Contains(
                        targetHealth))
                {
                    continue;
                }


                // Remember this target.
                damagedTargets.Add(
                    targetHealth
                );


                // Deal damage.
                targetHealth.TakeDamage(
                    attackDamage
                );
            }
        }

        #endregion


        #region Cooldown

        /// <summary>
        /// Prevents the Player from attacking
        /// continuously without delay.
        /// </summary>
        private IEnumerator AttackCooldownRoutine()
        {
            // Lock attacking.
            canAttack = false;


            // Wait for our cooldown.
            yield return
                new WaitForSeconds(
                    attackCooldown
                );


            // Allow another attack.
            canAttack = true;
        }

        #endregion
    }
}