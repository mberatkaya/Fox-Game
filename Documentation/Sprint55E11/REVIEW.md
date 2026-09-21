# Sprint 5.5-E.11 runtime investigation

**NO-GO — RUNTIME INPUT/MAP/INTERACTION HOTFIX FAILED** (the complete acceptance gate remains unmet).

**NEEDS USER PHYSICAL INPUT REVIEW.** Proven minimap and close-range interaction defects have fixes, but the reported physical M failure from Bootstrap has not been explained or verified fixed. Do not merge PR #7, continue the playthrough, polish quests, or start Sprint 6.

## Findings, separated by symptom

1. **M does not open the map: unresolved for the user's Bootstrap entry.** Before changing production code, `BootstrapMapAndInteractionBindings` opened/closed with queued M, closed with Escape, restored movement/look locks, and reached the NPC. Its only failure was the prompt's binding label (`BaselineBindings.xml`). A later native Windows review had services present, every action enabled, no modal lock, an enabled map component and an enabled private map action with one control (`NativeInput.txt`). Computer Use M produced no performed event; Computer Use W did not establish reliable movement either. These observations do not contradict the user's physical test and do not establish a Game View focus root cause. No arbitrary keyboard polling, competing listener, speculative input fallback, or false physical PASS has been added.
2. **Stationary minimap icon: reproduced and fixed.** `QuestMapUI.Update` recomputed the minimap viewport center from the player's X/Z every frame. Within the interior of the world bounds, the live player's UV therefore remained `(0.5, 0.5)` even after a 10 m translation. The live `FoxController` reference was correct; the camera-following map coordinates canceled the player's motion. `Baseline.xml` records the zero displacement failure. The viewport now stays stable within a 30%-of-span travel margin and scrolls at that margin. The player's position and yaw update in `LateUpdate` after movement. X maps to UI X and Z to UI Y; the north-up map itself never rotates. Existing nearby objective visibility, nearest distant edge marker, quest IDs and highlighting remain unchanged.
3. **NPC interaction unavailable at close range: reproduced and fixed.** `PlayerInteractor.FindFocusedInteractable` measured a 3D angle from the fox's low interaction origin to the tall production NPC collider center. The 95-degree cone accepted a 2 m approach but rejected 1.3 m with `Focused == null` (`CloseRangeBaseline.xml`). Facing is now tested in the horizontal plane. The overlap still uses the authored 2.1 m sphere, all target layers and `QueryTriggerInteraction.Collide`; parent `IInteractable` resolution still selects `NPC_Guide`. Its trigger capsule (radius approximately 0.85 m) and separate physical blocker (approximately 0.8 m) were already present and are unchanged. `GuideNpc` and the Quaternius visual root were not rewritten.
4. **Misleading interaction prompt: reproduced and fixed.** The canonical `.inputactions` asset declared `KeyboardMouse`/`Gamepad` control schemes but assigned no binding groups. `BuildBindingLabel` filtered for `KeyboardMouse`, found no display binding and fell back to `Interact`. Keyboard/mouse and gamepad bindings now carry their corresponding groups. The production prompt displays **F - Konuş**; F reaches the existing interactor and opens the existing dialogue panel. This display defect is not claimed as the cause of the physical M failure.
5. **Additional direct-Forest startup defect: reproduced and fixed, not the user's reported entry.** The Forest scene had no service bootstrap. Move/Look worked independently, while `QuestMapUI` returned before both input and marker processing and `GuideNpc.CanInteract` returned false when `GameServices.HasCurrent` was false. A standalone `GameBootstrap` root now references the same content/mixer as Bootstrap. Its early execution order initializes services before scene subscribers. The existing duplicate-owner guard leaves exactly one persistent service owner when entering through Bootstrap. The user explicitly confirmed their test started from Bootstrap, so this finding is not substituted for finding 1.

## Production action audit

Canonical asset: `Assets/_Game/Settings/TilkiInputActions.inputactions`, GUID `a816b161fcee99e4f8bf9d63bcec50f0`. It is the only `.inputactions` file under Assets. No generated C# wrapper and no production `PlayerInput` component are used. There is one map, `Player`; no second disabled action map/default-map mismatch. Forest's camera and the PlayerFox prefab/runtime controller and interactor reference this asset. Runtime tests check shared object identity; EditMode tests check serialized asset paths across scene loads. The Forest EventSystem uses a legacy StandaloneInputModule, not an InputSystemUIInputModule owning these actions.

| Action | Map | Keyboard/mouse; gamepad | Runtime consumer | Runtime result |
|---|---|---|---|---|
| Move | Player | WASD; left stick | FoxController.Update | Enabled; existing movement/lock regression passes; user previously confirmed physical movement |
| Look | Player | mouse delta; right stick | ThirdPersonCameraController.LateUpdate | Enabled; existing camera/lock regression passes; user previously confirmed physical look |
| Jump | Player | Space; south button | FoxController.Update | Enabled; existing anti-flight regression retained |
| Sprint | Player | Left Shift; left-stick press | FoxController.Update | Enabled; unchanged consumer |
| Interact | Player | F; west button | PlayerInteractor.Update | Enabled; queued F opens production dialogue at 2 m and close approach after correction |
| WorldMap | Player | M | QuestMapUI owns a clone of the authored action, polls it in LateUpdate | Enabled; queued M opens/closes; physical Bootstrap failure remains unverified |
| Pause | Player | Escape; Start | QuestMapUI private close action; CardMatchingPanelUI cancel consumer | Enabled; queued Escape closes map and restores locks |

Map action clones are intentional independent lifetime ownership, not a stale asset or generated wrapper. `Start` resolves/enables them and builds a hidden overlay. `OnDisable` closes/releases the modal and disables its actions; `OnEnable` re-enables them; `OnDestroy` disposes actions/generated images. No callback subscription was lost because these consumers poll actions rather than subscribing. Normal startup has lock=false, lookLock=false. Map opening acquires the gameplay lock and look lock without disabling its close actions; closing/disable restores prior cursor/look state and releases its lock. Existing Light Path pause/resume regression remains required. The map's input lifecycle has not been speculatively rewritten to claim a fix for finding 1.

## Modified implementation and regression files

- `QuestMapUI.cs`: stable local viewport center and late-frame coordinate/facing update. The player reference is resolved once from the live FoxController at initialization; no per-frame player search.
- `PlayerInteractor.cs`: horizontal facing check only. Binding, overlap radius, collider ownership and interaction invocation remain production paths.
- `TilkiInputActions.inputactions`: missing binding groups, retaining all action/binding IDs and keys.
- `GameBootstrap.cs` and the additive Forest service root: direct-Forest initialization before subscribers; same content configuration, no save migration.
- `Sprint55E11RuntimeTests.cs`: production Bootstrap and Forest entry, action identity/enabled state, four-axis tracking, yaw, hidden/active overlay, M/M/Escape, disable/re-enable, lock restoration, 2 m/1.3 m NPC focus, F prompt and dialogue. Uses Input System state events and the production consumers; only test positioning teleports the real fox.
- `Sprint55E11InputWiringTests.cs`: serialized production scene/prefab references, bindings and service content configuration.
- `Sprint55E11InputReview.cs`: opt-in editor-only observation of performed actions and consumer/lock state. No normal-play logging, input injection or gameplay mutation. The review temporarily protects the original save and restores its bytes on Play Mode exit.

## Evidence and physical review

The five PNGs in this directory are **automated production-scene evidence**, not physical input QA. The first two show different live player marker positions (spawn and 10 m east). The other three show an open world map, `F - Konuş` and NPC dialogue. They were captured during the rendered PlayMode test run and visually inspected.

For the unresolved physical path, open **Tilki Oyunu → Sprint 5.5-E.11 → Review physical input (preserves save)**. Test WASD, sprint, jump, mouse, M/open, M/close, M/open then Escape/close, approach Guide and F. The resulting `NativeInput.txt` distinguishes action delivery, map state, modal ownership and NPC focus. Exit Play Mode normally to restore the original save. An interrupted review leaves a `.sprint55e11-backup`; the next review refuses to overwrite it.

Automated tests cannot establish a physical PASS. The user must verify all physical keys, visible tracking, movement/look restoration, prompt and dialogue before the gate can change. The exact cause of the original Bootstrap/M failure must still be established if it reproduces.

## Validation and repository state

- Full EditMode: **80/80 passed**, including the existing objective-marker progression tests.
- Full rendered PlayMode: **24/24 passed**, including the three new production runtime scenarios, the existing map lock/timer tests, movement/camera restoration and anti-flight/collision tests.
- SprintValidation: **PASS**, `Sprint5Validation.ValidateFromCommandLine` (the project's existing E.1 validation entry point).
- Physical QA: **not passed**. See the unresolved M finding and opt-in review procedure above.

Baseline failures are retained in `Baseline.xml`, `BaselineBindings.xml` and `CloseRangeBaseline.xml`; final full-suite results are `EditMode.xml` and `PlayMode.xml`. `Focused.xml` was the intermediate pass before the close-range assertion was added; it is not the final acceptance evidence.

Work began on `feature/sprint-5-5-world-character-overhaul` at `d10c8d3f95167bee8a7efa5e806a0a11a1141000`. The ten pre-existing tracked modifications and pre-existing untracked assets were kept out of the hotfix staging. Only the additive 49-line service root change was staged from the already-dirty Forest scene; its prior scene edits remain unstaged. `PreflightDirty.txt` records the initial state; `RemainingDirty.txt` lists changes left outside the hotfix. No reset, clean, force checkout/push, PR merge or save migration was performed.
