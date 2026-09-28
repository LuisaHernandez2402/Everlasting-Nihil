using System.Collections;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Simple reusable boss AI for Everlasting Nihil.
    ///
    /// The boss repeatedly:
    /// 1. Approaches the Player.
    /// 2. Stops when close enough.
    /// 3. Telegraphs an attack.
    /// 4. Lunges toward the Player.
    /// 5. Recovers.
    /// 6. Repeats.
    ///
    /// This script intentionally does NOT use Update().
    /// Boss behavior is handled through a coroutine and physics
    /// movement is performed through the Rigidbody2D.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public class BossController : MonoBehaviour
    {
        #region Movement Settings

        [Header("Movement")]

        // How quickly the boss walks toward the Player.
        [SerializeField] private float moveSpeed = 3f;

        // Once the Player is this close,
        // the boss prepares an attack.
        [SerializeField] private float attackRange = 2.5f;

        #endregion

        #region Attack Settings

        [Header("Attack")]

        // How fast the boss lunges during its attack.
        [SerializeField] private float lungeSpeed = 10f;

        // How long the boss moves during the lunge.
        [SerializeField] private float lungeDuration = 0.25f;

        // Delay before the attack happens.
        // This gives the Player time to react.
        [SerializeField] private float telegraphDuration = 0.65f;

        // Delay after an attack before the boss
        // can begin approaching again.
        [SerializeField] private float recoveryDuration = 1f;

        // Child GameObject containing the boss's
        // attack collider and DamageDealer.
        [SerializeField] private GameObject attackHitbox;

        #endregion

        #region Visual Settings

        [Header("Visuals")]

        // Sprite used for flipping the boss.
        [SerializeField] private SpriteRenderer bossSprite;

        // Optional object shown while the boss
        // is preparing an attack.
        //
        // This can literally just be a red sprite,
        // exclamation mark, etc. for the jam build.
        [SerializeField] private GameObject attackWarning;

        // Check this if the original boss artwork
        // naturally faces right.
        [SerializeField] private bool spriteFacesRight = true;

        #endregion

        #region References

        private Rigidbody2D rb;
        private Health health;
        private Transform player;

        #endregion

        #region State

        // Prevent multiple combat routines
        // from running simultaneously.
        private Coroutine combatRoutine;

        // Becomes true when Health reaches zero.
        private bool isDead;

        // Current direction the boss is facing.
        // 1 = right
        // -1 = left
        private int facingDirection = 1;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Cache components attached to this boss.
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();

            // Attack hitbox should NOT hurt the Player
            // until an attack is actually happening.
            if (attackHitbox != null)
                attackHitbox.SetActive(false);

            // Hide the telegraph visual at startup.
            if (attackWarning != null)
                attackWarning.SetActive(false);
        }

        private void Start()
        {
            // Find our persistent Player after the scene loads.
            PlayerMovement playerMovement =
                FindFirstObjectByType<PlayerMovement>();

            if (playerMovement != null)
            {
                player = playerMovement.transform;

                // Start fighting immediately.
                StartCombat();
            }
            else
            {
                Debug.LogWarning(
                    $"{name} could not find the Player.",
                    this
                );
            }
        }

        private void OnEnable()
        {
            // Listen for this boss reaching zero health.
            if (health != null)
                health.Died += HandleDeath;
        }

        private void OnDisable()
        {
            // Always unsubscribe from events.
            if (health != null)
                health.Died -= HandleDeath;
        }

        #endregion

        #region Combat

        /// <summary>
        /// Starts this boss's combat loop.
        /// </summary>
        public void StartCombat()
        {
            // Don't start another routine if we're
            // already fighting or already dead.
            if (combatRoutine != null || isDead)
                return;

            combatRoutine =
                StartCoroutine(CombatRoutine());
        }

        /// <summary>
        /// Main boss behavior loop.
        /// </summary>
        private IEnumerator CombatRoutine()
        {
            while (!isDead)
            {
                // If the Player somehow disappears,
                // wait instead of throwing errors.
                if (player == null)
                {
                    rb.linearVelocity =
                        new Vector2(
                            0f,
                            rb.linearVelocity.y
                        );

                    yield return null;
                    continue;
                }

                // -------------------------
                // APPROACH PHASE
                // -------------------------

                // Keep approaching until we're
                // close enough to attack.
                while (!isDead &&
                       player != null &&
                       HorizontalDistanceToPlayer() >
                       attackRange)
                {
                    MoveTowardPlayer();

                    // Wait for the next physics tick.
                    yield return new WaitForFixedUpdate();
                }

                if (isDead)
                    break;

                // Stop before telegraphing.
                StopHorizontalMovement();

                // Face toward the Player before attacking.
                FacePlayer();

                // -------------------------
                // TELEGRAPH PHASE
                // -------------------------

                if (attackWarning != null)
                    attackWarning.SetActive(true);

                yield return new WaitForSeconds(
                    telegraphDuration
                );

                if (attackWarning != null)
                    attackWarning.SetActive(false);

                if (isDead)
                    break;

                // -------------------------
                // ATTACK PHASE
                // -------------------------

                yield return StartCoroutine(
                    LungeAttack()
                );

                if (isDead)
                    break;

                // -------------------------
                // RECOVERY PHASE
                // -------------------------

                StopHorizontalMovement();

                yield return new WaitForSeconds(
                    recoveryDuration
                );
            }

            StopHorizontalMovement();

            combatRoutine = null;
        }

        /// <summary>
        /// Moves horizontally toward the Player.
        /// </summary>
        private void MoveTowardPlayer()
        {
            if (player == null)
                return;

            float direction =
                Mathf.Sign(
                    player.position.x -
                    transform.position.x
                );

            // If the Player is directly above/below,
            // keep our existing facing direction.
            if (direction == 0f)
                direction = facingDirection;

            SetFacingDirection(
                direction > 0f ? 1 : -1
            );

            // Only control horizontal velocity.
            // Gravity continues controlling Y.
            rb.linearVelocity =
                new Vector2(
                    direction * moveSpeed,
                    rb.linearVelocity.y
                );
        }

        /// <summary>
        /// Performs the boss's simple lunge attack.
        /// </summary>
        private IEnumerator LungeAttack()
        {
            // Capture the direction NOW.
            // This prevents the attack from magically
            // tracking the Player during the lunge.
            float direction =
                facingDirection;

            // Turn on the damaging hitbox.
            if (attackHitbox != null)
                attackHitbox.SetActive(true);

            float timer = 0f;

            while (timer < lungeDuration &&
                   !isDead)
            {
                rb.linearVelocity =
                    new Vector2(
                        direction * lungeSpeed,
                        rb.linearVelocity.y
                    );

                timer += Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            // Attack is finished.
            if (attackHitbox != null)
                attackHitbox.SetActive(false);

            StopHorizontalMovement();
        }

        #endregion

        #region Facing

        /// <summary>
        /// Makes the boss face the Player.
        /// </summary>
        private void FacePlayer()
        {
            if (player == null)
                return;

            float difference =
                player.position.x -
                transform.position.x;

            if (Mathf.Abs(difference) < 0.01f)
                return;

            SetFacingDirection(
                difference > 0f ? 1 : -1
            );
        }

        /// <summary>
        /// Changes the boss's visual direction and
        /// moves the attack hitbox to the correct side.
        /// </summary>
        private void SetFacingDirection(
            int newDirection
        )
        {
            if (newDirection == 0)
                return;

            facingDirection = newDirection;

            // Flip the artwork.
            if (bossSprite != null)
            {
                bool shouldFlip =
                    spriteFacesRight
                        ? facingDirection < 0
                        : facingDirection > 0;

                bossSprite.flipX = shouldFlip;
            }

            // Move the attack hitbox to whichever
            // side the boss is facing.
            if (attackHitbox != null)
            {
                Vector3 position =
                    attackHitbox.transform.localPosition;

                position.x =
                    Mathf.Abs(position.x) *
                    facingDirection;

                attackHitbox.transform.localPosition =
                    position;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns horizontal distance from boss to Player.
        ///
        /// We intentionally only care about X because
        /// this boss fights on a 2D platform.
        /// </summary>
        private float HorizontalDistanceToPlayer()
        {
            if (player == null)
                return Mathf.Infinity;

            return Mathf.Abs(
                player.position.x -
                transform.position.x
            );
        }

        /// <summary>
        /// Stops horizontal movement while preserving
        /// gravity/falling velocity.
        /// </summary>
        private void StopHorizontalMovement()
        {
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y
                );
        }

        #endregion

        #region Death

        /// <summary>
        /// Called by the Health component when
        /// this boss reaches zero HP.
        /// </summary>
        private void HandleDeath()
        {
            if (isDead)
                return;

            isDead = true;

            // Immediately disable the damaging hitbox.
            if (attackHitbox != null)
                attackHitbox.SetActive(false);

            if (attackWarning != null)
                attackWarning.SetActive(false);

            StopHorizontalMovement();

            // Stop the boss from physically blocking
            // the Player after death.
            Collider2D bossCollider =
                GetComponent<Collider2D>();

            if (bossCollider != null)
                bossCollider.enabled = false;

            Debug.Log(
                $"{name} has been defeated.",
                this
            );

            // Hide the artwork for now.
            // Later this could trigger a death animation.
            if (bossSprite != null)
                bossSprite.gameObject.SetActive(false);
        }

        #endregion
    }
}
