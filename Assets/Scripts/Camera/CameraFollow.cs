using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Smoothly follows the Player while keeping
    /// the camera inside the current area's boundaries.
    ///
    /// The camera uses LateUpdate instead of Update
    /// so Player movement happens before camera movement.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        #region Follow Settings

        [Header("Follow Settings")]

        // The Transform that the camera follows.
        // Normally this will be the Player.
        [SerializeField] private Transform target;

        // Controls how quickly the camera
        // catches up to the Player.
        [SerializeField] private float followSpeed = 5f;

        // Allows the camera to sit slightly
        // away from the Player's exact position.
        [SerializeField] private Vector2 offset =
            new Vector2(0f, 1f);

        #endregion


        #region Axis Settings

        [Header("Axis Settings")]

        // Allows horizontal camera following.
        [SerializeField] private bool followX = true;

        // Allows vertical camera following.
        [SerializeField] private bool followY = true;

        #endregion


        #region Dead Zone Settings

        [Header("Dead Zone Settings")]

        // The Player can move this far vertically
        // before the camera begins following them.
        [SerializeField] private float verticalDeadZone = 1.5f;

        #endregion


        #region Boundary Settings

        [Header("Boundary Settings")]

        // Collider representing the current
        // area the camera is allowed to show.
        [SerializeField] private BoxCollider2D cameraBounds;

        // Determines whether camera boundaries
        // are currently being used.
        [SerializeField] private bool useBounds = true;

        #endregion


        #region Components

        // Reference to the Camera component
        // attached to this GameObject.
        private Camera cameraComponent;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Cache the Camera component so we
            // don't repeatedly search for it.
            cameraComponent =
                GetComponent<Camera>();
        }


        /// <summary>
        /// Moves the camera after the Player's
        /// movement has already been calculated.
        /// </summary>
        private void LateUpdate()
        {
            // We cannot follow anything if
            // no target has been assigned.
            if (target == null)
                return;


            // Begin with our current position.
            Vector3 desiredPosition =
                transform.position;


            // -----------------------------
            // HORIZONTAL FOLLOW
            // -----------------------------

            if (followX)
            {
                // Follow the Player horizontally
                // while including our X offset.
                desiredPosition.x =
                    target.position.x +
                    offset.x;
            }


            // -----------------------------
            // VERTICAL FOLLOW
            // -----------------------------

            if (followY)
            {
                // Calculate how far the Player
                // currently is from the camera.
                float verticalDifference =
                    target.position.y -
                    transform.position.y;


                // Only follow vertically after
                // leaving the dead zone.
                if (Mathf.Abs(verticalDifference) >
                    verticalDeadZone)
                {
                    desiredPosition.y =
                        target.position.y +
                        offset.y;
                }
            }


            // -----------------------------
            // CAMERA DEPTH
            // -----------------------------

            // Preserve our camera's Z position.
            // A typical 2D camera uses -10.
            desiredPosition.z =
                transform.position.z;


            // -----------------------------
            // CAMERA BOUNDARIES
            // -----------------------------

            if (useBounds &&
                cameraBounds != null)
            {
                // Restrict the desired position
                // before smoothing toward it.
                desiredPosition =
                    ClampToBounds(
                        desiredPosition
                    );
            }


            // -----------------------------
            // SMOOTH FOLLOW
            // -----------------------------

            // Smoothly move toward the final
            // calculated camera position.
            transform.position =
                Vector3.Lerp(
                    transform.position,
                    desiredPosition,
                    followSpeed *
                    Time.deltaTime
                );
        }

        #endregion


        #region Boundary Logic

        /// <summary>
        /// Prevents any edge of the camera view
        /// from moving outside cameraBounds.
        /// </summary>
        private Vector3 ClampToBounds(
            Vector3 desiredPosition
        )
        {
            // Get the world-space bounds
            // of our BoxCollider2D.
            Bounds bounds =
                cameraBounds.bounds;


            // Orthographic Size represents half
            // of the camera's vertical view.
            float cameraHalfHeight =
                cameraComponent.orthographicSize;


            // Calculate half of the camera's
            // horizontal view using aspect ratio.
            float cameraHalfWidth =
                cameraHalfHeight *
                cameraComponent.aspect;


            // Calculate how far the camera's
            // CENTER may travel horizontally.
            float minimumX =
                bounds.min.x +
                cameraHalfWidth;

            float maximumX =
                bounds.max.x -
                cameraHalfWidth;


            // Calculate how far the camera's
            // CENTER may travel vertically.
            float minimumY =
                bounds.min.y +
                cameraHalfHeight;

            float maximumY =
                bounds.max.y -
                cameraHalfHeight;


            // If the boundary is narrower than the
            // camera itself, center the camera instead
            // of producing invalid clamp values.
            if (minimumX > maximumX)
            {
                desiredPosition.x =
                    bounds.center.x;
            }
            else
            {
                // Restrict horizontal movement.
                desiredPosition.x =
                    Mathf.Clamp(
                        desiredPosition.x,
                        minimumX,
                        maximumX
                    );
            }


            // Do the same thing vertically.
            if (minimumY > maximumY)
            {
                desiredPosition.y =
                    bounds.center.y;
            }
            else
            {
                // Restrict vertical movement.
                desiredPosition.y =
                    Mathf.Clamp(
                        desiredPosition.y,
                        minimumY,
                        maximumY
                    );
            }


            // Return the corrected position.
            return desiredPosition;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Changes which Transform the
        /// camera currently follows.
        /// </summary>
        public void SetTarget(
            Transform newTarget
        )
        {
            // Store the new target.
            target = newTarget;
        }


        /// <summary>
        /// Changes the camera's current boundary.
        ///
        /// This will eventually allow different rooms
        /// to have different camera limits.
        /// </summary>
        public void SetBounds(
            BoxCollider2D newBounds
        )
        {
            // Store the new boundary.
            cameraBounds = newBounds;
        }


        /// <summary>
        /// Enables or disables camera boundaries.
        /// </summary>
        public void SetBoundsEnabled(
            bool enabled
        )
        {
            // Update our boundary state.
            useBounds = enabled;
        }

        #endregion
    }
}