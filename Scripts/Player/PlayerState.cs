using Unity.Netcode;
using UnityEngine;

namespace DomeCraft.Player
{
    /// <summary>
    /// Tracks the networked stats of a single player character.
    ///
    /// All fields use <see cref="NetworkVariable{T}"/> so their values are
    /// automatically replicated from the server to all connected clients.
    /// Only the server (or host) should write to these variables; clients
    /// observe changes via <see cref="NetworkVariable{T}.OnValueChanged"/>.
    ///
    /// Extend this class to add additional stats such as Stamina, Experience,
    /// or Gold as the project grows.
    /// </summary>
    public class PlayerState : NetworkBehaviour
    {
        // ─── Constants ───────────────────────────────────────────────────────────

        /// <summary>Default maximum health points for a new character.</summary>
        public const float DefaultMaxHealth = 100f;

        /// <summary>Default maximum mana points for a new character.</summary>
        public const float DefaultMaxMana = 50f;

        // ─── Networked Variables ─────────────────────────────────────────────────

        /// <summary>
        /// Current health of the player.
        /// Range: [0, <see cref="MaxHealth"/>].
        /// Managed exclusively by the server.
        /// </summary>
        public NetworkVariable<float> Health { get; } =
            new NetworkVariable<float>(
                DefaultMaxHealth,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );

        /// <summary>
        /// Maximum health of the player.
        /// Set once during initialisation; may be modified by equipment or buffs.
        /// </summary>
        public NetworkVariable<float> MaxHealth { get; } =
            new NetworkVariable<float>(
                DefaultMaxHealth,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );

        /// <summary>
        /// Current mana of the player.
        /// Range: [0, <see cref="MaxMana"/>].
        /// Managed exclusively by the server.
        /// </summary>
        public NetworkVariable<float> Mana { get; } =
            new NetworkVariable<float>(
                DefaultMaxMana,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );

        /// <summary>
        /// Maximum mana of the player.
        /// Set once during initialisation; may be modified by equipment or buffs.
        /// </summary>
        public NetworkVariable<float> MaxMana { get; } =
            new NetworkVariable<float>(
                DefaultMaxMana,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
            );

        // ─── Unity / NGO Lifecycle ───────────────────────────────────────────────

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // Subscribe to value-change callbacks so UI or effects can react.
            Health.OnValueChanged    += OnHealthChanged;
            Mana.OnValueChanged      += OnManaChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            Health.OnValueChanged    -= OnHealthChanged;
            Mana.OnValueChanged      -= OnManaChanged;
        }

        // ─── Server-side Stat Mutators ───────────────────────────────────────────

        /// <summary>
        /// Applies <paramref name="amount"/> points of damage to the player.
        /// Health is clamped to zero; further logic (death, respawn) should be
        /// added here or handled by subscribing to <see cref="Health.OnValueChanged"/>.
        /// </summary>
        /// <param name="amount">Positive damage value.</param>
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageServerRpc(float amount)
        {
            float newHealth = Mathf.Clamp(Health.Value - amount, 0f, MaxHealth.Value);
            Health.Value = newHealth;

            if (newHealth <= 0f)
            {
                Debug.Log($"[PlayerState] Player {OwnerClientId} has died.");
                // TODO: Trigger death / respawn flow.
            }
        }

        /// <summary>
        /// Restores <paramref name="amount"/> health points up to <see cref="MaxHealth"/>.
        /// </summary>
        /// <param name="amount">Positive heal value.</param>
        [ServerRpc(RequireOwnership = false)]
        public void HealServerRpc(float amount)
        {
            Health.Value = Mathf.Clamp(Health.Value + amount, 0f, MaxHealth.Value);
        }

        /// <summary>
        /// Consumes <paramref name="amount"/> mana points.
        /// Returns <c>false</c> when the player does not have enough mana.
        /// </summary>
        /// <param name="amount">Positive mana cost.</param>
        /// <returns>
        /// <c>true</c> if the cost was successfully deducted; <c>false</c> otherwise.
        /// </returns>
        public bool ConsumeMana(float amount)
        {
            if (!IsServer) return false;

            if (Mana.Value < amount)
            {
                Debug.Log($"[PlayerState] Player {OwnerClientId} has insufficient mana.");
                return false;
            }

            Mana.Value = Mathf.Clamp(Mana.Value - amount, 0f, MaxMana.Value);
            return true;
        }

        /// <summary>
        /// Restores <paramref name="amount"/> mana points up to <see cref="MaxMana"/>.
        /// </summary>
        /// <param name="amount">Positive mana restore value.</param>
        public void RestoreMana(float amount)
        {
            if (!IsServer) return;
            Mana.Value = Mathf.Clamp(Mana.Value + amount, 0f, MaxMana.Value);
        }

        // ─── Value Changed Callbacks ─────────────────────────────────────────────

        /// <summary>
        /// Invoked on all clients whenever <see cref="Health"/> changes.
        /// Override or extend to drive UI health bars, visual effects, etc.
        /// </summary>
        private void OnHealthChanged(float previousValue, float newValue)
        {
            Debug.Log($"[PlayerState] Player {OwnerClientId} health: {previousValue} → {newValue}");
        }

        /// <summary>
        /// Invoked on all clients whenever <see cref="Mana"/> changes.
        /// Override or extend to drive UI mana bars, visual effects, etc.
        /// </summary>
        private void OnManaChanged(float previousValue, float newValue)
        {
            Debug.Log($"[PlayerState] Player {OwnerClientId} mana: {previousValue} → {newValue}");
        }
    }
}
