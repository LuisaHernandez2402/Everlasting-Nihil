using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls a Boss arena encounter.
    ///
    /// When the Player enters:
    /// - The arena closes.
    /// - The Boss fight begins.
    ///
    /// When the Boss dies:
    /// - The arena opens.
    /// - The reward becomes available.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class BossArenaController : MonoBehaviour
    {
        #region References

        [Header("Boss References")]

        // Health component belonging to
        // the Boss inside this arena.
        [SerializeField]
        private Health bossHealth;


        [Header("Arena References")]

        // Collider that blocks the entrance
        // after the Player enters.
        [SerializeField]
        private Collider2D entranceBlocker;

        // Optional visual object for the
        // entrance gate.
        [SerializeField]
        private GameObject entranceGateVisual;

        // Optional reward object.
        //
        // For our game this will usually
        // be one of the three main crystals.
        [SerializeField]
        private GameObject rewardObject;

        #endregion


        #region Events

        [Header("Events")]

        // Called when the Boss fight begins.
        //
        // Later this could start music,
        // display the Boss's name, etc.
        [SerializeField]
        private UnityEvent onFightStarted;

        // Called when the Boss is defeated.
        //
        // Later this can trigger particles,
        // music changes, camera effects, etc.
        [SerializeField]
        private UnityEvent onBossDefeated;

        #endregion


        #region State

        // Prevents the arena from
        // starting multiple times.
        private bool fightStarted;

        // Prevents the victory sequence
        // from happening multiple times.
        private bool fightCompleted;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // -----------------------------
            // DETECTION TRIGGER
            // -----------------------------

            // The collider on THIS GameObject
            // detects when the Player enters.
            Collider2D detectionTrigger =
                GetComponent<Collider2D>();

            detectionTrigger.isTrigger = true;


            // -----------------------------
            // ENTRANCE
            // -----------------------------

            // The entrance should initially
            // be OPEN.
            SetEntranceClosed(false);


            // -----------------------------
            // REWARD
            // -----------------------------

            // Hide the crystal until the
            // Boss has been defeated.
            if (rewardObject != null)
            {
                rewardObject.SetActive(false);
            }
        }


        private void OnEnable()
        {
            // Listen for the Boss dying.
            if (bossHealth != null)
            {
                bossHealth.Died +=
                    HandleBossDefeated;
            }
        }


        private void OnDisable()
        {
            // Always remove our event subscription
            // when this component is disabled.
            if (bossHealth != null)
            {
                bossHealth.Died -=
                    HandleBossDefeated;
            }
        }


        private void OnTriggerEnter2D(
            Collider2D other
        )
        {
            // Don't restart an existing
            // or completed fight.
            if (fightStarted ||
                fightCompleted)
            {
                return;
            }


            // Check whether the entering object
            // belongs to the Player.
            PlayerMovement player =
                other.GetComponentInParent<PlayerMovement>();


            // Ignore anything else.
            if (player == null)
                return;


            // Begin the encounter.
            StartFight();
        }

        #endregion


        #region Fight Start

        /// <summary>
        /// Begins the Boss encounter.
        /// </summary>
        private void StartFight()
        {
            // Prevent duplicate starts.
            if (fightStarted)
                return;


            fightStarted = true;


            // -----------------------------
            // CLOSE ARENA
            // -----------------------------

            SetEntranceClosed(true);


            // -----------------------------
            // GAME STATE
            // -----------------------------

            // Tell our GameManager that
            // a Boss fight has started.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartBossFight();
            }


            // -----------------------------
            // EVENTS
            // -----------------------------

            onFightStarted?.Invoke();


            Debug.Log(
                "Boss fight started!"
            );
        }

        #endregion


        #region Boss Defeat

        /// <summary>
        /// Called automatically when
        /// Boss Health reaches zero.
        /// </summary>
        private void HandleBossDefeated()
        {
            // Prevent this from running twice.
            if (fightCompleted)
                return;


            fightCompleted = true;


            // -----------------------------
            // OPEN ARENA
            // -----------------------------

            SetEntranceClosed(false);


            // -----------------------------
            // REVEAL REWARD
            // -----------------------------

            if (rewardObject != null)
            {
                rewardObject.SetActive(true);
            }


            // -----------------------------
            // GAME STATE
            // -----------------------------

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndBossFight();
            }


            // -----------------------------
            // EVENTS
            // -----------------------------

            onBossDefeated?.Invoke();


            Debug.Log(
                "Boss defeated! Reward unlocked."
            );
        }

        #endregion


        #region Arena Gate

        /// <summary>
        /// Opens or closes the Boss
        /// arena entrance.
        /// </summary>
        private void SetEntranceClosed(
            bool closed
        )
        {
            // Physical barrier.
            if (entranceBlocker != null)
            {
                entranceBlocker.enabled =
                    closed;
            }


            // Visual gate.
            if (entranceGateVisual != null)
            {
                entranceGateVisual.SetActive(
                    closed
                );
            }
        }

        #endregion
    }
}