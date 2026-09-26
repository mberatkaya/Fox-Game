# Sprint 5.5-E.2 — Card Matching and user settings

Branch: `feature/sprint-5-5-world-character-overhaul`.
Starting HEAD: `194a8265ac398f144dbcf54a8f463b3a8e8045f1`.
The complete initial dirty/untracked inventory is in [starting-status.txt](starting-status.txt).
Existing Forest, NPC, environment, font, smoke-test and third-party changes are excluded from the commits for this task.
PR #7 is not merged. Sprint 6 is not started.

## Card Matching root cause

`CardMatchingController.HideMismatchedSelection` emitted `CardChanged` while `isResolvingMismatch` was still true and the first/second selection indices were still assigned. `CardMatchingPanelUI.HandleCardChanged` synchronously called `RefreshInteractivity`, which queried `CanReveal` for every card. Every query returned false. The controller subsequently cleared its flags, but emitted no further notification. The controller could accept programmatic reveals, while the actual UI buttons remained disabled. Older controller-only tests missed this distinction.

The fix commits both Hidden states, clears both selection indices and releases resolving state **before** notifying views. All buttons are refreshed after that complete transition. The mismatch start also publishes its input lock, so third/fourth buttons are visibly disabled. The existing 0.85-second delay uses unscaled time, so pausing scaled time cannot strand the coroutine. Normal completion clears the coroutine handle before publishing; cancellation stops the outstanding coroutine. No minigame restart is used to resolve a mismatch.

Same-card repeats are still rejected by `CanReveal`; matched cards remain matched and disabled. Four pairs, eight cards, quest ID, pair progress, completion callback and turn-in/save semantics are unchanged. Closing an unfinished panel retains the previous cancel-run behavior, stops its coroutine and releases only its own movement lock; reopening starts a clean playable run. Disabling the panel now cancels the run too. Camera look restores the previously captured state, including a prior owner's lock. Cursor state is restored. Labels use centered, single-line auto sizing (14–30).

The new stress test operates the production `Button` components through ten real timed mismatch/reset cycles, rejects same-card and rapid third selection, then completes all pairs with an additional mismatch after a match. A separate test closes/disables during the timer, waits beyond the abandoned timer, reopens, and checks cursor and camera ownership, including an existing lock owner.

## Settings architecture and menu integration

`GameSettingsData` defines version 1, explicit defaults and range/finite-value normalization. `SettingsService` returns copies for editing and atomically replaces `tilki-user-settings.json` in `Application.persistentDataPath`. Missing, malformed and unsupported-version data fall back to explicit defaults. Save failures retain the previous committed settings. This file is independent of `tilki-oyunu-save.json`; new-game/save deletion/final progression cannot reset user preferences.

`SettingsRuntime` belongs to the persistent `GameBootstrap`. It reapplies preferences when scenes load, updates the existing mixer, uses the existing InputActionAsset, and owns a temporary URP asset copy and a dedicated high-priority post-exposure volume. It restores shared input/quality state on teardown. `Resources/TilkiSettings.asset` references the existing input asset, so keyboard settings are available before gameplay starts. No duplicate enabled gameplay action system is introduced; the disabled binding draft exists only for editing overrides.

There was no main/pause menu in the production Bootstrap→Forest path. Bootstrap now opens a main menu containing Devam Et, Yeni Oyun, Ayarlar and Çıkış. Existing save data enables Devam Et; overwriting a save through Yeni Oyun requests confirmation. The new pause menu contains Devam Et, Ayarlar and Ana Menü. Both Ayarlar entries open the same `SettingsMenuUI` and service. Tests explicitly opt into direct gameplay startup in the existing suite fixture; gameplay assertions were not removed or weakened.

All edits, including audio and key bindings, stay in a working copy until **Uygula**. **Geri** discards that copy. **Varsayılana Döndür** fills the draft with defaults, and Uygula saves them. Display changes are proposed before committing anything; **Evet** commits, **Geri Al**, Escape, disable or 12-second timeout restores the actual previous display mode/resolution. `ISettingsDisplay` allows testing this transaction without changing the automated editor's resolution.

Pause acquires the existing counted gameplay lock, captures camera/cursor/time-scale state, and restores its prior ownership on resume. Settings opened from Pause return to the still-locked Pause menu. Escape is consumed per frame. Settings/rebind/confirmation take precedence, then the existing world map or card panel, then Pause. Dialogue and other locked gameplay do not acquire a competing pause layer. World-map actions now reference the original asset instead of stale clones, so rebinding applies to M/map and Pause too.

## Every visible setting

All editable values persist in the dedicated JSON on Apply; the language line is informational.

| Setting | Type / values | Default | Runtime effect | Persistent |
|---|---|---|---|---|
| Dil | Static label | Türkçe | Truthfully identifies the existing language | Fixed |
| Mini Harita | Açık / Kapalı | Açık | Hides/shows minimap root; live tracking continues | Yes |
| Görev İşaretçileri | Açık / Kapalı | Açık | Hides optional map/minimap target icons; player, quest HUD and quest state remain | Yes |
| Ekran Modu | Tam Ekran / Kenarlıksız Pencere / Pencere | Kenarlıksız | ExclusiveFullScreen / FullScreenWindow / Windowed | Yes, after confirmation |
| Çözünürlük | Unique width × height from Screen.resolutions, plus current | Current display/native resolution | Screen.SetResolution, preserving current refresh-rate ratio | Yes, after confirmation |
| Dikey Eşitleme | Açık / Kapalı | Açık | QualitySettings.vSyncCount 1 / 0 | Yes |
| Kare Hızı Sınırı | 30 / 60 / 120 / 144 / Sınırsız | 60 | Application.targetFrameRate; control disabled with explanatory text when VSync is on | Yes |
| Grafik Kalitesi | Düşük / Orta / Yüksek / Çok Yüksek | Yüksek | Actual Unity LOD/quality and runtime URP budgets; see below | Yes |
| Parlaklık | Slider 80–120 | 100 | Dedicated ColorAdjustments.postExposure, log2(multiplier), about −0.322…+0.263 EV | Yes |
| Ana Ses | Slider 0–100 | 100 | MasterVolume exposed mixer parameter | Yes |
| Müzik | Slider 0–100 | 100 | MusicVolume exposed mixer parameter | Yes |
| Ortam Sesleri | Slider 0–100 | 100 | AmbienceVolume exposed mixer parameter | Yes |
| Efektler | Slider 0–100 | 100 | SFXVolume exposed mixer parameter | Yes |
| Kamera Hassasiyeti | Slider 0.25–1.75 | 1.00 | Multiplies existing camera sensitivity; midpoint preserves approved feel | Yes |
| Dikey Bakışı Ters Çevir | Kapalı / Açık | Kapalı | Reverses look Y before pitch calculation | Yes |
| İleri | Interactive keyboard rebind | W | Existing Move/up composite binding | Yes |
| Geri | Interactive keyboard rebind | S | Existing Move/down composite binding | Yes |
| Sol | Interactive keyboard rebind | A | Existing Move/left composite binding | Yes |
| Sağ | Interactive keyboard rebind | D | Existing Move/right composite binding | Yes |
| Zıpla | Interactive keyboard rebind | Space | Existing Jump action | Yes |
| Koş | Interactive keyboard rebind | Left Shift | Existing Sprint action, hold behavior preserved | Yes |
| Etkileşim | Interactive keyboard rebind | F | Existing Interact action | Yes |
| Duraklat | Interactive keyboard rebind | Escape | Existing Pause action; hard Escape remains an emergency menu exit | Yes |
| Harita | Interactive keyboard rebind | M | Existing WorldMap action, including live map UI | Yes |
| Tuşları Varsayılana Döndür | Button | Original bindings | Removes draft binding overrides; Apply commits | Yes, on Apply |
| Arayüz Ölçeği | Slider 80–120% | 100% | Screen-space CanvasScaler reference resolutions; world-space objects excluded | Yes |
| İşaretçi Boyutu | Küçük / Normal / Büyük | Normal | Map/minimap icon scale 0.8 / 1 / 1.2 | Yes |
| Diyalog Metin Boyutu | Küçük / Normal / Büyük | Normal | Dialogue TMP font limits 0.85 / 1 / 1.15 with auto sizing | Yes |
| Varsayılana Döndür | Button | Explicit defaults above | Resets entire draft, including bindings | Yes, on Apply |

### Graphics and audio details

| Preset | Unity quality index | Render scale | Shadow distance | Main shadow map | MSAA |
|---|---:|---:|---:|---:|---:|
| Düşük | 1 | 75% | 25 m | 512 | Off |
| Orta | 2 | 85% | 45 m | 1024 | Off |
| Yüksek | 5 (approved project baseline) | 100% | 65 m | 2048 | Off |
| Çok Yüksek | 5 | 100% | 85 m | 4096 | 4× |

High matches the existing quality index 5 and URP baseline. The source URP asset is never edited to apply user settings. Brightness does not modify the Directional Light or the atmosphere profile. VSync takes precedence over the saved FPS preference; disabling VSync activates the saved cap.

The existing mixer snapshot has no attenuation overrides (0 dB), and production AudioSource levels provide the existing balance. Accordingly, 100 on the four sliders means **unchanged mixer gain**, not resetting source volumes or increasing the approved mix. Each channel uses `20 * log10(linear)`, clamped to −80 dB at zero. No AudioSource iteration is used. Channel isolation and finite mute values are tested.

Rebinding uses Input System interactive rebinding and binding-override JSON. Escape cancels capture. Conflicts with another Player binding are consistently rejected. The original binding is restored on rejection. No language switching, map-rotation selector, sprint-toggle mode, separate dialogue volume, camera-shake control, HDR/ray-tracing, independent texture dropdown or text-disable switch is advertised. There is no independent dialogue mixer path or supported localization/shake feature to expose. Shadow, render scale and MSAA are deliberately managed by functional quality presets rather than adding redundant controls.

## Verification and evidence

Baseline: EditMode **80/80**, PlayMode **24/24**. Results are preserved as `baseline-*.xml`.
Expanded EditMode: **95/95**. Normal Game View E.2 integration: **7/7**, including actual keyboard/mouse Input System events, real card timers, independent mixer values, graphics/brightness effects, display timeout with a safe adapter, and re-created-service persistence.

All ten requested screenshots are in [Screenshots](Screenshots). These are actual production Game View captures from automated integration scenarios, not mockups. They were visually inspected for fitting labels, readable settings and completed/mismatch card states. The screenshot harness enables only with `-tilkiE2Screenshots`; batch mode does not render them, so the capture run used normal Editor mode.

The existing full suites cover map/minimap tracking, NPC interaction, fox movement/camera/jump, collisions, quest progression, Light Path timing, Final Camp and save/load. No movement speed, quest timing, waffle position, world scale, environment asset, character animation or atmosphere authoring changes are included.

The automated persistence test destroys and recreates the bootstrap/settings service and verifies committed values restore from disk; it does not claim a human-reviewed standalone executable restart. Physical display switching across the user's monitors, listening to the mix, full manual quest playthrough and subjective UI/camera review remain **NEEDS USER REVIEW**. Main-menu and in-game automated integration are **PASS**, with those physical review qualifications.

## Final quality gate

| Check | Baseline | Final | Result |
|---|---:|---:|---|
| Full EditMode | 80/80 | 95/95 | PASS |
| Full PlayMode | 24/24 | 31/31 | PASS |
| E.2 normal Game View integration | — | 7/7 | PASS |
| SprintValidation (`Sprint5Validation.ValidateFromCommandLine`) | — | exit 0 | PASS |
| Main menu and shared settings | — | Automated UI/runtime checks and screenshots | PASS |
| In-game pause/settings/ownership | — | Live Input System events and production scene | PASS |
| Persistence after service/bootstrap restart | — | Exact selected values restored from JSON | PASS |
| Standalone restart and physical display/audio review | — | Human verification still needed | NEEDS USER REVIEW |

Final results are in `final-editmode.xml`, `final-playmode.xml`, `gameview-playmode.xml` and `validation.log`; execution logs are retained in `Logs/`. No existing tests were removed or relaxed. The fixture's direct-gameplay startup and isolated settings path keep tests independent of the new main menu and the user's own preferences.

Commits: `6c5a8f1` fixes card state publication; `1dfc495` adds the shared settings menus, persistence and keyboard rebinding. The following `test(settings): validate card and settings regressions` commit contains the tests and this evidence package; its exact final HEAD is reported in the task response.

At start there were 10 modified tracked files and 291 untracked entries. These unrelated changes remain outside this task's commits. The full inventory is preserved in `starting-status.txt`; the final inventory is in `remaining-status.txt`.

**READY FOR USER SPRINT 5.5-E.2 REVIEW — DO NOT MERGE PR #7**
