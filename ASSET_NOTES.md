# Asset Notes

Date Downloaded: 2026-09-07

## Unity Asset Store - Toon Fox

Asset Name: Toon Fox
Author: Pxltiger
Official Source: Unity Asset Store
Source Page: https://assetstore.unity.com/packages/3d/characters/animals/toon-fox-183005
Unity Asset Store Package ID: 183005
Package Version: 1.0
Original Unity Version: 2019.4.10
License: Standard Unity Asset Store EULA
Files Imported: `Assets/Fox/FBXs/Fox.fbx`, `Assets/Fox/Prefabs/Fox.prefab`, `Assets/Fox/Animations/*.fbx`, `Assets/Fox/Materials/M_Fox.mat`, `Assets/Fox/Textures/T_Fox_BC.png`, `T_Fox_Normal.png`, `T_Fox_AO.png`
Usage: Player character visual and animations under `PlayerFox/VisualRoot/ToonFox`
Modified: Package source files are left in their imported `Assets/Fox` structure. Game-specific wrapper assets are `Assets/_Game/Prefabs/Characters/ToonFoxVisual.prefab`, `Assets/_Game/Art/Characters/FoxAnimatorController.controller`, and `Assets/_Game/Art/Characters/ToonFox_URP.mat`.
Attribution Requirements: Follow the Standard Unity Asset Store EULA.
Notes: Production locomotion uses `Fox_Idle`, `Fox_Walk_InPlace`, and `Fox_Run_InPlace`. Air state uses the included `Fox_Jump_InAir` clip. Root motion is disabled on the PlayerFox production animator; movement remains owned by `FoxController` and `CharacterController`.

## OpenGameArt - Fox (Not Used In Production)

Asset Name: Fox
Author: br-n518
Official Source: OpenGameArt
Source Page: https://opengameart.org/content/fox-0
License: CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/
Date Downloaded: 2026-09-08
Files Imported: `Assets/ThirdParty/br-n518/Fox/fox.blend`, `Assets/ThirdParty/br-n518/Fox/Fox_br-n518.fbx`, `Assets/ThirdParty/br-n518/Fox/fox_diffuse.png`, `Assets/ThirdParty/br-n518/Fox/fox_normal.png`
Usage: NOT USED IN PRODUCTION. Superseded by Pxltiger Toon Fox during Sprint 5.5-A.2.
Modified: Previously exported `fox.blend` to `Fox_br-n518.fbx` with Blender 4.5.0 portable for Unity import; previous Unity material wrapper was `Assets/_Game/Art/Characters/OpenGameArtFox_URP.mat`.
Attribution Requirements: None required by CC0
Notes: Retained only as historical Sprint 5.5-A.1 fallback documentation unless the abandoned fallback files are removed.

## Quaternius - Ultimate Animated Animal Pack / Fox (Superseded)

Asset Name: Ultimate Animated Animal Pack
Author: Quaternius
Official Source: Quaternius website
Source Page: https://quaternius.com/packs/ultimateanimatedanimals.html
License: CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/ and included `License.txt`
Files Imported: `Assets/ThirdParty/Quaternius/UltimateAnimatedAnimals/Fox/Fox.fbx`, `License.txt`
Usage: Player fox visual and locomotion animation clips
Modified: No source asset edits; Unity import metadata generated
Notes: Superseded by the OpenGameArt br-n518 fox during Sprint 5.5-A.1, then by Pxltiger Toon Fox during Sprint 5.5-A.2 after the official Asset Store package was imported.

## Quaternius - Ultimate Stylized Nature Pack

Asset Name: Ultimate Stylized Nature Pack
Author: Quaternius
Official Source: Quaternius website
Source Page: https://quaternius.com/packs/ultimatestylizednature.html
License: CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/ and included `License.txt`
Files Imported: selected FBX files for BirchTree, MapleTree, NormalTree, PineTree, Bush, Grass, Flower clumps, Rock variants, plus selected texture PNGs
Usage: Non-colliding Forest production visual layer under `Environment_Visuals`
Modified: No source asset edits; Unity import metadata generated
Notes: Only a small subset is imported to preserve readability and avoid repository bloat.

## Quaternius - Stylized Nature MegaKit Standard

Asset Name: Stylized Nature MegaKit Standard
Author: Quaternius
Official Source: Quaternius website
Source Page: https://quaternius.com/packs/stylizednaturemegakit.html
Download Page Used: https://opengameart.org/content/stylized-nature-megakit
License: Creative Commons CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/ and included `License_Standard.txt`
Date Downloaded: 2026-09-11
Files Imported: selected FBX files only under `Assets/ThirdParty/Quaternius/StylizedNatureMegaKit/`: `CommonTree_1`-`CommonTree_5`, `Pine_1`-`Pine_5`, `TwistedTree_1`-`TwistedTree_5`, `DeadTree_1`, `DeadTree_2`, `Bush_Common`, `Bush_Common_Flowers`, `Fern_1`, `Flower_3_Group`, `Flower_4_Group`, `Grass_Common_Short`, `Grass_Common_Tall`, `Grass_Wispy_Short`, `Grass_Wispy_Tall`, `Plant_1`, `Plant_1_Big`, `Plant_7`, `Plant_7_Big`, `Clover_1`, `Clover_2`, `Mushroom_Common`, `Mushroom_Laetiporus`, `Rock_Medium_1`-`Rock_Medium_3`, selected pebble and rock path FBX files, and `License_Standard.txt`.
Usage: Primary Sprint 5.5-C production forest dressing: tree clusters, natural boundaries, shoreline rocks, creek reeds/plants, meadow flowers, Heart Garden flowers, Light Grove understory, and terrain grass detail prototypes.
Modified: Source FBX files are unedited. Game-specific URP materials, terrain layers, deterministic placement, bridge visuals, and wrapper scene organization are project-owned under `Assets/_Game/Art/Environment/Sprint55C` and `World/Environment`.
Notes: The official itch download flow rate-limited this environment, so the Standard zip was downloaded from OpenGameArt where the uploader is `quaternius` and the page links back to the official Quaternius source page. The raw zip was not committed; only the selected production subset was imported.

## Kenney - UI Pack

Asset Name: UI Pack
Author: Kenney
Official Source: Kenney website
Source Page: https://kenney.nl/assets/ui-pack
License: Creative Commons CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/ and included `License.txt`
Files Imported: selected green/yellow button PNGs and `License.txt`
Usage: Available for warm UI skin/decorative treatment
Modified: No source asset edits
Notes: The full 430-file pack is not committed.

## Kenney - Interface Sounds

Asset Name: Interface Sounds
Author: Kenney
Official Source: Kenney website
Source Page: https://kenney.nl/assets/interface-sounds
License: Creative Commons CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/ and included `License.txt`
Files Imported: `click_001.ogg`, `open_001.ogg`, `close_001.ogg`, `License.txt`
Usage: UI click/open/close and soft negative feedback
Modified: No source asset edits

## OpenGameArt - Sunset Walk / Ambient / Quiet / Sweet / Loop

Asset Name: Sunset Walk / Ambient / Quiet / Sweet / Loop
Author: KiluaBoy
Official Source: OpenGameArt
Source Page: https://opengameart.org/content/sunset-walk-ambient-quiet-sweet-loop
License: CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/
Files Imported: `Assets/ThirdParty/OpenGameArt/Music/SunsetWalk.ogg`
Usage: Low-volume background music loop
Modified: No source asset edits

## OpenGameArt - Forest Ambience

Asset Name: Forest Ambience
Author: TinyWorlds
Official Source: OpenGameArt
Source Page: https://opengameart.org/content/forest-ambience
License: CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/
Files Imported: `Assets/ThirdParty/OpenGameArt/Ambience/Forest_Ambience.mp3`
Usage: Low-volume forest ambience loop
Modified: No source asset edits

## OpenGameArt - Different steps on wood, stone, leaves, gravel and mud

Asset Name: Different steps on wood, stone, leaves, gravel and mud
Author: TinyWorlds
Official Source: OpenGameArt
Source Page: https://opengameart.org/content/different-steps-on-wood-stone-leaves-gravel-and-mud
License: CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/
Files Imported: `leaves01.ogg`, `leaves02.ogg`
Usage: Fox footstep cadence SFX
Modified: No source asset edits

## OpenGameArt - Bell dings/chimes

Asset Name: Bell dings/chimes
Author: PWL
Official Source: OpenGameArt
Source Page: https://opengameart.org/content/bell-dingschimes
License: CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/
Files Imported: `bell_ding1.wav`, `bell_ding2.wav`, `bell_ding3.wav`
Usage: Memory pickup, Light Path, card match, and final completion chimes
Modified: No source asset edits

## OpenGameArt - Playing Card Sounds

Asset Name: Playing Card Sounds
Author: BMacZero / Brian MacIntosh
Official Source: OpenGameArt
Source Page: https://opengameart.org/content/playing-card-sounds
License: CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/
Files Imported: `shuffle.wav`, `contact1.wav`, `contact2.wav`, `cut.wav`
Usage: Card Matching open/reveal/contact feedback
Modified: No source asset edits

## OpenGameArt - Fireplace Sound loop

Asset Name: Fireplace Sound loop
Author: PagDev
Official Source: OpenGameArt
Source Page: https://opengameart.org/content/fireplace-sound-loop
License: CC0
License URL/reference: https://creativecommons.org/publicdomain/zero/1.0/
Files Imported: `fire.wav`
Usage: 3D final camp campfire loop
Modified: No source asset edits
