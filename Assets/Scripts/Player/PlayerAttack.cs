using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Handles the Player's basic melee attack.
    ///
    /// When the attack input is pressed,
    /// the script checks a small area in front
    /// of the Player for damageable targets.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerAttack : MonoBehaviour
    {
        #region Attack Settings

        [Header("Attack Settings")]

        // Point in front of the Player where
        // the attack hitbox is created.
        [SerializeField]
        private Transform attackPoint;

        // Radius of the melee attack.
        [SerializeField]
        private float attackRadius = 0.8f;

        // Amount of damage dealt per attack.
        [SerializeField]
        private int attackDamage = 20;

        // Minimum amount of time between attacks.
        [SerializeField]
        private float attackCooldown = 0.35f;

        // Layers that can be damaged
        // by the Player's attack.
        //
        // Set this to the Boss layer.
        [SerializeField]
        private LayerMask damageableLayers;

        #endregion


        #region Components

        // Handles the Player's input events.
        private PlayerInputHandler inputHandler;

        #endregion


        #region Attack State

        // Prevents attacking again while
        // the current attack is cooling down.
        private bool canAttack = true;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Cache our input handler.
            inputHandler =
                GetComponent<PlayerInputHandler>();
        }


        private void OnEnable()
        {
            // Listen for the attack input.
            if (inputHandler != null)
            {
                inputHandler.AttackPressed +=
                    HandleAttackPressed;
            }
        }


        private void OnDisable()
        {
            // Remove the input subscription.
            if (inputHandler != null)
            {
                inputHandler.AttackPressed -=
                    HandleAttackPressed;
            }
        }

        #endregion


        #region Attack Input

        /// <summary>
        /// Called whenever the Player
        /// presses the attack button.
        /// </summary>
        private void HandleAttackPressed()
        {
            // Don't attack during cooldown.
            if (!canAttack)
                return;


            // Make sure an AttackPoint
            // has actually been assigned.
            if (attackPoint == null)
            {
                Debug.LogWarning(
                    "PlayerAttack has no AttackPoint assigned."
                );

                return;
            }


            // Perform the attack.
            PerformAttack();


            // Begin our cooldown.
            StartCoroutine(
                AttackCooldownRoutine()
            );
        }

        #endregion


        #region Attack Logic

        /// <summary>
        /// Searches the attack area for
        /// objects containing Health.
        /// </summary>
        private void PerformAttack()
        {
            // Find every collider inside
            // our melee attack radius.
            Collider2D[] hits =
                Physics2D.OverlapCircleAll(
                    attackPoint.position,
                    attackRadius,
                    damageableLayers
                );


            // Keep track of Health components
            // already damaged by this swing.
            //
            // This prevents a Boss with multiple
            // colliders from taking damage several
            // times from one attack.
            HashSet<Health> damagedTargets =
                new HashSet<Health>();


            // Check every collider we hit.
            foreach (Collider2D hit in hits)
            {
                // Try finding Health directly
                // on this object.
                Health health =
                    hit.GetComponent<Health>();


                // If Health isn't directly on the
                // collider, check its parent.
                if (health == null)
                {
                    health =
                        hit.GetComponentInParent<Health>();
                }


                // Ignore objects without Health.
                if (health == null)
                    continue;


                // Ignore a target we've already
                // damaged during this attack.
                if (damagedTargets.Contains(health))
                    continue;


                // Damage the target.
                health.TakeDamage(
                    attackDamage
                );


                // Remember this target so another
                // collider doesn't damage it again.
                damagedTargets.Add(
                    health
                );


                Debug.Log(
                    $"Player hit {health.gameObject.name} " +
                    $"for {attackDamage} damage."
                );
            }
        }

        #endregion


        #region Attack Cooldown

        /// <summary>
        /// Prevents attack spamming for
        /// a short amount of time.
        /// </summary>
        private IEnumerator AttackCooldownRoutine()
        {
            // Lock attacking.
            canAttack = false;


            // Wait for the cooldown.
            yield return new WaitForSeconds(
                attackCooldown
            );


            // Allow another attack.
            canAttack = true;
        }

        #endregion


        #region Editor Visualization

        /// <summary>
        /// Displays the attack hitbox
        /// inside the Scene view.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // We need an AttackPoint
            // before drawing the radius.
            if (attackPoint == null)
                return;


            // Draw the attack range.
            Gizmos.DrawWireSphere(
                attackPoint.position,
                attackRadius
            );
        }

        #endregion
    }
}