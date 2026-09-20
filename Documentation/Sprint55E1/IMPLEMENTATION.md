# Sprint 5.5-E.1 — Quest UX & Narrative Overhaul

Branch: `feature/sprint-5-5-world-character-overhaul`.
Preflight HEAD: `366ec4de097e9e411e43683c3534e97f1d2f618b`.
PR #7 is not merged. Sprint 6 is not started.

## Navigation implementation

`ForestGameplayBootstrap` creates one scene-local `QuestMapUI`. A 256×256 texture is generated once from the production terrain's splat weights and height. The path layer is identified by its authored name rather than a hard-coded splat index. Actual lake/creek mesh triangles are projected onto it. The north-up extent fits all current objective positions, LM_ production anchors and water bounds with 24 m padding, rather than the unused 512×512 terrain buffer. There is no additional camera, RenderTexture, shadow pass, post-processing pass, package, or persistent navigation store.

A cream framed 254×254 minimap is anchored bottom-left on a 1920×1080 reference canvas. It crops a north-up 150 m neighborhood from the shared texture, follows the player within the mapped core, and shows nearby relevant targets plus the nearest off-screen target clamped to the edge. A compact label shows that target and whole-meter distance. M opens a 720×720 north-up overlay with muted landmark names and a compact icon legend. Player position and heading update from the actual fox transform. World-to-map conversion includes the frame inset. Main landmarks include Başlangıç, Rehber Korusu, Göl, Işık Korusu, Kalp Bahçesi, Final Tepesi and Köprü. Future landmarks are subdued base-map labels, not active objective markers.

`MapMarker` is reusable scene-target metadata (type, Turkish label, persistent ID, ordered node index). Its visibility is evaluated against the existing QuestService and, for an in-progress light run, LightPathController. No quest or save fields were renamed and no migration was introduced.

| Quest state | Visible active markers |
|---|---|
| Fresh save / next quest not accepted | Player + relevant Guide |
| collect_memories Active | Player + each uncollected ingredient |
| Any quest ReadyToTurnIn | Player + prominent Guide return marker |
| collect_memories Completed, light_path not accepted | Player + Guide |
| light_path Active, not running | Player + Işık Korusu start |
| Light run in progress | Player + dominant next required node; future small nodes faint on full map only |
| light_path Completed, card_matching not accepted | Player + Guide |
| card_matching Active | Player + Kalp Bahçesi activity |
| All three quests Completed | Player + Final Kampı |
| finalCompleted true | Player; old active quest/final markers absent |

The map is available immediately on a fresh save; it does not require a quest unlock. Final Camp's active marker requires all three existing quest IDs to be Completed, rather than just ReadyToTurnIn.

## Input ownership

Added `Player/WorldMap` bound to `<Keyboard>/m` in the existing Input System asset. No M conflict existed. The map clones its toggle and existing Pause action (Escape/gamepad Start) so disabling the map never disables the shared gameplay action map. Opening acquires GameplayInputLock (movement and interaction), remembers camera look-lock and cursor state, then locks look and frees the cursor. M or Escape restores those states. Disable/destruction also releases owned state. Map opening is refused while another modal owns the gameplay lock or the finale owns the camera. The current timed run is paused while the map is open; closing or disabling the map resumes it. No global timeScale changes are used.

## Ingredient presentation

| Persistent ID | Turkish item | World visual | Map icon |
|---|---|---|---|
| memory_01 | Un | Cream flour sack, folded top and label | Sack silhouette |
| memory_02 | Süt | White milk bottle, red cap | Bottle silhouette |
| memory_03 | Yumurta | Ivory egg in a small cup | Oval egg |
| memory_04 | Tereyağı | Golden butter block on wrapper | Butter rectangle |
| memory_05 | Çilek | Red berry, green leaves, pale seeds | Berry silhouette |

The project asset search found no suitable ingredient assets. Project-owned Unity primitives and URP materials provide five recognizable, roughly 0.7–1.2 m silhouettes, replacing the old tiny token renderers at runtime while retaining serialized IDs and interaction colliders. Existing float/rotation and pickup audio are reused. A small emissive floating beacon is active only for relevant, uncollected ingredients. Models/materials are cleaned up with their owner.

Quest title: **Bir Waffle Anısı**. HUD: **Waffle Malzemeleri: 3/5**. Pickup feedback uses **Un bulundu.**, **Süt bulundu.**, **Yumurta bulundu.**, **Tereyağı bulundu.**, **Çilek bulundu.**

Intro:
> Burada bir anının kokusu kalmış... Un, süt, yumurta, tereyağı ve çilek.
>
> Beş malzemeyi bulursan belki birlikte hatırlarız. Haritana işaretledim; M ile açabilirsin.

Completion:
> Şimdi hatırladım... Bu waffle, birlikte yediğimiz en güzel şeydi.
>
> O günün sıcaklığı hâlâ burada.

## Light Path and other UI

Previous production timer: 40 s. New scene/default/builder timer: 75 s. Accepting the quest does not start the timer. The existing explicit F interaction at Işık Korusu starts it; first-node auto-start was deliberately not introduced, because the existing start/retry architecture already supports a clear, deliberate start.

Intro:
> Işık Korusu’nu haritana işaretledim. Işıkları sırayla takip et; ulaştığın her ışık bir sonrakini belirginleştirecek.
>
> Acele etme. Hazır olduğunda başlangıçta F’ye bas. O zaman 75 saniyen başlayacak.

Available node scale increases from 1.18 to 2; completed light intensity decreases from 0.75 to 0.25. Emission is explicitly enabled on owned node material instances. Future nodes remain dim. Node triggers now require a fox collider, so incidental non-player trigger overlaps cannot break a run. Wrong order still resets the existing run. Timeout and wrong-order text point back to the start and F retry. Feedback lasts seven seconds. The route and player speed are unchanged.

Existing QuestHUD now displays the initial Guide destination, active ingredient progress, light start/running instructions, Kalp Bahçesi, NPC return, and final destination. It polls at 5 Hz as a fallback for transient run/reset/final state. Existing event updates remain. The third quest title is Kalpleri Eşleştir. Card labels use a single line with 14–30 pt autosizing; matching rules are unchanged.

## Save reconstruction

On scene load MapMarker registration discovers the actual five MemoryCollectible objects and their original IDs. Every visibility decision reads QuestService.GetQuestStatus / HasCollectedMemory / IsFinalCampUnlocked / SaveData.finalCompleted. Collected ingredient renderers also reconstruct from existing saved IDs. A partial light run remains transient and restarts from the existing start after reload. Final completion retains its existing save semantics.

## Normal-input review tool

Menu: **Tilki Oyunu → Sprint 5.5-E.1 → Play production review**. It preserves the current save in a `.sprint55e1-backup`, starts Bootstrap with a fresh save, observes quest transitions and screenshots, then restores original save bytes on leaving Play Mode. An interrupted backup blocks another review until restored. The observer never teleports, injects movement, invokes interactions or completes objectives. `NormalInputObservations.txt` contains elapsed wall-clock observations, not inferred travel estimates. `NormalInputReviewSave.json` preserves the review state separately.

Validation results, actual normal-input measurements, screenshot availability, final commit and quality decision are recorded in REVIEW.md.
