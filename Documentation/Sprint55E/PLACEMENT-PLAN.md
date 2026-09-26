# Pre-move placement and dependency audit

Read-only Unity scene audit: [full serialized references](Baseline/placement.txt).
Positions are world x/y/z. Landmark positions were read from the loaded scene, not assumed from builder constants.

| Gameplay object | Current position | Intended landmark | Dependencies | Safe to move? |
|---|---|---|---|---|
| Player Spawn / PlayerFox | 0/9.721/-150 | Spawn Meadow | ForestGameplayBootstrap, PlayerSpawnPoint, camera target | Already placed; preserve height; face NPC |
| NPC Guide | -18/9.648/-108 | NPC Grove | Three quest definitions, dialogue assets/UI; visual and blocker children | Already placed; preserve |
| memory_01 | 1.6/0.8/-6.2 | Spawn–NPC–Memory A transition | Memory_01, collect_memories, feedback UI/audio, production renderer | Yes; root and separate dressing anchor |
| memory_02 | -1.35/0.95/-0.8 | Memory Route A clearing | Memory_02; same shared quest/UI | Yes; root and dressing anchor |
| memory_03 | 7.35/0.75/-2.7 | Lake approach beyond bridge | Memory_03; same shared quest/UI | Yes; ground and shoreline checks required |
| memory_04 | -7.6/0.75/2.15 | Memory Route B / Light approach | Memory_04; same shared quest/UI | Yes; ground and obstruction checks required |
| memory_05 | 0.7/0.85/8.6 | Final Hill approach | Memory_05; same shared quest/UI | Yes; keep below camp summit |
| Light Path controller | 0/0/0 | Light Grove | light_path; ordered node array 00–04 | Move root with children, then ground each node |
| Light Path start | 1.5/0.55/5.1 | Light Grove south entrance | Existing controller | Yes |
| Node 00 | 4.2/0.65/7.4 | Light Grove southwest | Controller, visualRoot; array index 0 | Yes; preserve order |
| Node 01 | 7.2/0.65/9.6 | Light Grove west | Controller, visualRoot; array index 1 | Yes; preserve order |
| Node 02 | 10.1/0.65/7.1 | Light Grove northwest | Controller, visualRoot; array index 2 | Yes; preserve order |
| Node 03 | 12/0.65/3.9 | Light Grove northeast | Controller, visualRoot; array index 3 | Yes; preserve order |
| Node 04 | 9/0.65/1 | Light Grove east | Controller, visualRoot; array index 4 | Yes; preserve order |
| Heart Garden / card start | -7.3/0.52/6.4; start -7.3/0.7/5.6 | Heart Garden | card_matching, panel, four pair definitions; start controller | Yes; move complete root and ground start |
| Final Camp | 18/49.162/145 | Final Hill | FinalSequence, fire renderer/light | Already placed; preserve mood/audio |
| Final Sequence | Camp component | Final Hill | Main Camera + ThirdPersonCameraController, child focus, final panel/message, input lock, light/audio | Preserve all references and transforms |

All placement changes retain existing instances, persistent IDs, reference relationships and gameplay rules. Placement candidates must be above terrain and outside solid environment colliders. Such checks do not prove manual route readability or duration.
