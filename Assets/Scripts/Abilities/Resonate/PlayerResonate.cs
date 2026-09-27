using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls the Player's Resonate ability.
    ///
    /// Resonate works using a two-step selection:
    ///
    /// Press Resonate near Node A
    ///      ↓
    /// Node A becomes selected
    ///
    /// Press Resonate near Node B
    ///      ↓
    /// Node A and Node B become linked
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerResonate : MonoBehaviour
    {
        #region Resonate Settings

        [Header("Resonate Settings")]

        // Position used as the center of our
        // resonance detection area.
        [SerializeField] private Transform resonatePoint;

        // How far away the Player can detect
        // resonance nodes.
        [SerializeField] private float resonateRadius = 2f;

        // Layers containing objects that can
        // participate in ability interactions.
        //
        // I recommend using one shared "AbilityTarget"
        // layer for Awaken, Resonate, and Sever.
        [SerializeField] private LayerMask abilityTargetLayer;

        #endregion


        #region Components

        // Player's central input handler.
        private PlayerInputHandler input;

        #endregion


        #region Selection State

        // Stores the first node selected by the Player.
        //
        // Once another node is selected, the two
        // will be connected.
        private ResonanceNode selectedNode;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get the Player's input handler.
            input = GetComponent<PlayerInputHandler>();
        }


        private void OnEnable()
        {
            // Listen for the Resonate input event.
            input.ResonatePressed += HandleResonatePressed;
        }


        private void OnDisable()
        {
            // Stop listening when this component
            // becomes disabled.
            input.ResonatePressed -= HandleResonatePressed;
        }

        #endregion


        #region Resonate Input

        /// <summary>
        /// Called whenever the Player presses
        /// the Resonate button.
        /// </summary>
        private void HandleResonatePressed()
        {
            // Find the closest ResonanceNode
            // within our detection radius.
            ResonanceNode nearbyNode =
                FindClosestResonanceNode();


            // If there isn't a valid node nearby,
            // don't do anything.
            if (nearbyNode == null)
            {
                Debug.Log(
                    "No Resonance Node nearby."
                );

                return;
            }


            // If we haven't selected our first node yet,
            // select this one.
            if (selectedNode == null)
            {
                selectedNode = nearbyNode;


                Debug.Log(
                    $"{selectedNode.gameObject.name} " +
                    $"selected for resonance."
                );


                return;
            }


            // Pressing Resonate on the same selected
            // node cancels the selection.
            if (nearbyNode == selectedNode)
            {
                Debug.Log(
                    $"Resonance selection canceled."
                );


                selectedNode = null;

                return;
            }


            // We now have two different nodes.
            //
            // Create the resonance connection.
            selectedNode.LinkWith(nearbyNode);


            // Clear the selection because the
            // connection is complete.
            selectedNode = null;
        }

        #endregion


        #region Node Detection

        /// <summary>
        /// Searches around resonatePoint and returns
        /// the closest valid ResonanceNode.
        /// </summary>
        private ResonanceNode FindClosestResonanceNode()
        {
            // If we forgot to assign resonatePoint,
            // use the Player's position as a fallback.
            Vector2 searchPosition =
                resonatePoint != null
                ? resonatePoint.position
                : transform.position;


            // Find all colliders within the
            // resonance detection circle.
            Collider2D[] hits =
                Physics2D.OverlapCircleAll(
                    searchPosition,
                    resonateRadius,
                    abilityTargetLayer
                );


            // Store the closest node we find.
            ResonanceNode closestNode = null;


            // Start with an infinitely large distance.
            float closestDistance =
                Mathf.Infinity;


            // Check every collider we detected.
            foreach (Collider2D hit in hits)
            {
                // Look for ResonanceNode directly
                // on the object or one of its parents.
                ResonanceNode node =
                    hit.GetComponentInParent<ResonanceNode>();


                // Ignore objects without ResonanceNode.
                if (node == null)
                    continue;


                // Measure the distance between the Player's
                // search point and this resonance node.
                float distance =
                    Vector2.Distance(
                        searchPosition,
                        node.transform.position
                    );


                // If this node is closer than our
                // previous closest node, remember it.
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }


            // Return whichever node was closest.
            return closestNode;
        }

        #endregion


        #region Debug Visualization

        private void OnDrawGizmosSelected()
        {
            // Choose where the detection circle
            // should be displayed.
            Vector3 searchPosition =
                resonatePoint != null
                ? resonatePoint.position
                : transform.position;


            // Draw the Resonate detection radius
            // while the Player is selected in Unity.
            Gizmos.DrawWireSphere(
                searchPosition,
                resonateRadius
            );
        }

        #endregion
    }
}