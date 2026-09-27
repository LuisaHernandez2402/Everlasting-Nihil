using UnityEngine;
using UnityEngine.Events;

namespace EverlastingNihil
{
    /// <summary>
    /// Represents an object that can participate
    /// in the Resonate and Sever mechanics.
    ///
    /// A ResonanceNode can:
    /// - Connect to another node.
    /// - Send reactions through that connection.
    /// - Receive reactions.
    /// - Have its connection severed.
    /// </summary>
    [RequireComponent(typeof(ReactionEmitter))]
    public class ResonanceNode : MonoBehaviour
    {
        #region Sever Events

        [Header("Sever Reaction")]

        // Optional Inspector event triggered when
        // this node's connection is intentionally severed.
        //
        // Later this can trigger things such as:
        //
        // - A platform falling
        // - A crystal exploding
        // - A door mechanism changing
        // - A boss becoming vulnerable
        // - A chain reaction continuing
        [SerializeField] private UnityEvent onSevered;

        #endregion


        #region Components

        // Sends reactions from this object.
        private ReactionEmitter reactionEmitter;

        // Optional component capable of receiving
        // a reaction.
        private IReactable reactable;

        // Handles the visible resonance connection.
        private ResonanceVisual resonanceVisual;

        #endregion


        #region Resonance State

        // The node currently connected to this object.
        private ResonanceNode linkedNode;

        // Allows other scripts to see which node
        // we're connected to.
        public ResonanceNode LinkedNode => linkedNode;

        // Returns true if this node currently
        // has a resonance connection.
        public bool IsLinked => linkedNode != null;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get the required ReactionEmitter.
            reactionEmitter =
                GetComponent<ReactionEmitter>();


            // Search for an optional IReactable component.
            reactable =
                GetComponent<IReactable>();


            // Search for our optional visual component.
            resonanceVisual =
                GetComponent<ResonanceVisual>();
        }


        private void OnEnable()
        {
            // Listen for reactions emitted
            // by this object.
            reactionEmitter.ReactionEmitted +=
                HandleReactionEmitted;
        }


        private void OnDisable()
        {
            // Stop listening when this component
            // becomes disabled.
            reactionEmitter.ReactionEmitted -=
                HandleReactionEmitted;
        }

        #endregion


        #region Linking

        /// <summary>
        /// Creates a two-way resonance connection
        /// between this node and another node.
        /// </summary>
        public void LinkWith(ResonanceNode otherNode)
        {
            // Cannot connect to nothing.
            if (otherNode == null)
                return;


            // Cannot connect a node to itself.
            if (otherNode == this)
                return;


            // Remove any existing connection from
            // this node before creating the new one.
            Unlink();


            // Remove the other node's existing
            // connection as well.
            otherNode.Unlink();


            // Store our new connection.
            linkedNode = otherNode;


            // Make the connection two-way.
            otherNode.linkedNode = this;


            // Draw the visible resonance connection.
            if (resonanceVisual != null)
            {
                resonanceVisual.ShowConnection(
                    otherNode
                );
            }


            Debug.Log(
                $"{gameObject.name} resonated with " +
                $"{otherNode.gameObject.name}."
            );
        }


        /// <summary>
        /// Removes the current connection WITHOUT
        /// triggering a Sever reaction.
        ///
        /// This is used internally when connections
        /// are replaced or cleaned up.
        /// </summary>
        public void Unlink()
        {
            // Nothing to remove if we're not connected.
            if (linkedNode == null)
                return;


            // Save the previous node before
            // clearing our connection.
            ResonanceNode previousNode =
                linkedNode;


            // Remove our side.
            linkedNode = null;


            // Remove the other side if it still
            // points back toward us.
            if (previousNode.linkedNode == this)
            {
                previousNode.linkedNode = null;
            }


            // Hide our resonance line.
            if (resonanceVisual != null)
            {
                resonanceVisual.HideConnection();
            }


            // The other node might currently own
            // the visible LineRenderer.
            ResonanceVisual previousVisual =
                previousNode.GetComponent<ResonanceVisual>();


            // Hide its line as well.
            if (previousVisual != null)
            {
                previousVisual.HideConnection();
            }


            Debug.Log(
                $"{gameObject.name} resonance connection removed."
            );
        }

        #endregion


        #region Sever

        /// <summary>
        /// Intentionally severs this node's current
        /// resonance connection.
        ///
        /// Unlike Unlink(), this also fires the
        /// node's Sever reaction.
        /// </summary>
        public void Sever()
        {
            // Sever should only work if this node
            // actually has a connection.
            if (linkedNode == null)
            {
                Debug.Log(
                    $"{gameObject.name} has no resonance " +
                    $"connection to sever."
                );

                return;
            }


            // Save the connected node before Unlink()
            // removes the reference.
            ResonanceNode previousNode =
                linkedNode;


            // Remove the actual resonance connection.
            Unlink();


            Debug.Log(
                $"Resonance between {gameObject.name} and " +
                $"{previousNode.gameObject.name} was severed!"
            );


            // Trigger any puzzle behavior assigned
            // through the Inspector.
            onSevered?.Invoke();
        }

        #endregion


        #region Reaction Transmission

        /// <summary>
        /// Called whenever this object's
        /// ReactionEmitter emits.
        /// </summary>
        private void HandleReactionEmitted()
        {
            // Without a resonance connection,
            // the reaction has nowhere to travel.
            if (linkedNode == null)
                return;


            Debug.Log(
                $"Reaction traveled from " +
                $"{gameObject.name} to " +
                $"{linkedNode.gameObject.name}."
            );


            // Send the reaction to the linked node.
            linkedNode.ReceiveResonance();
        }


        /// <summary>
        /// Receives a reaction sent through
        /// a resonance connection.
        /// </summary>
        private void ReceiveResonance()
        {
            // If this object can react,
            // trigger that reaction.
            if (reactable != null)
            {
                reactable.React();

                return;
            }


            Debug.Log(
                $"{gameObject.name} received resonance, " +
                $"but has no IReactable component."
            );
        }

        #endregion
    }
}