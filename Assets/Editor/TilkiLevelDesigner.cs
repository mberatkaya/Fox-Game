using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TilkiLevelDesigner
{
    private const string ScenePath = "Assets/Scenes/Main.unity";
    private const string RootName = "Level Design Landmarks";

    [MenuItem("Tools/Tilki Macera/Enhance Level Design")]
    public static void EnhanceMainScene()
    {
        EnhanceMainScene(showDialog: true);
    }

    public static void EnhanceMainScene(bool showDialog)
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath)
        {
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        GameObject existing = GameObject.Find(RootName);
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        Directory.CreateDirectory("Assets/Art/Materials");

        Material wood = Material("Cabin Wood", new Color(0.48f, 0.28f, 0.13f));
        Material woodDark = Material("Cabin Dark Wood", new Color(0.22f, 0.12f, 0.07f));
        Material roof = Material("Berry Roof", new Color(0.55f, 0.16f, 0.18f));
        Material moss = Material("Moss", new Color(0.22f, 0.46f, 0.21f));
        Material reed = Material("Reed", new Color(0.62f, 0.56f, 0.31f));
        Material flowerPink = Material("Flower Pink", new Color(0.95f, 0.43f, 0.58f));
        Material flowerBlue = Material("Flower Blue", new Color(0.35f, 0.56f, 0.92f));
        Material flowerWhite = Material("Flower White", new Color(0.96f, 0.92f, 0.82f));
        Material lantern = Material("Lantern Gold", new Color(1f, 0.66f, 0.18f));
        Material bridge = Material("Bridge Planks", new Color(0.58f, 0.38f, 0.18f));
        Material sign = Material("Sign Text Dark", new Color(0.12f, 0.08f, 0.04f));
        Material pebble = Material("Pebble", new Color(0.58f, 0.57f, 0.51f));
        Material mushroomCap = Material("Mushroom Cap", new Color(0.72f, 0.14f, 0.14f));
        Material mushroomStem = Material("Mushroom Stem", new Color(0.91f, 0.77f, 0.58f));

        GameObject root = new GameObject(RootName);
        CreateEntranceArea(root.transform, wood, woodDark, roof, lantern, sign);
        CreateCampAndFinalArea(root.transform, wood, woodDark, roof, lantern, flowerPink, flowerWhite, pebble);
        CreatePondArea(root.transform, reed, bridge, lantern, pebble, flowerBlue, flowerWhite);
        CreateLightPathArena(root.transform, wood, lantern, pebble, moss);
        CreateHeartGardenArea(root.transform, wood, roof, flowerPink, flowerBlue, flowerWhite, moss);
        CreateForestDressing(root.transform, wood, woodDark, moss, flowerPink, flowerBlue, flowerWhite, mushroomCap, mushroomStem, pebble);
        CreateWayfindingSigns(root.transform, wood, sign);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (showDialog)
        {
            EditorUtility.DisplayDialog("Tilki Macera", "Level design assetleri ve alanlari sahneye eklendi.", "Tamam");
        }
    }

    private static void CreateEntranceArea(Transform root, Material wood, Material woodDark, Material roof, Material lantern, Material sign)
    {
        GameObject area = Group("Entrance Memory Nook", root);
        CreateArch(area.transform, new Vector3(0f, 0f, -11.5f), wood, lantern);
        CreateSign(area.transform, new Vector3(-2.4f, 0.25f, -10.4f), 28f, "Ani Patikasi", wood, sign);
        CreateCabin(area.transform, new Vector3(5.8f, 0f, -8f), 32f, wood, woodDark, roof, "Kucuk Kulube");
        CreateLantern(area.transform, new Vector3(-1.9f, 0.8f, -7.2f), lantern);
        CreateLantern(area.transform, new Vector3(2.1f, 0.8f, -6.7f), lantern);
    }

    private static void CreateCampAndFinalArea(Transform root, Material wood, Material woodDark, Material roof, Material lantern, Material flowerPink, Material flowerWhite, Material pebble)
    {
        GameObject area = Group("Final Camp Clearing", root);
        CreateCampfire(area.transform, new Vector3(-9f, 0f, 12.2f), woodDark, lantern, pebble);
        CreateBench(area.transform, new Vector3(-11.4f, 0f, 12.2f), 90f, wood, woodDark);
        CreateBench(area.transform, new Vector3(-6.6f, 0f, 12.2f), -90f, wood, woodDark);
        CreateGarland(area.transform, new Vector3(-6f, 1.9f, 14f), 4.5f, lantern);
        CreateFlowerPatch(area.transform, new Vector3(-5.2f, 0.05f, 16.4f), flowerPink, flowerWhite, 18, 2.6f);
        CreateSign(area.transform, new Vector3(-9.3f, 0.25f, 15.7f), -20f, "Final Kampi", wood, woodDark);
        CreateCabin(area.transform, new Vector3(-14.8f, 0f, 15.8f), -24f, wood, woodDark, roof, "Hatira Evi");
    }

    private static void CreatePondArea(Transform root, Material reed, Material bridge, Material lantern, Material pebble, Material flowerBlue, Material flowerWhite)
    {
        GameObject area = Group("Pond Detail Area", root);
        for (int i = 0; i < 16; i++)
        {
            float angle = i / 16f * Mathf.PI * 2f;
            Vector3 position = new Vector3(-10f + Mathf.Cos(angle) * 3.2f, 0.15f, 8f + Mathf.Sin(angle) * 2.2f);
            CreateReeds(area.transform, position, angle * Mathf.Rad2Deg, reed);
        }

        CreateBridge(area.transform, new Vector3(-10f, 0.38f, 8f), 28f, bridge);
        CreateLantern(area.transform, new Vector3(-6.9f, 0.9f, 6.2f), lantern);
        CreateLantern(area.transform, new Vector3(-13.1f, 0.9f, 9.8f), lantern);
        CreatePebbleRing(area.transform, new Vector3(-10f, 0.08f, 8f), 4.2f, 2.7f, pebble);
        CreateFlowerPatch(area.transform, new Vector3(-14.2f, 0.05f, 5f), flowerBlue, flowerWhite, 16, 2.3f);
    }

    private static void CreateLightPathArena(Transform root, Material wood, Material lantern, Material pebble, Material moss)
    {
        GameObject area = Group("Light Path Arena Dressing", root);
        CreateSign(area.transform, new Vector3(5.6f, 0.25f, 2.8f), 45f, "Isik Yolu", wood, pebble);

        Vector3[] nodeHints =
        {
            new Vector3(4f, 0.02f, 4f),
            new Vector3(7f, 0.02f, 7f),
            new Vector3(10f, 0.02f, 5f),
            new Vector3(13f, 0.02f, 8f),
            new Vector3(15f, 0.02f, 12f)
        };

        for (int i = 0; i < nodeHints.Length; i++)
        {
            CreateFlatStone(area.transform, nodeHints[i], 0.95f + i * 0.06f, pebble);
            CreateLantern(area.transform, nodeHints[i] + new Vector3(0.8f, 0.4f, -0.55f), lantern);
        }

        CreateLowHill(area.transform, new Vector3(12.4f, 0.03f, 10.4f), new Vector3(5.5f, 0.35f, 4.2f), moss);
    }

    private static void CreateHeartGardenArea(Transform root, Material wood, Material roof, Material flowerPink, Material flowerBlue, Material flowerWhite, Material moss)
    {
        GameObject area = Group("Heart Garden", root);
        CreateLowHill(area.transform, new Vector3(6.4f, 0.03f, 15.2f), new Vector3(5.8f, 0.28f, 4.8f), moss);
        CreatePergola(area.transform, new Vector3(6.3f, 0f, 15.1f), wood, roof);
        CreateFlowerPatch(area.transform, new Vector3(4.3f, 0.1f, 14.2f), flowerPink, flowerWhite, 22, 2.5f);
        CreateFlowerPatch(area.transform, new Vector3(8.3f, 0.1f, 16.1f), flowerBlue, flowerWhite, 22, 2.5f);
        CreateSign(area.transform, new Vector3(6.5f, 0.25f, 12.3f), 0f, "Kalp Bahcesi", wood, roof);
    }

    private static void CreateForestDressing(Transform root, Material wood, Material woodDark, Material moss, Material flowerPink, Material flowerBlue, Material flowerWhite, Material mushroomCap, Material mushroomStem, Material pebble)
    {
        GameObject area = Group("Forest Dressing", root);
        Vector3[] extraTrees =
        {
            new Vector3(-18f, 0f, -14f), new Vector3(-14f, 0f, -17f), new Vector3(-8f, 0f, -16f),
            new Vector3(10f, 0f, -18f), new Vector3(16f, 0f, -15f), new Vector3(20f, 0f, -7f),
            new Vector3(20f, 0f, 8f), new Vector3(17f, 0f, 18f), new Vector3(10f, 0f, 20f),
            new Vector3(-16f, 0f, 20f), new Vector3(-22f, 0f, 10f), new Vector3(-22f, 0f, -8f)
        };

        for (int i = 0; i < extraTrees.Length; i++)
        {
            float scale = 0.85f + (i % 4) * 0.16f;
            CreateStylizedTree(area.transform, extraTrees[i], scale, woodDark, moss);
        }

        CreateFlowerPatch(area.transform, new Vector3(-2.5f, 0.05f, -4.2f), flowerPink, flowerWhite, 14, 1.8f);
        CreateFlowerPatch(area.transform, new Vector3(3.8f, 0.05f, -0.9f), flowerBlue, flowerWhite, 12, 1.6f);
        CreateFlowerPatch(area.transform, new Vector3(-7.1f, 0.05f, 7.4f), flowerPink, flowerBlue, 14, 1.8f);
        CreateFlowerPatch(area.transform, new Vector3(8.1f, 0.05f, 6.4f), flowerWhite, flowerBlue, 14, 1.8f);

        Vector3[] mushroomSpots =
        {
            new Vector3(-3.2f, 0.02f, -9.3f), new Vector3(2.8f, 0.02f, -5.8f), new Vector3(-13f, 0.02f, 3.2f),
            new Vector3(13.7f, 0.02f, -3.4f), new Vector3(11.8f, 0.02f, 13.8f), new Vector3(-15.2f, 0.02f, 13.2f)
        };

        foreach (Vector3 spot in mushroomSpots)
        {
            CreateMushrooms(area.transform, spot, mushroomStem, mushroomCap);
        }

        for (int i = 0; i < 34; i++)
        {
            float angle = i * 37f * Mathf.Deg2Rad;
            float radius = 5f + (i % 9) * 2.4f;
            Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 0.04f, Mathf.Sin(angle) * radius);
            if (position.magnitude < 4f || Vector3.Distance(position, new Vector3(-10f, 0f, 8f)) < 5f)
            {
                continue;
            }

            CreateFlatStone(area.transform, position, 0.35f + (i % 3) * 0.12f, pebble);
        }

        CreateBench(area.transform, new Vector3(1.8f, 0f, 5.8f), 12f, wood, woodDark);
        CreateBench(area.transform, new Vector3(-4.5f, 0f, 2.8f), -35f, wood, woodDark);
    }

    private static void CreateWayfindingSigns(Transform root, Material wood, Material text)
    {
        GameObject area = Group("Wayfinding Signs", root);
        CreateSign(area.transform, new Vector3(1.7f, 0.25f, -2f), -18f, "F ile konus", wood, text);
        CreateSign(area.transform, new Vector3(-6.4f, 0.25f, 8.8f), 44f, "Golete dogru", wood, text);
        CreateSign(area.transform, new Vector3(11.6f, 0.25f, 3.2f), -32f, "Isiklari izle", wood, text);
        CreateSign(area.transform, new Vector3(3.7f, 0.25f, 13.1f), 21f, "Bahce burada", wood, text);
    }

    private static GameObject Group(string name, Transform parent)
    {
        GameObject group = new GameObject(name);
        group.transform.SetParent(parent, false);
        return group;
    }

    private static void CreateCabin(Transform parent, Vector3 position, float yaw, Material wall, Material darkWood, Material roof, string label)
    {
        GameObject cabin = Group(label, parent);
        cabin.transform.position = position;
        cabin.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        Primitive("Cabin Body", PrimitiveType.Cube, cabin.transform, new Vector3(0f, 1f, 0f), new Vector3(3f, 2f, 2.4f), wall);
        Primitive("Cabin Roof", PrimitiveType.Cube, cabin.transform, new Vector3(0f, 2.25f, 0f), new Vector3(3.5f, 0.42f, 2.9f), roof).transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Primitive("Door", PrimitiveType.Cube, cabin.transform, new Vector3(0f, 0.65f, -1.22f), new Vector3(0.7f, 1.25f, 0.08f), darkWood);
        Primitive("Left Window", PrimitiveType.Cube, cabin.transform, new Vector3(-1.05f, 1.25f, -1.24f), new Vector3(0.48f, 0.42f, 0.06f), roof);
        Primitive("Right Window", PrimitiveType.Cube, cabin.transform, new Vector3(1.05f, 1.25f, -1.24f), new Vector3(0.48f, 0.42f, 0.06f), roof);
    }

    private static void CreateArch(Transform parent, Vector3 position, Material wood, Material lantern)
    {
        GameObject arch = Group("Memory Arch", parent);
        arch.transform.position = position;
        Primitive("Left Post", PrimitiveType.Cylinder, arch.transform, new Vector3(-1.15f, 1f, 0f), new Vector3(0.18f, 1f, 0.18f), wood);
        Primitive("Right Post", PrimitiveType.Cylinder, arch.transform, new Vector3(1.15f, 1f, 0f), new Vector3(0.18f, 1f, 0.18f), wood);
        Primitive("Top Beam", PrimitiveType.Cube, arch.transform, new Vector3(0f, 2.05f, 0f), new Vector3(2.7f, 0.2f, 0.22f), wood);
        CreateLantern(arch.transform, new Vector3(0f, 1.55f, 0f), lantern);
    }

    private static void CreatePergola(Transform parent, Vector3 position, Material wood, Material fabric)
    {
        GameObject pergola = Group("Garden Pergola", parent);
        pergola.transform.position = position;
        for (int x = -1; x <= 1; x += 2)
        {
            for (int z = -1; z <= 1; z += 2)
            {
                Primitive("Post", PrimitiveType.Cylinder, pergola.transform, new Vector3(x * 1.35f, 1.05f, z * 1.05f), new Vector3(0.14f, 1.05f, 0.14f), wood);
            }
        }

        Primitive("Front Beam", PrimitiveType.Cube, pergola.transform, new Vector3(0f, 2.15f, -1.05f), new Vector3(3.1f, 0.18f, 0.18f), wood);
        Primitive("Back Beam", PrimitiveType.Cube, pergola.transform, new Vector3(0f, 2.15f, 1.05f), new Vector3(3.1f, 0.18f, 0.18f), wood);
        Primitive("Canopy", PrimitiveType.Cube, pergola.transform, new Vector3(0f, 2.35f, 0f), new Vector3(3.35f, 0.14f, 2.45f), fabric);
    }

    private static void CreateBench(Transform parent, Vector3 position, float yaw, Material wood, Material darkWood)
    {
        GameObject bench = Group("Bench", parent);
        bench.transform.position = position;
        bench.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        Primitive("Seat", PrimitiveType.Cube, bench.transform, new Vector3(0f, 0.55f, 0f), new Vector3(2f, 0.18f, 0.55f), wood);
        Primitive("Back", PrimitiveType.Cube, bench.transform, new Vector3(0f, 0.98f, 0.28f), new Vector3(2f, 0.55f, 0.14f), wood);
        Primitive("Left Leg", PrimitiveType.Cube, bench.transform, new Vector3(-0.75f, 0.28f, -0.15f), new Vector3(0.16f, 0.55f, 0.16f), darkWood);
        Primitive("Right Leg", PrimitiveType.Cube, bench.transform, new Vector3(0.75f, 0.28f, -0.15f), new Vector3(0.16f, 0.55f, 0.16f), darkWood);
    }

    private static void CreateBridge(Transform parent, Vector3 position, float yaw, Material wood)
    {
        GameObject bridge = Group("Small Bridge", parent);
        bridge.transform.position = position;
        bridge.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        for (int i = -3; i <= 3; i++)
        {
            Primitive("Plank", PrimitiveType.Cube, bridge.transform, new Vector3(i * 0.45f, 0f, 0f), new Vector3(0.38f, 0.12f, 3.2f), wood);
        }

        Primitive("Left Rail", PrimitiveType.Cube, bridge.transform, new Vector3(0f, 0.55f, -1.75f), new Vector3(3.4f, 0.14f, 0.14f), wood);
        Primitive("Right Rail", PrimitiveType.Cube, bridge.transform, new Vector3(0f, 0.55f, 1.75f), new Vector3(3.4f, 0.14f, 0.14f), wood);
    }

    private static void CreateCampfire(Transform parent, Vector3 position, Material log, Material flame, Material stone)
    {
        GameObject campfire = Group("Campfire", parent);
        campfire.transform.position = position;
        CreatePebbleRing(campfire.transform, Vector3.zero, 1.15f, 1.15f, stone);
        for (int i = 0; i < 4; i++)
        {
            GameObject stick = Primitive("Log", PrimitiveType.Cylinder, campfire.transform, new Vector3(0f, 0.22f, 0f), new Vector3(0.12f, 0.9f, 0.12f), log);
            stick.transform.rotation = Quaternion.Euler(76f, i * 45f, 0f);
        }

        GameObject fire = Primitive("Warm Flame", PrimitiveType.Sphere, campfire.transform, new Vector3(0f, 0.58f, 0f), new Vector3(0.45f, 0.72f, 0.45f), flame);
        RemoveCollider(fire);
        Light light = fire.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.48f, 0.18f);
        light.intensity = 2f;
        light.range = 6f;
    }

    private static void CreateLantern(Transform parent, Vector3 localOrWorldPosition, Material material)
    {
        GameObject lantern = Group("Warm Lantern", parent);
        lantern.transform.localPosition = localOrWorldPosition;
        GameObject body = Primitive("Lantern Body", PrimitiveType.Sphere, lantern.transform, Vector3.zero, new Vector3(0.24f, 0.24f, 0.24f), material);
        RemoveCollider(body);
        Light light = body.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.72f, 0.26f);
        light.intensity = 1.15f;
        light.range = 4.5f;
    }

    private static void CreateGarland(Transform parent, Vector3 position, float width, Material material)
    {
        GameObject garland = Group("Final Garland", parent);
        garland.transform.position = position;
        for (int i = 0; i < 7; i++)
        {
            float t = i / 6f;
            float x = Mathf.Lerp(-width * 0.5f, width * 0.5f, t);
            float y = Mathf.Sin(t * Mathf.PI) * -0.35f;
            CreateLantern(garland.transform, new Vector3(x, y, 0f), material);
        }
    }

    private static void CreateReeds(Transform parent, Vector3 position, float yaw, Material material)
    {
        GameObject reeds = Group("Reeds", parent);
        reeds.transform.position = position;
        reeds.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        for (int i = 0; i < 4; i++)
        {
            float offset = (i - 1.5f) * 0.08f;
            GameObject reed = Primitive("Reed Stem", PrimitiveType.Cylinder, reeds.transform, new Vector3(offset, 0.36f, 0f), new Vector3(0.035f, 0.36f + i * 0.04f, 0.035f), material);
            reed.transform.rotation = Quaternion.Euler(i * 5f, 0f, (i - 1.5f) * 7f);
        }
    }

    private static void CreateFlowerPatch(Transform parent, Vector3 center, Material primary, Material secondary, int count, float radius)
    {
        GameObject patch = Group("Flower Patch", parent);
        patch.transform.position = center;
        for (int i = 0; i < count; i++)
        {
            float angle = i * 137.5f * Mathf.Deg2Rad;
            float distance = radius * Mathf.Sqrt((i + 0.5f) / count);
            Vector3 position = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
            CreateFlower(patch.transform, position, i % 2 == 0 ? primary : secondary);
        }
    }

    private static void CreateFlower(Transform parent, Vector3 localPosition, Material material)
    {
        GameObject flower = Group("Flower", parent);
        flower.transform.localPosition = localPosition;
        Primitive("Stem", PrimitiveType.Cylinder, flower.transform, new Vector3(0f, 0.16f, 0f), new Vector3(0.025f, 0.16f, 0.025f), material);
        GameObject bloom = Primitive("Bloom", PrimitiveType.Sphere, flower.transform, new Vector3(0f, 0.36f, 0f), new Vector3(0.16f, 0.08f, 0.16f), material);
        RemoveCollider(bloom);
    }

    private static void CreateMushrooms(Transform parent, Vector3 position, Material stemMaterial, Material capMaterial)
    {
        GameObject cluster = Group("Mushroom Cluster", parent);
        cluster.transform.position = position;
        for (int i = 0; i < 3; i++)
        {
            Vector3 offset = new Vector3((i - 1) * 0.22f, 0f, (i % 2) * 0.15f);
            Primitive("Stem", PrimitiveType.Cylinder, cluster.transform, offset + new Vector3(0f, 0.18f, 0f), new Vector3(0.07f, 0.18f, 0.07f), stemMaterial);
            GameObject cap = Primitive("Cap", PrimitiveType.Sphere, cluster.transform, offset + new Vector3(0f, 0.39f, 0f), new Vector3(0.22f, 0.1f, 0.22f), capMaterial);
            RemoveCollider(cap);
        }
    }

    private static void CreateStylizedTree(Transform parent, Vector3 position, float scale, Material bark, Material leaves)
    {
        GameObject tree = Group("Decor Tree", parent);
        tree.transform.position = position;
        Primitive("Trunk", PrimitiveType.Cylinder, tree.transform, new Vector3(0f, 0.9f * scale, 0f), new Vector3(0.34f * scale, 0.9f * scale, 0.34f * scale), bark);
        GameObject lower = Primitive("Lower Crown", PrimitiveType.Sphere, tree.transform, new Vector3(0f, 2f * scale, 0f), new Vector3(1.7f * scale, 1.2f * scale, 1.7f * scale), leaves);
        GameObject upper = Primitive("Upper Crown", PrimitiveType.Sphere, tree.transform, new Vector3(0.15f * scale, 2.85f * scale, 0f), new Vector3(1.25f * scale, 1f * scale, 1.25f * scale), leaves);
        RemoveCollider(lower);
        RemoveCollider(upper);
    }

    private static void CreateSign(Transform parent, Vector3 position, float yaw, string text, Material wood, Material textMaterial)
    {
        GameObject sign = Group("Sign - " + text, parent);
        sign.transform.position = position;
        sign.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        Primitive("Post", PrimitiveType.Cylinder, sign.transform, new Vector3(0f, 0.55f, 0f), new Vector3(0.08f, 0.55f, 0.08f), wood);
        Primitive("Board", PrimitiveType.Cube, sign.transform, new Vector3(0f, 1.12f, 0f), new Vector3(1.55f, 0.48f, 0.12f), wood);

        GameObject label = new GameObject("Text", typeof(TextMesh));
        label.transform.SetParent(sign.transform, false);
        label.transform.localPosition = new Vector3(0f, 1.07f, -0.07f);
        label.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        label.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
        TextMesh textMesh = label.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 28;
        label.GetComponent<MeshRenderer>().sharedMaterial = textMaterial;
    }

    private static void CreateLowHill(Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        GameObject hill = Primitive("Soft Ground Mound", PrimitiveType.Sphere, parent, position, scale, material);
        hill.transform.position = position;
        hill.transform.localScale = scale;
        RemoveCollider(hill);
    }

    private static void CreateFlatStone(Transform parent, Vector3 position, float scale, Material material)
    {
        GameObject stone = Primitive("Flat Stone", PrimitiveType.Sphere, parent, position, new Vector3(scale, 0.14f, scale * 0.75f), material);
        stone.transform.rotation = Quaternion.Euler(0f, position.x * 11f + position.z * 5f, 0f);
    }

    private static void CreatePebbleRing(Transform parent, Vector3 center, float radiusX, float radiusZ, Material material)
    {
        for (int i = 0; i < 12; i++)
        {
            float angle = i / 12f * Mathf.PI * 2f;
            Vector3 position = center + new Vector3(Mathf.Cos(angle) * radiusX, 0f, Mathf.Sin(angle) * radiusZ);
            CreateFlatStone(parent, position, 0.22f + (i % 3) * 0.04f, material);
        }
    }

    private static Material Material(string name, Color color)
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

    private static GameObject Primitive(string name, PrimitiveType type, Transform parent, Vector3 localPosition, Vector3 scale, Material material)
    {
        GameObject primitive = GameObject.CreatePrimitive(type);
        primitive.name = name;
        primitive.transform.SetParent(parent, false);
        primitive.transform.localPosition = localPosition;
        primitive.transform.localScale = scale;
        primitive.GetComponent<Renderer>().sharedMaterial = material;
        return primitive;
    }

    private static void RemoveCollider(GameObject target)
    {
        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }
    }
}
