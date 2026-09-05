# Tilki Oyunu Architecture

## Bootstrap

`GameBootstrap` is the composition root for Sprint 0. It initializes `GameServices` with the save, scene, and audio services, then persists across scene loads. This avoids a web of unrelated singletons while staying native to Unity.

## Save

`SaveGameData` is a versioned JSON data model. `SaveService` reads and writes `tilki-oyunu-save.json` under `Application.persistentDataPath` by default and supports:

- `Save`
- `Load`
- `HasSave`
- `DeleteSave`

Missing or corrupt save files return a fresh save instead of crashing.

## Game State

Sprint 0 uses a deliberately small `GameState` enum:

- `NotStarted`
- `Playing`
- `FinalAvailable`
- `Completed`

Quest-specific progress lives in `QuestProgress`, not in the static quest definitions.

## Data Definitions

Static content is represented as ScriptableObjects:

- `QuestDefinition`
- `DialogueDefinition`
- `MemoryDefinition`
- `FinalMessageDefinition`
- `GameContentConfig`

Runtime save data stays separate from these definitions.

## Scene Flow

Scene names and paths are centralized in `SceneIds`.

Planned flow:

1. `Bootstrap`
2. `Forest`
3. Future final state remains in the gameplay scene unless a later sprint needs a separate scene.

`SystemsTest` exists for lightweight validation and should not ship as an enabled build scene.

## Audio

`AudioService` is null-safe and accepts an optional `AudioMixer`. The intended mixer structure is:

- Master
- Music
- SFX

The service reserves mixer parameter names `MasterVolume`, `MusicVolume`, and `SFXVolume`.

## Input

`TilkiInputActions.inputactions` defines the planned player action map:

- Move
- Look
- Jump
- Sprint
- Interact
- Pause

The prototype still uses legacy input calls, so input handling remains set to Both until the controller is migrated.

## Interaction

The foundation interaction contract is `IInteractable` plus `InteractionContext`. This is intentionally small so NPCs, memory objects, light nodes, and final camp interactions can share one interaction surface later.

## Prototype Boundary

The existing `TilkiMacera` prototype is preserved for reference and playtesting. New systems should be implemented under `TilkiOyunu.Foundation` first, then old prototype pieces can be migrated in small, testable steps.
