using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls the Mole Guardian side quest.
    ///
    /// When the Player first approaches:
    ///
    /// 1. The Mole rises from underground.
    /// 2. The Mole blocks access to the crystal.
    /// 3. The Player is asked to collect
    ///    Burrow Crystals.
    /// 4. After collecting enough crystals,
    ///    the Mole burrows back underground.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MoleGuardian : MonoBehaviour
    {
        #region Quest Settings

        [Header("Quest Settings")]

        // Number of Burrow Crystals required
        // before the Mole allows passage.
        [SerializeField]
        private int requiredCrystals = 5;

        #endregion


        #region Mole References

        [Header("Mole References")]

        // Visual object containing the
        // Mole's SpriteRenderer.
        [SerializeField]
        private GameObject moleVisual;

        // Physical collider that prevents
        // the Player from reaching the crystal.
        [SerializeField]
        private Collider2D pathBlocker;

        #endregion


        #region Emergence Settings

        [Header("Emergence Settings")]

        // How far underground the Mole
        // begins before appearing.
        [SerializeField]
        private float emergeDistance = 2f;

        // How long it takes the Mole
        // to rise out of the ground.
        [SerializeField]
        private float emergeDuration = 0.5f;

        #endregion


        #region Burrow Settings

        [Header("Burrow Settings")]

        // How far downward the Mole travels
        // when leaving after quest completion.
        [SerializeField]
        private float burrowDistance = 2f;

        // How long the Mole takes
        // to disappear underground.
        [SerializeField]
        private float burrowDuration = 0.5f;

        #endregion


        #region Events

        [Header("Events")]

        // Fired when the Mole begins
        // emerging from underground.
        //
        // Great place for particles,
        // sounds, or screen shake later.
        [SerializeField]
        private UnityEvent onEmergeStarted;

        // Fired once the Mole has completely
        // emerged from the ground.
        [SerializeField]
        private UnityEvent onEmergeFinished;

        // Fired when the Player does not
        // have enough Burrow Crystals.
        [SerializeField]
        private UnityEvent onRequirementNotMet;

        // Fired when the Player successfully
        // completes the Mole's request.
        [SerializeField]
        private UnityEvent onQuestCompleted;

        #endregion


        #region Quest State

        // Has the Mole already appeared?
        private bool hasEmerged;

        // Prevents the emergence coroutine
        // from starting multiple times.
        private bool isEmerging;

        // Has the Player completed
        // the Mole's quest?
        private bool questCompleted;

        #endregion


        #region Position Data

        // Normal visible position of the Mole.
        private Vector3 emergedPosition;

        // Starting underground position.
        private Vector3 undergroundPosition;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // -----------------------------
            // DETECTION TRIGGER
            // -----------------------------

            // The collider on this GameObject
            // detects the approaching Player.
            Collider2D detectionTrigger =
                GetComponent<Collider2D>();

            detectionTrigger.isTrigger = true;


            // -----------------------------
            // MOLE STARTING POSITION
            // -----------------------------

            if (moleVisual != null)
            {
                // Remember where the Mole should
                // stand after emerging.
                emergedPosition =
                    moleVisual.transform.localPosition;


                // Calculate a position underneath
                // the ground.
                undergroundPosition =
                    emergedPosition +
                    Vector3.down *
                    emergeDistance;


                // Start the Mole underground.
                moleVisual.transform.localPosition =
                    undergroundPosition;


                // IMPORTANT:
                //
                // Keep the GameObject active.
                // We need to animate its Transform.
                moleVisual.SetActive(true);
            }


            // -----------------------------
            // BLOCKER
            // -----------------------------

            // The path should already be blocked
            // even though the Mole is underground.
            //
            // This prevents the Player from running
            // past while the Mole is emerging.
            if (pathBlocker != null)
            {
                pathBlocker.enabled = true;
            }
        }

        #endregion


        #region Player Detection

        private void OnTriggerEnter2D(
            Collider2D other
        )
        {
            // Ignore everything after
            // quest completion.
            if (questCompleted)
                return;


            // Find PlayerInventory.
            PlayerInventory inventory =
                other.GetComponentInParent<PlayerInventory>();


            // Ignore anything that isn't
            // part of the Player.
            if (inventory == null)
                return;


            // -----------------------------
            // FIRST ENCOUNTER
            // -----------------------------

            if (!hasEmerged &&
                !isEmerging)
            {
                StartCoroutine(
                    EmergeRoutine(inventory)
                );

                return;
            }


            // Don't process dialogue while
            // the Mole is still emerging.
            if (isEmerging)
                return;


            // -----------------------------
            // RETURN VISIT
            // -----------------------------

            CheckQuestRequirement(
                inventory
            );
        }

        #endregion


        #region Emergence

        /// <summary>
        /// Animates the Mole rising
        /// from underneath the ground.
        /// </summary>
        private IEnumerator EmergeRoutine(
            PlayerInventory inventory
        )
        {
            // Prevent another emergence.
            isEmerging = true;


            Debug.Log(
                "Something is moving underground..."
            );


            // Trigger optional effects.
            onEmergeStarted?.Invoke();


            // Make sure the path is blocked
            // before the animation begins.
            if (pathBlocker != null)
            {
                pathBlocker.enabled = true;
            }


            // If no visual exists, skip
            // directly to the quest.
            if (moleVisual == null)
            {
                hasEmerged = true;
                isEmerging = false;

                CheckQuestRequirement(
                    inventory
                );

                yield break;
            }


            // Start underground.
            moleVisual.transform.localPosition =
                undergroundPosition;


            // Track animation time.
            float elapsedTime = 0f;


            // Move upward until the Mole reaches
            // its normal visible position.
            while (elapsedTime <
                   emergeDuration)
            {
                // Increase our timer.
                elapsedTime +=
                    Time.deltaTime;


                // Convert time into 0-1 progress.
                float progress =
                    Mathf.Clamp01(
                        elapsedTime /
                        emergeDuration
                    );


                // Smooth the movement slightly
                // so it doesn't look completely
                // robotic.
                float smoothProgress =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        progress
                    );


                // Move from underground
                // to the visible position.
                moleVisual.transform.localPosition =
                    Vector3.Lerp(
                        undergroundPosition,
                        emergedPosition,
                        smoothProgress
                    );


                yield return null;
            }


            // Guarantee the final position
            // is exact.
            moleVisual.transform.localPosition =
                emergedPosition;


            // Emergence is complete.
            hasEmerged = true;

            isEmerging = false;


            Debug.Log(
                "The Mole Guardian emerged!"
            );


            // Trigger optional effects.
            onEmergeFinished?.Invoke();


            // Now tell the Player what
            // the Mole wants.
            CheckQuestRequirement(
                inventory
            );
        }

        #endregion


        #region Quest Logic

        /// <summary>
        /// Checks whether the Player has collected
        /// enough Burrow Crystals.
        /// </summary>
        private void CheckQuestRequirement(
            PlayerInventory inventory
        )
        {
            // -----------------------------
            // REQUIREMENT COMPLETE
            // -----------------------------

            if (inventory.HasBurrowCrystals(
                    requiredCrystals))
            {
                CompleteQuest();

                return;
            }


            // -----------------------------
            // REQUIREMENT NOT COMPLETE
            // -----------------------------

            int currentAmount =
                inventory.BurrowCrystalCount;


            Debug.Log(
                $"Mole: You want that crystal? " +
                $"Bring me {requiredCrystals} Burrow Crystals. " +
                $"You currently have {currentAmount}/{requiredCrystals}."
            );


            // Later this can display
            // actual dialogue UI.
            onRequirementNotMet?.Invoke();
        }

        #endregion


        #region Quest Completion

        /// <summary>
        /// Completes the side quest
        /// and makes the Mole leave.
        /// </summary>
        private void CompleteQuest()
        {
            // Prevent duplicate completion.
            if (questCompleted)
                return;


            // Mark the quest complete.
            questCompleted = true;


            Debug.Log(
                "Mole: Huh... you actually found them. " +
                "Fine. A deal's a deal."
            );


            // Trigger optional completion effects.
            onQuestCompleted?.Invoke();


            // Send the Mole back underground.
            StartCoroutine(
                BurrowAwayRoutine()
            );
        }

        #endregion


        #region Burrow Away

        /// <summary>
        /// Animates the Mole returning
        /// underground after quest completion.
        /// </summary>
        private IEnumerator BurrowAwayRoutine()
        {
            // If there is no visual,
            // simply open the path.
            if (moleVisual == null)
            {
                OpenPath();

                yield break;
            }


            // Start from the Mole's
            // current visible position.
            Vector3 startPosition =
                moleVisual.transform.localPosition;


            // Calculate where the Mole
            // should disappear.
            Vector3 endPosition =
                startPosition +
                Vector3.down *
                burrowDistance;


            // Track animation time.
            float elapsedTime = 0f;


            // Move downward.
            while (elapsedTime <
                   burrowDuration)
            {
                // Increase our timer.
                elapsedTime +=
                    Time.deltaTime;


                // Calculate 0-1 progress.
                float progress =
                    Mathf.Clamp01(
                        elapsedTime /
                        burrowDuration
                    );


                // Smooth the movement.
                float smoothProgress =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        progress
                    );


                // Move underground.
                moleVisual.transform.localPosition =
                    Vector3.Lerp(
                        startPosition,
                        endPosition,
                        smoothProgress
                    );


                yield return null;
            }


            // Guarantee final position.
            moleVisual.transform.localPosition =
                endPosition;


            // Hide the Mole completely.
            moleVisual.SetActive(
                false
            );


            // Finally remove the barrier.
            OpenPath();
        }

        #endregion


        #region Path Control

        /// <summary>
        /// Removes the barrier preventing
        /// access to the Sever Crystal.
        /// </summary>
        private void OpenPath()
        {
            // Disable the physical blocker.
            if (pathBlocker != null)
            {
                pathBlocker.enabled =
                    false;
            }


            Debug.Log(
                "The path to the Sever Crystal is open."
            );
        }

        #endregion
    }
}