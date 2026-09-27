using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Handles the Player's Sever ability.
    ///
    /// Sever finds the closest linked ResonanceNode
    /// and destroys its active connection.
    ///
    /// The Player must own the Sever Crystal
    /// before this ability can be used.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerAbilityManager))]
    public class PlayerSever : MonoBehaviour
    {
        #region Sever Settings

        [Header("Sever Settings")]

        // Center point used to search
        // for linked ResonanceNodes.
        [SerializeField] private Transform severPoint;

        // Maximum range of the Sever ability.
        [SerializeField] private float severRadius = 2f;

        // Determines which layers Sever
        // is allowed to target.
        //
        // Set this to AbilityTarget.
        [SerializeField] private LayerMask abilityTargetLayer;

        #endregion


        #region Components

        // Handles Player input events.
        private PlayerInputHandler inputHandler;

        // Determines whether Sever
        // has been unlocked.
        private PlayerAbilityManager abilityManager;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Cache our input handler.
            inputHandler =
                GetComponent<PlayerInputHandler>();

            // Cache our ability progression manager.
            abilityManager =
                GetComponent<PlayerAbilityManager>();
        }


        private void OnEnable()
        {
            // Subscribe to the Sever input event.
            if (inputHandler != null)
            {
                inputHandler.SeverPressed +=
                    HandleSeverPressed;
            }
        }


        private void OnDisable()
        {
            // Remove our input subscription.
            if (inputHandler != null)
            {
                inputHandler.SeverPressed -=
                    HandleSeverPressed;
            }
        }

        #endregion


        #region Sever Input

        /// <summary>
        /// Called when the Player presses
        /// the Sever input.
        /// </summary>
        private void HandleSeverPressed()
        {
            // -----------------------------
            // ABILITY UNLOCK CHECK
            // -----------------------------

            // Sever cannot be used until
            // its crystal has been collected.
            if (abilityManager == null ||
                !abilityManager.CanSever)
            {
                Debug.Log(
                    "Sever is locked. " +
                    "Find the Sever Crystal first."
                );

                return;
            }


            // -----------------------------
            // SEVER POINT CHECK
            // -----------------------------

            // Stop safely if SeverPoint
            // wasn't assigned.
            if (severPoint == null)
            {
                Debug.LogWarning(
                    "PlayerSever has no SeverPoint assigned."
                );

                return;
            }


            // Find the closest linked node.
            ResonanceNode node =
                FindClosestLinkedNode();


            // Nothing nearby can currently be severed.
            if (node == null)
            {
                Debug.Log(
                    "No linked ResonanceNode is within Sever range."
                );

                return;
            }


            // Break the connection.
            node.Sever();
        }

        #endregion


        #region Node Detection

        /// <summary>
        /// Finds the closest ResonanceNode that
        /// currently has an active connection.
        /// </summary>
        private ResonanceNode FindClosestLinkedNode()
        {
            // Find all AbilityTarget colliders
            // within Sever's range.
            Collider2D[] targets =
                Physics2D.OverlapCircleAll(
                    severPoint.position,
                    severRadius,
                    abilityTargetLayer
                );


            // Store the closest valid node.
            ResonanceNode closestNode = null;

            // Begin with an infinitely
            // large comparison distance.
            float closestDistance =
                Mathf.Infinity;


            // Check each detected object.
            foreach (Collider2D target in targets)
            {
                // Try to find a ResonanceNode.
                ResonanceNode node =
                    target.GetComponent<ResonanceNode>();


                // Ignore objects without a node.
                if (node == null)
                    continue;


                // Sever only works on nodes
                // that currently have a connection.
                if (!node.IsLinked)
                    continue;


                // Calculate distance to this node.
                float distance =
                    Vector2.Distance(
                        severPoint.position,
                        node.transform.position
                    );


                // Ignore it if another valid node
                // is already closer.
                if (distance >= closestDistance)
                    continue;


                // Store our new closest node.
                closestDistance = distance;

                closestNode = node;
            }


            return closestNode;
        }

        #endregion


        #region Editor Visualization

        /// <summary>
        /// Shows Sever's detection radius
        /// inside the Scene view.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // We cannot draw the radius without
            // a SeverPoint.
            if (severPoint == null)
                return;


            // Draw Sever's range.
            Gizmos.DrawWireSphere(
                severPoint.position,
                severRadius
            );
        }

        #endregion
    }
}