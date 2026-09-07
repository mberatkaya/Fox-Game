# Tilki Oyunu

Tilki Oyunu is a short, warm, stylized 3D fox adventure built as a small personal gift experience. The current flow is an offline single-player Windows PC game:

`Bootstrap -> Forest -> NPC -> Memory Quest -> Light Path -> Card Matching / Kalp Bahcesi -> Final Camp`

## Tech

- Unity `6000.0.23f1`
- Universal Render Pipeline `17.0.3`
- Input System package with keyboard/mouse and gamepad bindings
- Target platform: Windows PC

## Controls

- Move: `WASD` / left stick
- Look: mouse / right stick
- Jump: `Space` / gamepad south
- Sprint: `Left Shift` / left stick press
- Interact: `F` / gamepad west
- Cancel/Pause: `Escape` / gamepad start

## Gameplay Loop

The player controls the fox in the Forest scene. A guide NPC introduces the quest chain, then the player completes three small activities:

1. Collect five memories.
2. Follow the Light Path nodes in order.
3. Match all card pairs in Kalp Bahcesi.

Completing all three quests unlocks the Final Camp. The final interaction opens a small message sequence with warm camera/light/audio presentation and persists `finalCompleted` in the save data.

## Key Content

- `Assets/_Game/Prefabs/Characters/PlayerFox.prefab` contains the gameplay root, `CharacterController`, interaction origin, camera target, and the Quaternius fox visual under `VisualRoot`.
- `Assets/_Game/Data` contains editable quest, memory, dialogue, config, and final message ScriptableObjects.
- `Assets/_Game/Scenes/Gameplay/Forest.unity` contains the current playable forest, quest/minigame objects, production visual layer, scene audio, UI, and final camp.
- `Assets/ThirdParty` contains the narrow imported subset of approved CC0 assets used by Sprint 5.

## Save/Load

`SaveService` writes `tilki-oyunu-save.json` under `Application.persistentDataPath`. The save tracks quest progress, collected memories, Light Path/Card Matching completion, final unlock, final completion, and player position support.

## Assets

Third-party assets are CC0 and documented in `ASSET_NOTES.md`:

- Quaternius Ultimate Animated Animal Pack: fox FBX
- Quaternius Ultimate Stylized Nature Pack: selected trees, rocks, bushes, grass, flowers, textures
- Kenney UI Pack: selected UI sprites
- Kenney Interface Sounds: selected UI sounds
- OpenGameArt: Sunset Walk music, Forest Ambience, leaf footsteps, chimes, playing-card sounds, fireplace loop

## Run

1. Open the repository in Unity `6000.0.23f1`.
2. Open `Assets/_Game/Scenes/Bootstrap/Bootstrap.unity`.
3. Press Play. Bootstrap loads the Forest scene automatically.

## Build

Use Windows PC as the target. Build settings should include:

- `Assets/_Game/Scenes/Bootstrap/Bootstrap.unity`
- `Assets/_Game/Scenes/Gameplay/Forest.unity`

`SystemsTest` and legacy prototype scenes are for development/reference and should not ship in the release build.

## Development Workflow

- Work on feature branches, not directly on `main`.
- Use the menu item `Tilki Oyunu/Sprint 5/Apply Production Polish` only when intentionally refreshing Sprint 5 scene/prefab wiring.
- Run `Tilki Oyunu/Sprint 5/Validate` before release-oriented changes.
- Do not commit raw downloaded archives or unused full asset packs.
