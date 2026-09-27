using System;
using UnityEngine;

namespace EverlastingNihil
{
    /// <summary>
    /// Represents the major states the game
    /// can currently be in.
    ///
    /// Other systems can check the current state
    /// or listen for state changes.
    /// </summary>
    public enum GameState
    {
        // Normal gameplay.
        Playing,

        // The game is paused.
        Paused,

        // A cutscene or scripted sequence is happening.
        Cutscene,

        // The Player is currently fighting a boss.
        BossFight,

        // The Player has reached a game-over state.
        GameOver
    }


    /// <summary>
    /// Central manager for Everlasting Nihil.
    ///
    /// The GameManager is responsible for broad
    /// game-level information such as:
    ///
    /// - Current game state
    /// - Pausing and resuming
    /// - Global state-change events
    ///
    /// It should NOT contain every gameplay system.
    /// Individual systems should still handle
    /// their own specific responsibilities.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton

        // Gives other scripts easy access to
        // the active GameManager.
        //
        // Example:
        //
        // GameManager.Instance.PauseGame();
        public static GameManager Instance
        {
            get;
            private set;
        }

        #endregion


        #region Game State

        // Stores the game's current state.
        //
        // Other scripts can read this value,
        // but only GameManager can directly change it.
        public GameState CurrentState
        {
            get;
            private set;
        }

        // Stores the state we were in before pausing.
        //
        // This means if we pause during a boss fight,
        // we can return to BossFight instead of
        // incorrectly returning to normal Playing.
        private GameState stateBeforePause;

        #endregion


        #region Events

        // Fired whenever the overall game state changes.
        //
        // Other systems can subscribe to this instead
        // of constantly checking the GameManager.
        public event Action<GameState> GameStateChanged;

        #endregion


        #region Unity Methods

        private void Awake()
        {
            // -----------------------------
            // SINGLETON SETUP
            // -----------------------------

            // If there isn't already a GameManager,
            // this object becomes the active instance.
            if (Instance == null)
            {
                Instance = this;


                // Keep the GameManager alive when
                // changing between scenes.
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // If another GameManager already exists,
                // destroy this duplicate.
                //
                // This prevents multiple managers
                // from existing after scene changes.
                Destroy(gameObject);

                return;
            }


            // -----------------------------
            // STARTING STATE
            // -----------------------------

            // Start the game in normal gameplay.
            CurrentState = GameState.Playing;

            // Make sure time is running normally.
            Time.timeScale = 1f;
        }


        private void OnDestroy()
        {
            // Only clear Instance if THIS object
            // is the active GameManager.
            //
            // This prevents a duplicate GameManager
            // from accidentally clearing the real one.
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #endregion


        #region State Management

        /// <summary>
        /// Changes the game's current state.
        /// </summary>
        public void ChangeState(GameState newState)
        {
            // Don't perform another state change
            // if we're already in that state.
            if (CurrentState == newState)
                return;


            // Store the new state.
            CurrentState = newState;


            // Print the change while we're developing.
            Debug.Log(
                $"Game State changed to: {CurrentState}"
            );


            // Tell any listening systems that
            // the game state changed.
            GameStateChanged?.Invoke(CurrentState);
        }

        #endregion


        #region Pause System

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            // Don't pause twice.
            if (CurrentState == GameState.Paused)
                return;


            // Remember what was happening before
            // the game was paused.
            stateBeforePause = CurrentState;


            // Change our logical game state.
            ChangeState(GameState.Paused);


            // Stop Unity's scaled game time.
            //
            // Physics and most gameplay systems
            // using scaled time will stop.
            Time.timeScale = 0f;
        }


        /// <summary>
        /// Resumes the game from pause.
        /// </summary>
        public void ResumeGame()
        {
            // Resume should only happen
            // while we're actually paused.
            if (CurrentState != GameState.Paused)
                return;


            // Start Unity's game time again.
            Time.timeScale = 1f;


            // Return to whatever state existed
            // before pausing.
            ChangeState(stateBeforePause);
        }


        /// <summary>
        /// Switches between paused and unpaused.
        ///
        /// This will eventually be called when
        /// the Player presses Escape / Start.
        /// </summary>
        public void TogglePause()
        {
            // Resume if we're currently paused.
            if (CurrentState == GameState.Paused)
            {
                ResumeGame();

                return;
            }


            // Otherwise pause the game.
            PauseGame();
        }

        #endregion


        #region Boss State

        /// <summary>
        /// Tells the GameManager that a
        /// boss encounter has started.
        /// </summary>
        public void StartBossFight()
        {
            ChangeState(
                GameState.BossFight
            );
        }


        /// <summary>
        /// Tells the GameManager that the
        /// current boss encounter has ended.
        /// </summary>
        public void EndBossFight()
        {
            // Only return to Playing if we
            // were actually fighting a boss.
            if (CurrentState != GameState.BossFight)
                return;


            ChangeState(
                GameState.Playing
            );
        }

        #endregion


        #region Game Over

        /// <summary>
        /// Changes the game into its
        /// Game Over state.
        /// </summary>
        public void GameOver()
        {
            ChangeState(
                GameState.GameOver
            );
        }

        #endregion
    }
}