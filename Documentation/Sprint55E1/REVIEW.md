# Sprint 5.5-E.1 quality review

**NO-GO — SPRINT 5.5-E.1 QUALITY GATE NOT PASSED**

The implementation and automated checks are delivered. The required normal keyboard/mouse fresh-save-to-final playthrough is not complete, and screenshots 03–09 are missing. This is not a gameplay approval.

Branch: `feature/sprint-5-5-world-character-overhaul`.
Preflight HEAD: `366ec4de097e9e411e43683c3534e97f1d2f618b`.
Implementation commit: `0c8fa0b7d2b13721cb487bf4246acc5d5162cf75` — `feat(quest): add quest-aware maps and readable waffle objectives`.
The final evidence commit hash is supplied in the task response. PR #7 has not been merged or pushed, and Sprint 6 has not started.

## Implementation

See [IMPLEMENTATION.md](IMPLEMENTATION.md) for the navigation approach, input ownership/restoration, marker-state table, persistent-ID/item/visual/icon table, final Turkish dialogue, 40 → 75 second timer change, explicit F start, node guidance, card labels and save reconstruction.

The minimap is available from the first frame of gameplay, shows a 150 m neighborhood, nearby active targets and the nearest distant target at the edge, with approximate distance. M opens a stable full-world overlay. The existing input action asset supplies WorldMap and Pause. Input ownership tests cover M/M/Escape, blocked movement and camera look, restored movement and camera delta, prior cursor state, disable cleanup and another modal already holding the lock. Static terrain/water rendering adds no map camera or per-frame scene-render pass. Full-map future light nodes are small and faint; only the next required node has a prominent label.

## Validation

| Check | Result | Evidence |
|---|---|---|
| EditMode | PASS — 79/79 | [EditMode.xml](EditMode.xml) |
| Full PlayMode regression suite | PASS — 21/21 | [PlayMode.xml](PlayMode.xml) |
| Focused map tests after final navigation changes | PASS — 2/2 | [Focused.xml](Focused.xml) |
| SprintValidation | PASS | [Validation.txt](Validation.txt) |
| Original QuestService, SaveService, SaveGameData, GameServices source | Unchanged | Git diff against preflight HEAD |
| Pre-existing Forest edits | Preserved | [PreflightSceneCheck.txt](PreflightSceneCheck.txt) |

Tests exercised duplicate pickup IDs, persistence/reload, state-dependent markers, NPC turn-in, final prerequisites, deliberate light start, map timer pause, wrong-node reset, real timeout/retry/completion, card rules and saved final completion. Existing movement, animation, jumping, collision, presentation and save tests passed on the current working tree, including its pre-existing user edits. Those automated tests do not prove normal-player discoverability, travel comfort or visual/audio quality through a full run.

An initial smoke assertion expecting a hidden fresh-save QuestHUD was updated to require visible initial guidance. Native editor focus filtering required the synthetic input tests to configure and restore their input-test environment. A minimap header alignment issue, a terrain splat-layer assumption and water-bound clipping were found during image inspection and corrected. The current screenshot outputs were visually inspected.

## Normal-input attempt and actual measurements

The production Bootstrap/Forest scene was opened with a fresh save using the read-only review observer. The Guide, fox, first-destination HUD and bottom-left minimap were observed. Computer Use keyboard calls reached editor shortcuts (Ctrl+P stopped Play Mode), but attempts to open the map with native M did not produce the map. Unity Game View keyboard focus was inspected. Synthetic Input System M events pass the PlayMode tests. The cause of the native-input discrepancy is not established; it must be resolved or checked with physical input before acceptance.

The attempt never reached the first NPC conversation. No teleport or direct component interaction was used to pretend it was a normal-input run. No travel time has been extrapolated from automated completion. The review observer restored the user's original save on leaving Play Mode; no interrupted review backup remains.

| Requested actual measurement | Result |
|---|---|
| Spawn → NPC | Not measured — NPC not reached |
| Quest 1 | Not measured |
| Quest 2 | Not measured |
| Quest 3 | Not measured |
| Final approach | Not measured |
| Total fresh-save → finalCompleted | Not completed; no valid duration |

[NormalInputObservations.txt](NormalInputObservations.txt) contains only the recorded Spawn event. These missing values are not zero-second times. The 8–15 minute target and the comfort of 75 seconds remain unverified.

## Acceptance gates

FAIL below means the required evidence is missing or the gate was not verified; it is not a fabricated finding that the feature necessarily fails for a physical player.

| Gate | Decision | Basis |
|---|---|---|
| Minimap | PASS — technical/initial visual | Bottom-left, readable player/Guide, north-up local map and distance; automated state tests |
| World Map | FAIL — normal-input gate unverified | Full overlay and M/Escape pass synthetic tests and image QA; native M attempt unresolved |
| Quest 1 discoverability | FAIL — unverified | Names/models/icons implemented; no normal collection/return playthrough |
| Quest 2 clarity | FAIL — unverified | Instructions/next-node guidance implemented; no normal route playthrough |
| Quest 2 timing | FAIL — unverified | 75 seconds and retry pass tests; normal-player comfort not measured |
| Quest 3 discoverability | FAIL — unverified | Correct marker state tested; no normal approach/card playthrough |
| Final navigation | FAIL — unverified | Unlock/reload visibility tested; no normal final approach |

## Screenshots

- [01_minimap.png](Screenshots/01_minimap.png): current initial map/HUD, from graphical Input System test capture.
- [02_world_map.png](Screenshots/02_world_map.png): current full map, from graphical Input System test capture.
- Missing: `03_waffle_ingredient.png`, `04_waffle_map_markers.png`, `05_light_path_map.png`, `06_light_path_hud.png`, `07_card_quest_marker.png`, `08_final_camp_marker.png`, `09_npc_turnin.png`.

The two screenshots are technical visual evidence, not proof of a normal completed playthrough. No fake substitutes were generated for the missing seven.

## Remaining working-tree changes

Ten tracked paths were already dirty before this task: the TMP fallback font, NPC animation/material/prefab, three existing world/NPC builders, Forest, the production EditMode tests and production smoke tests. Only the E.1 scalar scene edits and updated HUD smoke assertion were staged from the two overlapping files. Their unrelated changes remain dirty. Existing untracked TreePack assets, previous sprint documents and root logs were preserved.

See [PreflightDirty.txt](PreflightDirty.txt) and [RemainingDirty.txt](RemainingDirty.txt) for exact paths. Validation ran against the current working tree; it does not certify a clean checkout without the user's existing dirty work.

## Required continuation

Run **Tilki Oyunu → Sprint 5.5-E.1 → Play production review** with physical keyboard/mouse, verify M/M/Escape and movement/look restoration, then complete all three quests and the finale without teleports or direct helpers. The observer records actual transition times and captures the missing named views when those states are visited (open the map at each stage/turn-in). Review ingredient silhouettes, ordered light readability, card labels, audio/water/atmosphere and the real 75-second comfort before changing this decision.

**NO-GO — SPRINT 5.5-E.1 QUALITY GATE NOT PASSED**
