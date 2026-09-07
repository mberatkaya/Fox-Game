# Tilki Oyunu Architecture

## Scene Flow

`GameBootstrap` lives in the Bootstrap scene and initializes services before loading `Forest`. The Forest scene owns the current playable chain: guide NPC, memory collectibles, Light Path, Card Matching, and Final Camp. No separate final scene is required.

## Services

`GameServices` is the small service locator for the current run. It holds:

- `SaveService`
- `SceneService`
- `AudioService`
- `QuestService`

`GameBootstrap` creates these once and persists across scene loads.

## Save

`SaveGameData` is versioned JSON. `SaveService` reads/writes `tilki-oyunu-save.json`, normalizes missing fields, and falls back to a fresh save for missing/corrupt data. Runtime state includes quest progress, collected memory ids, Light Path completion, Card Matching completion, final unlock/completion, and player position support.

## Quests

`QuestService` keeps static `QuestDefinition` data separate from saved `QuestProgress`. The final camp unlock remains data-driven by the three required quest ids:

- `collect_memories`
- `light_path`
- `card_matching`

Sprint 5 does not add new quests or minigames.

## Interaction

Player interactions use `IInteractable` and `InteractionContext`. NPCs, memories, minigame starts, and the final camp all share this simple contract. `PlayerInteractor` displays the current `InteractionLabel` through `InteractionPromptUI`.

## Character

`PlayerFox.prefab` keeps movement/collision on the root:

- `CharacterController`
- `FoxController`
- `PlayerInteractor`
- `Interaction Origin`
- `Camera Target`
- `VisualRoot/QuaterniusFox/FoxModel`

`FoxController` owns movement. `FoxAnimationDriver` only reads controller state and writes Animator parameters (`Speed`, `Grounded`, `VerticalVelocity`) with root motion disabled. `FootstepAudio` uses grounded speed cadence and does not affect movement.

## Camera

`ThirdPersonCameraController` follows the player camera target with orbit input and obstruction checks. `FinalSequenceController` briefly frames the camp/fox during the final sequence without introducing Cinemachine or a new camera framework.

## Dialogue And Narrative

Dialogue, memory text, quest copy, and the final message live in ScriptableObjects under `Assets/_Game/Data`. Sprint 5 updates Turkish copy to be warmer and simpler without adding private/personal details.

## Minigames

`LightPathController` and `LightPathNode` preserve Sprint 3 logic. Sprint 5 adds visual color polish and `LightPathAudioFeedback`.

`CardMatchingController` preserves Sprint 4 matching logic. Sprint 5 adds warmer card colors, input-action cancel support in `CardMatchingPanelUI`, and `CardMatchingAudioFeedback`.

## Final

`FinalCampController` still reflects locked/unlocked quest state. When unlocked, it delegates to `FinalSequenceController`, which handles:

- input lock
- short camera framing
- warm camp light transition
- final message UI
- completion chime
- `saveData.finalCompleted = true`
- `GameState.Completed`

## Audio

`AudioService` remains null-safe for global volume parameters. Scene audio is handled by scoped `AudioSource` components:

- `SceneLoopAudio` for music and forest ambience
- `FootstepAudio` for fox leaf footsteps
- `MemoryAudioFeedback`
- `LightPathAudioFeedback`
- `CardMatchingAudioFeedback`
- campfire loop on the final camp

The intended mixer layout is `Master -> Music / Ambience / SFX`; existing services tolerate a missing or partial mixer.

## UI

UI remains TextMeshPro/uGUI. Sprint 5 adds a small `GameUITheme` ScriptableObject for forest/wood/fox/cream/gold colors and applies it to:

- Quest HUD
- dialogue panel
- memory feedback card
- Card Matching panel/cards
- final message panel

## Editor Tools

Sprint builders are manual tools only. `Sprint5PolishBuilder` is idempotent for the current Sprint 5 layer and should not run automatically on import or editor load. `Sprint5Validation` checks the production asset subset, player visual/animator, scene polish hooks, final message, and missing scripts.
