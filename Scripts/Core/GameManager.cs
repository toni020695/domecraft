using Unity.Netcode;
using UnityEngine;

namespace DomeCraft.Core
{
    /// <summary>
    /// Enumerates the high-level states the game can be in.
    /// Additional states (e.g. Loading, CharacterSelect) can be added here
    /// as the project grows.
    /// </summary>
    public enum GameState
    {
        /// <summary>Players are waiting in the lobby before a session begins.</summary>
        Lobby,

        /// <summary>An active game session is in progress.</summary>
        Playing,
    }

    /// <summary>
    /// Singleton that owns the authoritative <see cref="GameState"/> and exposes
    /// simple state-transition helpers.  Designed to live on a persistent
    /// GameObject (DontDestroyOnLoad) so it survives scene loads.
    ///
    /// Extend this class to add more complex state machines, event buses, or
    /// loading screens as the project evolves.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────────

        /// <summary>Global access point to the single <see cref="GameManager"/> instance.</summary>
        public static GameManager Instance { get; private set; }

        // ─── State ───────────────────────────────────────────────────────────────

        /// <summary>The current high-level game state.</summary>
        public GameState CurrentState { get; private set; } = GameState.Lobby;

        // ─── Events ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Raised whenever <see cref="CurrentState"/> changes.
        /// Listeners receive the new <see cref="GameState"/> as the argument.
        /// </summary>
        public event System.Action<GameState> OnGameStateChanged;

        // ─── Unity Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[GameManager] Duplicate instance detected – destroying the new one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameManager] Initialized.");
        }

        // ─── Public API ─────────────────────────────────────────────────────────

        /// <summary>
        /// Transitions the game to the <see cref="GameState.Lobby"/> state.
        /// Call this when returning to the lobby (e.g. after a session ends).
        /// </summary>
        public void GoToLobby()
        {
            TransitionTo(GameState.Lobby);
        }

        /// <summary>
        /// Transitions the game to the <see cref="GameState.Playing"/> state.
        /// Call this once all players are ready and the session should start.
        /// </summary>
        public void StartPlaying()
        {
            TransitionTo(GameState.Playing);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Internal helper that changes <see cref="CurrentState"/> and fires
        /// <see cref="OnGameStateChanged"/>.  Guards against no-op transitions.
        /// </summary>
        /// <param name="newState">The state to transition to.</param>
        private void TransitionTo(GameState newState)
        {
            if (CurrentState == newState)
            {
                Debug.LogWarning($"[GameManager] Already in state {newState}. Transition ignored.");
                return;
            }

            Debug.Log($"[GameManager] State transition: {CurrentState} → {newState}");
            CurrentState = newState;
            OnGameStateChanged?.Invoke(CurrentState);
        }
    }
}
