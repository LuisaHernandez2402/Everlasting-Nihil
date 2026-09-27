using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Allows an object to participate in the
    /// Player's Resonate ability.
    ///
    /// A ResonanceNode listens for reactions emitted
    /// by its own ReactionEmitter.
    ///
    /// If this node is linked to another node,
    /// the reaction is sent to that linked object.
    /// </summary>
    [RequireComponent(typeof(ReactionEmitter))]
    public class ResonanceNode : MonoBehaviour
    {
        #region Components

        // ReactionEmitter attached to this object.
        //
        // When this emitter fires, we can send that
        // reaction through the resonance connection.
        private ReactionEmitter reactionEmitter;

        // Optional component capable of receiving reactions.
        //
        // Not every resonance object necessarily needs
        // to be reactable itself.
        private IReactable reactable;

        #endregion


        #region Resonance State

        // The other ResonanceNode currently connected
        // to this node.
        private ResonanceNode linkedNode;

        // Public read-only access to the linked node.
        public ResonanceNode LinkedNode => linkedNode;

        // Returns true when this node currently
        // has a resonance connection.
        public bool IsLinked => linkedNode != null;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get our ReactionEmitter.
            reactionEmitter =
                GetComponent<ReactionEmitter>();


            // Look for any component on this GameObject
            // implementing IReactable.
            //
            // Example:
            //
            // Crystal
            // ├── ReactionEmitter
            // ├── ResonanceNode
            // └── AwakenTestReaction : IReactable
            reactable =
                GetComponent<IReactable>();
        }


        private void OnEnable()
        {
            // Listen for reactions emitted by this object.
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
            // We cannot link to nothing.
            if (otherNode == null)
                return;


            // Prevent an object from resonating
            // with itself.
            if (otherNode == this)
                return;


            // Remove any previous connections first.
            Unlink();


            // If the other node already has a connection,
            // remove that connection as well.
            otherNode.Unlink();


            // Connect this node to the other node.
            linkedNode = otherNode;


            // Connect the other node back to this node.
            //
            // This makes resonance a two-way relationship.
            otherNode.linkedNode = this;


            Debug.Log(
                $"{gameObject.name} resonated with " +
                $"{otherNode.gameObject.name}."
            );
        }


        /// <summary>
        /// Removes the current resonance connection.
        /// </summary>
        public void Unlink()
        {
            // If there is no connection,
            // there is nothing to remove.
            if (linkedNode == null)
                return;


            // Save the old connection before clearing it.
            ResonanceNode previousNode = linkedNode;


            // Remove our connection.
            linkedNode = null;


            // Make sure the other node also stops
            // pointing back toward us.
            if (previousNode.linkedNode == this)
            {
                previousNode.linkedNode = null;
            }


            Debug.Log(
                $"{gameObject.name} resonance connection removed."
            );
        }

        #endregion


        #region Reaction Transmission

        /// <summary>
        /// Called whenever this object's ReactionEmitter
        /// emits a reaction.
        /// </summary>
        private void HandleReactionEmitted()
        {
            // If we're not resonating with anything,
            // the reaction has nowhere to travel.
            if (linkedNode == null)
                return;


            Debug.Log(
                $"Reaction traveled from {gameObject.name} " +
                $"to {linkedNode.gameObject.name}."
            );


            // Send the reaction to the connected node.
            linkedNode.ReceiveResonance();
        }


        /// <summary>
        /// Receives a reaction sent through
        /// a resonance connection.
        /// </summary>
        private void ReceiveResonance()
        {
            // If this object contains something implementing
            // IReactable, tell it to react.
            if (reactable != null)
            {
                reactable.React();
            }
            else
            {
                Debug.Log(
                    $"{gameObject.name} received resonance, " +
                    $"but has no IReactable component."
                );
            }
        }

        #endregion
    }
}