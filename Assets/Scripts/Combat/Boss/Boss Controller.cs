using System.Collections;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls the basic behavior of the Boss.
    ///
    /// Combat loop:
    ///
    /// Approach Player
    ///      ↓
    /// Telegraph Attack
    ///      ↓
    /// Lunge toward Player
    ///      ↓
    /// Recovery
    ///      ↓
    /// Repeat
    ///
    /// The attack hitbox is ONLY active during
    /// the actual lunge.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public class BossController : MonoBehaviour
    {
        #region Movement Settings

        [Header("Movement Settings")]

        // Normal movement speed while the Boss
        // approaches the Player.
        [SerializeField]
        private float moveSpeed = 3f;

        // Distance at which the Boss stops
        // approaching and prepares an attack.
        [SerializeField]
        private float attackRange = 2.5f;

        #endregion


        #region Attack Settings

        [Header("Attack Settings")]

        // Horizontal speed used during
        // the Boss's lunge attack.
        [SerializeField]
        private float lungeSpeed = 10f;

        // How long the Boss continues lunging.
        [SerializeField]
        private float lungeDuration = 0.25f;

        // How long the Boss warns the Player
        // before beginning the attack.
        [SerializeField]
        private float telegraphDuration = 0.65f;

        // How long the Boss remains still
        // after attacking.
        //
        // This is the Player's main opportunity
        // to attack the Boss.
        [SerializeField]
        private float recoveryDuration = 1f;

        #endregion


        #region References

        [Header("References")]

        // SpriteRenderer used to visually
        // flip the Boss left and right.
        [SerializeField]
        private SpriteRenderer bossSprite;

        // Child GameObject containing:
        //
        // - BoxCollider2D
        // - DamageDealer
        //
        // This object will only be enabled
        // during the actual lunge attack.
        [SerializeField]
        private GameObject attackHitbox;

        // Optional visual warning displayed
        // while the Boss prepares an attack.
        [SerializeField]
        private GameObject attackWarning;

        #endregion


        #region Components

        // Boss physics body.
        private Rigidbody2D rb;

        // Boss health system.
        private Health health;

        // Transform of the Player.
        private Transform player;

        #endregion


        #region Boss State

        // Prevents multiple combat routines
        // from starting simultaneously.
        private bool combatRunning;

        // Stops all Boss behavior after death.
        private bool isDead;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Cache the Boss's Rigidbody2D.
            rb =
                GetComponent<Rigidbody2D>();


            // Cache the Boss's Health component.
            health =
                GetComponent<Health>();


            // -----------------------------
            // STARTING HITBOX STATE
            // -----------------------------

            // The attack hitbox should NOT hurt
            // the Player while the Boss is idle.
            if (attackHitbox != null)
            {
                attackHitbox.SetActive(false);
            }


            // -----------------------------
            // STARTING WARNING STATE
            // -----------------------------

            // The warning should also start hidden.
            if (attackWarning != null)
            {
                attackWarning.SetActive(false);
            }
        }


        private void OnEnable()
        {
            // Listen for the Boss dying.
            if (health != null)
            {
                health.Died +=
                    HandleDeath;
            }
        }


        private void OnDisable()
        {
            // Remove the death subscription
            // when this component is disabled.
            if (health != null)
            {
                health.Died -=
                    HandleDeath;
            }
        }


        private void Start()
        {
            // Find the Player through the
            // PlayerMovement component.
            PlayerMovement playerMovement =
                FindFirstObjectByType<PlayerMovement>();


            // Stop if no Player exists.
            if (playerMovement == null)
            {
                Debug.LogWarning(
                    "BossController could not find the Player."
                );

                return;
            }


            // Store the Player's Transform.
            player =
                playerMovement.transform;


            // Start Boss combat.
            StartCombat();
        }

        #endregion


        #region Combat Loop

        /// <summary>
        /// Starts the Boss combat routine.
        /// </summary>
        private void StartCombat()
        {
            // Prevent duplicate routines.
            if (combatRunning)
                return;


            // We need a Player before combat
            // can begin.
            if (player == null)
                return;


            StartCoroutine(
                CombatRoutine()
            );
        }


        /// <summary>
        /// Controls the Boss's complete
        /// repeating combat sequence.
        /// </summary>
        private IEnumerator CombatRoutine()
        {
            // Mark combat as active.
            combatRunning = true;


            // Continue fighting until death.
            while (!isDead)
            {
                // -----------------------------
                // APPROACH
                // -----------------------------

                yield return
                    ApproachPlayer();


                // Check whether the Boss died
                // during the previous action.
                if (isDead)
                    break;


                // -----------------------------
                // TELEGRAPH
                // -----------------------------

                yield return
                    TelegraphAttack();


                if (isDead)
                    break;


                // -----------------------------
                // LUNGE
                // -----------------------------

                yield return
                    PerformLunge();


                if (isDead)
                    break;


                // -----------------------------
                // RECOVERY
                // -----------------------------

                yield return
                    Recover();
            }


            // Combat has finished.
            combatRunning = false;
        }

        #endregion


        #region Approach

        /// <summary>
        /// Moves toward the Player until
        /// attack range is reached.
        /// </summary>
        private IEnumerator ApproachPlayer()
        {
            // The attack hitbox must stay disabled
            // while simply approaching.
            SetAttackHitbox(
                false
            );


            // Continue moving until close enough.
            while (!isDead &&
                   GetHorizontalDistanceToPlayer() >
                   attackRange)
            {
                // Determine which direction
                // the Player is located.
                float direction =
                    GetDirectionToPlayer();


                // Face the Player.
                FaceDirection(
                    direction
                );


                // Move horizontally toward them.
                rb.linearVelocity =
                    new Vector2(
                        direction * moveSpeed,
                        rb.linearVelocity.y
                    );


                // Wait for the next physics step.
                yield return
                    new WaitForFixedUpdate();
            }


            // Stop horizontal movement.
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y
                );
        }

        #endregion


        #region Telegraph

        /// <summary>
        /// Warns the Player that an
        /// attack is about to happen.
        /// </summary>
        private IEnumerator TelegraphAttack()
        {
            // Boss should not damage the Player
            // during the warning.
            SetAttackHitbox(
                false
            );


            // Stop horizontal movement.
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y
                );


            // Face the Player before attacking.
            FaceDirection(
                GetDirectionToPlayer()
            );


            // Show our warning visual.
            if (attackWarning != null)
            {
                attackWarning.SetActive(
                    true
                );
            }


            // Give the Player time to react.
            yield return
                new WaitForSeconds(
                    telegraphDuration
                );


            // Hide the warning.
            if (attackWarning != null)
            {
                attackWarning.SetActive(
                    false
                );
            }
        }

        #endregion


        #region Lunge Attack

        /// <summary>
        /// Performs the Boss's damaging
        /// horizontal lunge.
        /// </summary>
        private IEnumerator PerformLunge()
        {
            // Lock in the Player's direction
            // when the attack begins.
            //
            // The Boss will NOT track the Player
            // after the lunge starts.
            float attackDirection =
                GetDirectionToPlayer();


            // Face that direction.
            FaceDirection(
                attackDirection
            );


            // -----------------------------
            // ENABLE DAMAGE
            // -----------------------------

            // The Boss becomes dangerous now.
            SetAttackHitbox(
                true
            );


            // Launch horizontally.
            rb.linearVelocity =
                new Vector2(
                    attackDirection * lungeSpeed,
                    rb.linearVelocity.y
                );


            // Continue lunging.
            yield return
                new WaitForSeconds(
                    lungeDuration
                );


            // -----------------------------
            // DISABLE DAMAGE
            // -----------------------------

            // The attack is finished.
            SetAttackHitbox(
                false
            );


            // Stop horizontal movement.
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y
                );
        }

        #endregion


        #region Recovery

        /// <summary>
        /// Leaves the Boss vulnerable after
        /// completing an attack.
        /// </summary>
        private IEnumerator Recover()
        {
            // Make absolutely sure the Boss
            // cannot deal contact damage here.
            SetAttackHitbox(
                false
            );


            // Keep the Boss still.
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y
                );


            // Give the Player a chance
            // to punish the attack.
            yield return
                new WaitForSeconds(
                    recoveryDuration
                );
        }

        #endregion


        #region Attack Hitbox

        /// <summary>
        /// Enables or disables the Boss's
        /// damaging attack hitbox.
        /// </summary>
        private void SetAttackHitbox(
            bool active
        )
        {
            // Stop safely if no hitbox
            // has been assigned.
            if (attackHitbox == null)
                return;


            // Enable or disable the entire
            // hitbox GameObject.
            attackHitbox.SetActive(
                active
            );
        }

        #endregion


        #region Direction

        /// <summary>
        /// Returns the horizontal distance
        /// between Boss and Player.
        /// </summary>
        private float GetHorizontalDistanceToPlayer()
        {
            // Safety check.
            if (player == null)
                return 0f;


            return Mathf.Abs(
                player.position.x -
                transform.position.x
            );
        }


        /// <summary>
        /// Returns:
        ///
        /// +1 = Player is right
        /// -1 = Player is left
        /// </summary>
        private float GetDirectionToPlayer()
        {
            // Default to right if the
            // Player reference is missing.
            if (player == null)
                return 1f;


            if (player.position.x >=
                transform.position.x)
            {
                return 1f;
            }


            return -1f;
        }


        /// <summary>
        /// Turns the Boss and moves its attack
        /// hitbox to the correct side.
        /// </summary>
        private void FaceDirection(
            float direction
        )
        {
            // -----------------------------
            // FLIP SPRITE
            // -----------------------------

            if (bossSprite != null)
            {
                // Assumes the original Boss
                // artwork faces right.
                bossSprite.flipX =
                    direction < 0f;
            }


            // -----------------------------
            // MOVE ATTACK HITBOX
            // -----------------------------

            if (attackHitbox != null)
            {
                // Get the hitbox's Transform.
                Transform hitboxTransform =
                    attackHitbox.transform;


                // Read its current local position.
                Vector3 position =
                    hitboxTransform.localPosition;


                // Preserve the hitbox's distance
                // from the Boss while changing
                // which side it appears on.
                position.x =
                    Mathf.Abs(position.x) *
                    (direction >= 0f ? 1f : -1f);


                // Apply the new position.
                hitboxTransform.localPosition =
                    position;
            }
        }

        #endregion


        #region Death

        /// <summary>
        /// Called when the Boss's Health
        /// reaches zero.
        /// </summary>
        private void HandleDeath()
        {
            // Mark the Boss as dead.
            isDead = true;


            // Stop movement immediately.
            if (rb != null)
            {
                rb.linearVelocity =
                    Vector2.zero;
            }


            // A dead Boss obviously shouldn't
            // continue damaging the Player.
            SetAttackHitbox(
                false
            );


            // Hide the warning if the Boss
            // died during its telegraph.
            if (attackWarning != null)
            {
                attackWarning.SetActive(
                    false
                );
            }


            Debug.Log(
                $"{gameObject.name} has been defeated!"
            );
        }

        #endregion
    }
}