using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint55WorldLayoutBuilder
    {
        public const float TerrainSize = 512f;
        public const float TerrainHeight = 55f;
        public const int HeightmapResolution = 257;
        public const int AlphamapResolution = 256;
        public const string TerrainDataPath = "Assets/_Game/Art/Environment/Sprint55B_WorldTerrain.asset";
        public const string WorldRootName = "World";
        public const string PrimaryTerrainName = "Sprint55B_PrimaryTerrain";

        private const string TerrainFolder = "Assets/_Game/Art/Environment";
        private const string TreePackRoot = "Assets/TreePackVol.1";
        private const float TerrainOrigin = -TerrainSize * 0.5f;
        private const int MinimumTreeCollectionInstances = 300;
        private const float BridgeYaw = 126.87f;
        private const float BridgeDeckLength = 28f;
        private const float BridgeDeckWidth = 4.4f;
        private const float BridgeRailX = 1.95f;

        private static readonly string[] TreeCollectionPrefabPaths =
        {
            "Assets/TreePackVol.1/Prefabs/0/Tree1.prefab",
            "Assets/TreePackVol.1/Prefabs/0/Tree3.prefab",
            "Assets/TreePackVol.1/Prefabs/0/Tree5.prefab",
            "Assets/TreePackVol.1/Prefabs/0/Tree8.prefab",
            "Assets/TreePackVol.1/Prefabs/0/Tree12.prefab",
            "Assets/TreePackVol.1/Prefabs/2/Tree.prefab",
            "Assets/TreePackVol.1/Prefabs/2/Tree 2.prefab",
            "Assets/TreePackVol.1/Prefabs/3/Tree 1.prefab",
            "Assets/TreePackVol.1/Prefabs/3/Tree 3.prefab",
            "Assets/TreePackVol.1/Prefabs/3/Tree 7.prefab",
            "Assets/TreePackVol.1/Prefabs/4/Tree1.prefab",
            "Assets/TreePackVol.1/Prefabs/4/Tree4.prefab",
            "Assets/TreePackVol.1/Prefabs/5/Tree 4.prefab",
            "Assets/TreePackVol.1/Prefabs/5/Tree 7.prefab",
        };

        private static readonly TreeCluster[] TreeClusters =
        {
            new("SpawnMeadowRing", new Vector2(0f, -150f), new Vector2(62f, 46f), 48, 0.62f, 1.08f, 25f),
            new("NPCGrove", new Vector2(-18f, -108f), new Vector2(46f, 36f), 36, 0.58f, 1.03f, 15f),
            new("MemoryEast", new Vector2(66f, -60f), new Vector2(60f, 44f), 46, 0.58f, 1.1f, 10f),
            new("MemoryWest", new Vector2(-78f, 18f), new Vector2(64f, 48f), 50, 0.62f, 1.12f, 12f),
            new("CreekApproach", new Vector2(32f, -8f), new Vector2(54f, 34f), 38, 0.55f, 0.95f, 12f),
            new("LakeShore", new Vector2(136f, 56f), new Vector2(88f, 66f), 64, 0.58f, 1.03f, 36f),
            new("LightGrove", new Vector2(-58f, 84f), new Vector2(70f, 54f), 64, 0.64f, 1.18f, 15f),
            new("HeartGardenRing", new Vector2(-158f, -44f), new Vector2(54f, 46f), 42, 0.58f, 1.06f, 20f),
            new("FinalHillCrown", new Vector2(18f, 145f), new Vector2(76f, 50f), 48, 0.62f, 1.16f, 24f),
            new("NorthBackground", new Vector2(0f, 214f), new Vector2(198f, 30f), 54, 0.72f, 1.22f, 0f),
            new("WestBackground", new Vector2(-214f, 0f), new Vector2(30f, 198f), 54, 0.72f, 1.22f, 0f),
            new("EastBackground", new Vector2(214f, 22f), new Vector2(30f, 170f), 42, 0.68f, 1.14f, 0f),
            new("SouthBackground", new Vector2(10f, -216f), new Vector2(176f, 28f), 42, 0.68f, 1.12f, 0f),
        };

        private static readonly LayoutAnchor[] Anchors =
        {
            new("LM_SpawnMeadow", new Vector3(0f, 0f, -150f)),
            new("LM_NPCGrove", new Vector3(-18f, 0f, -108f)),
            new("LM_MemoryRoute_A", new Vector3(60f, 0f, -62f)),
            new("LM_MemoryRoute_B", new Vector3(-76f, 0f, 18f)),
            new("LM_Bridge", new Vector3(24.6f, 0f, -19.9f)),
            new("LM_Lake", new Vector3(136f, 0f, 56f)),
            new("LM_LightGrove", new Vector3(-58f, 0f, 84f)),
            new("LM_HeartGarden", new Vector3(-158f, 0f, -44f)),
            new("LM_FinalHill", new Vector3(18f, 0f, 145f)),
        };

        [MenuItem("Tilki Oyunu/Sprint 5.5/Build Terrain Graybox")]
        public static void BuildTerrainGrayboxFromMenu()
        {
            BuildTerrainGraybox();
        }

        public static void BuildTerrainGrayboxFromCommandLine()
        {
            BuildTerrainGraybox();
            EditorApplication.Exit(0);
        }

        public static IReadOnlyList<LayoutAnchor> LayoutAnchors => Anchors;

        private static void BuildTerrainGraybox()
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            EnsureFolder(TerrainFolder);
            ImportTreePackIfPresent();
            PreserveExistingSmallBridge();

            TerrainData terrainData = EnsureTerrainData();
            Terrain terrain = BuildWorldHierarchy(terrainData);
            ConfigureTerrain(terrain, terrainData);
            BuildGraybox(terrain);
            BuildTreeCollectionForest(terrain);
            PlaceLandmarks(terrain);
            AlignTraversalObjects(terrain);
            DisableArenaBlockoutVisuals();

            EditorUtility.SetDirty(terrainData);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Sprint 5.5-B terrain graybox built.");
        }

        private static TerrainData EnsureTerrainData()
        {
            TerrainData terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(TerrainDataPath);
            if (terrainData == null)
            {
                terrainData = new TerrainData();
                AssetDatabase.CreateAsset(terrainData, TerrainDataPath);
            }

            terrainData.heightmapResolution = HeightmapResolution;
            terrainData.alphamapResolution = AlphamapResolution;
            terrainData.baseMapResolution = 512;
            terrainData.size = new Vector3(TerrainSize, TerrainHeight, TerrainSize);
            terrainData.terrainLayers = EnsureTerrainLayers();
            terrainData.SetHeights(0, 0, GenerateHeights());
            terrainData.SetAlphamaps(0, 0, GenerateAlphamaps(terrainData));
            return terrainData;
        }

        private static TerrainLayer[] EnsureTerrainLayers()
        {
            return new[]
            {
                EnsureTerrainLayer("Sprint55B_LowMeadow", new Color(0.43f, 0.52f, 0.38f)),
                EnsureTerrainLayer("Sprint55B_PathDirt", new Color(0.48f, 0.42f, 0.32f)),
                EnsureTerrainLayer("Sprint55B_SlopeRock", new Color(0.42f, 0.43f, 0.42f)),
                EnsureTerrainLayer("Sprint55B_LakebedSilt", new Color(0.28f, 0.36f, 0.37f)),
            };
        }

        private static TerrainLayer EnsureTerrainLayer(string name, Color color)
        {
            string texturePath = $"{TerrainFolder}/{name}_Texture.asset";
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null)
            {
                texture = new Texture2D(4, 4, TextureFormat.RGBA32, false)
                {
                    name = $"{name}_Texture",
                    wrapMode = TextureWrapMode.Repeat,
                    filterMode = FilterMode.Point
                };
                Color[] pixels = new Color[16];
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = color;
                }

                texture.SetPixels(pixels);
                texture.Apply();
                AssetDatabase.CreateAsset(texture, texturePath);
            }

            string layerPath = $"{TerrainFolder}/{name}.terrainlayer";
            TerrainLayer layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath);
            if (layer == null)
            {
                layer = new TerrainLayer();
                AssetDatabase.CreateAsset(layer, layerPath);
            }

            layer.name = name;
            layer.diffuseTexture = texture;
            layer.tileSize = new Vector2(18f, 18f);
            EditorUtility.SetDirty(layer);
            return layer;
        }

        private static float[,] GenerateHeights()
        {
            float[,] heights = new float[HeightmapResolution, HeightmapResolution];
            for (int z = 0; z < HeightmapResolution; z++)
            {
                for (int x = 0; x < HeightmapResolution; x++)
                {
                    Vector2 point = HeightmapPointToWorld(x, z);
                    heights[z, x] = Mathf.Clamp01(HeightAt(point) / TerrainHeight);
                }
            }

            return heights;
        }

        private static float[,,] GenerateAlphamaps(TerrainData terrainData)
        {
            int width = terrainData.alphamapWidth;
            int height = terrainData.alphamapHeight;
            int layers = terrainData.terrainLayers.Length;
            float[,,] maps = new float[height, width, layers];

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 point = AlphamapPointToWorld(x, z, width, height);
                    float elevation = HeightAt(point);
                    float path = PathInfluence(point);
                    float water = Mathf.Max(LakeInfluence(point), CreekInfluence(point));
                    float rock = Mathf.InverseLerp(24f, 46f, elevation);

                    maps[z, x, 0] = Mathf.Clamp01(1f - path * 0.75f - water * 0.8f - rock * 0.35f);
                    maps[z, x, 1] = Mathf.Clamp01(path);
                    maps[z, x, 2] = Mathf.Clamp01(rock * 0.8f + ShorelineInfluence(point) * 0.45f);
                    maps[z, x, 3] = Mathf.Clamp01(water);

                    NormalizeAlpha(maps, z, x, layers);
                }
            }

            return maps;
        }

        private static Terrain BuildWorldHierarchy(TerrainData terrainData)
        {
            GameObject existingWorld = GameObject.Find(WorldRootName);
            if (existingWorld != null)
            {
                UnityEngine.Object.DestroyImmediate(existingWorld);
            }

            GameObject world = new(WorldRootName);
            GameObject terrainRoot = EnsureChild(world.transform, "Terrain");
            EnsureChild(world.transform, "Graybox");
            EnsureChild(world.transform, "TreeCollectionForest");
            EnsureChild(world.transform, "Landmarks");

            GameObject terrainObject = new(PrimaryTerrainName, typeof(Terrain), typeof(TerrainCollider));
            terrainObject.transform.SetParent(terrainRoot.transform, false);
            terrainObject.transform.position = new Vector3(TerrainOrigin, 0f, TerrainOrigin);
            terrainObject.isStatic = true;

            Terrain terrain = terrainObject.GetComponent<Terrain>();
            terrain.terrainData = terrainData;
            terrain.drawInstanced = true;
            terrain.allowAutoConnect = false;
            terrainObject.GetComponent<TerrainCollider>().terrainData = terrainData;
            return terrain;
        }

        private static void ConfigureTerrain(Terrain terrain, TerrainData terrainData)
        {
            terrain.terrainData = terrainData;
            terrain.GetComponent<TerrainCollider>().terrainData = terrainData;
        }

        private static void BuildGraybox(Terrain terrain)
        {
            Transform graybox = GameObject.Find($"{WorldRootName}/Graybox").transform;
            Transform lakeRoot = EnsureChild(graybox, "Lake").transform;
            Transform creekRoot = EnsureChild(graybox, "Creek").transform;
            Transform pathsRoot = EnsureChild(graybox, "Paths").transform;
            Transform startRoot = EnsureChild(graybox, "Start").transform;
            Transform bridgeRoot = EnsureChild(graybox, "Bridge").transform;
            Transform blockersRoot = EnsureChild(graybox, "BoundaryBlockers").transform;

            Material water = EnsureMaterial("Sprint55B_TempWater", new Color(0.25f, 0.48f, 0.58f, 0.64f));
            Material marker = EnsureMaterial("Sprint55B_GrayboxMarker", new Color(0.62f, 0.64f, 0.59f, 1f));
            Material blocker = EnsureMaterial("Sprint55B_SightlineBlocker", new Color(0.34f, 0.39f, 0.37f, 1f));
            Material platform = EnsureMaterial("Sprint55B_StartPlatform", new Color(0.38f, 0.34f, 0.27f, 1f));

            CreateMeshSurface(lakeRoot, "GB_Lake_TempWater", CreateLakeMesh(terrain), water);
            CreateMeshSurface(creekRoot, "GB_Creek_TempWater", CreateCreekMesh(terrain), water);
            CreateSolidPlatform(startRoot, "Start Platform", AnchorPosition("LM_SpawnMeadow"), new Vector3(18f, 0.18f, 14f), platform, terrain);

            CreateMarker(pathsRoot, "GB_SpawnMeadow_Readability", new Vector3(0f, 0f, -150f), new Vector3(18f, 0.08f, 12f), marker, terrain);
            CreateMarker(pathsRoot, "GB_NPCGrove_Readability", new Vector3(-18f, 0f, -108f), new Vector3(15f, 0.08f, 12f), marker, terrain);
            CreateMarker(pathsRoot, "GB_FinalHill_Summit", new Vector3(18f, 0f, 145f), new Vector3(20f, 0.08f, 16f), marker, terrain);

            CreateBlocker(blockersRoot, "GB_FutureForestMass_CentralFold", new Vector3(-22f, 0f, 26f), new Vector3(34f, 15f, 18f), 18f, blocker, terrain);
            CreateBlocker(blockersRoot, "GB_FutureForestMass_LightGroveSouth", new Vector3(-78f, 0f, 54f), new Vector3(34f, 14f, 20f), -28f, blocker, terrain);
            CreateBlocker(blockersRoot, "GB_FutureForestMass_LakeWest", new Vector3(70f, 0f, 52f), new Vector3(28f, 12f, 16f), 34f, blocker, terrain);
            CreateBlocker(blockersRoot, "GB_FutureForestMass_FinalApproach", new Vector3(-8f, 0f, 105f), new Vector3(44f, 17f, 18f), -10f, blocker, terrain);
            CreateBlocker(blockersRoot, "GB_NaturalEdge_NorthRidge", new Vector3(0f, 0f, 214f), new Vector3(300f, 20f, 22f), 0f, blocker, terrain);
            CreateBlocker(blockersRoot, "GB_NaturalEdge_WestRise", new Vector3(-214f, 0f, -8f), new Vector3(22f, 18f, 300f), 0f, blocker, terrain);

            pathsRoot.gameObject.SetActive(false);
            blockersRoot.gameObject.SetActive(false);

            Transform bridge = EnsureSmallBridge().transform;
            bridge.SetParent(bridgeRoot, true);
        }

        private static void PlaceLandmarks(Terrain terrain)
        {
            Transform landmarksRoot = GameObject.Find($"{WorldRootName}/Landmarks").transform;
            foreach (LayoutAnchor anchor in Anchors)
            {
                GameObject landmark = new(anchor.Name);
                landmark.transform.SetParent(landmarksRoot, false);
                landmark.transform.position = WithTerrainY(anchor.Position, terrain, 0.08f);
            }
        }

        private static void AlignTraversalObjects(Terrain terrain)
        {
            MoveToTerrain("Player Spawn Point", AnchorPosition("LM_SpawnMeadow"), terrain, 0.24f);
            MoveToTerrain("PlayerFox", AnchorPosition("LM_SpawnMeadow"), terrain, 0.24f);
            MoveToTerrain("NPC_Guide", AnchorPosition("LM_NPCGrove"), terrain, 0.02f);
            MoveToTerrain("Camp Placeholder", AnchorPosition("LM_FinalHill"), terrain, 0.05f);

            GameObject bridge = GameObject.Find("Small Bridge");
            if (bridge != null)
            {
                Vector3 bridgePosition = WithTerrainY(AnchorPosition("LM_Bridge"), terrain, 0.98f);
                bridge.transform.SetPositionAndRotation(bridgePosition, Quaternion.Euler(0f, BridgeYaw, 0f));
                bridge.transform.localScale = Vector3.one;
            }
        }

        private static void PreserveExistingSmallBridge()
        {
            GameObject bridge = FindSceneObjectIncludingInactive("Small Bridge");
            if (bridge != null)
            {
                bridge.transform.SetParent(null, true);
                bridge.SetActive(true);
            }
        }

        private static GameObject EnsureSmallBridge()
        {
            GameObject bridge = FindSceneObjectIncludingInactive("Small Bridge");
            if (bridge == null)
            {
                Material bridgeMaterial = EnsureMaterial("Sprint55B_GrayboxBridge", new Color(0.43f, 0.36f, 0.28f, 1f));
                bridge = new GameObject("Small Bridge");

                GameObject walkway = GameObject.CreatePrimitive(PrimitiveType.Cube);
                walkway.name = "Bridge Walkway";
                walkway.transform.SetParent(bridge.transform, false);
                walkway.transform.localPosition = new Vector3(0f, 0.14f, 0f);
                walkway.transform.localScale = new Vector3(BridgeDeckWidth, 0.28f, BridgeDeckLength);
                walkway.GetComponent<Renderer>().sharedMaterial = bridgeMaterial;
            }

            EnsureBridgeRailCollider(bridge.transform, "Bridge Left Rail Collider", -BridgeRailX);
            EnsureBridgeRailCollider(bridge.transform, "Bridge Right Rail Collider", BridgeRailX);
            bridge.SetActive(true);
            return bridge;
        }

        private static void EnsureBridgeRailCollider(Transform bridge, string name, float localX)
        {
            Transform rail = bridge.Find(name);
            if (rail == null)
            {
                rail = new GameObject(name).transform;
                rail.SetParent(bridge, false);
            }

            rail.localPosition = new Vector3(localX, 0.6f, 0f);
            rail.localRotation = Quaternion.identity;
            rail.localScale = Vector3.one;
            BoxCollider collider = rail.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = rail.gameObject.AddComponent<BoxCollider>();
            }

            collider.isTrigger = false;
            collider.size = new Vector3(0.2f, 1f, BridgeDeckLength - 0.8f);
            collider.center = Vector3.zero;
        }

        private static void DisableArenaBlockoutVisuals()
        {
            string[] names =
            {
                "North Boundary Ridge",
                "South Boundary Ridge",
                "East Boundary Ridge",
                "West Boundary Ridge",
                "Safe Clearing Ground",
                "Blocked Placeholder Pond",
                "Pond North Stones",
                "Pond South Stones",
                "Path Start To Bridge",
                "Path Bridge To Camp",
                "Path To Pond"
            };

            for (int i = 0; i < names.Length; i++)
            {
                GameObject gameObject = GameObject.Find(names[i]);
                if (gameObject != null)
                {
                    gameObject.SetActive(false);
                    EditorUtility.SetDirty(gameObject);
                }
            }
        }

        private static Mesh CreateLakeMesh(Terrain terrain)
        {
            Vector2[] outline =
            {
                new(94f, 42f), new(108f, 22f), new(135f, 18f), new(164f, 31f),
                new(178f, 58f), new(166f, 84f), new(141f, 97f), new(112f, 89f),
                new(94f, 69f)
            };

            return CreateFanMesh("Sprint55B_LakeMesh", outline, new Vector2(136f, 57f), terrain, 0.18f);
        }

        private static Mesh CreateCreekMesh(Terrain terrain)
        {
            Vector2[] center =
            {
                new(104f, 34f), new(70f, 18f), new(38f, -2f), new(17f, -30f), new(1f, -62f)
            };

            List<Vector3> vertices = new();
            List<int> triangles = new();
            for (int i = 0; i < center.Length; i++)
            {
                Vector2 forward = i == center.Length - 1 ? center[i] - center[i - 1] : center[i + 1] - center[i];
                Vector2 normal = new(-forward.normalized.y, forward.normalized.x);
                float halfWidth = i == 0 ? 5.5f : 4.2f;
                Vector2 left = center[i] + normal * halfWidth;
                Vector2 right = center[i] - normal * halfWidth;
                vertices.Add(ToWaterVertex(left, terrain, 0.2f));
                vertices.Add(ToWaterVertex(right, terrain, 0.2f));
            }

            for (int i = 0; i < center.Length - 1; i++)
            {
                int start = i * 2;
                triangles.Add(start);
                triangles.Add(start + 2);
                triangles.Add(start + 1);
                triangles.Add(start + 1);
                triangles.Add(start + 2);
                triangles.Add(start + 3);
            }

            Mesh mesh = new() { name = "Sprint55B_CreekMesh" };
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateFanMesh(string name, Vector2[] outline, Vector2 center, Terrain terrain, float yOffset)
        {
            List<Vector3> vertices = new() { ToWaterVertex(center, terrain, yOffset) };
            for (int i = 0; i < outline.Length; i++)
            {
                vertices.Add(ToWaterVertex(outline[i], terrain, yOffset));
            }

            List<int> triangles = new();
            for (int i = 1; i <= outline.Length; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i == outline.Length ? 1 : i + 1);
            }

            Mesh mesh = new() { name = name };
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Vector3 ToWaterVertex(Vector2 point, Terrain terrain, float yOffset)
        {
            return WithTerrainY(new Vector3(point.x, 0f, point.y), terrain, yOffset);
        }

        private static void CreateMeshSurface(Transform parent, string name, Mesh mesh, Material material)
        {
            GameObject surface = new(name, typeof(MeshFilter), typeof(MeshRenderer));
            surface.transform.SetParent(parent, false);
            surface.GetComponent<MeshFilter>().sharedMesh = mesh;
            surface.GetComponent<MeshRenderer>().sharedMaterial = material;
        }

        private static void CreateMarker(Transform parent, string name, Vector3 position, Vector3 scale, Material material, Terrain terrain)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = name;
            marker.transform.SetParent(parent, false);
            marker.transform.position = WithTerrainY(position, terrain, 0.05f);
            marker.transform.localScale = scale;
            marker.GetComponent<Renderer>().sharedMaterial = material;
            UnityEngine.Object.DestroyImmediate(marker.GetComponent<Collider>());
        }

        private static void CreateBlocker(Transform parent, string name, Vector3 position, Vector3 scale, float yaw, Material material, Terrain terrain)
        {
            GameObject blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blocker.name = name;
            blocker.transform.SetParent(parent, false);
            Vector3 terrainPosition = WithTerrainY(position, terrain, 0f);
            blocker.transform.SetPositionAndRotation(terrainPosition + Vector3.up * (scale.y * 0.5f), Quaternion.Euler(0f, yaw, 0f));
            blocker.transform.localScale = scale;
            blocker.GetComponent<Renderer>().sharedMaterial = material;
            UnityEngine.Object.DestroyImmediate(blocker.GetComponent<Collider>());
        }

        private static void CreateSolidPlatform(Transform parent, string name, Vector3 position, Vector3 scale, Material material, Terrain terrain)
        {
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = name;
            platform.transform.SetParent(parent, false);
            Vector3 terrainPosition = WithTerrainY(position, terrain, 0f);
            platform.transform.position = terrainPosition + Vector3.up * (scale.y * 0.5f);
            platform.transform.localScale = scale;
            platform.GetComponent<Renderer>().sharedMaterial = material;

            BoxCollider collider = platform.GetComponent<BoxCollider>();
            collider.isTrigger = false;
            collider.size = Vector3.one;
            collider.center = Vector3.zero;
        }

        private static void BuildTreeCollectionForest(Terrain terrain)
        {
            Transform forestRoot = GameObject.Find($"{WorldRootName}/TreeCollectionForest").transform;
            List<GameObject> prefabs = LoadTreeCollectionPrefabs();
            if (prefabs.Count == 0)
            {
                Debug.LogWarning("TreePackVol.1 prefableri bulunamadı; Sprint 5.5-B orman yerleşimi atlandı.");
                return;
            }

            int placed = 0;
            for (int i = 0; i < TreeClusters.Length; i++)
            {
                placed += PlaceTreeCluster(forestRoot, terrain, prefabs, TreeClusters[i], 971 + i * 113);
            }

            if (placed < MinimumTreeCollectionInstances)
            {
                Debug.LogWarning($"Sprint 5.5-B Tree Collection forest only placed {placed} trees; expected at least {MinimumTreeCollectionInstances}.");
            }

            Debug.Log($"Sprint 5.5-B Tree Collection forest placed {placed} TreePackVol.1 trees.");
        }

        private static List<GameObject> LoadTreeCollectionPrefabs()
        {
            List<GameObject> prefabs = new();
            for (int i = 0; i < TreeCollectionPrefabPaths.Length; i++)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TreeCollectionPrefabPaths[i]);
                if (prefab != null)
                {
                    prefabs.Add(prefab);
                }
            }

            return prefabs;
        }

        private static int PlaceTreeCluster(Transform forestRoot, Terrain terrain, IReadOnlyList<GameObject> prefabs, TreeCluster cluster, int seed)
        {
            Transform clusterRoot = EnsureChild(forestRoot, cluster.Name).transform;
            int placed = 0;
            int attempts = cluster.Count * 8;
            for (int i = 0; i < attempts && placed < cluster.Count; i++)
            {
                Vector2 point = SampleClusterPoint(cluster, seed + i * 31);
                if (!IsTreePlacementAllowed(point, cluster.Center, cluster.ClearingRadius))
                {
                    continue;
                }

                GameObject prefab = prefabs[(seed + placed * 7) % prefabs.Count];
                GameObject instance = PrefabUtility.InstantiatePrefab(prefab, clusterRoot) as GameObject;
                if (instance == null)
                {
                    instance = UnityEngine.Object.Instantiate(prefab, clusterRoot);
                }

                instance.name = $"TreePack_{cluster.Name}_{placed + 1:000}";
                float scale = Mathf.Lerp(cluster.MinScale, cluster.MaxScale, Deterministic01(seed + placed * 41));
                Vector3 position = WithTerrainY(new Vector3(point.x, 0f, point.y), terrain, -0.03f);
                Quaternion rotation = Quaternion.Euler(0f, Deterministic01(seed + placed * 53) * 360f, 0f);
                instance.transform.SetPositionAndRotation(position, rotation);
                instance.transform.localScale = Vector3.one * scale;
                RemoveTreeColliders(instance);
                placed++;
            }

            return placed;
        }

        private static Vector2 SampleClusterPoint(TreeCluster cluster, int seed)
        {
            float angle = Deterministic01(seed) * Mathf.PI * 2f;
            float radius = Mathf.Sqrt(Deterministic01(seed + 17));
            return cluster.Center + new Vector2(Mathf.Cos(angle) * cluster.Radius.x * radius, Mathf.Sin(angle) * cluster.Radius.y * radius);
        }

        private static bool IsTreePlacementAllowed(Vector2 point, Vector2 clusterCenter, float clearingRadius)
        {
            if (Mathf.Abs(point.x) > 238f || Mathf.Abs(point.y) > 238f)
            {
                return false;
            }

            if (clearingRadius > 0f && Vector2.Distance(point, clusterCenter) < clearingRadius)
            {
                return false;
            }

            if (Vector2.Distance(point, new Vector2(0f, -150f)) < 24f)
            {
                return false;
            }

            if (Vector2.Distance(point, new Vector2(24.6f, -19.9f)) < 14f)
            {
                return false;
            }

            if (PathInfluence(point) > 0.42f || LakeInfluence(point) > 0.16f || CreekInfluence(point) > 0.28f)
            {
                return false;
            }

            return true;
        }

        private static void RemoveTreeColliders(GameObject instance)
        {
            Collider[] colliders = instance.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                UnityEngine.Object.DestroyImmediate(colliders[i]);
            }
        }

        private static float Deterministic01(int value)
        {
            uint x = (uint)value;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            return (x & 0x00FFFFFF) / (float)0x01000000;
        }

        private static void MoveToTerrain(string name, Vector3 position, Terrain terrain, float yOffset)
        {
            GameObject gameObject = GameObject.Find(name);
            if (gameObject == null)
            {
                return;
            }

            gameObject.transform.position = WithTerrainY(position, terrain, yOffset);
            EditorUtility.SetDirty(gameObject);
        }

        private static Vector3 AnchorPosition(string name)
        {
            for (int i = 0; i < Anchors.Length; i++)
            {
                if (Anchors[i].Name == name)
                {
                    return Anchors[i].Position;
                }
            }

            return Vector3.zero;
        }

        private static Vector3 WithTerrainY(Vector3 position, Terrain terrain, float yOffset)
        {
            position.y = terrain.SampleHeight(position) + terrain.transform.position.y + yOffset;
            return position;
        }

        private static float HeightAt(Vector2 point)
        {
            float northRise = Mathf.InverseLerp(-170f, 170f, point.y) * 15f;
            float baseHeight = 7.2f + northRise;

            float finalHill = 26f * Gaussian(point, new Vector2(20f, 145f), 78f, 58f);
            float lightGroveBowl = 5f * Gaussian(point, new Vector2(-58f, 84f), 50f, 38f);
            float heartShelf = 6f * Gaussian(point, new Vector2(-158f, -44f), 56f, 46f);
            float centralFold = 9.5f * Gaussian(point, new Vector2(-18f, 32f), 46f, 34f);
            float lakeDepression = 15.5f * LakeInfluence(point);
            float creekChannel = 6.5f * CreekInfluence(point);
            float edgeRise = 16f * EdgeInfluence(point);
            float handcraftedUndulation = 1.7f * Mathf.Sin((point.x + 24f) * 0.027f) + 1.1f * Mathf.Sin((point.y - 16f) * 0.021f);

            float height = baseHeight + finalHill + lightGroveBowl + heartShelf + centralFold + edgeRise + handcraftedUndulation - lakeDepression - creekChannel;
            return Mathf.Clamp(height, 3f, 54f);
        }

        private static float PathInfluence(Vector2 point)
        {
            Vector2[] main =
            {
                new(0f, -150f), new(-18f, -108f), new(60f, -62f), new(24.6f, -19.9f),
                new(24f, 62f), new(18f, 145f)
            };

            Vector2[] heart =
            {
                new(-18f, -108f), new(-84f, -92f), new(-158f, -44f), new(-94f, 4f), new(-58f, 84f)
            };

            Vector2[] light =
            {
                new(24.6f, -19.9f), new(6f, 30f), new(-58f, 84f)
            };

            Vector2[] lake =
            {
                new(24.6f, -19.9f), new(82f, 16f), new(136f, 56f), new(74f, 88f)
            };

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

        private static float ShorelineInfluence(Vector2 point)
        {
            float lake = LakeInfluence(point);
            float creek = CreekInfluence(point);
            return Mathf.Clamp01(1f - Mathf.Abs(lake - 0.22f) * 5f) * 0.7f + creek * 0.25f;
        }

        private static float CreekInfluence(Vector2 point)
        {
            Vector2[] creek =
            {
                new(104f, 34f), new(70f, 18f), new(38f, -2f), new(17f, -30f), new(1f, -62f)
            };

            float distance = DistanceToPolyline(point, creek);
            return Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(3.5f, 13f, distance));
        }

        private static float EdgeInfluence(Vector2 point)
        {
            float distanceToEdge = Mathf.Min(TerrainSize * 0.5f - Mathf.Abs(point.x), TerrainSize * 0.5f - Mathf.Abs(point.y));
            return Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(18f, 62f, distanceToEdge));
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

        private static Vector2 HeightmapPointToWorld(int x, int z)
        {
            float worldX = TerrainOrigin + (x / (float)(HeightmapResolution - 1)) * TerrainSize;
            float worldZ = TerrainOrigin + (z / (float)(HeightmapResolution - 1)) * TerrainSize;
            return new Vector2(worldX, worldZ);
        }

        private static Vector2 AlphamapPointToWorld(int x, int z, int width, int height)
        {
            float worldX = TerrainOrigin + (x / (float)(width - 1)) * TerrainSize;
            float worldZ = TerrainOrigin + (z / (float)(height - 1)) * TerrainSize;
            return new Vector2(worldX, worldZ);
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

        private static Material EnsureMaterial(string name, Color color)
        {
            string path = $"{TerrainFolder}/{name}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }

            material.name = name;
            material.color = color;
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            if (material.HasProperty("_Surface"))
            {
                material.SetFloat("_Surface", color.a < 0.99f ? 1f : 0f);
            }

            EditorUtility.SetDirty(material);
            return material;
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

        private static void ImportTreePackIfPresent()
        {
            if (AssetDatabase.IsValidFolder(TreePackRoot))
            {
                AssetDatabase.ImportAsset(TreePackRoot, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ImportRecursive);
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

        public readonly struct LayoutAnchor
        {
            public LayoutAnchor(string name, Vector3 position)
            {
                Name = name;
                Position = position;
            }

            public string Name { get; }
            public Vector3 Position { get; }
        }

        private readonly struct TreeCluster
        {
            public TreeCluster(string name, Vector2 center, Vector2 radius, int count, float minScale, float maxScale, float clearingRadius)
            {
                Name = name;
                Center = center;
                Radius = radius;
                Count = count;
                MinScale = minScale;
                MaxScale = maxScale;
                ClearingRadius = clearingRadius;
            }

            public string Name { get; }
            public Vector2 Center { get; }
            public Vector2 Radius { get; }
            public int Count { get; }
            public float MinScale { get; }
            public float MaxScale { get; }
            public float ClearingRadius { get; }
        }
    }
}
