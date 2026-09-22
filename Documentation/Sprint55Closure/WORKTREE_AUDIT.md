# Sprint 5.5 worktree audit

Initial branch: `feature/sprint-5-5-world-character-overhaul`. Initial HEAD: `1c6878cef5509cc25d4e74348e387dcb839062f1`.

Modified tracked files: 10. Untracked individual files: 662 (Git short status collapses directories). Nothing was initially staged.

PR #7 topology verified through authenticated GitHub connector: Sprint 5 parent -> main, open, mergeable. Feature branch has 27 local commits above parent and is not yet on origin. GitHub CLI not installed/on PATH; connector authenticated as mberatkaya; git fetch succeeded.

## Initial classification

| Category | Count |
|---|---:|
| Must remain untouched/uncommitted | 1 |
| Must ship with Sprint 5.5 | 12 |
| Tests needed to protect implementation | 4 |
| Not required by runtime | 604 |
| .meta files required by production assets | 5 |
| Screenshots/XML/logs/review files | 46 |

## Exact dirty/untracked inventory

| Status | Path | Bytes | Classification | Reason |
|---|---|---:|---|---|
| modified | `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset` | 9748 | Must remain untouched/uncommitted | TMP dynamic fallback serialization/cache change outside closure production patch. |
| modified | `Assets/_Game/Animations/NPC/GuideNpc_RelaxedIdle.anim` | 3888 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| modified | `Assets/_Game/Art/NPC/GuideNpc_WarmAccent_URP.mat` | 3737 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| modified | `Assets/_Game/Editor/Sprint55C2GuideNpcProductionBuilder.cs` | 22629 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| modified | `Assets/_Game/Editor/Sprint55EnvironmentDressingBuilder.cs` | 74141 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| modified | `Assets/_Game/Editor/Sprint55WorldLayoutBuilder.cs` | 39707 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| modified | `Assets/_Game/Prefabs/NPC/NPC_Guide_Visual.prefab` | 98555 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| modified | `Assets/_Game/Scenes/Gameplay/Forest.unity` | 4373116 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| modified | `Assets/_Game/Tests/EditMode/Sprint55C2ProductionPassTests.cs` | 15104 | Tests needed to protect implementation | Guide fit/idle and two-way bridge traversal regression coverage. |
| modified | `Assets/_Game/Tests/PlayMode/Sprint5ProductionSmokeTests.cs` | 33879 | Tests needed to protect implementation | Guide fit/idle and two-way bridge traversal regression coverage. |
| untracked | `Assets/TreePackVol.1/Doc.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Doc/Readme.pdf` | 66394 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Doc/Readme.pdf.meta` | 339 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Doc/Tree_Offers.pdf` | 1620729 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Doc/Tree_Offers.pdf.meta` | 344 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/1.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/1/Leave2.mat` | 2332 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/1/Leave2.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/1/Trunk2.mat` | 2425 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/1/Trunk2.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/2/Trunk3.mat` | 2425 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/2/Trunk3.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/2/fruit3.mat` | 2332 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/2/fruit3.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/2/fruit5.mat` | 2332 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/2/fruit5.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/2/fruit6.mat` | 2332 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/2/fruit6.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/2/fruit7.mat` | 2332 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/2/fruit7.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/3.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/3/Leave4.mat` | 2331 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/3/Leave4.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/3/Trunk4.mat` | 2425 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/3/Trunk4.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/3/fruit4.mat` | 2332 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/3/fruit4.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/4/fruit5.mat` | 2332 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/4/fruit5.mat.meta` | 380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/6/2.mat` | 2380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/6/2.mat.meta` | 375 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/6/3.mat` | 2380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/6/3.mat.meta` | 375 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/Palm.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/Palm/New Material 1.mat` | 2370 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/Palm/New Material 1.mat.meta` | 434 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/Palm/New Material.mat` | 2394 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Materials/Palm/New Material.mat.meta` | 432 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/New Terrain 1.asset` | 1072268 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/New Terrain 1.asset.meta` | 378 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/New Terrain.asset` | 705572 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/New Terrain.asset.meta` | 376 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree0_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree0_Textures/diffuse.png` | 1641931 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree0_Textures/diffuse.png.meta` | 2037 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree0_Textures/normal_specular.png` | 20544 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree0_Textures/normal_specular.png.meta` | 2045 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree0_Textures/shadow.png` | 15778 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree0_Textures/shadow.png.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree0_Textures/translucency_gloss.png` | 20539 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree0_Textures/translucency_gloss.png.meta` | 2048 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10.prefab` | 484569 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10_Textures/diffuse.png` | 1660608 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10_Textures/normal_specular.png` | 21314 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10_Textures/shadow.png` | 161634 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree10_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11.prefab` | 479485 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11_Textures/diffuse.png` | 1660608 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11_Textures/normal_specular.png` | 21314 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11_Textures/shadow.png` | 161634 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree11_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree13.prefab` | 1403670 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree13.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree14.prefab` | 875088 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree14.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree15.prefab` | 1357871 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree15.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2.prefab` | 177426 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2.prefab.meta` | 382 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2_Textures/diffuse.png` | 1660608 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2_Textures/diffuse.png.meta` | 2037 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2_Textures/normal_specular.png` | 21314 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2_Textures/normal_specular.png.meta` | 2045 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2_Textures/shadow.png` | 161634 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2_Textures/shadow.png.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree2_Textures/translucency_gloss.png.meta` | 2048 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4.prefab` | 545487 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4.prefab.meta` | 382 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4_Textures/diffuse.png` | 1660608 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4_Textures/diffuse.png.meta` | 2037 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4_Textures/normal_specular.png` | 21314 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4_Textures/normal_specular.png.meta` | 2045 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4_Textures/shadow.png` | 161634 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4_Textures/shadow.png.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree4_Textures/translucency_gloss.png.meta` | 2048 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6.prefab` | 552304 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6.prefab.meta` | 382 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6_Textures/diffuse.png` | 1660608 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6_Textures/diffuse.png.meta` | 2037 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6_Textures/normal_specular.png` | 21314 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6_Textures/normal_specular.png.meta` | 2045 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6_Textures/shadow.png` | 161634 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6_Textures/shadow.png.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree6_Textures/translucency_gloss.png.meta` | 2048 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7.prefab` | 779552 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7.prefab.meta` | 382 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7_Textures/diffuse.png` | 1699714 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7_Textures/diffuse.png.meta` | 2037 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7_Textures/normal_specular.png` | 21950 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7_Textures/normal_specular.png.meta` | 2045 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7_Textures/shadow.png` | 158787 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7_Textures/shadow.png.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree7_Textures/translucency_gloss.png.meta` | 2048 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9.prefab` | 508969 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9.prefab.meta` | 382 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9_Textures/diffuse.png` | 1660608 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9_Textures/diffuse.png.meta` | 2037 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9_Textures/normal_specular.png` | 21314 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9_Textures/normal_specular.png.meta` | 2045 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9_Textures/shadow.png` | 161634 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9_Textures/shadow.png.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/0/Tree9_Textures/translucency_gloss.png.meta` | 2048 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree.prefab` | 329254 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree.prefab.meta` | 381 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree_Textures/diffuse.png` | 1480856 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree_Textures/diffuse.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree_Textures/normal_specular.png` | 21314 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree_Textures/normal_specular.png.meta` | 2044 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree_Textures/shadow.png` | 38805 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree_Textures/shadow.png.meta` | 2034 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/1/Tree_Textures/translucency_gloss.png.meta` | 2047 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1.prefab` | 474464 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1_Textures/diffuse.png` | 1127839 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1_Textures/normal_specular.png` | 22110 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1_Textures/shadow.png` | 374070 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1_Textures/translucency_gloss.png` | 21990 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 1_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3.prefab` | 365890 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3_Textures/diffuse.png` | 1071383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3_Textures/normal_specular.png` | 20543 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3_Textures/shadow.png` | 393976 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3_Textures/translucency_gloss.png` | 21990 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/2/Tree 3_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2.prefab` | 328457 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2_Textures/diffuse.png` | 1594305 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2_Textures/normal_specular.png` | 749474 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2_Textures/shadow.png` | 161634 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 2_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4.prefab` | 765156 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4_Textures/diffuse.png` | 1594305 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4_Textures/normal_specular.png` | 749474 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4_Textures/shadow.png` | 161634 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 4_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 5.prefab` | 450322 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 5.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6.prefab` | 164810 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6_Textures/diffuse.png` | 1594305 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6_Textures/normal_specular.png` | 749474 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6_Textures/shadow.png` | 161634 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/3/Tree 6_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2.prefab` | 2139460 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2.prefab.meta` | 425 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2_Textures/diffuse.png` | 1588647 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2_Textures/diffuse.png.meta` | 2037 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2_Textures/normal_specular.png` | 21948 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2_Textures/normal_specular.png.meta` | 2045 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2_Textures/shadow.png` | 233877 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2_Textures/shadow.png.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree2_Textures/translucency_gloss.png.meta` | 2048 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3.prefab` | 2355764 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3.prefab.meta` | 425 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3_Textures/diffuse.png` | 1588647 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3_Textures/diffuse.png.meta` | 2037 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3_Textures/normal_specular.png` | 21948 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3_Textures/normal_specular.png.meta` | 2045 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3_Textures/shadow.png` | 233877 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3_Textures/shadow.png.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree3_Textures/translucency_gloss.png.meta` | 2048 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5.prefab` | 2121347 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5.prefab.meta` | 425 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5_Textures/diffuse.png` | 1588647 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5_Textures/diffuse.png.meta` | 2037 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5_Textures/normal_specular.png` | 21948 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5_Textures/normal_specular.png.meta` | 2045 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5_Textures/shadow.png` | 233877 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5_Textures/shadow.png.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/4/Tree5_Textures/translucency_gloss.png.meta` | 2048 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 10.prefab` | 499085 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 10.prefab.meta` | 384 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3.prefab` | 266006 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3_Textures/diffuse.png` | 1647514 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3_Textures/normal_specular.png` | 21948 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3_Textures/shadow.png` | 172939 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 3_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5.prefab` | 261762 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5_Textures/diffuse.png` | 1647514 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5_Textures/normal_specular.png` | 21948 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5_Textures/shadow.png` | 172939 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 5_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6.prefab` | 265865 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6_Textures/diffuse.png` | 1647514 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6_Textures/normal_specular.png` | 21948 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6_Textures/shadow.png` | 172939 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 6_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8.prefab` | 491250 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8_Textures/diffuse.png` | 1741726 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8_Textures/normal_specular.png` | 22454 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8_Textures/shadow.png` | 123860 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8_Textures/translucency_gloss.png` | 22377 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 8_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9.prefab` | 491921 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9.prefab.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9_Textures/diffuse.png` | 1690866 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9_Textures/diffuse.png.meta` | 2038 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9_Textures/normal_specular.png` | 22454 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9_Textures/normal_specular.png.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9_Textures/shadow.png` | 126283 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9_Textures/shadow.png.meta` | 2036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9_Textures/translucency_gloss.png` | 22377 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/5/Tree 9_Textures/translucency_gloss.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm.meta` | 215 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1.prefab` | 266095 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1.prefab.meta` | 429 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1_Textures/diffuse.png` | 2090306 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1_Textures/diffuse.png.meta` | 2041 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1_Textures/normal_specular.png` | 21948 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1_Textures/normal_specular.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1_Textures/shadow.png` | 261786 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1_Textures/shadow.png.meta` | 2039 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 1_Textures/translucency_gloss.png.meta` | 2052 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2.prefab` | 458943 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2.prefab.meta` | 429 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2_Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2_Textures/diffuse.png` | 2090306 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2_Textures/diffuse.png.meta` | 2041 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2_Textures/normal_specular.png` | 21948 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2_Textures/normal_specular.png.meta` | 2049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2_Textures/shadow.png` | 261786 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2_Textures/shadow.png.meta` | 2039 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2_Textures/translucency_gloss.png` | 21924 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 2_Textures/translucency_gloss.png.meta` | 2052 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 3.prefab` | 459094 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 3.prefab.meta` | 429 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 4.prefab` | 266662 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 4.prefab.meta` | 429 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 5.prefab` | 248988 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 5.prefab.meta` | 429 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 6.prefab` | 266025 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 6.prefab.meta` | 429 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 7.prefab` | 257903 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 7.prefab.meta` | 429 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 8.prefab` | 268115 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Prefabs/Palm/Tree 8.prefab.meta` | 429 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Lighting_Profile_TreeShowcase.asset` | 3392 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Lighting_Profile_TreeShowcase.asset.meta` | 422 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Post_Profile_TreeShowcase.asset` | 11737 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Post_Profile_TreeShowcase.asset.meta` | 412 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Showcase.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Showcase/Showcase_.unity` | 92370 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Showcase/Showcase_.unity.meta` | 356 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk Skybox.mat` | 1827 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk Skybox.mat.meta` | 385 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_back.tif` | 563744 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_back.tif.meta` | 2034 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_down.tif` | 25956 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_down.tif.meta` | 2034 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_front.tif` | 351076 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_front.tif.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_left.tif` | 326248 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_left.tif.meta` | 2034 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_right.tif` | 505004 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_right.tif.meta` | 2035 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_up.tif` | 674104 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/DawnDusk/DawnDusk_up.tif.meta` | 2032 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2 Skybox.mat` | 1831 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2 Skybox.mat.meta` | 383 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_back.tif` | 710908 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_back.tif.meta` | 2030 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_down.tif` | 28840 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_down.tif.meta` | 2030 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_front.tif` | 733632 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_front.tif.meta` | 2031 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_left.tif` | 648028 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_left.tif.meta` | 2030 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_right.tif` | 722928 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_right.tif.meta` | 2031 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_up.tif` | 1545828 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Skybox/Sunny2/Sunny2_up.tif.meta` | 2028 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Textures.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Textures/BF1_Sun.flare` | 1429 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Textures/BF1_Sun.flare.meta` | 403 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Textures/cheap plastic 1.psd` | 14896834 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/Textures/cheap plastic 1.psd.meta` | 1681 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/TreeShowcase.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/TreeShowcase.unity` | 804058 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/TreeShowcase.unity.meta` | 350 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/TreeShowcase/LightingData.asset` | 5388912 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/TreeShowcase/LightingData.asset.meta` | 397 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/TreeShowcase/ReflectionProbe-0.exr` | 87347 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/TreeShowcase/ReflectionProbe-0.exr.meta` | 2064 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/TreeShowcaseSettings.lighting` | 1783 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Sample/TreeShowcaseSettings.lighting.meta` | 406 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Scripts.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Scripts/Tree_Packs.cs` | 529 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Scripts/Tree_Packs.cs.meta` | 434 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/4ca84268e761f81a8b89dcdee4a12b50.psd` | 4854967 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/4ca84268e761f81a8b89dcdee4a12b50.psd.meta` | 2046 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Leave_4K_.psd` | 77317910 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Leave_4K_.psd.meta` | 2588 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/Materials.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/Materials/Grass.mat` | 2017 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/Materials/Grass.mat.meta` | 401 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0010_1_alphamasked_S.png` | 372418 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0010_1_alphamasked_S.png.meta` | 2073 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0043_1_alphamasked_S.png` | 222823 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0043_1_alphamasked_S.png.meta` | 2073 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0050_1_alphamasked_S.png` | 346996 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0050_1_alphamasked_S.png.meta` | 2073 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0052_1_alphamasked_S.png` | 565830 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0052_1_alphamasked_S.png.meta` | 2073 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0055_1_alphamasked_S.png` | 487661 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0055_1_alphamasked_S.png.meta` | 2073 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0070_1_alphamasked_S.png` | 123861 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Grass/TexturesCom_NaturePlants0070_1_alphamasked_S.png.meta` | 2073 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Branches0011_1_alphamasked_S.png` | 987269 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Branches0011_1_alphamasked_S.png.meta` | 1928 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Branches0012_1_alphamasked_S.png` | 410893 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Branches0012_1_alphamasked_S.png.meta` | 1928 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Branches0013_1_alphamasked_S.png` | 1349119 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Branches0013_1_alphamasked_S.png.meta` | 1928 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Branches0018_1_alphamasked_S (2).png` | 940172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Branches0018_1_alphamasked_S (2).png.meta` | 1936 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Branches0040_1_alphamasked_S.png` | 621654 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Branches0040_1_alphamasked_S.png.meta` | 1928 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0089_1_alphamasked_S.png` | 465307 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0089_1_alphamasked_S.png.meta` | 1926 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0153_1_alphamasked_S.psd` | 17592630 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0153_1_alphamasked_S.psd.meta` | 2631 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0153_2_alphamasked_S.png` | 456711 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0153_2_alphamasked_S.png.meta` | 2351 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0153_3_alphamasked_S.png` | 51448 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0153_3_alphamasked_S.png.meta` | 2351 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0155_2_alphamasked_S.png` | 497119 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0155_2_alphamasked_S.png.meta` | 2351 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0161_1_alphamasked_S.png` | 484507 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0161_1_alphamasked_S.png.meta` | 1926 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0188_1_alphamasked_S 1.png` | 4516934 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0188_1_alphamasked_S 1.png.meta` | 1932 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0188_1_alphamasked_S.png` | 631609 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_Leaves0188_1_alphamasked_S.png.meta` | 1926 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_LeavesTropical0039_1_alphamasked_S.png` | 307720 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_LeavesTropical0039_1_alphamasked_S.png.meta` | 1934 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_LeavesTropical0145_1_alphamasked_S.png` | 339453 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_LeavesTropical0145_1_alphamasked_S.png.meta` | 1934 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_LeavesTropical0218_1_alphamasked_S.png` | 696032 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_LeavesTropical0218_1_alphamasked_S.png.meta` | 1934 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_LeavesTropical0218_1_alphamasked_S.psd` | 2168893 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_LeavesTropical0218_1_alphamasked_S.psd.meta` | 2074 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_LeavesTropical0233_1_alphamasked_S.png` | 623549 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_LeavesTropical0233_1_alphamasked_S.png.meta` | 1934 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_NaturePlants0005_1_alphamasked_S.png` | 914242 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_NaturePlants0005_1_alphamasked_S.png.meta` | 1932 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_NaturePlants0025_1_alphamasked_S.png` | 806630 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/TexturesCom_NaturePlants0025_1_alphamasked_S.png.meta` | 1932 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/graa - Copy.png` | 76463 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/graa - Copy.png.meta` | 1899 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/graa.png` | 441238 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Leaf/graa.png.meta` | 1892 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Palm.meta` | 172 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Palm/diffuse.png` | 6250587 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Palm/diffuse.png.meta` | 1895 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Palm/palm.psd` | 17176058 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Palm/palm.psd.meta` | 1892 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/23b1616607d7767de03034a661c98837.png` | 2221531 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/23b1616607d7767de03034a661c98837.png.meta` | 2346 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/23b1616607d7767de03034a661c98837_1.png` | 1663609 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/23b1616607d7767de03034a661c98837_1.png.meta` | 2348 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/9504886-Birch-Bark-Texture-Stock-Photo.png` | 1819567 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/9504886-Birch-Bark-Texture-Stock-Photo.png.meta` | 2352 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkDecidious0174_1_seamless_S.png` | 2188211 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkDecidious0174_1_seamless_S.png.meta` | 2356 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkDecidious0209_1_S.png` | 1515706 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkDecidious0209_1_S.png.meta` | 2347 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkDecidious0212_1_M.png` | 2855765 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkDecidious0212_1_M.png.meta` | 2347 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkPalm0008_S.png` | 2123474 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkPalm0008_S.png.meta` | 2340 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkPalm0008_S_1.png` | 1646357 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkPalm0008_S_1.png.meta` | 2342 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkPalm0014_1_seamless_S.png` | 1128303 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkPalm0014_1_seamless_S.png.meta` | 2351 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkPine0007_1_S.png` | 1021005 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkPine0007_1_S.png.meta` | 2342 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkPine0018_1_S.png` | 1476827 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkPine0018_1_S.png.meta` | 2342 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkTropical0022_2_S.png` | 1306487 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BarkTropical0022_2_S.png.meta` | 2346 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BronzeCopper0097_2_M.png` | 2970418 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_BronzeCopper0097_2_M.png.meta` | 2346 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_Moss0030_S.png` | 1668874 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_Moss0030_S.png.meta` | 2336 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_PineBark2_512_albedo.tif` | 1575228 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_PineBark2_512_albedo.tif.meta` | 2061 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_PineBark2_512_normal.tif` | 1575228 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_PineBark2_512_normal.tif.meta` | 2061 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_WoodPlanksOld0072_4_seamless_S.png` | 1453341 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/TexturesCom_WoodPlanksOld0072_4_seamless_S.png.meta` | 2356 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/birch-bark-26035360.png` | 226147 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/birch-bark-26035360.png.meta` | 2333 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/depositphotos_3085431-stock-photo-birch-tree-texture.png` | 947493 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/depositphotos_3085431-stock-photo-birch-tree-texture.png.meta` | 2366 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/depositphotos_53485557-stock-photo-birch-bark-texture-abstract-in.png` | 940892 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/TreePackVol.1/Textures/Textures/Trunk/depositphotos_53485557-stock-photo-birch-bark-texture-abstract-in.png.meta` | 2379 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Assets/_Game/Art/NPC/GuideNpc_FittedBelt.asset` | 9165 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| untracked | `Assets/_Game/Art/NPC/GuideNpc_FittedBelt.asset.meta` | 188 | .meta files required by production assets | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| untracked | `Assets/_Game/Art/NPC/GuideNpc_FittedCape.asset` | 11945 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| untracked | `Assets/_Game/Art/NPC/GuideNpc_FittedCape.asset.meta` | 188 | .meta files required by production assets | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| untracked | `Assets/_Game/Art/NPC/GuideNpc_FittedOutfit.asset` | 1893602 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| untracked | `Assets/_Game/Art/NPC/GuideNpc_FittedOutfit.asset.meta` | 188 | .meta files required by production assets | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| untracked | `Assets/_Game/Editor/ForestBridgePlacement.cs` | 1473 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| untracked | `Assets/_Game/Editor/ForestBridgePlacement.cs.meta` | 59 | .meta files required by production assets | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| untracked | `Assets/_Game/Editor/GuideNpcOutfitBuilder.cs` | 15177 | Must ship with Sprint 5.5 | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| untracked | `Assets/_Game/Editor/GuideNpcOutfitBuilder.cs.meta` | 59 | .meta files required by production assets | Final guide outfit/rig/idle, bridge bank alignment and prototype cleanup; required by production prefab or authoring code. |
| untracked | `Assets/_Game/Editor/WorldScaleReview.cs` | 3020 | Tests needed to protect implementation | Optional screenshot capture menu used by existing production smoke tests. |
| untracked | `Assets/_Game/Editor/WorldScaleReview.cs.meta` | 59 | Tests needed to protect implementation | Optional screenshot capture menu used by existing production smoke tests. |
| untracked | `Documentation/Sprint55C2/Screenshots/10_npc_scale.png` | 583239 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `Documentation/Sprint55C2/Screenshots/11_bridge_banks.png` | 592805 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `Documentation/Sprint55C2/Screenshots/12_npc_outfit_front.png` | 615209 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `Documentation/Sprint55C2/Screenshots/13_npc_outfit_back.png` | 469806 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `Documentation/Sprint55C2/Screenshots/14_npc_staff_grip.png` | 660016 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/Superhero_Female_FullBody.bin` | 990808 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/Superhero_Female_FullBody.gltf` | 31656 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/Superhero_Male_FullBody.bin` | 720076 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/Superhero_Male_FullBody.gltf` | 30989 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Eye_Brown.png` | 35958 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Eye_Normal.png` | 15099 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Hair_1_BaseColor.png` | 1570376 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Hair_1_BaseColor_png.png` | 1570376 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Hair_1_Normal.png` | 4326126 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Hair_2_BaseColor.png` | 1729467 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Hair_2_BaseColor_png.png` | 1729467 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Hair_2_Normal.png` | 4710013 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Superhero_Female_Dark_BaseColor.png` | 1319410 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Superhero_Female_Normal.png` | 3940537 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Superhero_Female_Roughness.png` | 3277022 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Superhero_Male_Dark.png` | 1384380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Superhero_Male_Normal.png` | 4252937 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Godot - UE/T_Superhero_Male_Roughness.png` | 3152049 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/Normals Unity - Godot/T_Eye_Normal.png` | 15099 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/Normals Unity - Godot/T_Hair_1_Normal.png` | 4326126 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/Normals Unity - Godot/T_Hair_2_Normal.png` | 4710013 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/Normals Unity - Godot/T_Superhero_Female_Normal.png` | 3940537 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/Normals Unity - Godot/T_Superhero_Male_Normal.png` | 4252937 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Eye_Brown.png` | 35958 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Eye_Normal.png` | 15097 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Hair_1_BaseColor.png` | 1570376 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Hair_1_Normal.png` | 3770683 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Hair_2_BaseColor.png` | 1729467 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Hair_2_Normal.png` | 4109628 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Superhero_Female_Dark_BaseColor.png` | 1319410 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Superhero_Female_Light_BaseColor.png` | 1317541 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Superhero_Female_Normal.png` | 3420584 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Superhero_Female_Roughness.png` | 1997719 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Superhero_Male_Dark.png` | 1384380 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Superhero_Male_Ligh.png` | 1389170 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Superhero_Male_Normal.png` | 3687166 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Textures/T_Superhero_Male_Roughness.png` | 1912686 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Unity/Superhero_Female_FullBody.fbx` | 1006508 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Unity/Superhero_Male_FullBody.fbx` | 826876 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Base Characters/Unreal-Engine-README.txt` | 289 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unity)/Eyebrows_Female.fbx` | 73596 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unity)/Eyebrows_Regular.fbx` | 47132 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unity)/Hair_Beard.fbx` | 48444 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unity)/Hair_Buns.fbx` | 135388 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unity)/Hair_Buzzed.fbx` | 45036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unity)/Hair_BuzzedFemale.fbx` | 43004 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unity)/Hair_Long.fbx` | 125036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unity)/Hair_SimpleParted.fbx` | 62668 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unreal Engine)/Eyebrows_Female.fbx` | 73548 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unreal Engine)/Eyebrows_Regular.fbx` | 47276 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unreal Engine)/Hair_Beard.fbx` | 48540 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unreal Engine)/Hair_Buns.fbx` | 134476 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unreal Engine)/Hair_Buzzed.fbx` | 44716 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unreal Engine)/Hair_BuzzedFemale.fbx` | 43036 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unreal Engine)/Hair_Long.fbx` | 123612 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/FBX (Unreal Engine)/Hair_SimpleParted.fbx` | 61932 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Eyebrows_Female.bin` | 67248 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Eyebrows_Female.gltf` | 2855 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Eyebrows_Regular.bin` | 26576 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Eyebrows_Regular.gltf` | 2018 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_Beard.bin` | 25276 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_Beard.gltf` | 2005 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_Buns.bin` | 131768 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_Buns.gltf` | 2243 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_Buzzed.bin` | 19892 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_Buzzed.gltf` | 2006 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_BuzzedFemale.bin` | 19892 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_BuzzedFemale.gltf` | 2191 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_Long.bin` | 115996 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_Long.gltf` | 2056 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_SimpleParted.bin` | 32032 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/Hair_SimpleParted.gltf` | 2010 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/T_Hair_1_BaseColor.png` | 1570376 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/T_Hair_1_Normal.png` | 4326126 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/T_Hair_2_BaseColor.png` | 1729467 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Origin at 0/glTF (Godot)/T_Hair_2_Normal.png` | 4710013 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/FBX (Unity)/Eyebrows_Female.fbx` | 194428 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/FBX (Unity)/Eyebrows_Regular.fbx` | 168636 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/FBX (Unity)/Hair_Beard.fbx` | 169804 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/FBX (Unity)/Hair_Buns.fbx` | 259420 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/FBX (Unity)/Hair_Buzzed.fbx` | 165884 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/FBX (Unity)/Hair_BuzzedFemale.fbx` | 164492 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/FBX (Unity)/Hair_Long.fbx` | 249404 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/FBX (Unity)/Hair_SimpleParted.fbx` | 181628 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Eyebrows_Female.bin` | 100592 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Eyebrows_Female.gltf` | 25053 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Eyebrows_Regular.bin` | 43656 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Eyebrows_Regular.gltf` | 23117 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_Beard.bin` | 41356 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_Beard.gltf` | 23104 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_Buns.bin` | 205916 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_Buns.gltf` | 23807 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_Buzzed.bin` | 33372 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_Buzzed.gltf` | 23108 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_BuzzedFemale.bin` | 33372 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_BuzzedFemale.gltf` | 23779 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_Long.bin` | 181756 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_Long.gltf` | 23813 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_SimpleParted.bin` | 51332 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/Hair_SimpleParted.gltf` | 23114 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/T_Hair_1_BaseColor.png` | 1570376 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/T_Hair_1_Normal.png` | 4326126 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/T_Hair_2_BaseColor.png` | 1729467 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Rigged to Head Bone/glTF (Godot -Unreal)/T_Hair_2_Normal.png` | 4710013 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Textures/Normals Unity - Godot/T_Hair_1_Normal.png` | 4326126 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Textures/Normals Unity - Godot/T_Hair_2_Normal.png` | 4710013 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Textures/T_Hair_1_BaseColor.png` | 1570376 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Textures/T_Hair_1_Normal.png` | 3770683 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Textures/T_Hair_2_BaseColor.png` | 1729467 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Hairstyles/Textures/T_Hair_2_Normal.png` | 4109628 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/License_Standard.txt` | 806 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `Universal Base Characters[Standard]/Preview.png` | 2109533 | Not required by runtime | Raw/unused package content; preserve locally and do not commit unless dependency scan proves required. |
| untracked | `fix-bridge-span-apply.log` | 48930 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-bridge-span-editmode.log` | 91739 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-bridge-span-playmode.log` | 64837 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-bridge-span-validation.log` | 25488 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-npc-bigger-apply-2.log` | 148191 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-npc-bigger-apply.log` | 18128 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-npc-bigger-editmode.log` | 92052 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-npc-bigger-playmode.log` | 64503 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-npc-bigger-validation.log` | 25191 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-npc-readable-apply.log` | 42540 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-npc-readable-editmode.log` | 91784 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-npc-readable-playmode.log` | 66369 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-npc-readable-screenshots.log` | 30026 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-npc-readable-validation.log` | 25534 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-player-obstacle-collision-apply.log` | 160195 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-player-obstacle-editmode.log` | 92649 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-player-obstacle-playmode.log` | 64526 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-player-obstacle-validation.log` | 25676 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-editmode-2.log` | 92093 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-editmode-final.log` | 92103 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-editmode.log` | 92094 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-env-2.log` | 48662 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-env-3.log` | 47521 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-env.log` | 159875 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-npc-2.log` | 35244 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-npc.log` | 35447 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-playmode-2.log` | 64554 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-playmode.log` | 64549 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `fix-scale-bridge-clean-validation.log` | 25201 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `npc-fit-apply.log` | 84436 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `npc-fit-editmode.log` | 93753 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `npc-fit-final-apply.log` | 36936 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `npc-fit-focused.log` | 49576 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `npc-fit-inspect.log` | 56118 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `npc-fit-playmode.log` | 72771 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `npc-fit-validation.log` | 25328 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `world-scale-editmode.log` | 92359 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `world-scale-npc-apply.log` | 35657 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `world-scale-npc-larger-apply.log` | 148514 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `world-scale-playmode.log` | 74174 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |
| untracked | `world-scale-validation.log` | 25296 | Screenshots/XML/logs/review files | Generated local evidence; preserve uncommitted. |

## Ignored local content

Preserve all entries below; generated caches, IDE state, test results and prior review evidence are not closure production changes. Directory entries cover every descendant.

- `Assembly-CSharp-Editor.csproj`
- `Assembly-CSharp.csproj`
- `Documentation/Sprint55D/Apply.log`
- `Documentation/Sprint55D/Baseline/Audit.log`
- `Documentation/Sprint55D/Baseline/EditMode.log`
- `Documentation/Sprint55D/Baseline/Forest.before.unity`
- `Documentation/Sprint55D/Baseline/PlayMode.log`
- `Documentation/Sprint55D/Baseline/Restored/`
- `Documentation/Sprint55D/Baseline/Validation.log`
- `Documentation/Sprint55D/Baseline/user-changes-lf.patch`
- `Documentation/Sprint55D/Baseline/user-changes.patch`
- `Documentation/Sprint55D/Capture.log`
- `Documentation/Sprint55D/EditMode.log`
- `Documentation/Sprint55D/PlayMode.log`
- `Documentation/Sprint55D/PlayReview.log`
- `Documentation/Sprint55D/PlayReview/01_spawn_atmosphere.png`
- `Documentation/Sprint55D/PlayReview/02_forest_depth.png`
- `Documentation/Sprint55D/PlayReview/03_lake_water.png`
- `Documentation/Sprint55D/PlayReview/04_bridge_creek.png`
- `Documentation/Sprint55D/PlayReview/05_light_grove.png`
- `Documentation/Sprint55D/PlayReview/06_heart_garden.png`
- `Documentation/Sprint55D/PlayReview/07_final_hill_distance.png`
- `Documentation/Sprint55D/PlayReview/08_final_camp.png`
- `Documentation/Sprint55D/PlayReview/09_npc_lighting.png`
- `Documentation/Sprint55D/PlayReview/10_fox_forest.png`
- `Documentation/Sprint55D/Validation.log`
- `Documentation/Sprint55D/atmosphere-only.patch`
- `Documentation/Sprint55D/final-git-status.txt`
- `Documentation/Sprint55D/final-head.txt`
- `Documentation/Sprint55E/Baseline/EditMode.log`
- `Documentation/Sprint55E/Baseline/Forest.unity`
- `Documentation/Sprint55E/Baseline/PlayMode.log`
- `Documentation/Sprint55E/Baseline/Validation.log`
- `Documentation/Sprint55E/Baseline/user.patch`
- `Documentation/Sprint55E/EditMode-final.log`
- `Documentation/Sprint55E/EditMode-retry.log`
- `Documentation/Sprint55E/EditMode.log`
- `Documentation/Sprint55E/GameView-retry.log`
- `Documentation/Sprint55E/GameView.log`
- `Documentation/Sprint55E/Placement.log`
- `Documentation/Sprint55E/PlayMode.log`
- `Documentation/Sprint55E/RecordingFrames/`
- `Documentation/Sprint55E/Validation.log`
- `Documentation/Sprint55E1/EditMode.log`
- `Documentation/Sprint55E1/Focused.log`
- `Documentation/Sprint55E1/NormalInputEditor.log`
- `Documentation/Sprint55E1/PlayMode.log`
- `Documentation/Sprint55E1/ResumeDirty.txt`
- `Documentation/Sprint55E1/SprintValidation.log`
- `Documentation/Sprint55E11/Baseline.log`
- `Documentation/Sprint55E11/BaselineBindings.log`
- `Documentation/Sprint55E11/CloseRangeBaseline.log`
- `Documentation/Sprint55E11/EditMode.log`
- `Documentation/Sprint55E11/Focused.log`
- `Documentation/Sprint55E11/Forest.before.unity`
- `Documentation/Sprint55E11/NativeEditor.log`
- `Documentation/Sprint55E11/PlayMode.log`
- `Documentation/Sprint55E11/SprintValidation.log`
- `Documentation/Sprint55E2/Logs/`
- `Library/`
- `Logs/`
- `TilkiOyunu.Atmosphere.Tests.csproj`
- `TilkiOyunu.Foundation.PlayModeTests.csproj`
- `TilkiOyunu.Foundation.Tests.csproj`
- `TilkiOyunu.Foundation.csproj`
- `TilkiOyunu.sln`
- `UserSettings/`
- `fix-bridge-span-editmode-results.xml`
- `fix-bridge-span-playmode-results.xml`
- `fix-npc-bigger-editmode-results.xml`
- `fix-npc-bigger-playmode-results.xml`
- `fix-npc-readable-editmode-results.xml`
- `fix-npc-readable-playmode-results.xml`
- `fix-player-obstacle-editmode-results.xml`
- `fix-player-obstacle-playmode-results.xml`
- `fix-scale-bridge-clean-editmode-2-results.xml`
- `fix-scale-bridge-clean-editmode-final-results.xml`
- `fix-scale-bridge-clean-editmode-results.xml`
- `fix-scale-bridge-clean-playmode-2-results.xml`
- `fix-scale-bridge-clean-playmode-results.xml`
- `npc-fit-editmode-results.xml`
- `npc-fit-focused-results.xml`
- `npc-fit-playmode-results.xml`
- `sprint4-builder-1.log`
- `sprint4-builder-2.log`
- `sprint4-editmode-2.log`
- `sprint4-editmode-final.log`
- `sprint4-editmode-final2.log`
- `sprint4-editmode-final3.log`
- `sprint4-editmode-rerun.log`
- `sprint4-editmode-results.xml`
- `sprint4-editmode.log`
- `sprint4-playmode-final.log`
- `sprint4-playmode-results.xml`
- `sprint4-playmode.log`
- `sprint4-validation-1.log`
- `sprint4-validation-2.log`
- `sprint4-validation-3.log`
- `sprint4-validation-4.log`
- `sprint4-validation-5.log`
- `sprint4-validation-final.log`
- `sprint5-builder-2.log`
- `sprint5-builder-3.log`
- `sprint5-builder-4.log`
- `sprint5-builder-5.log`
- `sprint5-builder.log`
- `sprint5-editmode-2.log`
- `sprint5-editmode-results.xml`
- `sprint5-editmode.log`
- `sprint5-playmode-2.log`
- `sprint5-playmode-results.xml`
- `sprint5-playmode.log`
- `sprint5-prefix-builder.log`
- `sprint5-prefix-editmode-final.log`
- `sprint5-prefix-editmode-final2.log`
- `sprint5-prefix-editmode-trimmed.log`
- `sprint5-prefix-editmode.log`
- `sprint5-prefix-playmode-2.log`
- `sprint5-prefix-playmode-3.log`
- `sprint5-prefix-playmode-4.log`
- `sprint5-prefix-playmode-5.log`
- `sprint5-prefix-playmode-6.log`
- `sprint5-prefix-playmode-final.log`
- `sprint5-prefix-playmode-final2.log`
- `sprint5-prefix-playmode-trimmed.log`
- `sprint5-prefix-playmode.log`
- `sprint5-prefix-validation-final.log`
- `sprint5-prefix-validation-final2.log`
- `sprint5-prefix-validation-trimmed.log`
- `sprint5-prefix-validation.log`
- `sprint5-validation-2.log`
- `sprint5-validation.log`
- `sprint55-after-editmode-results.xml`
- `sprint55-after-editmode.log`
- `sprint55-after-playmode-results.xml`
- `sprint55-after-playmode.log`
- `sprint55-after-validation.log`
- `sprint55-apply-2.log`
- `sprint55-apply.log`
- `sprint55-baseline-editmode-results.xml`
- `sprint55-baseline-editmode-x86.log`
- `sprint55-baseline-editmode.log`
- `sprint55-baseline-playmode-results.xml`
- `sprint55-baseline-playmode.log`
- `sprint55-baseline-validation.log`
- `sprint55-fixes-apply.log`
- `sprint55-fixes-editmode-results.xml`
- `sprint55-fixes-editmode.log`
- `sprint55-fixes-playmode-results.xml`
- `sprint55-fixes-playmode.log`
- `sprint55-fixes-validation.log`
- `sprint55-fox-report.log`
- `sprint55a1-apply-2.log`
- `sprint55a1-apply-3.log`
- `sprint55a1-apply-4.log`
- `sprint55a1-apply.log`
- `sprint55a1-baked-apply-2.log`
- `sprint55a1-baked-apply.log`
- `sprint55a1-editmode-2.log`
- `sprint55a1-editmode-3.log`
- `sprint55a1-editmode-4.log`
- `sprint55a1-editmode-5.log`
- `sprint55a1-editmode-results.xml`
- `sprint55a1-editmode.log`
- `sprint55a1-playmode-2.log`
- `sprint55a1-playmode-3-results.xml`
- `sprint55a1-playmode-3.log`
- `sprint55a1-playmode-results.xml`
- `sprint55a1-playmode.log`
- `sprint55a1-reimport-avatar-2.log`
- `sprint55a1-reimport-avatar.log`
- `sprint55a1-validation.log`
- `sprint55a2-apply.log`
- `sprint55a2-editmode-results.xml`
- `sprint55a2-editmode.log`
- `sprint55a2-playmode-results.xml`
- `sprint55a2-playmode.log`
- `sprint55a2-validation.log`
- `sprint55c-apply-2.log`
- `sprint55c-apply-3.log`
- `sprint55c-apply-4.log`
- `sprint55c-apply-5.log`
- `sprint55c-apply-6.log`
- `sprint55c-apply.log`
- `sprint55c-editmode-final-results.xml`
- `sprint55c-editmode-final.log`
- `sprint55c-editmode-results.xml`
- `sprint55c-editmode.log`
- `sprint55c-playmode-final-results.xml`
- `sprint55c-playmode-final.log`
- `sprint55c-playmode-results.xml`
- `sprint55c-playmode.log`
- `sprint55c-screenshots-2.log`
- `sprint55c-screenshots-3.log`
- `sprint55c-screenshots-4.log`
- `sprint55c-screenshots.log`
- `sprint55c-validation-final.log`
- `sprint55c-validation.log`
- `sprint55c1-editmode-2.log`
- `sprint55c1-editmode-results.xml`
- `sprint55c1-editmode.log`
- `sprint55c1-environment-apply-2.log`
- `sprint55c1-environment-apply.log`
- `sprint55c1-fox-apply.log`
- `sprint55c1-playmode-results.xml`
- `sprint55c1-playmode.log`
- `sprint55c1-screenshots.log`
- `sprint55c1-validation.log`
- `sprint55c2-compile.log`
- `sprint55c2-editmode-results.xml`
- `sprint55c2-editmode.log`
- `sprint55c2-environment-apply-2.log`
- `sprint55c2-environment-apply.log`
- `sprint55c2-fox-apply.log`
- `sprint55c2-guide-apply.log`
- `sprint55c2-playmode-results.xml`
- `sprint55c2-playmode.log`
- `sprint55c2-screenshots.log`
- `sprint55c2-validation.log`
- `tmp-import-2.log`
- `tmp-import-3.log`
- `tmp-import-4.log`
- `tmp-import-5.log`
- `tmp-import-6.log`
- `tmp-import-cli.log`
- `tmp-import.log`
- `world-scale-editmode-results.xml`
- `world-scale-playmode-results.xml`

## Asset checks

- Duplicate GUIDs: 0. Unresolved serialized GUIDs after package lookup: 5. Tracked missing metadata: 0. Orphan tracked metadata: 0.
- No Git LFS policy is configured. Existing `.gitattributes` uses Unity text serialization and binary media rules; keep it unchanged. Do not add raw archives or duplicated source formats.
- Full initial binary patch, SHA-256 hashes and machine-readable inventory are preserved under ignored `Logs/Sprint55Closure/`.
- Universal Animation Library is absent; production uses the documented project-owned guide idle. No license is invented.


## Reviewed production delta and dependency findings

- Seven initially modified production files plus five new production assets/helpers and their metadata complete the existing guide/bridge work. Two modified test files and the optional WorldScaleReview helper protect it. TMP fallback is the one unrelated tracked change and remains untouched.
- Forest serialized-object review: rebuilt bridge children plus two approach ramps; removed six garden prototype objects; four existing transforms and two capsule colliders changed. Quest/save IDs and gameplay scripts are unchanged by this pending patch. No unrelated scene patch was identified.
- All production assets have metadata; no duplicate GUIDs or orphan tracked metadata were found. Only the three fitted guide meshes were newly referenced untracked assets. The two other new authoring helpers are required by existing modified builders. No untracked TreePack or raw Base Characters file is referenced.
- Five initial GUID scan hits were reviewed: four are internal audio mixer exposed-parameter identifiers, not missing assets. The fifth was an absent optional metallic/specular texture in original M_Fox.mat. Closure clears those two unused slots; production ToonFox_URP remains unchanged.
- The 216 already-tracked TreePack files serve historical authoring/validation; the production scene no longer contains TreeCollectionForest. No additional TreePack import is needed.
- Largest pending new production asset is GuideNpc_FittedOutfit.asset (1,893,602 bytes); no new FBX duplicate or archive is staged. Largest existing file is about 23 MB. An existing duplicate TreePack leaf texture is retained to avoid unrelated asset cleanup.
- Added closure-only development build/asset validation helper checks enabled scene ordering, missing component references/scripts/prefabs and material/shader presence, then builds Windows x64 with BuildOptions.Development into ignored Builds. This is validation infrastructure, not Sprint 6 work.
- ASSET_NOTES now records the fitted derivatives, historical TreePack provenance without inventing a license, absent animation library, and project-owned UI/geometry.
- GitHub repository: authenticated account mberatkaya; no protection/rulesets, statuses, check runs, workflow runs or required reviews on the parent/main branches at initial inspection. Required remote checks are therefore not configured (not described as a passing CI run). Existing convention is a merge commit; automatic branch deletion is disabled.
- Baseline patch/hash inventory remains in ignored Logs/Sprint55Closure. Initial individual file counts include 10 modified + 662 untracked; there were 301 collapsed short-status entries. The initial categories sum to 672, with .meta split out as shown above. No initially dirty documentation file existed.
