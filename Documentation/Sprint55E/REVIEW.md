# Sprint 5.5-E — gameplay integration review

## Gate

**NO-GO — SPRINT 5.5-E QUALITY GATE NOT PASSED**

Automated technical checks pass. The mandatory continuous manual fresh-save playthrough, its actual 8–15 minute timing, full-world route readability, and listening review remain unverified. Automated interaction tests and teleported evidence are not a manual playthrough. PR #7 was not merged and Sprint 6 was not started.

## Repository preflight

- Branch: `feature/sprint-5-5-world-character-overhaul`.
- Starting HEAD: `3f4884c0af7b86cb8abbd25f9801f1690f721f00`.
- Initial tracked changes: TMP fallback font, GuideNpc_RelaxedIdle.anim, GuideNpc_WarmAccent_URP.mat, Sprint55C2GuideNpcProductionBuilder.cs, Sprint55EnvironmentDressingBuilder.cs, Sprint55WorldLayoutBuilder.cs, NPC_Guide_Visual.prefab, Forest.unity, Sprint55C2ProductionPassTests.cs, Sprint5ProductionSmokeTests.cs.
- Full dirty/untracked inventory: [initial status](Baseline/git-status.txt). Untracked content includes TreePack files, NPC meshes/builders, prior screenshots/logs and Universal Base Characters. These are excluded from sprint commits.
- [Preservation audit](preservation-audit.txt): nine dirty tracked files outside Forest exactly match the initial user patch. Forest has only 33 changed lines relative to the initial working scene, all placement/rotation values; all gameplay references, IDs, colliders, terrain, lighting and assets remain intact.
- [Pre-move table](PLACEMENT-PLAN.md), [serialized reference audit](Baseline/placement.txt), [after audit](placement-after.txt).

## Placement

The audit discovered that five memories, Light Path and Heart Garden remained at prototype coordinates roughly 16–21 metres below terrain despite the passing baseline. Existing instances were relocated; no replacements or gameplay architecture changes were made.

| Object | Region | World position x / y / z | Reason |
|---|---|---|---|
| Spawn | Spawn Meadow | 0 / 9.721 / -150 | Preserve safe platform; face Guide |
| NPC | NPC Grove | -18 / 9.648 / -108 | Preserve production model/rig and central reference |
| memory_01 | First memory route approach | 18 / 12.072 / -86 | Introductory discovery beyond NPC |
| memory_02 | Memory Route A | 60 / 12.721 / -62 | Eastern clearing |
| memory_03 | Lake approach | 92.250 / 17.731 / 43.098 | Lake-side exploration; local adjustment cleared geometry/slope |
| memory_04 | Memory Route B | -76 / 16.426 / 18 | Western exploration and Light Grove approach |
| memory_05 | Final Hill approach | 8 / 41.383 / 112 | Farther exploration below the summit |
| Light Path start | Light Grove | -58 / 25.426 / 70 | Southern entrance into ordered route |
| Card start | Heart Garden | -158 / 19.146 / -44.8 | Existing flower clearing, open approach |
| Final Camp | Final Hill | 18 / 49.162 / 145 | Preserve elevated destination, fire/audio and vista |

Memories span eastern, western, lake and northern hill areas, with pairwise spacing above 25 m. IDs `memory_01`–`memory_05` and production memory renderers are unchanged. Static checks verify above-terrain placement, bounds and at least one clear interaction approach. These checks do not establish that every long route is navigable or that every collectible is easy to notice under normal play.

### Light Path

Five ordered nodes, world positions:

| Node | Position |
|---|---|
| 00 | -65 / 25.605 / 77 |
| 01 | -68 / 26.273 / 86 |
| 02 | -63 / 27.735 / 94 |
| 03 | -53 / 29.203 / 94 |
| 04 | -47 / 28.928 / 86 |

A curved route crosses the grove from southwest through north to east. Static terrain-following capsule samples at roughly one-metre intervals pass along each straight segment; slope and node spacing checks pass. Node order, 40-second timer, wrong-node reset, timeout, retry and quest completion semantics are unchanged. Existing fog/emission settings were preserved. Game View evidence shows nearby nodes; full manual visibility/navigation review remains required.

### Heart Garden and finale

The entire existing Heart Garden root moved, preserving four pair definitions, eight cards, UI and start/controller references. Automated checks cover mismatch hiding, matched cards remaining matched, completion, panel close, cursor override release and camera input restoration.

Final Camp stays at the existing summit. Its three-completed-quest gate remains unchanged. Tests reject early finale activation, verify unlock after all turn-ins, prevent a concurrent second sequence, verify production camera ownership and release, and reload final completion. The finale uses the existing gameplay camera under external control, not a newly introduced second camera. The original FinalMessageDefinition remains assigned.

## Save and progression results

The integration test loads Bootstrap and interacts with the actual scene components and actual memory IDs. It invokes interaction methods directly; it does not prove physical E-key proximity or manual traversal.

| Test state | Expected | Actual | Result |
|---|---|---|---|
| Fresh save | No memories; finale/minigames gated | Empty IDs; early starts rejected | PASS |
| Accept memories via Guide dialogue | NotStarted → Active | Existing dialogue callback starts quest | PASS |
| Partial memories 2/5 | memory_01 and memory_02 persist, stay hidden; remaining available | Verified after Bootstrap reload | PASS |
| Duplicate pickup | No duplicate progress | Each ID counted once | PASS |
| Five memories / turn-in | ReadyToTurnIn → Completed | Actual collectible interaction and Guide turn-in | PASS |
| Completed memory quest reload | Completed preserved | Preserved | PASS |
| Light Path prerequisites | No early start | Rejected until active through Guide | PASS |
| Wrong node / timeout / retry | Reset and usable again | Wrong node and actual timer expiry tested | PASS |
| Active incomplete Light Path reload | Quest remains Active, run resets | Inactive run, index 0 | PASS |
| Ordered nodes / turn-in | ReadyToTurnIn → Completed | Actual node activation and Guide turn-in | PASS |
| Card prerequisites | No early start | Rejected before intended progression | PASS |
| Cards | Mismatch hides, matches remain, four pairs complete | Verified via controller and panel | PASS |
| Card turn-in/reload | Completed remains | Preserved | PASS |
| Final before prerequisites | Cannot start | Rejected, including cards ready but not turned in | PASS |
| Final after prerequisites | Unlock, sequence, camera ownership/release | Verified | PASS |
| finalCompleted | Saved and survives reload | Saved and survives Bootstrap reload | PASS |

No save format migration or player-position persistence was introduced. The pre-existing PlayMode tests deleted the default save; a suite-level backup/restore guard now preserves an existing file and refuses to overwrite an interrupted-run backup. The opt-in evidence helper independently backs up/restores its save. The initial baseline test run preceded discovery of the old delete behavior; no save file was found by the subsequent check, so this report does not claim a pre-baseline backup existed.

## Regression checks

| Area | Result and limits |
|---|---|
| Fox | Existing animation, collider, jump and footsteps code/assets untouched; baseline smoke tests pass; Game View renders Toon Fox |
| NPC | Production model, rig, outfit, idle and collider untouched; dialogue callback progression passes; normal input/manual approach review pending |
| Tree / rock / bridge collision | Existing collision data unchanged; baseline tests and local interaction/Light Path checks pass; full-route manual collision review pending |
| Camera | Spawn faces Guide; existing SnapToTarget preserved; initial/finale ownership tests pass; evidence has no blank/origin view |
| Atmosphere / water | No settings/assets changed; atmosphere suite passes; no new water/lighting design |
| Audio | Audio code/assets/routing unchanged; listening review not performed |

## Test baseline → after

| Suite | Before | After |
|---|---|---|
| EditMode | 71/71 PASS | 76/76 PASS |
| PlayMode | 18/18 PASS | 19/19 PASS |
| Sprint5Validation | PASS | PASS |

Results: [baseline EditMode](Baseline/EditMode.xml), [baseline PlayMode](Baseline/PlayMode.xml), [EditMode](EditMode.xml), [PlayMode](PlayMode.xml). One initial new clearance test incorrectly counted the raised spawn platform as obstruction; it was corrected to sample clearance above walkable support, then passed. No existing tests were weakened.

Known logs: Unity licensing handshake/access-token retries recovered; existing Assets/Scripts/UIController.cs FindObjectOfType deprecation warnings occurred during compilation. No new gameplay exception was observed in the automated tests/evidence. This is not a claim about an unperformed full manual run.

## Evidence and manual gate

Required named images are under [Screenshots](Screenshots/), with extra card UI/matched-state images. `Sprint55GameplayReview` is an opt-in automated evidence helper using production camera/UI, direct component interactions and transient player teleports. It restores save state and exits Play Mode. Editor gizmos were disabled for the final capture.

Fresh save duration: **NOT MEASURED — no continuous manual playthrough completed.**

[Automated Game View recording](Automated-GameView-Review.gif): 55 captured frames at approximately 1 fps, encoded as a labelled GIF. The capture completed and the default save directory returned to its pre-review state (no save/backup file).

Observed visual review issue: the existing card label "Yaprak" wraps onto two lines. Card UI layout was not modified by this sprint; this is recorded for review rather than silently treated as polished. The photographed start objects and camp also retain their existing simple geometry.

The automated recording is explicitly labelled as such and must not be used as the 8–15 minute metric. No arbitrary delays, movement-speed changes, new map topology, minimap or quest arrows were added. Placement tuning was limited to the existing regions and a local lake-memory clearance adjustment.

Remaining blockers:

1. One continuous normal-input fresh-save route from Spawn to finalCompleted, with actual wall-clock time.
2. Verify Spawn → NPC takes roughly 10–20 seconds in normal traversal.
3. Confirm all five memory routes, NPC returns, Light Path visibility and all environment collision routes in normal play.
4. Review movement/animation/audio and complete representative existing-save manual testing.

| Area | Sprint acceptance |
|---|---|
| Memory Quest | NEEDS USER REVIEW (automated progression PASS) |
| Light Path | NEEDS USER REVIEW (automated rules and local route checks PASS) |
| Card Matching | NEEDS USER REVIEW (automated UI/state PASS) |
| Final Camp | NEEDS USER REVIEW (automated gate/camera/save PASS) |
| Save/Load | PASS (automated) |
| Full E2E | NEEDS USER REVIEW |

**NO-GO — SPRINT 5.5-E QUALITY GATE NOT PASSED. DO NOT MERGE PR #7.**

## Commit and remaining worktree record

- `9b14f52` — feat(gameplay): reposition quests into production world. Only the 33-line scene placement delta is committed; initial user scene edits remain unstaged.
- The following test/evidence commit contains the new editor review helper, tests and this report.
- [Remaining user changes](remaining-user-changes.txt): 10 modified tracked files and 662 pre-existing untracked files preserved.
- [Machine-readable validation summary](validation-summary.json).
