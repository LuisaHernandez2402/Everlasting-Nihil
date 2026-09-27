using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Handles the Player's Resonate ability.
    ///
    /// Resonate allows the Player to select two
    /// ResonanceNodes and create a connection
    /// between them.
    ///
    /// The Player must own the Resonate Crystal
    /// before this ability can be used.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerAbilityManager))]
    public class PlayerResonate : MonoBehaviour
    {
        #region Resonate Settings

        [Header("Resonate Settings")]

        // Center point used when searching
        // for nearby ResonanceNodes.
        [SerializeField] private Transform resonatePoint;

        // Maximum distance at which the Player
        // can select a ResonanceNode.
        [SerializeField] private float resonateRadius = 2f;

        // Determines which layers can be
        // targeted by Resonate.
        //
        // Set this to AbilityTarget.
        [SerializeField] private LayerMask abilityTargetLayer;

        #endregion


        #region Components

        // Handles Player input events.
        private PlayerInputHandler inputHandler;

        // Determines whether Resonate
        // has been unlocked.
        private PlayerAbilityManager abilityManager;

        #endregion


        #region Selection State

        // First ResonanceNode selected
        // by the Player.
        private ResonanceNode selectedNode;

        // Visual component belonging to
        // the currently selected node.
        private ResonanceVisual selectedVisual;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Cache the Player input system.
            inputHandler =
                GetComponent<PlayerInputHandler>();

            // Cache the ability progression system.
            abilityManager =
                GetComponent<PlayerAbilityManager>();
        }


        private void OnEnable()
        {
            // Subscribe to the Resonate input event.
            if (inputHandler != null)
            {
                inputHandler.ResonatePressed +=
                    HandleResonatePressed;
            }
        }


        private void OnDisable()
        {
            // Remove the input subscription
            // when this component is disabled.
            if (inputHandler != null)
            {
                inputHandler.ResonatePressed -=
                    HandleResonatePressed;
            }

            // Remove any leftover selection
            // when this component is disabled.
            ClearSelection();
        }

        #endregion


        #region Resonate Input

        /// <summary>
        /// Called when the Player presses
        /// the Resonate button.
        /// </summary>
        private void HandleResonatePressed()
        {
            // -----------------------------
            // ABILITY UNLOCK CHECK
            // -----------------------------

            // Resonate cannot be used until
            // its crystal has been collected.
            if (abilityManager == null ||
                !abilityManager.CanResonate)
            {
                Debug.Log(
                    "Resonate is locked. " +
                    "Find the Resonate Crystal first."
                );

                return;
            }


            // -----------------------------
            // RESONATE POINT CHECK
            // -----------------------------

            // Stop safely if the detection point
            // wasn't assigned.
            if (resonatePoint == null)
            {
                Debug.LogWarning(
                    "PlayerResonate has no ResonatePoint assigned."
                );

                return;
            }


            // Find the closest ResonanceNode
            // currently within range.
            ResonanceNode nearbyNode =
                FindClosestNode();


            // Nothing was close enough.
            if (nearbyNode == null)
            {
                Debug.Log(
                    "No ResonanceNode is within range."
                );

                return;
            }


            // -----------------------------
            // FIRST SELECTION
            // -----------------------------

            // If nothing has been selected yet,
            // make this our first node.
            if (selectedNode == null)
            {
                SelectNode(nearbyNode);

                return;
            }


            // -----------------------------
            // CANCEL SELECTION
            // -----------------------------

            // Pressing Resonate on the same node
            // cancels the current selection.
            if (selectedNode == nearbyNode)
            {
                ClearSelection();

                return;
            }


            // -----------------------------
            // CREATE CONNECTION
            // -----------------------------

            // We now have two different nodes,
            // so connect them.
            selectedNode.LinkWith(
                nearbyNode
            );


            // Remove the selection highlight
            // after creating the connection.
            ClearSelection();
        }

        #endregion


        #region Node Detection

        /// <summary>
        /// Finds the closest ResonanceNode
        /// inside the Player's Resonate radius.
        /// </summary>
        private ResonanceNode FindClosestNode()
        {
            // Detect all AbilityTarget colliders
            // within the Resonate radius.
            Collider2D[] targets =
                Physics2D.OverlapCircleAll(
                    resonatePoint.position,
                    resonateRadius,
                    abilityTargetLayer
                );


            // Store our closest result.
            ResonanceNode closestNode = null;

            // Start with an infinitely
            // large comparison distance.
            float closestDistance =
                Mathf.Infinity;


            // Check every detected collider.
            foreach (Collider2D target in targets)
            {
                // Try to find a ResonanceNode.
                ResonanceNode node =
                    target.GetComponent<ResonanceNode>();


                // Ignore objects that cannot resonate.
                if (node == null)
                    continue;


                // Calculate the distance from our
                // ResonatePoint to this node.
                float distance =
                    Vector2.Distance(
                        resonatePoint.position,
                        node.transform.position
                    );


                // Ignore it if we already found
                // something closer.
                if (distance >= closestDistance)
                    continue;


                // This is our new closest node.
                closestDistance = distance;

                closestNode = node;
            }


            return closestNode;
        }

        #endregion


        #region Selection

        /// <summary>
        /// Selects the first ResonanceNode.
        /// </summary>
        private void SelectNode(
            ResonanceNode node
        )
        {
            // Store the selected node.
            selectedNode = node;


            // Find its optional visual component.
            selectedVisual =
                selectedNode.GetComponent<ResonanceVisual>();


            // Highlight the node if it has
            // a ResonanceVisual.
            if (selectedVisual != null)
            {
                selectedVisual.SetSelected(
                    true
                );
            }


            Debug.Log(
                $"Selected ResonanceNode: " +
                $"{selectedNode.gameObject.name}"
            );
        }


        /// <summary>
        /// Clears the currently selected node.
        /// </summary>
        private void ClearSelection()
        {
            // Remove the visual highlight.
            if (selectedVisual != null)
            {
                selectedVisual.SetSelected(
                    false
                );
            }


            // Forget the current selection.
            selectedNode = null;

            selectedVisual = null;
        }

        #endregion


        #region Editor Visualization

        /// <summary>
        /// Shows Resonate's detection radius
        /// inside the Scene view.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // We cannot draw the radius without
            // a ResonatePoint.
            if (resonatePoint == null)
                return;


            // Draw the detection radius.
            Gizmos.DrawWireSphere(
                resonatePoint.position,
                resonateRadius
            );
        }

        #endregion
    }
}