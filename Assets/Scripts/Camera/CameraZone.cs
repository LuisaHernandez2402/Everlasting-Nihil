using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Defines a camera area inside the level.
    ///
    /// When the Player enters this trigger,
    /// the Main Camera switches to this zone's
    /// camera boundaries.
    ///
    /// No Update() is required.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CameraZone : MonoBehaviour
    {
        #region Camera Zone Settings

        [Header("Camera Zone Settings")]

        // The BoxCollider2D that represents
        // the camera limits for this room.
        [SerializeField] private BoxCollider2D cameraBounds;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // Get the collider attached to
            // this CameraZone GameObject.
            Collider2D zoneCollider =
                GetComponent<Collider2D>();

            // Camera zones should detect the Player,
            // not physically block them.
            zoneCollider.isTrigger = true;


            // Warn us if we forgot to assign
            // camera boundaries.
            if (cameraBounds == null)
            {
                Debug.LogWarning(
                    $"{gameObject.name} has no camera bounds assigned."
                );
            }
        }

        #endregion


        #region Trigger Detection

        /// <summary>
        /// Called automatically when something
        /// enters this CameraZone.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check whether the entering collider
            // belongs to our Player.
            PlayerMovement player =
                other.GetComponentInParent<PlayerMovement>();


            // Ignore anything that isn't the Player.
            if (player == null)
                return;


            // Find the active Main Camera.
            Camera mainCamera =
                Camera.main;


            // Stop safely if no Main Camera exists.
            if (mainCamera == null)
            {
                Debug.LogWarning(
                    "CameraZone could not find the Main Camera."
                );

                return;
            }


            // Get our CameraFollow component.
            CameraFollow cameraFollow =
                mainCamera.GetComponent<CameraFollow>();


            // Stop if the Main Camera doesn't
            // have CameraFollow.
            if (cameraFollow == null)
            {
                Debug.LogWarning(
                    "Main Camera does not have CameraFollow."
                );

                return;
            }


            // Give CameraFollow this room's
            // boundary collider.
            cameraFollow.SetBounds(
                cameraBounds
            );


            // Make sure boundary limiting is enabled.
            cameraFollow.SetBoundsEnabled(
                true
            );


            Debug.Log(
                $"Camera entered zone: {gameObject.name}"
            );
        }

        #endregion
    }
}