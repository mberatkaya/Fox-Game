using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TilkiMacera;

public static class TilkiAdventureBuilder
{
    private const string ScenePath = "Assets/Scenes/Main.unity";

    [MenuItem("Tools/Tilki Macera/Build Prototype Scene")]
    public static void BuildPrototypeScene()
    {
        BuildPrototypeScene(showDialog: true);
    }

    public static void BuildPrototypeScene(bool showDialog)
    {
        EnsureFolders();
        Material grass = CreateMaterial("Grass", new Color(0.34f, 0.57f, 0.28f));
        Material path = CreateMaterial("Path", new Color(0.66f, 0.52f, 0.32f));
        Material water = CreateMaterial("Water", new Color(0.22f, 0.54f, 0.78f, 0.72f));
        Material bark = CreateMaterial("Bark", new Color(0.36f, 0.2f, 0.1f));
        Material leaves = CreateMaterial("Leaves", new Color(0.12f, 0.45f, 0.22f));
        Material foxOrange = CreateMaterial("Fox Orange", new Color(0.95f, 0.38f, 0.08f));
        Material foxCream = CreateMaterial("Fox Cream", new Color(1f, 0.82f, 0.58f));
        Material dark = CreateMaterial("Soft Dark", new Color(0.08f, 0.06f, 0.05f));
        Material glow = CreateMaterial("Warm Glow", new Color(1f, 0.78f, 0.22f));
        Material stone = CreateMaterial("Stone", new Color(0.48f, 0.47f, 0.43f));

        QuestData[] quests = CreateQuestAssets();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(0.58f, 0.62f, 0.72f);

        CreateLighting();
        GameObject player = CreatePlayer(foxOrange, foxCream, dark);
        Camera camera = CreateCamera(player.transform);
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.cameraTransform = camera.transform;

        GameObject systems = new GameObject("Game Systems");
        QuestManager questManager = systems.AddComponent<QuestManager>();
        questManager.quests = new List<QuestData>(quests);
        MinigameManager minigameManager = systems.AddComponent<MinigameManager>();
        minigameManager.player = controller;
        systems.AddComponent<DialogueManager>();
        systems.AddComponent<UIController>();

        CreateWorld(grass, path, water, bark, leaves, stone);
        CreateQuestGiver(glow, stone);
        CreateCollectibles(glow);
        CreateLightPathNodes(glow);
        CreateFinalShrine(glow, stone);

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true)
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (showDialog)
        {
            EditorUtility.DisplayDialog("Tilki Macera", "Prototype scene hazir: Assets/Scenes/Main.unity", "Tamam");
        }
    }

    private static void EnsureFolders()
    {
        Directory.CreateDirectory("Assets/Art/Materials");
        Directory.CreateDirectory("Assets/Data");
        Directory.CreateDirectory("Assets/Scenes");
    }

    private static QuestData[] CreateQuestAssets()
    {
        return new[]
        {
            CreateQuest("01_KayipParcalar", "kayip_parcalar", "Kayip Parcalar",
                "Ormandaki 5 ani parcasini bul.",
                QuestKind.CollectMemories, 5,
                new[]
                {
                    "Bu ormanda bazi kucuk anilar sakli.",
                    "Tilki burnunu ve kalbini takip ederse hepsini bulabilir."
                },
                new[]
                {
                    "Bes ani parcasi yeniden bir araya geldi.",
                    "Orman artik biraz daha aydinlik."
                }),
            CreateQuest("02_IsikYolu", "isik_yolu", "Isik Yolu",
                "Parlayan isiklari dogru sirayla takip et.",
                QuestKind.LightPath, 1,
                new[]
                {
                    "Simdi isiklar sana yolu gosterecek.",
                    "Sira bozulmadan hepsine dokun."
                },
                new[]
                {
                    "Isik yolu seni tanidi.",
                    "Bir sonraki kapinin kilidi acildi."
                }),
            CreateQuest("03_KalpBahcesi", "kalp_bahcesi", "Kalp Bahcesi",
                "Bahcedeki 4 ani esini bul.",
                QuestKind.HeartGarden, 1,
                new[]
                {
                    "Kalp Bahcesi ayni anilari yan yana ister.",
                    "Kartlari ac ve esleri bul."
                },
                new[]
                {
                    "Bahcedeki butun esler bulundu.",
                    "Gizli final seni kamp alaninda bekliyor."
                })
        };
    }

    private static QuestData CreateQuest(string assetName, string id, string title, string description, QuestKind kind, int requiredCount, string[] intro, string[] completion)
    {
        string path = $"Assets/Data/{assetName}.asset";
        QuestData quest = AssetDatabase.LoadAssetAtPath<QuestData>(path);
        if (quest == null)
        {
            quest = ScriptableObject.CreateInstance<QuestData>();
            AssetDatabase.CreateAsset(quest, path);
        }

        quest.questId = id;
        quest.title = title;
        quest.description = description;
        quest.kind = kind;
        quest.requiredCount = requiredCount;
        quest.introLines = intro;
        quest.completionLines = completion;
        EditorUtility.SetDirty(quest);
        return quest;
    }

    private static void CreateLighting()
    {
        GameObject sun = new GameObject("Warm Sun");
        Light light = sun.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.15f;
        light.color = new Color(1f, 0.89f, 0.73f);
        sun.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
    }

    private static Camera CreateCamera(Transform player)
    {
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener), typeof(CameraController));
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.fieldOfView = 60f;
        CameraController cameraController = cameraObject.GetComponent<CameraController>();
        cameraController.target = player;
        cameraObject.transform.position = new Vector3(0f, 4f, -7f);
        return camera;
    }

    private static GameObject CreatePlayer(Material orange, Material cream, Material dark)
    {
        GameObject player = new GameObject("Tilki Player", typeof(CharacterController), typeof(PlayerController), typeof(PlayerInteractor));
        player.transform.position = new Vector3(0f, 0.2f, -8f);
        CharacterController characterController = player.GetComponent<CharacterController>();
        characterController.height = 1.2f;
        characterController.radius = 0.36f;
        characterController.center = new Vector3(0f, 0.62f, 0f);
        BuildFoxVisual(player.transform, orange, cream, dark);
        return player;
    }

    private static void BuildFoxVisual(Transform parent, Material orange, Material cream, Material dark)
    {
        GameObject root = new GameObject("Low Poly Fox Visual");
        root.transform.SetParent(parent, false);

        GameObject body = Primitive("Body", PrimitiveType.Capsule, root.transform, new Vector3(0f, 0.58f, 0f), new Vector3(0.72f, 0.56f, 1.12f), orange);
        body.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        RemoveCollider(body);

        GameObject chest = Primitive("Chest", PrimitiveType.Sphere, root.transform, new Vector3(0f, 0.56f, -0.34f), new Vector3(0.48f, 0.36f, 0.3f), cream);
        RemoveCollider(chest);

        GameObject head = Primitive("Head", PrimitiveType.Sphere, root.transform, new Vector3(0f, 0.96f, 0.54f), new Vector3(0.5f, 0.42f, 0.48f), orange);
        RemoveCollider(head);

        GameObject snout = Primitive("Snout", PrimitiveType.Sphere, root.transform, new Vector3(0f, 0.9f, 0.84f), new Vector3(0.28f, 0.18f, 0.24f), cream);
        RemoveCollider(snout);

        GameObject nose = Primitive("Nose", PrimitiveType.Sphere, root.transform, new Vector3(0f, 0.93f, 1f), new Vector3(0.08f, 0.06f, 0.06f), dark);
        RemoveCollider(nose);

        CreateCone("Left Ear", root.transform, new Vector3(-0.18f, 1.28f, 0.44f), 0.16f, 0.34f, orange);
        CreateCone("Right Ear", root.transform, new Vector3(0.18f, 1.28f, 0.44f), 0.16f, 0.34f, orange);

        for (int i = 0; i < 4; i++)
        {
            float x = i < 2 ? -0.24f : 0.24f;
            float z = i % 2 == 0 ? -0.32f : 0.34f;
            GameObject leg = Primitive($"Leg {i + 1}", PrimitiveType.Capsule, root.transform, new Vector3(x, 0.25f, z), new Vector3(0.16f, 0.42f, 0.16f), dark);
            RemoveCollider(leg);
        }

        GameObject tail = Primitive("Tail", PrimitiveType.Capsule, root.transform, new Vector3(0f, 0.72f, -0.78f), new Vector3(0.24f, 0.24f, 0.82f), orange);
        tail.transform.rotation = Quaternion.Euler(58f, 0f, 0f);
        RemoveCollider(tail);

        GameObject tailTip = Primitive("Tail Tip", PrimitiveType.Sphere, root.transform, new Vector3(0f, 1.02f, -1.08f), new Vector3(0.26f, 0.2f, 0.24f), cream);
        RemoveCollider(tailTip);
    }

    private static void CreateWorld(Material grass, Material path, Material water, Material bark, Material leaves, Material stone)
    {
        GameObject ground = Primitive("Forest Clearing", PrimitiveType.Plane, null, Vector3.zero, new Vector3(10f, 1f, 10f), grass);
        ground.isStatic = true;

        for (int i = 0; i < 8; i++)
        {
            GameObject pathTile = Primitive($"Path Tile {i + 1}", PrimitiveType.Cube, null, new Vector3(0f, 0.02f, -8f + i * 2.1f), new Vector3(2.2f, 0.04f, 1.3f), path);
            pathTile.transform.rotation = Quaternion.Euler(0f, Mathf.Sin(i * 0.7f) * 10f, 0f);
        }

        GameObject pond = Primitive("Small Pond", PrimitiveType.Cylinder, null, new Vector3(-10f, 0.05f, 8f), new Vector3(5.5f, 0.08f, 3.2f), water);
        pond.transform.rotation = Quaternion.Euler(0f, 28f, 0f);

        CreateBoundary(stone);
        Vector3[] treePositions =
        {
            new Vector3(-12f, 0f, -9f), new Vector3(-15f, 0f, 2f), new Vector3(-10f, 0f, 17f),
            new Vector3(12f, 0f, -13f), new Vector3(17f, 0f, -2f), new Vector3(14f, 0f, 14f),
            new Vector3(-3f, 0f, 18f), new Vector3(5f, 0f, -17f), new Vector3(-19f, 0f, -1f)
        };

        foreach (Vector3 position in treePositions)
        {
            CreateTree(position, bark, leaves);
        }
    }

    private static void CreateQuestGiver(Material glow, Material stone)
    {
        GameObject npc = new GameObject("Ay Isigi Quest Giver", typeof(CapsuleCollider), typeof(QuestGiver));
        npc.transform.position = new Vector3(0f, 0.6f, 3.5f);
        CapsuleCollider collider = npc.GetComponent<CapsuleCollider>();
        collider.height = 1.5f;
        collider.radius = 0.5f;

        GameObject body = Primitive("Lantern Body", PrimitiveType.Sphere, npc.transform, Vector3.zero, new Vector3(0.8f, 1.1f, 0.8f), stone);
        RemoveCollider(body);
        GameObject lightOrb = Primitive("Lantern Glow", PrimitiveType.Sphere, npc.transform, new Vector3(0f, 0.72f, 0f), new Vector3(0.46f, 0.46f, 0.46f), glow);
        RemoveCollider(lightOrb);
        Light light = lightOrb.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.74f, 0.32f);
        light.intensity = 1.8f;
        light.range = 5f;
    }

    private static void CreateCollectibles(Material glow)
    {
        Vector3[] positions =
        {
            new Vector3(-5f, 0.75f, -4f),
            new Vector3(4f, 0.75f, -1f),
            new Vector3(-7f, 0.75f, 7f),
            new Vector3(8f, 0.75f, 6f),
            new Vector3(1f, 0.75f, 12f)
        };

        string[] titles = { "Ilk Gulus", "Kucuk Not", "Sakin Gun", "Favori Sarki", "Yildizli Aksam" };
        string[] messages =
        {
            "Bazi anlar kucuktur ama butun ormani isitabilir.",
            "Bir cumle bile dogru kisiden gelince eve donus yolu gibi gelir.",
            "Sessiz bir gunun icinde bile birlikte olmanin sesi vardir.",
            "Bir sarki baslar ve dunya biraz daha bizim olur.",
            "Tilki gokyuzune bakti; finalin isigi oradaydi."
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject memory = Primitive($"Memory {i + 1}", PrimitiveType.Sphere, null, positions[i], new Vector3(0.42f, 0.42f, 0.42f), glow);
            SphereCollider collider = memory.GetComponent<SphereCollider>();
            collider.isTrigger = true;
            CollectibleMemory collectible = memory.AddComponent<CollectibleMemory>();
            collectible.memoryId = $"memory_{i + 1}";
            collectible.title = titles[i];
            collectible.message = messages[i];
            Light light = memory.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.72f, 0.28f);
            light.intensity = 0.9f;
            light.range = 3f;
        }
    }

    private static void CreateLightPathNodes(Material glow)
    {
        Vector3[] positions =
        {
            new Vector3(4f, 0.35f, 4f),
            new Vector3(7f, 0.35f, 7f),
            new Vector3(10f, 0.35f, 5f),
            new Vector3(13f, 0.35f, 8f),
            new Vector3(15f, 0.35f, 12f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject node = Primitive($"Light Path Node {i + 1}", PrimitiveType.Sphere, null, positions[i], new Vector3(0.65f, 0.65f, 0.65f), glow);
            SphereCollider collider = node.GetComponent<SphereCollider>();
            collider.isTrigger = true;
            LightPathNode lightNode = node.AddComponent<LightPathNode>();
            lightNode.sequenceIndex = i;
            lightNode.targetRenderer = node.GetComponent<Renderer>();
            Light light = node.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.8f, 0.35f);
            light.intensity = 1.25f;
            light.range = 4f;
            node.SetActive(false);
        }
    }

    private static void CreateFinalShrine(Material glow, Material stone)
    {
        GameObject shrine = new GameObject("Final Heart Shrine", typeof(BoxCollider), typeof(FinalShrine));
        shrine.transform.position = new Vector3(-6f, 0.8f, 14f);
        BoxCollider collider = shrine.GetComponent<BoxCollider>();
        collider.size = new Vector3(2f, 1.6f, 2f);

        GameObject baseStone = Primitive("Base Stone", PrimitiveType.Cylinder, shrine.transform, new Vector3(0f, -0.28f, 0f), new Vector3(2.4f, 0.35f, 2.4f), stone);
        RemoveCollider(baseStone);
        GameObject heart = Primitive("Final Glow", PrimitiveType.Sphere, shrine.transform, new Vector3(0f, 0.55f, 0f), new Vector3(0.75f, 0.75f, 0.75f), glow);
        RemoveCollider(heart);
        Light light = heart.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.55f, 0.32f);
        light.intensity = 2.5f;
        light.range = 7f;
    }

    private static void CreateTree(Vector3 position, Material bark, Material leaves)
    {
        GameObject tree = new GameObject("Tree");
        tree.transform.position = position;
        GameObject trunk = Primitive("Trunk", PrimitiveType.Cylinder, tree.transform, new Vector3(0f, 1f, 0f), new Vector3(0.45f, 2f, 0.45f), bark);
        RemoveCollider(trunk);
        GameObject crown = Primitive("Crown", PrimitiveType.Sphere, tree.transform, new Vector3(0f, 2.55f, 0f), new Vector3(2.1f, 2.1f, 2.1f), leaves);
        RemoveCollider(crown);
        CapsuleCollider collider = tree.AddComponent<CapsuleCollider>();
        collider.height = 3.2f;
        collider.radius = 0.8f;
        collider.center = new Vector3(0f, 1.6f, 0f);
    }

    private static void CreateBoundary(Material stone)
    {
        for (int i = 0; i < 28; i++)
        {
            float angle = i / 28f * Mathf.PI * 2f;
            Vector3 position = new Vector3(Mathf.Cos(angle) * 48f, 0.35f, Mathf.Sin(angle) * 48f);
            GameObject rock = Primitive($"Boundary Stone {i + 1}", PrimitiveType.Sphere, null, position, new Vector3(1.6f, 0.8f, 1.2f), stone);
            rock.transform.rotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);
        }
    }

    private static Material CreateMaterial(string name, Color color)
    {
        string path = $"Assets/Art/Materials/{name}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static GameObject Primitive(string name, PrimitiveType type, Transform parent, Vector3 localOrWorldPosition, Vector3 scale, Material material)
    {
        GameObject primitive = GameObject.CreatePrimitive(type);
        primitive.name = name;
        if (parent != null)
        {
            primitive.transform.SetParent(parent, false);
            primitive.transform.localPosition = localOrWorldPosition;
        }
        else
        {
            primitive.transform.position = localOrWorldPosition;
        }

        primitive.transform.localScale = scale;
        primitive.GetComponent<Renderer>().sharedMaterial = material;
        return primitive;
    }

    private static GameObject CreateCone(string name, Transform parent, Vector3 localPosition, float radius, float height, Material material)
    {
        GameObject cone = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
        cone.transform.SetParent(parent, false);
        cone.transform.localPosition = localPosition;
        cone.GetComponent<MeshFilter>().sharedMesh = BuildConeMesh(radius, height, 12);
        cone.GetComponent<MeshRenderer>().sharedMaterial = material;
        return cone;
    }

    private static Mesh BuildConeMesh(float radius, float height, int sides)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[sides + 2];
        int[] triangles = new int[sides * 6];
        vertices[0] = Vector3.up * height;
        vertices[1] = Vector3.zero;

        for (int i = 0; i < sides; i++)
        {
            float angle = i / (float)sides * Mathf.PI * 2f;
            vertices[i + 2] = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }

        int triangleIndex = 0;
        for (int i = 0; i < sides; i++)
        {
            int current = i + 2;
            int next = i == sides - 1 ? 2 : i + 3;
            triangles[triangleIndex++] = 0;
            triangles[triangleIndex++] = next;
            triangles[triangleIndex++] = current;
            triangles[triangleIndex++] = 1;
            triangles[triangleIndex++] = current;
            triangles[triangleIndex++] = next;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    private static void RemoveCollider(GameObject target)
    {
        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            UnityEngine.Object.DestroyImmediate(collider);
        }
    }
}
