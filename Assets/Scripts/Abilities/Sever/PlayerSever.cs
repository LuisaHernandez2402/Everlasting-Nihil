using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls the Player's Sever ability.
    ///
    /// Sever searches for the nearest linked
    /// ResonanceNode and breaks its connection.
    ///
    /// The actual connection logic belongs to
    /// ResonanceNode.
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerSever : MonoBehaviour
    {
        #region Sever Settings

        [Header("Sever Settings")]

        // Point used as the center of our
        // Sever detection radius.
        [SerializeField] private Transform severPoint;

        // Maximum distance at which the Player
        // can Sever a resonance connection.
        [SerializeField] private float severRadius = 2f;

        // Shared layer containing objects that
        // abilities can interact with.
        //
        // Use the same AbilityTarget layer
        // as Awaken and Resonate.
        [SerializeField] private LayerMask abilityTargetLayer;

        #endregion


        #region Components

        // Player's central input handler.
        private PlayerInputHandler input;

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
            // Listen for Sever input.
            input.SeverPressed +=
                HandleSeverPressed;
        }


        private void OnDisable()
        {
            // Stop listening when this component
            // becomes disabled.
            input.SeverPressed -=
                HandleSeverPressed;
        }

        #endregion


        #region Sever Input

        /// <summary>
        /// Called whenever the Player presses
        /// the Sever button.
        /// </summary>
        private void HandleSeverPressed()
        {
            // Find the closest linked resonance node.
            ResonanceNode node =
                FindClosestLinkedNode();


            // If there isn't a linked node nearby,
            // Sever has nothing to affect.
            if (node == null)
            {
                Debug.Log(
                    "No resonance connection nearby to sever."
                );

                return;
            }


            // Tell the node to intentionally
            // sever its connection.
            node.Sever();
        }

        #endregion


        #region Node Detection

        /// <summary>
        /// Finds the closest LINKED ResonanceNode
        /// inside the Sever radius.
        /// </summary>
        private ResonanceNode FindClosestLinkedNode()
        {
            // Use severPoint when assigned.
            //
            // Otherwise fall back to the Player's position.
            Vector2 searchPosition =
                severPoint != null
                    ? severPoint.position
                    : transform.position;


            // Find all AbilityTarget colliders
            // inside the Sever radius.
            Collider2D[] hits =
                Physics2D.OverlapCircleAll(
                    searchPosition,
                    severRadius,
                    abilityTargetLayer
                );


            // Store the closest valid node.
            ResonanceNode closestNode = null;


            // Begin with an infinitely large distance.
            float closestDistance =
                Mathf.Infinity;


            // Check every collider found.
            foreach (Collider2D hit in hits)
            {
                // Search this object and its parents
                // for a ResonanceNode.
                ResonanceNode node =
                    hit.GetComponentInParent<ResonanceNode>();


                // Ignore anything that isn't
                // a ResonanceNode.
                if (node == null)
                    continue;


                // Sever should only target nodes that
                // currently have a connection.
                if (!node.IsLinked)
                    continue;


                // Measure the distance to this node.
                float distance =
                    Vector2.Distance(
                        searchPosition,
                        node.transform.position
                    );


                // Keep whichever linked node
                // is closest to the Player.
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }


            // Return the closest linked node found.
            return closestNode;
        }

        #endregion


        #region Debug Visualization

        private void OnDrawGizmosSelected()
        {
            // Determine where the Sever radius
            // should be displayed.
            Vector3 searchPosition =
                severPoint != null
                    ? severPoint.position
                    : transform.position;


            // Draw the Sever range while the
            // Player is selected in the Scene view.
            Gizmos.DrawWireSphere(
                searchPosition,
                severRadius
            );
        }

        #endregion
    }
}