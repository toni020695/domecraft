# DomeCraft

A Unity 3D multiplayer RPG powered by **Netcode for GameObjects (NGO)** and written in C#.

---

## Project Structure

```
domecraft/
├── Materials/          # Material assets
├── Models/             # 3D model assets
├── Prefabs/            # Reusable prefab assets
├── Scenes/             # Unity scene files
└── Scripts/
    ├── Core/
    │   ├── GameManager.cs          # Singleton – manages Lobby / Playing states
    │   └── NetworkManagerSetup.cs  # Bootstraps NGO host / client / server
    ├── Data/
    │   ├── CharacterClass.cs       # Warrior & Mage data stubs
    │   └── Race.cs                 # Human, Elf & Dwarf data stubs
    └── Player/
        ├── PlayerState.cs          # NetworkVariable health & mana
        └── ThirdPersonController.cs # Camera-relative movement + NetworkTransform
```

---

## Core Scripts

| Script | Namespace | Purpose |
|---|---|---|
| `NetworkManagerSetup` | `DomeCraft.Core` | Wraps `NetworkManager` – `StartHost()`, `StartClient()`, `StartServer()`, `StopNetwork()` |
| `GameManager` | `DomeCraft.Core` | Singleton that tracks `GameState` (`Lobby` / `Playing`) and fires `OnGameStateChanged` |
| `ThirdPersonController` | `DomeCraft.Player` | Owner-authoritative movement via `CharacterController`; position replicated via `NetworkTransform` |
| `PlayerState` | `DomeCraft.Player` | `NetworkVariable<float>` for `Health`, `MaxHealth`, `Mana`, `MaxMana`; server-side mutators via ServerRpc |
| `CharacterClassData` | `DomeCraft.Data` | Immutable value object for Warrior / Mage base stats; factory methods `CreateWarrior()` / `CreateMage()` |
| `RaceData` | `DomeCraft.Data` | Immutable value object for Human / Elf / Dwarf stat modifiers; factory methods per race |

---

## Getting Started

1. **Open the project** in Unity 2022 LTS or later.
2. Install **Netcode for GameObjects** via the Package Manager (`com.unity.netcode.gameobjects`).
3. Add a `NetworkManager` component to a scene GameObject and configure the **Unity Transport** layer.
4. Attach `NetworkManagerSetup` to the same GameObject.
5. Call `NetworkManagerSetup.StartHost()` (from a UI button or `Start()`) to begin a local host session.
6. Place the `ThirdPersonController` and `PlayerState` scripts on your player prefab (registered in the `NetworkManager`'s *Network Prefabs* list).

---

## Coding Standards

- All public types and members carry **XML doc-comments** (`/// <summary>…</summary>`).
- Namespaces follow the pattern `DomeCraft.<Layer>` (e.g. `DomeCraft.Core`, `DomeCraft.Player`).
- Classes are kept single-responsibility and modular so an AI agent can safely extend any one file without side effects.
- `NetworkVariable` writes are server-authoritative; clients observe changes via `OnValueChanged`.

