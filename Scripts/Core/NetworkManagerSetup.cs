using Unity.Netcode;
using UnityEngine;

namespace DomeCraft.Core
{
    /// <summary>
    /// Bootstraps the Unity Netcode for GameObjects NetworkManager.
    /// Provides convenience methods to start the game as a Host (server + client)
    /// or as a pure Client. Attach this component to the same GameObject that
    /// holds the <see cref="NetworkManager"/> component.
    /// </summary>
    public class NetworkManagerSetup : MonoBehaviour
    {
        // ─── Inspector ──────────────────────────────────────────────────────────

        /// <summary>
        /// Reference to the scene's <see cref="NetworkManager"/> instance.
        /// If left null the component will attempt to find one automatically.
        /// </summary>
        [Tooltip("Drag the NetworkManager GameObject here, or leave null for auto-discovery.")]
        [SerializeField] private NetworkManager networkManager;

        // ─── Unity Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            if (networkManager == null)
            {
                networkManager = NetworkManager.Singleton;
            }

            if (networkManager == null)
            {
                Debug.LogError("[NetworkManagerSetup] No NetworkManager found in the scene.");
            }
        }

        // ─── Public API ─────────────────────────────────────────────────────────

        /// <summary>
        /// Starts a local Host session (acts as both server and client).
        /// Use this when the player wants to host a game session.
        /// </summary>
        public void StartHost()
        {
            if (!EnsureNetworkManager()) return;

            if (networkManager.StartHost())
            {
                Debug.Log("[NetworkManagerSetup] Host started successfully.");
            }
            else
            {
                Debug.LogError("[NetworkManagerSetup] Failed to start Host.");
            }
        }

        /// <summary>
        /// Starts a Client session that connects to a remote or local host.
        /// The connection address / port must be configured on the
        /// <see cref="NetworkManager"/> transport before calling this method.
        /// </summary>
        public void StartClient()
        {
            if (!EnsureNetworkManager()) return;

            if (networkManager.StartClient())
            {
                Debug.Log("[NetworkManagerSetup] Client started successfully.");
            }
            else
            {
                Debug.LogError("[NetworkManagerSetup] Failed to start Client.");
            }
        }

        /// <summary>
        /// Starts a dedicated Server session (no local client).
        /// Useful for headless server builds.
        /// </summary>
        public void StartServer()
        {
            if (!EnsureNetworkManager()) return;

            if (networkManager.StartServer())
            {
                Debug.Log("[NetworkManagerSetup] Server started successfully.");
            }
            else
            {
                Debug.LogError("[NetworkManagerSetup] Failed to start Server.");
            }
        }

        /// <summary>
        /// Gracefully shuts down the current network session regardless of role.
        /// </summary>
        public void StopNetwork()
        {
            if (!EnsureNetworkManager()) return;

            networkManager.Shutdown();
            Debug.Log("[NetworkManagerSetup] Network session stopped.");
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Guards public API methods; logs an error and returns false when the
        /// <see cref="NetworkManager"/> reference is missing.
        /// </summary>
        private bool EnsureNetworkManager()
        {
            if (networkManager != null) return true;

            Debug.LogError("[NetworkManagerSetup] NetworkManager is not assigned.");
            return false;
        }
    }
}
