using System.Collections;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Controls a fullscreen CanvasGroup used
    /// for scene transition fades.
    ///
    /// Alpha 0 = Screen visible.
    /// Alpha 1 = Screen completely covered.
    /// </summary>
    public class ScreenFader : MonoBehaviour
    {
        #region Settings

        [Header("Fade Settings")]

        // CanvasGroup covering the entire screen.
        [SerializeField] private CanvasGroup fadeCanvas;

        // Duration of each fade.
        [SerializeField] private float fadeDuration = 0.5f;

        #endregion


        #region Public Fade Methods

        /// <summary>
        /// Fades the screen from visible
        /// to completely black.
        /// </summary>
        public IEnumerator FadeOut()
        {
            // Fade toward alpha 1.
            yield return Fade(
                fadeCanvas.alpha,
                1f
            );
        }


        /// <summary>
        /// Fades the screen from black
        /// back to visible gameplay.
        /// </summary>
        public IEnumerator FadeIn()
        {
            // Fade toward alpha 0.
            yield return Fade(
                fadeCanvas.alpha,
                0f
            );
        }

        #endregion


        #region Fade Logic

        /// <summary>
        /// Gradually changes the CanvasGroup's alpha.
        /// </summary>
        private IEnumerator Fade(
            float startAlpha,
            float targetAlpha
        )
        {
            // Make sure a CanvasGroup exists.
            if (fadeCanvas == null)
            {
                Debug.LogWarning(
                    "ScreenFader has no CanvasGroup assigned."
                );

                yield break;
            }


            // Track elapsed fade time.
            float elapsed = 0f;


            // Continue until the fade duration
            // has been reached.
            while (elapsed < fadeDuration)
            {
                // Use unscaled time so fades still work
                // even if the game happens to be paused.
                elapsed += Time.unscaledDeltaTime;


                // Convert elapsed time into
                // a value between 0 and 1.
                float progress =
                    Mathf.Clamp01(
                        elapsed / fadeDuration
                    );


                // Smoothly interpolate the alpha.
                fadeCanvas.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        targetAlpha,
                        progress
                    );


                // Wait until the next rendered frame.
                yield return null;
            }


            // Guarantee the final alpha is exact.
            fadeCanvas.alpha =
                targetAlpha;
        }

        #endregion
    }
}