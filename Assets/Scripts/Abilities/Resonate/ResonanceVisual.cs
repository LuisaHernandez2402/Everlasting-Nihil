using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Handles the VISUAL side of a ResonanceNode.
    ///
    /// This script does not control the actual resonance mechanic.
    /// ResonanceNode handles the gameplay connection.
    ///
    /// This script handles:
    /// - Highlighting a selected node.
    /// - Drawing the visible resonance line.
    /// - Hiding the resonance line when disconnected.
    /// </summary>
    [RequireComponent(typeof(ResonanceNode))]
    [RequireComponent(typeof(LineRenderer))]
    public class ResonanceVisual : MonoBehaviour
    {
        #region Selection Settings

        [Header("Selection Visual")]

        // The SpriteRenderer belonging to this resonance object.
        //
        // We use this to change its color when the Player
        // selects it as the first resonance node.
        [SerializeField] private SpriteRenderer spriteRenderer;

        // The object's normal color.
        [SerializeField] private Color normalColor = Color.white;

        // The color used while this node is selected.
        [SerializeField] private Color selectedColor = Color.cyan;

        #endregion


        #region Line Settings

        [Header("Resonance Line")]

        // Controls how thick the resonance connection appears.
        [SerializeField] private float lineWidth = 0.08f;

        #endregion


        #region Components

        // Unity component responsible for drawing
        // the visible connection between two nodes.
        private LineRenderer lineRenderer;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get the LineRenderer attached to this object.
            lineRenderer = GetComponent<LineRenderer>();


            // If a SpriteRenderer wasn't manually assigned,
            // try to find one automatically.
            if (spriteRenderer == null)
            {
                spriteRenderer =
                    GetComponentInChildren<SpriteRenderer>();
            }


            // A straight resonance connection only needs
            // two positions:
            //
            // Position 0 = This node
            // Position 1 = Connected node
            lineRenderer.positionCount = 2;


            // World Space means the positions we provide
            // are actual positions inside the scene.
            lineRenderer.useWorldSpace = true;


            // Set the thickness of both ends of the line.
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;


            // There is no connection when the scene starts,
            // so hide the LineRenderer.
            lineRenderer.enabled = false;
        }

        #endregion


        #region Selection Visual

        /// <summary>
        /// Changes the node's appearance depending on
        /// whether the Player currently has it selected.
        /// </summary>
        public void SetSelected(bool isSelected)
        {
            // We can't change the color without
            // a SpriteRenderer.
            if (spriteRenderer == null)
                return;


            // Use selectedColor while selected.
            //
            // Otherwise return to normalColor.
            spriteRenderer.color =
                isSelected
                    ? selectedColor
                    : normalColor;
        }

        #endregion


        #region Connection Visual

        /// <summary>
        /// Draws a resonance connection from this
        /// object to another ResonanceNode.
        /// </summary>
        public void ShowConnection(ResonanceNode connectedNode)
        {
            // If there isn't actually another node,
            // make sure the line is hidden.
            if (connectedNode == null)
            {
                HideConnection();
                return;
            }


            // Turn the LineRenderer on.
            lineRenderer.enabled = true;


            // Start the line at this resonance node.
            lineRenderer.SetPosition(
                0,
                transform.position
            );


            // End the line at the connected node.
            lineRenderer.SetPosition(
                1,
                connectedNode.transform.position
            );
        }


        /// <summary>
        /// Removes the visible resonance connection.
        /// </summary>
        public void HideConnection()
        {
            // We don't destroy the LineRenderer.
            //
            // We simply disable it so we can use
            // it again for another connection later.
            lineRenderer.enabled = false;
        }

        #endregion
    }
}