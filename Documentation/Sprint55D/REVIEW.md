# Sprint 5.5-D — Atmosphere, lighting, water and audio

## Decision

**READY FOR USER SPRINT 5.5-D ATMOSPHERE REVIEW — DO NOT START SPRINT 5.5-E**

Technical tests pass. This is a review candidate, not a declaration that the entire artistic/audio acceptance checklist has passed. A real headphone/speaker listening review and normal interactive camera/UI walkthrough still require the user. PR #7 has not been merged; Sprint 5.5-E has not started.

## Preflight and preservation

- Branch: `feature/sprint-5-5-world-character-overhaul`.
- Starting HEAD: `5cf78a88d9ca6baacdfcb8f9b7ed69a076f033d0`.
- Unity `6000.0.23f1`, URP `17.0.3`, Windows PC.
- Fresh status: **10 modified tracked files, 662 untracked files**. Full inventory: [initial status](Baseline/git-status.txt).
- Initial tracked changes: TMP LiberationSans fallback; `GuideNpc_RelaxedIdle.anim`; `GuideNpc_WarmAccent_URP.mat`; `Sprint55C2GuideNpcProductionBuilder.cs`; `Sprint55EnvironmentDressingBuilder.cs`; `Sprint55WorldLayoutBuilder.cs`; `NPC_Guide_Visual.prefab`; `Forest.unity`; `Sprint55C2ProductionPassTests.cs`; `Sprint5ProductionSmokeTests.cs`.
- Initial untracked content includes TreePack files, fitted NPC meshes/builders, bridge/world review tools, five C.2 review images, the Universal Base Characters folder and prior run logs. None was staged into this sprint.
- The initial user patch and Forest snapshot are retained locally under `Baseline/` and ignored by this documentation folder. Reconstructing the original patch confirmed all nine initially dirty tracked files other than Forest remained identical to their initial content.
- Forest was edited on top of the current user version. The commit stages only the difference between that snapshot and this sprint's result. All initial scene changes remain unstaged.
- [Component difference audit](preservation-audit.txt) and [water geometry check](water-geometry-check.txt): no terrain, landmark, NPC, fox, quest, trigger or collider placement edits. The two original inline water meshes become mesh assets with identical vertex coordinates. Only sun rotation and the camp audio emitter's position change among existing transforms.

## Audit findings

[Before audit](Baseline/audit.txt) / [after audit](after-audit.txt).

The C.2 corrective work reduced the sun to 0.9 and terrain layers to metallic 0 / smoothness 0.02; these scalar values remain. A reverse-angle capture exposed residual distant glare: the layers selected DiffuseAlphaChannel, and their opaque RGBA textures made effective smoothness 1. Switching the four layers to Constant makes the existing 0.02 value effective. Texture RGB, texture alpha, terrain geometry and layer painting are unchanged. The sun had no shadows, distance fog was off, no scene Global Volume existed, the default profile was empty, and the renderer's post-process data reference was null. The live built-in sky exposure was **1.3**, despite a corrective builder having code to lower it; this audit used the actual loaded value.

Water used the project's URP Lit transparent material. No local Free Pack Water Shader or suitable dedicated creek/lake recording was found. The lake's original triangle winding faced downward: backface culling hid it from above. Neither water mesh had UVs/tangents. Audio routing was already valid. Final Camp's locked visual state overrode every camp renderer to gray and reduced the fire light to 0.15.

## Lighting

RGB values are Unity color values, rotations are Euler degrees.

| Setting | Before | After | Reason |
|---|---|---|---|
| Directional intensity | 0.9 | 0.9 | Preserve C.2 glare correction |
| Rotation | 52, -34, 0 | 48, -32, 0 | Moderate afternoon angle; readable form |
| Color | 1, 0.88, 0.72 | 1, 0.955, 0.87 | Warm-neutral rather than orange |
| Shadows / strength | None / 1 inactive | Soft / 0.72 | Give fox, trees, bridge and slopes depth |
| Shadow map | 2048 | 2048 | Retain existing budget |
| Shadow distance / cascades | 50 m / 1 | 65 m / 2, split 0.35 | Useful nearby detail without full-world shadows |
| URP soft shadows | Off | On | Soften foreground edges |
| Ambient source | Skybox | Trilight | Predictable cool, readable fill |
| Ambient sky | 0.48, 0.52, 0.44 | 0.47, 0.55, 0.62 | Cooler upper fill |
| Ambient equator | 0.114, 0.125, 0.133 | 0.30, 0.36, 0.39 | Readable shaded trunks/fur |
| Ambient ground | 0.047, 0.043, 0.035 | 0.19, 0.23, 0.22 | Avoid black lower silhouettes |
| Ambient intensity | 1 | 1 | No global brightness boost; Trilight uses the three colors |
| Skybox | Built-in procedural, exposure 1.3 | Project-owned copy, exposure 0.9 | Avoid mutating shared built-in sky |
| Sky ground color | Built-in dark ground | 0.57, 0.69, 0.72 | Remove dark horizon band from elevated camp vista |
| Reflection | Skybox, 128, 1 bounce, intensity 1 | Same source/resolution/bounces, intensity 0.35 | Restrained surface response |
| Terrain layers | Metallic 0; smoothness 0.02, but source DiffuseAlphaChannel (opaque alpha 1) | Same scalar values; source Constant | Make the matte value effective and remove distant hillside glare |

One main realtime shadow light. Existing local lights retain shadows OFF. No new lights were added: loaded Forest contains 15 point lights (14 initially enabled plus one disabled feedback light), mostly existing memory/Light Path feedback sources with 2.8–5.2 m ranges. Their runtime state remains controlled by the existing minigames. This inherited count exceeds the brief's preferred “few”; adding or relocating gameplay lights was avoided. The camp light alone expands to 7 m.

## Volume and fog

`Forest Atmosphere Volume` uses `ForestAtmosphere.asset`. The production camera enables post-processing, and `TilkiUniversalRenderer` now references URP's own post-process data.

| Effect | Enabled | Key settings | Reason |
|---|---|---|---|
| Color Adjustments | Yes | Exposure 0; contrast +4; saturation -3 | Modest separation, natural greens |
| Bloom | Yes | Threshold 1.2; intensity 0.16; scatter 0.55; HQ filtering off | Small emissive highlights, no terrain glow |
| Vignette | Yes | Intensity 0.08; smoothness 0.4 | Slight focus without dark corners |
| Tonemapping | Yes | Neutral | Compress strong emissive highlights |
| Motion Blur | **OFF** | Absent | Clean gameplay |
| Chromatic Aberration | **OFF** | Absent | Clean gameplay |
| Depth of Field / grain / lens distortion | OFF | Absent | No cinematic obstruction |

Fog: built-in URP-compatible linear distance fog, RGB **0.57 / 0.69 / 0.72**, **75–310 m**. Density is unused in Linear mode. The terrain is 512 × 512 m; the first 75 m remains clear, long views soften progressively toward the perimeter. No new fog package, volumetrics or day/night system. Sky ground tint matches the atmosphere to reduce exposed horizon contrast.

## Water

Existing **Universal Render Pipeline/Lit** shader retained. Dedicated Lake/Creek material copies prevent changes to older corrective-pass assets.

| Setting | Lake | Creek |
|---|---|---|
| RGB / opacity | 0.19, 0.43, 0.49 / 0.78 | 0.23, 0.48, 0.51 / 0.64 |
| Metallic / smoothness | 0 / 0.28 | 0 / 0.28 |
| Normal strength | 0.18 | 0.18 |
| UV speed, units/sec | 0.008, 0.004 | 0.018, 0.014 |
| Reflections | Environment reflections off | Environment reflections off |
| Shadow casting | Off | Off |

Upward-facing triangles fix the lake's missing surface. Mesh positions and shoreline shape are identical to baseline; UVs and tangents are added to asset copies. A tiny tileable project-generated normal texture and `CalmWaterMotion` animate the normal pattern. The creek's diagonal drift follows its general southwest direction. No vertex waves, simulation, refraction, SSR, depth texture or extra render pass. Two renderer property-block updates per frame, no per-frame material instantiation. Existing faceted shoreline and terrain-following water heights remain; this is not a physically level water simulation.

## Audio and camp

The existing `AudioService`, `TilkiAudioMixer` and its Master/Music/Ambience/SFX groups remain intact. All loaded sources route to their intended group. Broad ambience and music stay 2D. Dedicated water audio is skipped because no suitable local asset was found.

| Source | Mixer group | 2D/3D | Volume | Notes |
|---|---|---|---|---|
| Sunset Walk music | Music | 2D | 0.18 → 0.12 | Existing SceneLoopAudio, lower support level |
| Forest ambience | Ambience | 2D | 0.12 → 0.15 | Broad environmental bed |
| Footsteps | SFX | Blend 0.7 | Source 1 × one-shot 0.28 | Cadence, sprint distinction and airborne suppression unchanged |
| Memory | SFX | Runtime blend 0.35 | Source 1 × one-shot 0.58 | Existing clips and pickup events unchanged |
| Light Path chimes | SFX | 2D | Source 1 × one-shot 0.45 | Existing events/pitch changes unchanged |
| Cards | SFX | 2D | Source 1 × one-shot 0.42 | Existing card feedback unchanged |
| Campfire | Ambience | 3D, blend 1 | 0.18 → 0.20 | Fire-centered; min 2.5 m, max 22 m; custom smooth fade to zero; Doppler 0 |
| Final completion chime | SFX | Blend 0.65 | Source 1 × one-shot 0.55 | Unchanged |

Campfire attenuation curve, normalized distance/gain: `(0,1), (0.12,1), (0.4,0.36), (1,0)`. Automated validation checks its monotonic fade and silent outer boundary. This verifies configuration, not perceived loudness.

The original elevated camp, platform, tent and vista remain. Its existing light is warm RGB 1 / 0.64 / 0.34, 7 m range, no shadows. Authored intensity is 1.8; the existing controller uses 1.15 while locked and 1.8 when unlocked, with warm state colors. Final sequence still transitions to its existing 2.4 value. The state tint now targets the fire renderer, leaving the camp's wood visible. The interaction lock, quest conditions and save behavior are untouched.

A maximum of 24 small soft flame particles sit at the existing fire. The ember material supports restrained emission. Dedicated memory/node materials enable the emission keyword that their existing presentation expects; node state logic and IDs are unchanged.

**Real listening review: NEEDS USER REVIEW.** No claim is made that headphone/speaker balance, clipping, distortion or perceptual approach behavior was heard here. Check music + forest bed, walking/running, memory pickup, Light Path chime, card reveal/match, and fire approach from 22 m through 9 m to 2.5 m. Surface-dependent footsteps remain a possible later enhancement; no new surface-audio system was added.

## Tests and runtime review

| Gate | Fresh before | Fresh after |
|---|---|---|
| EditMode | 65 / 65 PASS | 71 / 71 PASS |
| PlayMode | 18 / 18 PASS | 18 / 18 PASS |
| Sprint5Validation | PASS | PASS |

Evidence: [baseline EditMode](Baseline/EditMode.xml), [baseline PlayMode](Baseline/PlayMode.xml), [final EditMode](EditMode.xml), [final PlayMode](PlayMode.xml). Full Unity logs remain local alongside these files. Six new cases validate presentation wiring, forbidden effects, matte terrain/fog, mixer routing/rolloff, and both upward-facing URP water surfaces. The baseline regression suite remains intact.

Fox animation/movement/sprint/jump anti-spam, terrain/tree/rock collision, camera setup, NPC visual/interaction, dialogue, memories, Light Path, cards, Final Camp gate/sequence and save/load retain their implementation and pass the existing automated regression coverage. This does not substitute for manually walking every route and checking each animation/UI state. Review camera poses are transient; no player position is serialized or saved.

Production-camera Play Mode captures include the animated fox and camp particles. Lake, bridge and distant-hill overview images use the same production camera rendered in Edit Mode, keeping the foreground free of a cropped review fox; the other seven are Play Mode frames. They use a 1600 × 900 HDR render target; they are world captures, **not screenshots of every overlay UI panel**. Existing UI Canvas configuration is unchanged. Normal interaction prompt, quest HUD, dialogue, card and final panel reading should be checked in the user walkthrough. Some minigame objects remain at their inherited temporary coordinates below the rebuilt terrain; region images are not evidence of final minigame placement. This sprint intentionally leaves final placement to 5.5-E.

## Performance

Measured on RTX 4070 Laptop GPU / i7-13700H, Editor Play Mode, 1600 × 900 HDR target, VSync off. Each view has 60 warmup frames and 180 sampled frames, with post-processing on and off. See [per-view samples](PlayReview/performance.csv), [measurement context](PlayReview/device.txt) and [summary](performance-summary.md).

Performance status is **PASS for these sampled editor views**. This is not a standalone build or minimum-spec certification. Only warmed samples are evaluated; camera-cut/readback costs are excluded. No release packaging was started.

## Visual acceptance awaiting user review

| Area | Status | Evidence / remaining check |
|---|---|---|
| Spawn lighting | NEEDS USER REVIEW | White terrain glare removed; warm-neutral sun and readable fox |
| Forest atmosphere | NEEDS USER REVIEW | Actual shadow depth and cooler fill; assess mood while walking |
| Fog | NEEDS USER REVIEW | Long views softened; inspect perimeter from Final Hill |
| Lake water | NEEDS USER REVIEW | Surface now visible from above; inspect shoreline/faceted shape |
| Creek water | NEEDS USER REVIEW | Teal surface and bridge shadows; inspect motion while crossing |
| Light Grove readability | NEEDS USER REVIEW | Region captured; final node placement deferred |
| Heart Garden | NEEDS USER REVIEW | Regional warmth/flowers; actual card UI requires walkthrough |
| Final Camp atmosphere | NEEDS USER REVIEW | Warm fire persists while locked; assess emotional emphasis |
| Audio balance | NEEDS USER REVIEW | Routing/rolloff pass; listening not performed |
| Performance | PASS | Sampled editor views; see limits above |

## Screenshots

All ten requested filenames are under [Screenshots](Screenshots/). These are transient production-camera captures; no scene/camera positions were saved.

1. [Spawn](Screenshots/01_spawn_atmosphere.png)
2. [Forest depth](Screenshots/02_forest_depth.png)
3. [Lake water](Screenshots/03_lake_water.png)
4. [Bridge / creek](Screenshots/04_bridge_creek.png)
5. [Light Grove](Screenshots/05_light_grove.png)
6. [Heart Garden](Screenshots/06_heart_garden.png)
7. [Final Hill from distance](Screenshots/07_final_hill_distance.png)
8. [Final Camp](Screenshots/08_final_camp.png)
9. [NPC lighting](Screenshots/09_npc_lighting.png)
10. [Fox in forest](Screenshots/10_fox_forest.png)

## Reproduction and handoff

Editor menu: `Tilki Oyunu > Sprint 5.5 D > Apply Presentation` for the idempotent authoring pass. It refuses to overwrite an unsaved open scene. `Play Review and Profile` temporarily poses the camera/fox, measures the views and exits Play Mode without saving scene changes. `Audit Forest` records a fresh audit; preserve the supplied baseline evidence before intentionally replacing it.

Use the Unity Test Runner for all EditMode and PlayMode tests and `Sprint5Validation.ValidateFromCommandLine` for the existing sprint validator. Review the saved screenshots and conduct the listening/UI walkthrough before declaring Sprint 5.5-D PASS.

Implementation commit: `b93df3d074aea4e8a85fbadd07594acef5e54f7c` — `feat(atmosphere): polish Forest lighting water and camp audio`. A second commit contains the presentation tests, repeatable camera review and this evidence package. Only D files and D-specific Forest hunks are committed. See the final task response for the final HEAD. Initial user edits and untracked files remain in the working tree; a complete final status is saved locally as `final-git-status.txt`.

**Do not merge PR #7 or automatically start Sprint 5.5-E.**
