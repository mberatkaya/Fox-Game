# Tilki Oyunu

## Concept

Tilki Oyunu is a short, warm, stylized 3D fox adventure intended as a small personal gift. The first playable target is an offline, single-player Windows PC experience lasting roughly 8-15 minutes.

Sprint 0 focuses on project foundation only. The existing `Assets/Scenes/Main.unity` prototype is preserved, but new production-facing work should grow under `Assets/_Game`.

## Target Platform

Windows PC.

## Unity Version

Unity `6000.0.23f1`.

## Render Pipeline

Universal Render Pipeline package `17.0.3` is installed. Run `Tools > Tilki Oyunu > Sprint 0 > Apply Project Foundation` once in Unity to create and assign the project URP asset under `Assets/_Game/Settings`.

## Project Structure

- `Assets/_Game/Art`: first-party visual assets, grouped by domain.
- `Assets/_Game/Audio`: music, SFX, and mixer assets.
- `Assets/_Game/Data`: editable ScriptableObject content for quests, dialogues, memories, and config.
- `Assets/_Game/Prefabs`: first-party prefabs.
- `Assets/_Game/Scenes`: Bootstrap, Gameplay, and Testing scenes.
- `Assets/_Game/Scripts`: foundation and future gameplay code.
- `Assets/_Game/Tests`: Unity Test Framework tests.
- `Assets/Scenes/Main.unity`: preserved prototype scene.
- `Assets/Scripts` and `Assets/Editor`: preserved prototype runtime/editor code.
- `ThirdParty`: imported external packages or assets only.
- `Documentation`: architecture and development notes.

## Scenes

- `Bootstrap`: initializes project services and content references.
- `Forest`: placeholder gameplay scene for future forest work.
- `SystemsTest`: lightweight systems validation scene.
- `Main`: legacy prototype scene, kept for reference and manual playtesting.

Scene names and paths are centralized in `TilkiOyunu.Foundation.SceneIds`.

## Controls

Planned input actions:

- Move
- Look
- Jump
- Sprint
- Interact
- Pause

The initial action asset is `Assets/_Game/Settings/TilkiInputActions.inputactions`. The existing prototype still uses the legacy input API, so project input handling is set to Both during the transition.

## Development

1. Open this folder in Unity `6000.0.23f1`.
2. Run `Tools > Tilki Oyunu > Sprint 0 > Apply Project Foundation`.
3. Use `Assets/_Game/Scenes/Bootstrap/Bootstrap.unity` as the startup scene for new work.
4. Keep feature development on branches. Do not merge directly into `main`.

## Build

The intended release target is Windows PC. After running the Sprint 0 setup command, build settings should include Bootstrap first, Forest second, SystemsTest disabled, and the legacy Main scene disabled.

## Asset Licensing

External assets must be tracked in `ASSET_NOTES.md` before they are used in a committed scene, prefab, material, or build.

## Git Workflow

Sprint branches should use the format `feature/<sprint-or-topic>`. Sprint 0 work is on:

`feature/sprint-0-project-foundation`

Open a pull request into `main`; do not merge the PR until it has been reviewed.
