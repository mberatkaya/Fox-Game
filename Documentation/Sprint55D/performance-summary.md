# Performance observations

Windows Editor Play Mode; RTX 4070 Laptop GPU / i7-13700H; 1600 x 900 HDR camera target. Each view: 60 warmup frames, then 180 samples; VSync off. Times include editor overhead. These are camera-render samples, not standalone-player benchmarks.

| View | FX on mean ms | FX off mean ms | FX on p95 ms | FX on max ms | FX on mean FPS |
|---|---:|---:|---:|---:|---:|
| 01_spawn_atmosphere | 1.959 | 1.749 | 2.334 | 2.648 | 510.4 |
| 02_forest_depth | 1.599 | 1.565 | 2.079 | 2.500 | 625.2 |
| 03_lake_water | 1.319 | 1.044 | 1.625 | 1.937 | 758.3 |
| 04_bridge_creek | 1.647 | 1.572 | 2.082 | 2.462 | 607.1 |
| 05_light_grove | 1.623 | 1.424 | 2.016 | 2.370 | 616.2 |
| 06_heart_garden | 1.645 | 1.330 | 1.997 | 2.353 | 607.9 |
| 07_final_hill_distance | 1.493 | 1.331 | 1.921 | 2.491 | 669.8 |
| 08_final_camp | 1.508 | 1.152 | 2.243 | 3.395 | 663.3 |
| 09_npc_lighting | 1.795 | 1.531 | 2.228 | 2.423 | 557.2 |
| 10_fox_forest | 1.222 | 1.043 | 1.576 | 2.083 | 818.0 |

FX-on mean frame times: 1.222-1.959 ms. Largest sampled FX-on frame: 3.395 ms. Mean on/off difference across views: 0.207 ms. This paired observation includes normal run-to-run variance.

PASS within these sampled views: no sample near the 33.3 ms / 30 FPS hotspot threshold. Worst mean view: Spawn. No baseline FPS run was recorded before implementation, so no quantitative before/after whole-sprint regression claim is made.

Lighting budget: one 2048-pixel directional shadow map, two cascades, 65 m distance; zero shadow-casting local lights; zero new lights. Existing small-radius quest point lights retained. Bloom uses low-cost filtering; water adds no render pass.

Camera cuts, image readback/PNG encoding and cold shader compilation are outside warmed sample windows. A normal gameplay walkthrough remains part of user acceptance; no minimum-spec or standalone-build result is implied.
