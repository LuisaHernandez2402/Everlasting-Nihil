using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls the Player's Resonate ability.
    ///
    /// Resonate uses two selections:
    ///
    /// First press:
    ///     Select Node A.
    ///
    /// Second press:
    ///     Select Node B.
    ///
    /// Result:
    ///     Node A and Node B become connected.
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerResonate : MonoBehaviour
    {
        #region Resonate Settings

        [Header("Resonate Settings")]

        // Point used as the center of our
        // Resonate detection area.
        [SerializeField] private Transform resonatePoint;

        // Maximum distance from resonatePoint
        // where the Player can select a node.
        [SerializeField] private float resonateRadius = 2f;

        // Layer containing objects our abilities
        // are allowed to interact with.
        //
        // Use the shared AbilityTarget layer.
        [SerializeField] private LayerMask abilityTargetLayer;

        #endregion


        #region Components

        // Player's central input handler.
        private PlayerInputHandler input;

        #endregion


        #region Selection State

        // The first ResonanceNode selected by the Player.
        private ResonanceNode selectedNode;

        // Visual belonging to the currently selected node.
        //
        // We keep this reference so we can turn the
        // selection highlight on and off.
        private ResonanceVisual selectedVisual;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get the Player's input handler.
            input =
                GetComponent<PlayerInputHandler>();
        }


        private void OnEnable()
        {
            // Listen for the Resonate input event.
            input.ResonatePressed +=
                HandleResonatePressed;
        }


        private void OnDisable()
        {
            // Stop listening when this component
            // becomes disabled.
            input.ResonatePressed -=
                HandleResonatePressed;
        }

        #endregion


        #region Resonate Input

        /// <summary>
        /// Called whenever the Player presses
        /// their Resonate button.
        /// </summary>
        private void HandleResonatePressed()
        {
            // Find the closest valid resonance node.
            ResonanceNode nearbyNode =
                FindClosestResonanceNode();


            // Don't continue if there isn't
            // a valid node nearby.
            if (nearbyNode == null)
            {
                Debug.Log(
                    "No Resonance Node nearby."
                );

                return;
            }


            // -----------------------------
            // FIRST SELECTION
            // -----------------------------

            // If we don't currently have a node selected,
            // this becomes our first node.
            if (selectedNode == null)
            {
                // Remember the node.
                selectedNode = nearbyNode;


                // Find its visual component.
                selectedVisual =
                    selectedNode.GetComponent<ResonanceVisual>();


                // Highlight the selected node.
                if (selectedVisual != null)
                {
                    selectedVisual.SetSelected(true);
                }


                Debug.Log(
                    $"{selectedNode.gameObject.name} " +
                    $"selected for resonance."
                );


                return;
            }


            // -----------------------------
            // CANCEL SELECTION
            // -----------------------------

            // Selecting the same node again cancels
            // the current selection.
            if (nearbyNode == selectedNode)
            {
                // Remove the highlight.
                if (selectedVisual != null)
                {
                    selectedVisual.SetSelected(false);
                }


                // Forget the selected node.
                selectedNode = null;
                selectedVisual = null;


                Debug.Log(
                    "Resonance selection canceled."
                );


                return;
            }


            // -----------------------------
            // SECOND SELECTION
            // -----------------------------

            // At this point:
            //
            // selectedNode = Node A
            // nearbyNode   = Node B
            //
            // Connect them.
            selectedNode.LinkWith(
                nearbyNode
            );


            // Remove the selection highlight because
            // the resonance connection is complete.
            if (selectedVisual != null)
            {
                selectedVisual.SetSelected(false);
            }


            // Clear our temporary selection.
            selectedNode = null;
            selectedVisual = null;
        }

        #endregion


        #region Node Detection

        /// <summary>
        /// Finds the closest ResonanceNode inside
        /// the Player's resonance range.
        /// </summary>
        private ResonanceNode FindClosestResonanceNode()
        {
            // Use resonatePoint if one is assigned.
            //
            // Otherwise use the Player's position.
            Vector2 searchPosition =
                resonatePoint != null
                    ? resonatePoint.position
                    : transform.position;


            // Search for all colliders inside
            // our resonance radius.
            Collider2D[] hits =
                Physics2D.OverlapCircleAll(
                    searchPosition,
                    resonateRadius,
                    abilityTargetLayer
                );


            // This will store the closest node found.
            ResonanceNode closestNode = null;


            // Begin with an infinitely large distance.
            float closestDistance =
                Mathf.Infinity;


            // Examine every collider found.
            foreach (Collider2D hit in hits)
            {
                // Search the collider and its parents
                // for a ResonanceNode.
                ResonanceNode node =
                    hit.GetComponentInParent<ResonanceNode>();


                // Ignore objects that aren't
                // resonance nodes.
                if (node == null)
                    continue;


                // Calculate the distance between our
                // search position and this node.
                float distance =
                    Vector2.Distance(
                        searchPosition,
                        node.transform.position
                    );


                // If this is the closest node we've
                // found so far, remember it.
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }


            // Return the closest valid node.
            return closestNode;
        }

        #endregion


        #region Debug Visualization

        private void OnDrawGizmosSelected()
        {
            // Determine where the detection
            // radius should be drawn.
            Vector3 searchPosition =
                resonatePoint != null
                    ? resonatePoint.position
                    : transform.position;


            // Draw our resonance range in the Scene view.
            Gizmos.DrawWireSphere(
                searchPosition,
                resonateRadius
            );
        }

        #endregion
    }
}