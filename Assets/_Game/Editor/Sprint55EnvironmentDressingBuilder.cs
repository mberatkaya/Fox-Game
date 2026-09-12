using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint55EnvironmentDressingBuilder
    {
        public const string MegaKitRoot = "Assets/ThirdParty/Quaternius/StylizedNatureMegaKit";
        public const string EnvironmentMaterialFolder = "Assets/_Game/Art/Environment/Sprint55C";
        public const string ScreenshotFolder = "Documentation/Sprint55C1/Screenshots";
        public const string EnvironmentRootPath = "World/Environment";
        public const string TerrainName = "Sprint55B_PrimaryTerrain";

        private const float TerrainSize = 512f;
        private const int AlphamapResolution = 256;
        private const int DetailResolution = 256;
        private const int DetailResolutionPerPatch = 8;
        private const float TerrainOrigin = -TerrainSize * 0.5f;

        private static readonly string[] TreeAssetNames =
        {
            "CommonTree_1", "CommonTree_2", "CommonTree_3", "CommonTree_4", "CommonTree_5",
            "Pine_1", "Pine_2", "Pine_3", "Pine_4", "Pine_5",
            "TwistedTree_1", "TwistedTree_2", "TwistedTree_3", "TwistedTree_4", "TwistedTree_5",
            "DeadTree_1", "DeadTree_2"
        };

        private static readonly string[] RockAssetNames =
        {
            "Rock_Medium_1", "Rock_Medium_2", "Rock_Medium_3",
            "Pebble_Round_1", "Pebble_Round_2", "Pebble_Round_3", "Pebble_Round_4",
            "Pebble_Square_1", "Pebble_Square_2"
        };

        private static readonly string[] BushAssetNames =
        {
            "Bush_Common", "Bush_Common_Flowers", "Fern_1", "Plant_1", "Plant_1_Big", "Plant_7", "Plant_7_Big",
            "Mushroom_Common", "Mushroom_Laetiporus", "Clover_1", "Clover_2"
        };

        private static readonly string[] FlowerAssetNames =
        {
            "Flower_3_Group", "Flower_4_Group", "Clover_1", "Clover_2"
        };

        private static readonly TreeCluster[] TreeClusters =
        {
            new("SpawnMeadow", new Vector2(0f, -150f), new Vector2(76f, 54f), 22, 0.9f, 1.2f, 29f, 0),
            new("NPCGrove", new Vector2(-18f, -108f), new Vector2(50f, 36f), 16, 0.82f, 1.12f, 17f, 2),
            new("MemoryEast", new Vector2(66f, -60f), new Vector2(66f, 48f), 18, 0.82f, 1.15f, 12f, 5),
            new("MemoryWest", new Vector2(-78f, 18f), new Vector2(72f, 52f), 22, 0.86f, 1.18f, 14f, 8),
            new("CreekApproach", new Vector2(32f, -8f), new Vector2(58f, 38f), 16, 0.8f, 1.05f, 15f, 1),
            new("LakeShore", new Vector2(136f, 56f), new Vector2(96f, 70f), 24, 0.86f, 1.12f, 38f, 4),
            new("LightGrove", new Vector2(-58f, 84f), new Vector2(78f, 58f), 24, 0.92f, 1.24f, 17f, 10),
            new("HeartGarden", new Vector2(-158f, -44f), new Vector2(60f, 50f), 18, 0.84f, 1.12f, 22f, 0),
            new("FinalHill", new Vector2(18f, 145f), new Vector2(84f, 54f), 22, 0.9f, 1.25f, 26f, 5),
            new("NorthBoundary", new Vector2(0f, 218f), new Vector2(210f, 34f), 30, 1.0f, 1.32f, 0f, 6),
            new("WestBoundary", new Vector2(-218f, 0f), new Vector2(34f, 210f), 28, 1.0f, 1.32f, 0f, 9),
            new("EastBoundary", new Vector2(218f, 22f), new Vector2(34f, 188f), 24, 0.96f, 1.26f, 0f, 3),
            new("SouthBoundary", new Vector2(10f, -218f), new Vector2(188f, 32f), 20, 0.94f, 1.22f, 0f, 1)
        };

        [MenuItem("Tilki Oyunu/Sprint 5.5/Apply Environment Dressing")]
        public static void ApplyEnvironmentDressingFromMenu()
        {
            ApplyEnvironmentDressing();
        }

        public static void ApplyEnvironmentDressingFromCommandLine()
        {
            ApplyEnvironmentDressing();
            Debug.Log(BuildEnvironmentReport());
            EditorApplication.Exit(0);
        }

        public static void CaptureEnvironmentReviewScreenshotsFromCommandLine()
        {
            CaptureEnvironmentReviewScreenshots();
            EditorApplication.Exit(0);
        }

        public static void ApplyEnvironmentDressing()
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            EnsureFolder(EnvironmentMaterialFolder);
            AssetDatabase.ImportAsset(MegaKitRoot, ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceSynchronousImport);

            Terrain terrain = RequireTerrain();
            ConfigureTerrainSurfaces(terrain);
            ConfigureTerrainDetails(terrain);

            Transform environment = EnsureEnvironmentHierarchy();
            BuildTreeDressing(environment, terrain);
            BuildRocksAndShoreline(environment, terrain);
            BuildBushesAndFlowers(environment, terrain);
            BuildProductionBridge(environment, terrain);
            ApplyGlareBlockerCorrection();
            DressMemoryVisuals(environment, terrain);
            DressLightPathNodes();
            HidePrototypeVisuals();

            EditorUtility.SetDirty(terrain.terrainData);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Sprint 5.5-C environment dressing applied.");
        }

        public static string BuildEnvironmentReport()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            GameObject environment = GameObject.Find("Environment");
            int treeCount = CountNamedChildren(environment != null ? environment.transform : null, "QuaterniusTree_");
            int rockCount = CountNamedChildren(environment != null ? environment.transform : null, "QuaterniusRock_");
            int plantCount = CountNamedChildren(environment != null ? environment.transform : null, "QuaterniusPlant_");
            int flowerCount = CountNamedChildren(environment != null ? environment.transform : null, "QuaterniusFlower_");
            int treeColliderCount = CountProductionTreeColliders(environment != null ? environment.transform : null);
            int blockingRockCount = CountBlockingRockColliders(environment != null ? environment.transform : null);
            HashSet<string> treeVariants = CollectTreeVariants(environment != null ? environment.transform : null);

            Terrain terrain = GameObject.Find(TerrainName)?.GetComponent<Terrain>();
            int detailPrototypes = terrain != null && terrain.terrainData != null ? terrain.terrainData.detailPrototypes.Length : 0;
            return
                "SPRINT55C_ENVIRONMENT_REPORT_BEGIN\n"
                + $"TreeCount={treeCount}\n"
                + $"TreeVariants={string.Join(", ", treeVariants)}\n"
                + $"RockCount={rockCount}\n"
                + $"PlantCount={plantCount}\n"
                + $"FlowerCount={flowerCount}\n"
                + $"TreeColliderCount={treeColliderCount}\n"
                + $"BlockingRockColliderCount={blockingRockCount}\n"
                + $"TerrainLayers={FormatTerrainLayers(terrain)}\n"
                + $"GrassDetailPrototypes={detailPrototypes}\n"
                + $"Bridge={GameObject.Find("Small Bridge") != null}\n"
                + $"GlareCorrection={BuildGlareReport()}\n"
                + "SPRINT55C_ENVIRONMENT_REPORT_END";
        }

        private static Transform EnsureEnvironmentHierarchy()
        {
            GameObject world = GameObject.Find(Sprint55WorldLayoutBuilder.WorldRootName);
            if (world == null)
            {
                world = new GameObject(Sprint55WorldLayoutBuilder.WorldRootName);
            }

            Transform environment = EnsureChild(world.transform, "Environment").transform;
            EnsureCleanChild(environment, "Trees");
            EnsureCleanChild(environment, "Rocks");
            EnsureCleanChild(environment, "Bushes");
            EnsureCleanChild(environment, "Flowers");
            EnsureCleanChild(environment, "Grass");
            EnsureCleanChild(environment, "Shoreline");
            EnsureCleanChild(environment, "Bridge");
            EnsureCleanChild(environment, "MemoryVisuals");
            return environment;
        }

        private static void ConfigureTerrainSurfaces(Terrain terrain)
        {
            TerrainData data = terrain.terrainData;
            TerrainLayer grass = EnsureTerrainLayer("Sprint55C_Grass", new Color(0.52f, 0.72f, 0.32f), new Color(0.64f, 0.82f, 0.42f));
            TerrainLayer forestDirt = EnsureTerrainLayer("Sprint55C_ForestDirt", new Color(0.38f, 0.3f, 0.18f), new Color(0.48f, 0.39f, 0.24f));
            TerrainLayer path = EnsureTerrainLayer("Sprint55C_PathDryGround", new Color(0.64f, 0.52f, 0.32f), new Color(0.76f, 0.64f, 0.42f));
            TerrainLayer rock = EnsureTerrainLayer("Sprint55C_Rock", new Color(0.48f, 0.5f, 0.42f), new Color(0.58f, 0.6f, 0.5f));
            data.terrainLayers = new[] { grass, forestDirt, path, rock };
            data.alphamapResolution = AlphamapResolution;
            data.SetAlphamaps(0, 0, GenerateAlphamaps(data));
            terrain.materialTemplate = EnsureTerrainMaterial();
        }

        private static Material EnsureTerrainMaterial()
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EnvironmentMaterialFolder}/Sprint55C_Terrain_URP.mat");
            Shader shader = Shader.Find("Universal Render Pipeline/Terrain/Lit") ?? Shader.Find("Nature/Terrain/Standard");
            if (material == null)
            {
                material = new Material(shader) { name = "Sprint55C_Terrain_URP" };
                AssetDatabase.CreateAsset(material, $"{EnvironmentMaterialFolder}/Sprint55C_Terrain_URP.mat");
            }
            else if (shader != null)
            {
                material.shader = shader;
            }

            SetMaterialFloatIfPresent(material, "_Metallic", 0f);
            SetMaterialFloatIfPresent(material, "_Smoothness", 0.035f);
            SetMaterialFloatIfPresent(material, "_SpecularHighlights", 0f);
            SetMaterialFloatIfPresent(material, "_EnvironmentReflections", 0f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static TerrainLayer EnsureTerrainLayer(string name, Color baseColor, Color accentColor)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{EnvironmentMaterialFolder}/{name}_Texture.asset");
            if (texture == null)
            {
                texture = new Texture2D(16, 16, TextureFormat.RGBA32, false)
                {
                    name = $"{name}_Texture",
                    wrapMode = TextureWrapMode.Repeat,
                    filterMode = FilterMode.Bilinear
                };
                AssetDatabase.CreateAsset(texture, $"{EnvironmentMaterialFolder}/{name}_Texture.asset");
            }
            else if (texture.width != 16 || texture.height != 16)
            {
                texture.Reinitialize(16, 16);
            }

            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Repeat;

            Color[] pixels = new Color[256];
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    float mix = ((x * 17 + y * 29) % 9) / 80f;
                    pixels[y * 16 + x] = Color.Lerp(baseColor, accentColor, mix);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            EditorUtility.SetDirty(texture);

            TerrainLayer layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>($"{EnvironmentMaterialFolder}/{name}.terrainlayer");
            if (layer == null)
            {
                layer = new TerrainLayer();
                AssetDatabase.CreateAsset(layer, $"{EnvironmentMaterialFolder}/{name}.terrainlayer");
            }

            layer.name = name;
            layer.diffuseTexture = texture;
            layer.tileSize = new Vector2(name.Contains("Path") ? 16f : 22f, name.Contains("Path") ? 16f : 22f);
            layer.metallic = 0f;
            layer.smoothness = 0.02f;
            EditorUtility.SetDirty(layer);
            return layer;
        }

        private static float[,,] GenerateAlphamaps(TerrainData data)
        {
            int width = data.alphamapWidth;
            int height = data.alphamapHeight;
            float[,,] maps = new float[height, width, 4];
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 point = AlphamapPointToWorld(x, z, width, height);
                    float path = PathInfluence(point);
                    float shoreline = Mathf.Max(LakeShoreInfluence(point), CreekInfluence(point) * 0.65f);
                    float elevationRock = Mathf.InverseLerp(28f, 48f, SampleHeightApprox(point));
                    float forest = ForestInfluence(point);
                    float clearing = ClearingInfluence(point);
                    forest *= 1f - clearing * 0.82f;
                    maps[z, x, 0] = Mathf.Clamp01(1.15f + clearing * 0.65f - path * 0.78f - shoreline * 0.34f - forest * 0.18f - elevationRock * 0.2f);
                    maps[z, x, 1] = Mathf.Clamp01(forest * 0.42f + shoreline * 0.18f);
                    maps[z, x, 2] = Mathf.Clamp01(path);
                    maps[z, x, 3] = Mathf.Clamp01(shoreline * 0.65f + elevationRock * 0.55f);
                    NormalizeAlpha(maps, z, x, 4);
                }
            }

            return maps;
        }

        private static void ConfigureTerrainDetails(Terrain terrain)
        {
            TerrainData data = terrain.terrainData;
            data.SetDetailResolution(DetailResolution, DetailResolutionPerPatch);
            DetailPrototype[] prototypes =
            {
                CreateDetailPrototype("Grass_Common_Short", 0.65f, 1.05f, 0.72f, 1.25f),
                CreateDetailPrototype("Grass_Wispy_Tall", 0.7f, 1.1f, 0.9f, 1.55f),
                CreateDetailPrototype("Grass_Common_Tall", 0.7f, 1.15f, 0.85f, 1.45f)
            };

            data.detailPrototypes = prototypes;
            for (int layer = 0; layer < prototypes.Length; layer++)
            {
                int[,] details = new int[DetailResolution, DetailResolution];
                for (int z = 0; z < DetailResolution; z++)
                {
                    for (int x = 0; x < DetailResolution; x++)
                    {
                        Vector2 point = DetailPointToWorld(x, z);
                        details[z, x] = DetailDensity(point, layer);
                    }
                }

                data.SetDetailLayer(0, 0, layer, details);
            }
        }

        private static DetailPrototype CreateDetailPrototype(string assetName, float minWidth, float maxWidth, float minHeight, float maxHeight)
        {
            return new DetailPrototype
            {
                prototype = LoadAsset(assetName),
                usePrototypeMesh = true,
                useInstancing = true,
                renderMode = DetailRenderMode.VertexLit,
                minWidth = minWidth,
                maxWidth = maxWidth,
                minHeight = minHeight,
                maxHeight = maxHeight,
                healthyColor = new Color(0.62f, 0.78f, 0.42f),
                dryColor = new Color(0.75f, 0.66f, 0.39f),
                noiseSpread = 0.45f
            };
        }

        private static int DetailDensity(Vector2 point, int layer)
        {
            if (PathInfluence(point) > 0.3f || LakeInfluence(point) > 0.08f || CreekInfluence(point) > 0.24f)
            {
                return 0;
            }

            float shore = LakeShoreInfluence(point) + CreekInfluence(point);
            float meadow = Mathf.Max(
                Gaussian(point, new Vector2(0f, -150f), 58f, 42f),
                Gaussian(point, new Vector2(-158f, -44f), 44f, 36f));
            float forest = ForestInfluence(point);
            float density = meadow * 5.5f + forest * 3.2f + shore * 2.2f;
            density *= 0.74f + Deterministic01((int)(point.x * 19f + point.y * 23f) + layer * 101) * 0.55f;
            if (layer == 1)
            {
                density *= shore > 0.2f ? 0.9f : 0.45f;
            }
            else if (layer == 2)
            {
                density *= forest > 0.35f ? 0.72f : 0.24f;
            }

            return Mathf.Clamp(Mathf.RoundToInt(density), 0, 7);
        }

        private static void BuildTreeDressing(Transform environment, Terrain terrain)
        {
            Transform root = environment.Find("Trees");
            List<GameObject> assets = LoadAssets(TreeAssetNames);
            int placed = 0;
            for (int i = 0; i < TreeClusters.Length; i++)
            {
                TreeCluster cluster = TreeClusters[i];
                Transform clusterRoot = EnsureChild(root, cluster.Name).transform;
                placed += PlaceTreeCluster(clusterRoot, terrain, assets, cluster, 5107 + i * 271);
            }

            Debug.Log($"Sprint 5.5-C placed {placed} Quaternius MegaKit tree instances.");
        }

        private static int PlaceTreeCluster(Transform root, Terrain terrain, IReadOnlyList<GameObject> assets, TreeCluster cluster, int seed)
        {
            int placed = 0;
            int attempts = cluster.Count * 10;
            for (int i = 0; i < attempts && placed < cluster.Count; i++)
            {
                Vector2 point = SampleClusterPoint(cluster, seed + i * 31);
                if (!IsTreePlacementAllowed(point, cluster.Center, cluster.ClearingRadius))
                {
                    continue;
                }

                int assetIndex = (cluster.AssetOffset + placed * 5 + seed) % assets.Count;
                GameObject instance = InstantiateAsset(assets[assetIndex], root, $"QuaterniusTree_{assets[assetIndex].name}_{cluster.Name}_{placed + 1:000}");
                float scale = Mathf.Lerp(cluster.MinScale, cluster.MaxScale, Deterministic01(seed + placed * 43));
                instance.transform.SetPositionAndRotation(
                    WithTerrainY(new Vector3(point.x, 0f, point.y), terrain, -0.03f),
                    Quaternion.Euler(0f, Deterministic01(seed + placed * 59) * 360f, 0f));
                instance.transform.localScale = Vector3.one * scale;
                RemoveColliders(instance);
                ApplyNatureMaterials(instance, assets[assetIndex].name);
                AddTreeTrunkCollider(instance, scale);

                placed++;
            }

            return placed;
        }

        private static void BuildRocksAndShoreline(Transform environment, Terrain terrain)
        {
            Transform rocksRoot = environment.Find("Rocks");
            Transform shorelineRoot = environment.Find("Shoreline");
            List<GameObject> rocks = LoadAssets(RockAssetNames);

            Vector2[] shoreline =
            {
                new(96f, 42f), new(106f, 27f), new(129f, 18f), new(155f, 25f), new(177f, 50f),
                new(171f, 77f), new(144f, 98f), new(115f, 90f), new(94f, 66f)
            };

            for (int i = 0; i < shoreline.Length; i++)
            {
                PlaceRockCluster(shorelineRoot, terrain, rocks, shoreline[i], $"Shoreline_{i + 1:00}", 4 + (i % 3), 6200 + i * 37, 0.75f, 1.45f);
            }

            Vector2[] creek =
            {
                new(104f, 34f), new(76f, 21f), new(48f, 4f), new(22f, -20f), new(6f, -50f)
            };
            for (int i = 0; i < creek.Length; i++)
            {
                PlaceRockCluster(shorelineRoot, terrain, rocks, creek[i], $"Creek_{i + 1:00}", 3 + (i % 2), 7100 + i * 41, 0.55f, 1.15f);
            }

            PlaceRockCluster(rocksRoot, terrain, rocks, new Vector2(-18f, 32f), "CentralFold", 9, 7301, 0.8f, 1.6f);
            PlaceRockCluster(rocksRoot, terrain, rocks, new Vector2(-78f, 54f), "LightGroveSouth", 8, 7302, 0.8f, 1.5f);
            PlaceRockCluster(rocksRoot, terrain, rocks, new Vector2(-8f, 105f), "FinalApproach", 10, 7303, 0.9f, 1.7f);
            PlaceRockCluster(rocksRoot, terrain, rocks, new Vector2(0f, 214f), "NorthRidge", 18, 7304, 1.1f, 2.1f);
            PlaceRockCluster(rocksRoot, terrain, rocks, new Vector2(-214f, -8f), "WestRise", 16, 7305, 1.0f, 2.0f);
        }

        private static void PlaceRockCluster(Transform root, Terrain terrain, IReadOnlyList<GameObject> assets, Vector2 center, string label, int count, int seed, float minScale, float maxScale)
        {
            Transform clusterRoot = EnsureChild(root, label).transform;
            for (int i = 0; i < count; i++)
            {
                float angle = Deterministic01(seed + i * 17) * Mathf.PI * 2f;
                float radius = Mathf.Lerp(2f, 10f, Deterministic01(seed + i * 29));
                Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                if (IsBridgeApproach(point))
                {
                    point += (point - new Vector2(22f, -18f)).normalized * 8f;
                }

                GameObject asset = assets[(seed + i * 3) % assets.Count];
                GameObject instance = InstantiateAsset(asset, clusterRoot, $"QuaterniusRock_{asset.name}_{label}_{i + 1:00}");
                float scale = Mathf.Lerp(minScale, maxScale, Deterministic01(seed + i * 43));
                instance.transform.SetPositionAndRotation(
                    WithTerrainY(new Vector3(point.x, 0f, point.y), terrain, -0.02f),
                    Quaternion.Euler(0f, Deterministic01(seed + i * 53) * 360f, 0f));
                instance.transform.localScale = Vector3.one * scale;
                RemoveColliders(instance);
                ApplyNatureMaterials(instance, "Rock");
                if (ShouldBlockRock(asset.name, scale, label))
                {
                    AddRockCollider(instance, scale);
                }
            }
        }

        private static void BuildBushesAndFlowers(Transform environment, Terrain terrain)
        {
            Transform bushes = environment.Find("Bushes");
            Transform flowers = environment.Find("Flowers");
            List<GameObject> bushAssets = LoadAssets(BushAssetNames);
            List<GameObject> flowerAssets = LoadAssets(FlowerAssetNames);

            PlacePlantRing(bushes, terrain, bushAssets, new Vector2(0f, -150f), "SpawnMeadowEdge", 28, 48f, 66f, 8101, 0.8f, 1.25f);
            PlacePlantRing(flowers, terrain, flowerAssets, new Vector2(0f, -150f), "SpawnFlowers", 18, 18f, 36f, 8102, 0.85f, 1.2f);
            PlacePlantRing(bushes, terrain, bushAssets, new Vector2(-18f, -108f), "NPCGroveUnderstory", 18, 19f, 38f, 8103, 0.75f, 1.15f);
            PlacePlantRing(bushes, terrain, bushAssets, new Vector2(22f, -18f), "BridgeReeds", 24, 9f, 22f, 8104, 0.7f, 1.2f);
            PlacePlantRing(bushes, terrain, bushAssets, new Vector2(136f, 56f), "LakeReeds", 34, 45f, 64f, 8105, 0.75f, 1.25f);
            PlacePlantRing(flowers, terrain, flowerAssets, new Vector2(-158f, -44f), "HeartGardenFlowers", 42, 12f, 38f, 8106, 0.85f, 1.35f);
            PlacePlantRing(bushes, terrain, bushAssets, new Vector2(-58f, 84f), "LightGroveFerns", 22, 22f, 48f, 8107, 0.85f, 1.3f);
            PlacePlantRing(bushes, terrain, bushAssets, new Vector2(18f, 145f), "FinalHillLowPlants", 16, 26f, 54f, 8108, 0.75f, 1.1f);

            PlacePlantBand(bushes, terrain, bushAssets, new Vector2(-18f, -108f), new Vector2(60f, -62f), "NpcToMemoryPathEdge", 30, 9f, 16f, 8301, 0.75f, 1.18f);
            PlacePlantBand(bushes, terrain, bushAssets, new Vector2(60f, -62f), new Vector2(22f, -18f), "MemoryToBridgePathEdge", 24, 8f, 15f, 8302, 0.72f, 1.12f);
            PlacePlantBand(bushes, terrain, bushAssets, new Vector2(22f, -18f), new Vector2(24f, 62f), "BridgeToLightPathEdge", 34, 10f, 18f, 8303, 0.8f, 1.22f);
            PlacePlantBand(bushes, terrain, bushAssets, new Vector2(6f, 30f), new Vector2(-58f, 84f), "LightGroveLayeredUnderstory", 38, 12f, 22f, 8304, 0.85f, 1.3f);
            PlacePlantBand(bushes, terrain, bushAssets, new Vector2(-94f, 4f), new Vector2(-58f, 84f), "HeartToLightTransition", 28, 10f, 19f, 8305, 0.75f, 1.18f);
            PlaceTreeBaseDressing(bushes, terrain, bushAssets);
        }

        private static void PlacePlantRing(Transform root, Terrain terrain, IReadOnlyList<GameObject> assets, Vector2 center, string label, int count, float innerRadius, float outerRadius, int seed, float minScale, float maxScale)
        {
            Transform clusterRoot = EnsureChild(root, label).transform;
            for (int i = 0; i < count; i++)
            {
                float angle = Deterministic01(seed + i * 19) * Mathf.PI * 2f;
                float radius = Mathf.Lerp(innerRadius, outerRadius, Mathf.Sqrt(Deterministic01(seed + i * 31)));
                Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                if (PathInfluence(point) > 0.46f || LakeInfluence(point) > 0.18f)
                {
                    continue;
                }

                GameObject asset = assets[(seed + i * 5) % assets.Count];
                bool isFlower = asset.name.Contains("Flower") || asset.name.Contains("Clover");
                GameObject instance = InstantiateAsset(asset, clusterRoot, $"{(isFlower ? "QuaterniusFlower" : "QuaterniusPlant")}_{label}_{i + 1:00}");
                instance.transform.SetPositionAndRotation(
                    WithTerrainY(new Vector3(point.x, 0f, point.y), terrain, 0f),
                    Quaternion.Euler(0f, Deterministic01(seed + i * 47) * 360f, 0f));
                instance.transform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, Deterministic01(seed + i * 53));
                RemoveColliders(instance);
                ApplyNatureMaterials(instance, asset.name);
            }
        }

        private static void PlacePlantBand(Transform root, Terrain terrain, IReadOnlyList<GameObject> assets, Vector2 start, Vector2 end, string label, int count, float innerOffset, float outerOffset, int seed, float minScale, float maxScale)
        {
            Transform clusterRoot = EnsureChild(root, label).transform;
            Vector2 segment = end - start;
            Vector2 tangent = segment.sqrMagnitude > 0.01f ? segment.normalized : Vector2.up;
            Vector2 normal = new(-tangent.y, tangent.x);
            for (int i = 0; i < count; i++)
            {
                float along = Mathf.Lerp(0.08f, 0.92f, Deterministic01(seed + i * 23));
                float side = Deterministic01(seed + i * 29) > 0.5f ? 1f : -1f;
                float offset = Mathf.Lerp(innerOffset, outerOffset, Deterministic01(seed + i * 31)) * side;
                Vector2 point = Vector2.Lerp(start, end, along) + normal * offset;
                point += tangent * Mathf.Lerp(-3f, 3f, Deterministic01(seed + i * 37));
                if (PathInfluence(point) > 0.28f || LakeInfluence(point) > 0.14f || CreekInfluence(point) > 0.22f)
                {
                    continue;
                }

                GameObject asset = assets[(seed + i * 7) % assets.Count];
                bool isFlower = asset.name.Contains("Flower") || asset.name.Contains("Clover");
                GameObject instance = InstantiateAsset(asset, clusterRoot, $"{(isFlower ? "QuaterniusFlower" : "QuaterniusPlant")}_{label}_{i + 1:00}");
                instance.transform.SetPositionAndRotation(
                    WithTerrainY(new Vector3(point.x, 0f, point.y), terrain, 0f),
                    Quaternion.Euler(0f, Deterministic01(seed + i * 47) * 360f, 0f));
                instance.transform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, Deterministic01(seed + i * 53));
                RemoveColliders(instance);
                ApplyNatureMaterials(instance, asset.name);
            }
        }

        private static void PlaceTreeBaseDressing(Transform root, Terrain terrain, IReadOnlyList<GameObject> assets)
        {
            Transform clusterRoot = EnsureChild(root, "TreeBaseDressing").transform;
            Vector2[] centers =
            {
                new(52f, -78f), new(78f, -36f), new(42f, 20f), new(-78f, 52f), new(-42f, 92f),
                new(112f, 86f), new(180f, 34f), new(-185f, -28f), new(10f, 178f), new(-20f, 134f)
            };

            int placed = 0;
            for (int c = 0; c < centers.Length; c++)
            {
                for (int i = 0; i < 7; i++)
                {
                    int seed = 8500 + c * 97 + i * 13;
                    float angle = Deterministic01(seed) * Mathf.PI * 2f;
                    float radius = Mathf.Lerp(2.2f, 7.5f, Deterministic01(seed + 11));
                    Vector2 point = centers[c] + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                    if (PathInfluence(point) > 0.34f || LakeInfluence(point) > 0.13f || CreekInfluence(point) > 0.2f)
                    {
                        continue;
                    }

                    GameObject asset = assets[(seed + i * 5) % assets.Count];
                    GameObject instance = InstantiateAsset(asset, clusterRoot, $"QuaterniusPlant_TreeBase_{placed + 1:000}");
                    instance.transform.SetPositionAndRotation(
                        WithTerrainY(new Vector3(point.x, 0f, point.y), terrain, 0f),
                        Quaternion.Euler(0f, Deterministic01(seed + 19) * 360f, 0f));
                    instance.transform.localScale = Vector3.one * Mathf.Lerp(0.72f, 1.18f, Deterministic01(seed + 23));
                    RemoveColliders(instance);
                    ApplyNatureMaterials(instance, asset.name);
                    placed++;
                }
            }
        }

        private static void BuildProductionBridge(Transform environment, Terrain terrain)
        {
            Transform bridgeRoot = environment.Find("Bridge");
            GameObject bridge = FindSceneObjectIncludingInactive("Small Bridge") ?? new GameObject("Small Bridge");
            bridge.transform.SetParent(bridgeRoot, true);
            bridge.transform.SetPositionAndRotation(WithTerrainY(new Vector3(22f, 0f, -18f), terrain, 0.72f), Quaternion.Euler(0f, 34f, 0f));
            bridge.transform.localScale = Vector3.one;
            bridge.SetActive(true);

            for (int i = bridge.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = bridge.transform.GetChild(i);
                if (!child.name.Contains("Rail Collider", StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
                }
            }

            Material wood = EnsureMaterial("Sprint55C_BridgeWarmWood", new Color(0.49f, 0.34f, 0.19f));
            Material darkWood = EnsureMaterial("Sprint55C_BridgeDarkWood", new Color(0.31f, 0.22f, 0.15f));

            GameObject walkway = CreateCube(bridge.transform, "Sprint55C_Bridge_Walkway", new Vector3(0f, 0.12f, 0f), new Vector3(4.05f, 0.25f, 8.2f), wood);
            BoxCollider walkwayCollider = walkway.GetComponent<BoxCollider>();
            walkwayCollider.isTrigger = false;

            for (int i = 0; i < 7; i++)
            {
                float z = Mathf.Lerp(-3.35f, 3.35f, i / 6f);
                CreateCube(bridge.transform, $"Sprint55C_Bridge_Plank_{i + 1:00}", new Vector3(0f, 0.32f, z), new Vector3(4.25f, 0.12f, 0.38f), i % 2 == 0 ? wood : darkWood);
            }

            CreateCube(bridge.transform, "Sprint55C_Bridge_LeftRailVisual", new Vector3(-1.8f, 0.76f, 0f), new Vector3(0.16f, 0.2f, 7.8f), darkWood);
            CreateCube(bridge.transform, "Sprint55C_Bridge_RightRailVisual", new Vector3(1.8f, 0.76f, 0f), new Vector3(0.16f, 0.2f, 7.8f), darkWood);
            for (int i = 0; i < 4; i++)
            {
                float z = Mathf.Lerp(-3.4f, 3.4f, i / 3f);
                CreateCube(bridge.transform, $"Sprint55C_Bridge_LeftPost_{i + 1:00}", new Vector3(-1.82f, 0.52f, z), new Vector3(0.22f, 0.92f, 0.22f), darkWood);
                CreateCube(bridge.transform, $"Sprint55C_Bridge_RightPost_{i + 1:00}", new Vector3(1.82f, 0.52f, z), new Vector3(0.22f, 0.92f, 0.22f), darkWood);
            }

            CreateCube(bridge.transform, "Sprint55C_Bridge_LeftSupport", new Vector3(-1.25f, -0.42f, -2.9f), new Vector3(0.28f, 1.0f, 0.28f), darkWood);
            CreateCube(bridge.transform, "Sprint55C_Bridge_RightSupport", new Vector3(1.25f, -0.42f, 2.9f), new Vector3(0.28f, 1.0f, 0.28f), darkWood);
            CreateCube(bridge.transform, "Sprint55C_Bridge_UnderBeam", new Vector3(0f, -0.08f, 0f), new Vector3(3.1f, 0.18f, 7.5f), darkWood);
            EnsureBridgeRailCollider(bridge.transform, "Bridge Left Rail Collider", -1.5f);
            EnsureBridgeRailCollider(bridge.transform, "Bridge Right Rail Collider", 1.5f);
        }

        private static void DressMemoryVisuals(Transform environment, Terrain terrain)
        {
            Transform root = environment.Find("MemoryVisuals");
            Material glow = EnsureMaterial("Sprint55C_MemoryLeafGlow", new Color(1f, 0.72f, 0.28f, 1f));
            foreach (MemoryCollectible collectible in UnityEngine.Object.FindObjectsByType<MemoryCollectible>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Transform existing = collectible.transform.Find("Sprint55C_MemoryVisual");
                if (existing != null)
                {
                    UnityEngine.Object.DestroyImmediate(existing.gameObject);
                }

                foreach (Renderer renderer in collectible.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.enabled = false;
                }

                GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                visual.name = "Sprint55C_MemoryVisual";
                visual.transform.SetParent(collectible.transform, false);
                visual.transform.localPosition = new Vector3(0f, 0.55f, 0f);
                visual.transform.localScale = new Vector3(0.28f, 0.12f, 0.42f);
                visual.GetComponent<Renderer>().sharedMaterial = glow;
                UnityEngine.Object.DestroyImmediate(visual.GetComponent<Collider>());

                SerializedObject serialized = new(collectible);
                SerializedProperty renderers = serialized.FindProperty("renderers");
                if (renderers != null)
                {
                    renderers.arraySize = 1;
                    renderers.GetArrayElementAtIndex(0).objectReferenceValue = visual.GetComponent<Renderer>();
                    serialized.ApplyModifiedPropertiesWithoutUndo();
                }

                GameObject anchor = new($"MemoryVisualAnchor_{collectible.name}");
                anchor.transform.SetParent(root, false);
                anchor.transform.position = WithTerrainY(collectible.transform.position, terrain, 0.02f);
            }
        }

        private static void DressLightPathNodes()
        {
            Material inactive = EnsureMaterial("Sprint55C_LightStoneInactive", new Color(0.32f, 0.34f, 0.25f));
            foreach (LightPathNode node in UnityEngine.Object.FindObjectsByType<LightPathNode>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                foreach (Renderer renderer in node.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.sharedMaterial = inactive;
                }
            }
        }

        private static void HidePrototypeVisuals()
        {
            GameObject oldForest = FindSceneObjectIncludingInactive("TreeCollectionForest");
            if (oldForest != null)
            {
                oldForest.SetActive(false);
                EditorUtility.SetDirty(oldForest);
            }

            GameObject visuals = GameObject.Find("Environment_Visuals");
            if (visuals != null)
            {
                for (int i = visuals.transform.childCount - 1; i >= 0; i--)
                {
                    UnityEngine.Object.DestroyImmediate(visuals.transform.GetChild(i).gameObject);
                }
            }

            GameObject platform = GameObject.Find("Start Platform");
            if (platform != null)
            {
                foreach (Renderer renderer in platform.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.enabled = false;
                }
            }

            string[] prototypeObjects =
            {
                "North Boundary Ridge", "South Boundary Ridge", "East Boundary Ridge", "West Boundary Ridge",
                "Blocked Placeholder Pond", "Pond North Stones", "Pond South Stones",
                "Path Start To Bridge", "Path Bridge To Camp", "Path To Pond",
                "GB_SpawnMeadow_Readability", "GB_NPCGrove_Readability", "GB_FinalHill_Summit"
            };

            foreach (string name in prototypeObjects)
            {
                GameObject found = FindSceneObjectIncludingInactive(name);
                if (found != null)
                {
                    found.SetActive(false);
                    EditorUtility.SetDirty(found);
                }
            }

            Material water = EnsureMaterial("Sprint55C_SimpleWater", new Color(0.22f, 0.48f, 0.56f, 0.7f));
            AssignMaterialIfFound("GB_Lake_TempWater", water);
            AssignMaterialIfFound("GB_Creek_TempWater", water);
        }

        public static void CaptureEnvironmentReviewScreenshots()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            Directory.CreateDirectory(ScreenshotFolder);
            Camera camera = Camera.main ?? UnityEngine.Object.FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
            if (camera == null)
            {
                throw new InvalidOperationException("Cannot capture Sprint 5.5-C screenshots because the Forest scene has no camera.");
            }

            Capture(camera, "01_spawn_meadow.png", new Vector3(0f, 18f, -186f), new Vector3(0f, 10f, -142f));
            Capture(camera, "02_dense_forest_path.png", new Vector3(62f, 20f, -82f), new Vector3(31f, 11f, -38f));
            Capture(camera, "03_bridge_lake.png", new Vector3(3f, 17f, -46f), new Vector3(25f, 9f, -17f));
            Capture(camera, "04_light_grove.png", new Vector3(-20f, 25f, 34f), new Vector3(-58f, 15f, 84f));
            Capture(camera, "05_heart_garden.png", new Vector3(-120f, 22f, -86f), new Vector3(-158f, 11f, -44f));
            Capture(camera, "06_final_hill.png", new Vector3(-28f, 34f, 88f), new Vector3(18f, 39f, 145f));
            Capture(camera, "07_npc_candidate.png", new Vector3(-38f, 18f, -136f), new Vector3(-18f, 9f, -108f));
            AssetDatabase.Refresh();
        }

        private static void Capture(Camera camera, string fileName, Vector3 position, Vector3 lookAt)
        {
            RenderTexture previousTarget = camera.targetTexture;
            RenderTexture renderTexture = new(1600, 900, 24);
            Texture2D texture = new(1600, 900, TextureFormat.RGB24, false);
            Vector3 previousPosition = camera.transform.position;
            Quaternion previousRotation = camera.transform.rotation;
            float previousFov = camera.fieldOfView;
            try
            {
                camera.transform.position = position;
                camera.transform.rotation = Quaternion.LookRotation((lookAt - position).normalized, Vector3.up);
                camera.fieldOfView = 56f;
                camera.targetTexture = renderTexture;
                camera.Render();
                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                texture.Apply();
                File.WriteAllBytes(Path.Combine(ScreenshotFolder, fileName), texture.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTarget;
                camera.transform.SetPositionAndRotation(previousPosition, previousRotation);
                camera.fieldOfView = previousFov;
                RenderTexture.active = null;
                renderTexture.Release();
                UnityEngine.Object.DestroyImmediate(renderTexture);
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        private static GameObject CreateCube(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = localPosition;
            cube.transform.localRotation = Quaternion.identity;
            cube.transform.localScale = localScale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            return cube;
        }

        private static void EnsureBridgeRailCollider(Transform bridge, string name, float localX)
        {
            Transform rail = bridge.Find(name);
            if (rail == null)
            {
                rail = new GameObject(name).transform;
                rail.SetParent(bridge, false);
            }

            rail.localPosition = new Vector3(localX, 0.56f, 0f);
            rail.localRotation = Quaternion.identity;
            rail.localScale = Vector3.one;
            BoxCollider collider = rail.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = rail.gameObject.AddComponent<BoxCollider>();
            }
            collider.isTrigger = false;
            collider.size = new Vector3(0.18f, 0.9f, 4.45f);
            collider.center = Vector3.zero;
        }

        private static void AddTreeTrunkCollider(GameObject tree, float scale)
        {
            CapsuleCollider collider = tree.GetComponent<CapsuleCollider>();
            if (collider == null)
            {
                collider = tree.AddComponent<CapsuleCollider>();
            }
            collider.radius = Mathf.Clamp(0.18f / Mathf.Max(scale, 0.01f), 0.12f, 0.28f);
            collider.height = Mathf.Clamp(2.8f / Mathf.Max(scale, 0.01f), 2.0f, 4.0f);
            collider.center = new Vector3(0f, collider.height * 0.5f, 0f);
            collider.direction = 1;
            collider.isTrigger = false;
        }

        private static bool ShouldBlockRock(string assetName, float scale, string label)
        {
            if (assetName.StartsWith("Rock_Medium", StringComparison.Ordinal))
            {
                return true;
            }

            return scale >= 1.18f && (label.Contains("Shoreline", StringComparison.Ordinal) || label.Contains("Ridge", StringComparison.Ordinal) || label.Contains("Rise", StringComparison.Ordinal));
        }

        private static void AddRockCollider(GameObject rock, float scale)
        {
            BoxCollider collider = rock.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = rock.AddComponent<BoxCollider>();
            }

            float inverseScale = 1f / Mathf.Max(scale, 0.01f);
            collider.center = new Vector3(0f, 0.36f * inverseScale, 0f);
            collider.size = new Vector3(1.35f * inverseScale, 0.78f * inverseScale, 1.2f * inverseScale);
            collider.isTrigger = false;
        }

        private static bool IsBridgeApproach(Vector2 point)
        {
            return Vector2.Distance(point, new Vector2(22f, -18f)) < 15f
                || DistanceToSegment(point, new Vector2(14f, -29f), new Vector2(32f, -7f)) < 6.5f;
        }

        private static void ApplyGlareBlockerCorrection()
        {
            foreach (Light light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (light.type == LightType.Directional && light.intensity > 1.15f)
                {
                    light.intensity = 1.05f;
                    EditorUtility.SetDirty(light);
                }
            }

            if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_Exposure") && RenderSettings.skybox.GetFloat("_Exposure") > 1.05f)
            {
                RenderSettings.skybox.SetFloat("_Exposure", 1.05f);
                EditorUtility.SetDirty(RenderSettings.skybox);
            }
        }

        private static string BuildGlareReport()
        {
            Light sun = null;
            foreach (Light light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (light.type == LightType.Directional)
                {
                    sun = light;
                    break;
                }
            }

            Material terrainMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{EnvironmentMaterialFolder}/Sprint55C_Terrain_URP.mat");
            string smoothness = terrainMaterial != null && terrainMaterial.HasProperty("_Smoothness")
                ? terrainMaterial.GetFloat("_Smoothness").ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)
                : "unavailable";
            string metallic = terrainMaterial != null && terrainMaterial.HasProperty("_Metallic")
                ? terrainMaterial.GetFloat("_Metallic").ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)
                : "unavailable";
            string sunIntensity = sun != null ? sun.intensity.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture) : "missing";
            return $"terrainSmoothness={smoothness};terrainMetallic={metallic};directionalIntensity={sunIntensity};rootCause=terrainSpecularResponse";
        }

        private static bool IsTreePlacementAllowed(Vector2 point, Vector2 clusterCenter, float clearingRadius)
        {
            if (Mathf.Abs(point.x) > 242f || Mathf.Abs(point.y) > 242f)
            {
                return false;
            }

            if (clearingRadius > 0f && Vector2.Distance(point, clusterCenter) < clearingRadius)
            {
                return false;
            }

            if (PathInfluence(point) > 0.38f || LakeInfluence(point) > 0.14f || CreekInfluence(point) > 0.28f)
            {
                return false;
            }

            if (Vector2.Distance(point, new Vector2(0f, -150f)) < 26f || Vector2.Distance(point, new Vector2(22f, -18f)) < 16f)
            {
                return false;
            }

            return true;
        }

        private static Vector2 SampleClusterPoint(TreeCluster cluster, int seed)
        {
            float angle = Deterministic01(seed) * Mathf.PI * 2f;
            float radius = Mathf.Sqrt(Deterministic01(seed + 17));
            return cluster.Center + new Vector2(Mathf.Cos(angle) * cluster.Radius.x * radius, Mathf.Sin(angle) * cluster.Radius.y * radius);
        }

        private static void ApplyNatureMaterials(GameObject instance, string assetName)
        {
            Material bark = EnsureMaterial("Sprint55C_Bark", new Color(0.39f, 0.27f, 0.16f));
            Material leaves = EnsureMaterial("Sprint55C_WarmLeaves", new Color(0.38f, 0.6f, 0.28f));
            Material pine = EnsureMaterial("Sprint55C_PineLeaves", new Color(0.22f, 0.44f, 0.29f));
            Material rock = EnsureMaterial("Sprint55C_RockMaterial", new Color(0.45f, 0.47f, 0.4f));
            Material flower = EnsureMaterial("Sprint55C_FlowersWarm", new Color(1f, 0.68f, 0.34f));
            Material plant = EnsureMaterial("Sprint55C_PlantWarmGreen", new Color(0.46f, 0.68f, 0.36f));

            foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                Material[] materials = renderer.sharedMaterials;
                if (materials.Length == 0)
                {
                    renderer.sharedMaterial = ChooseMaterial(assetName, renderer.name, bark, leaves, pine, rock, flower, plant);
                    continue;
                }

                for (int i = 0; i < materials.Length; i++)
                {
                    string key = $"{assetName} {renderer.name} {materials[i]?.name}".ToLowerInvariant();
                    if (key.Contains("rock") || key.Contains("pebble"))
                    {
                        materials[i] = rock;
                    }
                    else if (key.Contains("flower") || key.Contains("clover") || key.Contains("mushroom"))
                    {
                        materials[i] = flower;
                    }
                    else if (key.Contains("grass") || key.Contains("fern") || key.Contains("plant") || key.Contains("bush"))
                    {
                        materials[i] = plant;
                    }
                    else if (key.Contains("pine") || key.Contains("leaf") || key.Contains("leaves"))
                    {
                        materials[i] = key.Contains("pine") ? pine : leaves;
                    }
                    else
                    {
                        materials[i] = key.Contains("tree") ? bark : plant;
                    }
                }

                renderer.sharedMaterials = materials;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }
        }

        private static Material ChooseMaterial(string assetName, string rendererName, Material bark, Material leaves, Material pine, Material rock, Material flower, Material plant)
        {
            string key = $"{assetName} {rendererName}".ToLowerInvariant();
            if (key.Contains("rock") || key.Contains("pebble"))
            {
                return rock;
            }

            if (key.Contains("flower") || key.Contains("clover") || key.Contains("mushroom"))
            {
                return flower;
            }

            if (key.Contains("pine"))
            {
                return pine;
            }

            if (key.Contains("tree"))
            {
                return leaves;
            }

            return plant;
        }

        private static Material EnsureMaterial(string name, Color color)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EnvironmentMaterialFolder}/{name}.mat");
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (material == null)
            {
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, $"{EnvironmentMaterialFolder}/{name}.mat");
            }
            else if (shader != null)
            {
                material.shader = shader;
            }

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }
            else
            {
                material.color = color;
            }

            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", 0.14f);
            }

            SetMaterialFloatIfPresent(material, "_Metallic", 0f);
            SetMaterialFloatIfPresent(material, "_SpecularHighlights", 0f);
            SetMaterialFloatIfPresent(material, "_EnvironmentReflections", 0.35f);

            if (material.HasProperty("_Surface"))
            {
                material.SetFloat("_Surface", color.a < 0.99f ? 1f : 0f);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static void SetMaterialFloatIfPresent(Material material, string propertyName, float value)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                material.SetFloat(propertyName, value);
            }
        }

        private static void AssignMaterialIfFound(string objectName, Material material)
        {
            GameObject gameObject = FindSceneObjectIncludingInactive(objectName);
            if (gameObject == null)
            {
                return;
            }

            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                renderer.sharedMaterial = material;
            }
        }

        private static Terrain RequireTerrain()
        {
            GameObject terrainObject = GameObject.Find(TerrainName);
            Terrain terrain = terrainObject != null ? terrainObject.GetComponent<Terrain>() : null;
            if (terrain == null || terrain.terrainData == null)
            {
                throw new InvalidOperationException("Sprint 5.5-C requires the approved Sprint 5.5-B terrain to exist before dressing.");
            }

            return terrain;
        }

        private static List<GameObject> LoadAssets(IReadOnlyList<string> names)
        {
            List<GameObject> assets = new();
            for (int i = 0; i < names.Count; i++)
            {
                GameObject asset = LoadAsset(names[i]);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            if (assets.Count == 0)
            {
                throw new InvalidOperationException($"No Quaternius MegaKit assets loaded from {MegaKitRoot}.");
            }

            return assets;
        }

        private static GameObject LoadAsset(string name)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>($"{MegaKitRoot}/{name}.fbx");
        }

        private static GameObject InstantiateAsset(GameObject asset, Transform parent, string name)
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(asset, parent) as GameObject;
            if (instance == null)
            {
                instance = UnityEngine.Object.Instantiate(asset, parent);
            }

            instance.name = name;
            return instance;
        }

        private static void RemoveColliders(GameObject gameObject)
        {
            foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>(true))
            {
                UnityEngine.Object.DestroyImmediate(collider);
            }
        }

        private static Vector3 WithTerrainY(Vector3 position, Terrain terrain, float yOffset)
        {
            position.y = terrain.SampleHeight(position) + terrain.transform.position.y + yOffset;
            return position;
        }

        private static float PathInfluence(Vector2 point)
        {
            Vector2[] main = { new(0f, -150f), new(-18f, -108f), new(60f, -62f), new(22f, -18f), new(24f, 62f), new(18f, 145f) };
            Vector2[] heart = { new(-18f, -108f), new(-84f, -92f), new(-158f, -44f), new(-94f, 4f), new(-58f, 84f) };
            Vector2[] light = { new(22f, -18f), new(6f, 30f), new(-58f, 84f) };
            Vector2[] lake = { new(22f, -18f), new(82f, 16f), new(136f, 56f), new(74f, 88f) };
            float distance = Mathf.Min(DistanceToPolyline(point, main), DistanceToPolyline(point, heart), DistanceToPolyline(point, light), DistanceToPolyline(point, lake));
            return Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(4f, 12f, distance));
        }

        private static float LakeInfluence(Vector2 point)
        {
            Vector2 center = new(136f, 56f);
            float dx = (point.x - center.x) / 47f;
            float dz = (point.y - center.y) / 38f;
            float wave = 0.14f * Mathf.Sin(point.x * 0.08f) + 0.11f * Mathf.Cos(point.y * 0.07f);
            return Mathf.Clamp01(1f - (dx * dx + dz * dz) + wave);
        }

        private static float LakeShoreInfluence(Vector2 point)
        {
            return Mathf.Clamp01(1f - Mathf.Abs(LakeInfluence(point) - 0.22f) * 5f);
        }

        private static float CreekInfluence(Vector2 point)
        {
            Vector2[] creek = { new(104f, 34f), new(70f, 18f), new(38f, -2f), new(17f, -30f), new(1f, -62f) };
            float distance = DistanceToPolyline(point, creek);
            return Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(3.5f, 13f, distance));
        }

        private static float ForestInfluence(Vector2 point)
        {
            float influence = 0f;
            for (int i = 0; i < TreeClusters.Length; i++)
            {
                TreeCluster cluster = TreeClusters[i];
                float dx = (point.x - cluster.Center.x) / Mathf.Max(cluster.Radius.x, 0.1f);
                float dz = (point.y - cluster.Center.y) / Mathf.Max(cluster.Radius.y, 0.1f);
                influence = Mathf.Max(influence, Mathf.Clamp01(1f - (dx * dx + dz * dz)));
            }

            return influence;
        }

        private static float ClearingInfluence(Vector2 point)
        {
            return Mathf.Clamp01(Mathf.Max(
                Mathf.Max(Gaussian(point, new Vector2(0f, -150f), 38f, 28f), Gaussian(point, new Vector2(-18f, -108f), 24f, 20f)),
                Mathf.Max(Gaussian(point, new Vector2(-158f, -44f), 34f, 28f), Gaussian(point, new Vector2(18f, 145f), 42f, 30f))));
        }

        private static float SampleHeightApprox(Vector2 point)
        {
            float northRise = Mathf.InverseLerp(-170f, 170f, point.y) * 15f;
            return 7.2f + northRise + 26f * Gaussian(point, new Vector2(20f, 145f), 78f, 58f);
        }

        private static float Gaussian(Vector2 point, Vector2 center, float radiusX, float radiusZ)
        {
            float dx = (point.x - center.x) / radiusX;
            float dz = (point.y - center.y) / radiusZ;
            return Mathf.Exp(-(dx * dx + dz * dz));
        }

        private static float DistanceToPolyline(Vector2 point, Vector2[] polyline)
        {
            float min = float.PositiveInfinity;
            for (int i = 0; i < polyline.Length - 1; i++)
            {
                min = Mathf.Min(min, DistanceToSegment(point, polyline[i], polyline[i + 1]));
            }

            return min;
        }

        private static float DistanceToSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / ab.sqrMagnitude);
            return Vector2.Distance(point, a + ab * t);
        }

        private static Vector2 AlphamapPointToWorld(int x, int z, int width, int height)
        {
            return new Vector2(
                TerrainOrigin + (x / (float)(width - 1)) * TerrainSize,
                TerrainOrigin + (z / (float)(height - 1)) * TerrainSize);
        }

        private static Vector2 DetailPointToWorld(int x, int z)
        {
            return new Vector2(
                TerrainOrigin + (x / (float)(DetailResolution - 1)) * TerrainSize,
                TerrainOrigin + (z / (float)(DetailResolution - 1)) * TerrainSize);
        }

        private static void NormalizeAlpha(float[,,] maps, int z, int x, int layerCount)
        {
            float total = 0f;
            for (int layer = 0; layer < layerCount; layer++)
            {
                total += maps[z, x, layer];
            }

            if (total <= 0f)
            {
                maps[z, x, 0] = 1f;
                return;
            }

            for (int layer = 0; layer < layerCount; layer++)
            {
                maps[z, x, layer] /= total;
            }
        }

        private static bool IsBoundary(Vector2 point)
        {
            return Mathf.Abs(point.x) > 190f || Mathf.Abs(point.y) > 190f;
        }

        private static float Deterministic01(int value)
        {
            uint x = (uint)value;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            return (x & 0x00FFFFFF) / (float)0x01000000;
        }

        private static GameObject EnsureChild(Transform parent, string name)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return existing.gameObject;
            }

            GameObject child = new(name);
            child.transform.SetParent(parent, false);
            return child;
        }

        private static GameObject EnsureCleanChild(Transform parent, string name)
        {
            GameObject child = EnsureChild(parent, name);
            for (int i = child.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(child.transform.GetChild(i).gameObject);
            }

            return child;
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        private static GameObject FindSceneObjectIncludingInactive(string name)
        {
            GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < objects.Length; i++)
            {
                GameObject gameObject = objects[i];
                if (gameObject.name == name && gameObject.scene.IsValid())
                {
                    return gameObject;
                }
            }

            return null;
        }

        private static int CountNamedChildren(Transform root, string prefix)
        {
            if (root == null)
            {
                return 0;
            }

            int count = root.name.StartsWith(prefix, StringComparison.Ordinal) ? 1 : 0;
            for (int i = 0; i < root.childCount; i++)
            {
                count += CountNamedChildren(root.GetChild(i), prefix);
            }

            return count;
        }

        private static int CountProductionTreeColliders(Transform root)
        {
            if (root == null)
            {
                return 0;
            }

            int count = root.name.StartsWith("QuaterniusTree_", StringComparison.Ordinal) && root.GetComponent<Collider>() != null ? 1 : 0;
            for (int i = 0; i < root.childCount; i++)
            {
                count += CountProductionTreeColliders(root.GetChild(i));
            }

            return count;
        }

        private static int CountBlockingRockColliders(Transform root)
        {
            if (root == null)
            {
                return 0;
            }

            int count = root.name.StartsWith("QuaterniusRock_", StringComparison.Ordinal) && root.GetComponent<BoxCollider>() != null ? 1 : 0;
            for (int i = 0; i < root.childCount; i++)
            {
                count += CountBlockingRockColliders(root.GetChild(i));
            }

            return count;
        }

        private static HashSet<string> CollectTreeVariants(Transform root)
        {
            HashSet<string> variants = new();
            CollectTreeVariants(root, variants);
            return variants;
        }

        private static void CollectTreeVariants(Transform root, HashSet<string> variants)
        {
            if (root == null)
            {
                return;
            }

            if (root.name.StartsWith("QuaterniusTree_", StringComparison.Ordinal))
            {
                string[] parts = root.name.Split('_');
                if (parts.Length >= 3)
                {
                    variants.Add($"{parts[1]}_{parts[2]}");
                }
            }

            for (int i = 0; i < root.childCount; i++)
            {
                CollectTreeVariants(root.GetChild(i), variants);
            }
        }

        private static string FormatTerrainLayers(Terrain terrain)
        {
            if (terrain == null || terrain.terrainData == null)
            {
                return "MISSING";
            }

            List<string> names = new();
            TerrainLayer[] layers = terrain.terrainData.terrainLayers;
            for (int i = 0; i < layers.Length; i++)
            {
                names.Add(layers[i] != null ? layers[i].name : "NULL");
            }

            return string.Join(", ", names);
        }

        private readonly struct TreeCluster
        {
            public TreeCluster(string name, Vector2 center, Vector2 radius, int count, float minScale, float maxScale, float clearingRadius, int assetOffset)
            {
                Name = name;
                Center = center;
                Radius = radius;
                Count = count;
                MinScale = minScale;
                MaxScale = maxScale;
                ClearingRadius = clearingRadius;
                AssetOffset = assetOffset;
            }

            public string Name { get; }
            public Vector2 Center { get; }
            public Vector2 Radius { get; }
            public int Count { get; }
            public float MinScale { get; }
            public float MaxScale { get; }
            public float ClearingRadius { get; }
            public int AssetOffset { get; }
        }
    }
}
