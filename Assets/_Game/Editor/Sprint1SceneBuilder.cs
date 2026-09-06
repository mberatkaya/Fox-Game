using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint1SceneBuilder
    {
        private const string InputAssetPath = "Assets/_Game/Settings/TilkiInputActions.inputactions";
        private const string PlayerPrefabPath = "Assets/_Game/Prefabs/Characters/PlayerFox.prefab";

        [MenuItem("Tilki Oyunu/Sprint 1/Build Gameplay Prototype")]
        public static void BuildSprint1Content()
        {
            GameObject playerPrefab = BuildPlayerPrefab();
            ConfigureBootstrapScene();
            BuildForestScene(playerPrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static GameObject BuildPlayerPrefab()
        {
            InputActionAsset inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAssetPath);
            Material foxOrange = LoadMaterial("Assets/Art/Materials/Fox Orange.mat");
            Material foxCream = LoadMaterial("Assets/Art/Materials/Fox Cream.mat");

            GameObject root = new("PlayerFox");
            CharacterController controller = root.AddComponent<CharacterController>();
            controller.radius = 0.28f;
            controller.height = 1.1f;
            controller.center = new Vector3(0f, 0.55f, 0f);
            controller.stepOffset = 0.22f;
            controller.slopeLimit = 48f;
            controller.skinWidth = 0.045f;

            Transform cameraTarget = CreateChild(root.transform, "Camera Target", new Vector3(0f, 0.78f, 0f));
            Transform interactionOrigin = CreateChild(root.transform, "Interaction Origin", new Vector3(0f, 0.48f, 0.48f));
            Transform groundProbe = CreateChild(root.transform, "Ground Probe", new Vector3(0f, 0.08f, 0f));
            Transform visual = CreateChild(root.transform, "Visual", Vector3.zero);

            GameObject body = CreatePrimitiveChild(visual, PrimitiveType.Capsule, "Placeholder Fox Body", new Vector3(0f, 0.47f, 0f), new Vector3(0.44f, 0.34f, 0.74f), foxOrange);
            body.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            GameObject head = CreatePrimitiveChild(visual, PrimitiveType.Sphere, "Placeholder Fox Head", new Vector3(0f, 0.62f, 0.55f), new Vector3(0.42f, 0.34f, 0.38f), foxOrange);
            GameObject muzzle = CreatePrimitiveChild(visual, PrimitiveType.Sphere, "Cream Muzzle", new Vector3(0f, 0.56f, 0.78f), new Vector3(0.25f, 0.17f, 0.18f), foxCream);
            GameObject tail = CreatePrimitiveChild(visual, PrimitiveType.Capsule, "Cream Tail", new Vector3(0f, 0.47f, -0.68f), new Vector3(0.22f, 0.24f, 0.62f), foxCream);
            tail.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);

            StripCollider(body);
            StripCollider(head);
            StripCollider(muzzle);
            StripCollider(tail);

            FoxController foxController = root.AddComponent<FoxController>();
            PlayerInteractor interactor = root.AddComponent<PlayerInteractor>();

            SetObject(foxController, "inputActions", inputActions);
            SetObject(foxController, "groundProbe", groundProbe);
            SetObject(interactor, "inputActions", inputActions);
            SetObject(interactor, "origin", interactionOrigin);
            ConfigureMovement(foxController);

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            Object.DestroyImmediate(root);
            return AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
        }

        private static void ConfigureBootstrapScene()
        {
            Scene scene = EditorSceneManager.OpenScene(SceneIds.BootstrapPath, OpenSceneMode.Single);
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                GameBootstrap bootstrap = root.GetComponentInChildren<GameBootstrap>();
                if (bootstrap == null)
                {
                    continue;
                }

                SetBool(bootstrap, "loadForestOnStart", true);
                break;
            }

            EditorSceneManager.SaveScene(scene);
        }

        private static void BuildForestScene(GameObject playerPrefab)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Material grass = LoadMaterial("Assets/Art/Materials/Grass.mat");
            Material path = LoadMaterial("Assets/Art/Materials/Path.mat");
            Material water = LoadMaterial("Assets/Art/Materials/Water.mat");
            Material bark = LoadMaterial("Assets/Art/Materials/Bark.mat");
            Material leaves = LoadMaterial("Assets/Art/Materials/Leaves.mat");
            Material planks = LoadMaterial("Assets/Art/Materials/Bridge Planks.mat");
            Material stone = LoadMaterial("Assets/Art/Materials/Stone.mat");
            Material cabinWood = LoadMaterial("Assets/Art/Materials/Cabin Wood.mat");
            Material glow = LoadMaterial("Assets/Art/Materials/Warm Glow.mat");

            RenderSettings.ambientLight = new Color(0.48f, 0.52f, 0.44f);
            CreateSun();

            GameObject world = new("Sprint 1 Forest Blockout");
            CreateCube(world.transform, "Safe Clearing Ground", new Vector3(0f, -0.05f, 0f), new Vector3(34f, 0.1f, 34f), grass);
            CreateCube(world.transform, "North Boundary Ridge", new Vector3(0f, 0.6f, 17f), new Vector3(36f, 1.4f, 1.4f), stone);
            CreateCube(world.transform, "South Boundary Ridge", new Vector3(0f, 0.6f, -17f), new Vector3(36f, 1.4f, 1.4f), stone);
            CreateCube(world.transform, "East Boundary Ridge", new Vector3(17f, 0.6f, 0f), new Vector3(1.4f, 1.4f, 36f), stone);
            CreateCube(world.transform, "West Boundary Ridge", new Vector3(-17f, 0.6f, 0f), new Vector3(1.4f, 1.4f, 36f), stone);

            CreatePath(world.transform, path);
            CreateBridge(world.transform, planks);
            CreatePond(world.transform, water, stone);
            CreateCamp(world.transform, cabinWood, glow);
            CreateTrees(world.transform, bark, leaves);
            CreateFutureAreas(world.transform, stone);

            GameObject promptCanvas = CreatePromptUI();
            InteractionPromptUI promptUI = promptCanvas.GetComponent<InteractionPromptUI>();

            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            player.name = "PlayerFox";
            Transform cameraTarget = FindChild(player.transform, "Camera Target");
            Transform interactionOrigin = FindChild(player.transform, "Interaction Origin");
            FoxController foxController = player.GetComponent<FoxController>();
            PlayerInteractor interactor = player.GetComponent<PlayerInteractor>();

            GameObject spawn = new("Player Spawn Point");
            spawn.transform.SetPositionAndRotation(new Vector3(0f, 0f, -10.5f), Quaternion.Euler(0f, 0f, 0f));
            PlayerSpawnPoint spawnPoint = spawn.AddComponent<PlayerSpawnPoint>();
            SetObject(spawnPoint, "player", foxController);

            SetObject(interactor, "origin", interactionOrigin);
            SetObject(interactor, "promptUI", promptUI);

            GameObject cameraObject = new("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 3.2f, -14.5f);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 60f;
            cameraObject.AddComponent<AudioListener>();
            ThirdPersonCameraController cameraController = cameraObject.AddComponent<ThirdPersonCameraController>();
            SetObject(cameraController, "inputActions", AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAssetPath));
            SetObject(cameraController, "target", cameraTarget);
            SetObject(foxController, "cameraTransform", cameraObject.transform);

            GameObject gameplay = new("Forest Gameplay Bootstrap");
            ForestGameplayBootstrap gameplayBootstrap = gameplay.AddComponent<ForestGameplayBootstrap>();
            SetObject(gameplayBootstrap, "spawnPoint", spawnPoint);
            SetObject(gameplayBootstrap, "player", foxController);

            CreateTestInteractable(world.transform, glow);

            EditorSceneManager.SaveScene(scene, SceneIds.ForestPath);
        }

        private static GameObject CreatePromptUI()
        {
            GameObject canvasObject = new("Interaction Prompt Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(InteractionPromptUI));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            GameObject promptObject = new("Prompt Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            promptObject.transform.SetParent(canvasObject.transform, false);
            RectTransform rect = promptObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.34f, 0.08f);
            rect.anchorMax = new Vector2(0.66f, 0.15f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            TextMeshProUGUI text = promptObject.GetComponent<TextMeshProUGUI>();
            text.text = string.Empty;
            text.fontSize = 34f;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            text.color = new Color(1f, 0.94f, 0.78f);

            SetObject(canvasObject.GetComponent<InteractionPromptUI>(), "promptText", text);
            return canvasObject;
        }

        private static void CreateSun()
        {
            GameObject sun = new("Forest Sun");
            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.88f, 0.72f);
            light.intensity = 1.1f;
            sun.transform.rotation = Quaternion.Euler(52f, -34f, 0f);
        }

        private static void CreatePath(Transform parent, Material material)
        {
            CreateCube(parent, "Path Start To Bridge", new Vector3(0f, 0.01f, -6.3f), new Vector3(2.4f, 0.04f, 8.4f), material);
            CreateCube(parent, "Path Bridge To Camp", new Vector3(0f, 0.01f, 5.7f), new Vector3(2.2f, 0.04f, 7.8f), material);
            CreateCube(parent, "Path To Pond", new Vector3(6.1f, 0.01f, -1.5f), new Vector3(8.5f, 0.04f, 2f), material);
            CreateCube(parent, "Path To Trees", new Vector3(-7.5f, 0.01f, -1f), new Vector3(7f, 0.04f, 2f), material);
        }

        private static void CreateBridge(Transform parent, Material planks)
        {
            GameObject bridge = new("Small Bridge");
            bridge.transform.SetParent(parent, false);
            CreateCube(bridge.transform, "Bridge Walkway", new Vector3(0f, 0.2f, 0f), new Vector3(2.8f, 0.24f, 4.4f), planks);
            CreateCube(bridge.transform, "Left Rail", new Vector3(-1.55f, 0.66f, 0f), new Vector3(0.16f, 0.32f, 4.6f), planks);
            CreateCube(bridge.transform, "Right Rail", new Vector3(1.55f, 0.66f, 0f), new Vector3(0.16f, 0.32f, 4.6f), planks);
            bridge.transform.position = new Vector3(0f, 0f, -0.8f);
        }

        private static void CreatePond(Transform parent, Material water, Material stone)
        {
            GameObject pond = CreatePrimitive(parent, PrimitiveType.Cylinder, "Blocked Placeholder Pond", new Vector3(8.2f, 0.03f, -1.6f), new Vector3(5.4f, 0.08f, 3.7f), water);
            pond.GetComponent<Collider>().isTrigger = false;
            CreateCube(parent, "Pond North Stones", new Vector3(8.2f, 0.18f, 0.55f), new Vector3(5.8f, 0.35f, 0.5f), stone);
            CreateCube(parent, "Pond South Stones", new Vector3(8.2f, 0.18f, -3.75f), new Vector3(5.8f, 0.35f, 0.5f), stone);
        }

        private static void CreateCamp(Transform parent, Material wood, Material glow)
        {
            GameObject camp = new("Camp Placeholder");
            camp.transform.SetParent(parent, false);
            camp.transform.position = new Vector3(0f, 0f, 11.2f);
            CreateCube(camp.transform, "Camp Platform", Vector3.zero, new Vector3(5.8f, 0.12f, 4.2f), wood);
            CreateCube(camp.transform, "Tent Body", new Vector3(-1.6f, 0.7f, 0.35f), new Vector3(1.9f, 1.2f, 1.6f), wood);
            CreatePrimitive(camp.transform, PrimitiveType.Cylinder, "Camp Fire Base", new Vector3(1.35f, 0.16f, -0.35f), new Vector3(0.9f, 0.18f, 0.9f), glow);
            Light fire = new GameObject("Warm Camp Light").AddComponent<Light>();
            fire.transform.SetParent(camp.transform, false);
            fire.transform.localPosition = new Vector3(1.35f, 0.9f, -0.35f);
            fire.color = new Color(1f, 0.58f, 0.24f);
            fire.range = 5f;
            fire.intensity = 1.6f;
        }

        private static void CreateTrees(Transform parent, Material bark, Material leaves)
        {
            Vector3[] positions =
            {
                new(-9.5f, 0f, -6.2f), new(-12.5f, 0f, -3.8f), new(-8.5f, 0f, -0.9f), new(-13f, 0f, 2.2f),
                new(-10.2f, 0f, 5.2f), new(-5.7f, 0f, 3.5f), new(-6.2f, 0f, -5.3f), new(-14.2f, 0f, 7.8f),
                new(5.6f, 0f, 5.3f), new(8.8f, 0f, 6.9f), new(12.4f, 0f, 4.5f), new(13.1f, 0f, -6.6f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject tree = new($"Traversal Tree {i + 1:00}");
                tree.transform.SetParent(parent, false);
                tree.transform.position = positions[i];
                CreatePrimitive(tree.transform, PrimitiveType.Cylinder, "Trunk", new Vector3(0f, 0.75f, 0f), new Vector3(0.45f, 1.5f, 0.45f), bark);
                GameObject crown = CreatePrimitive(tree.transform, PrimitiveType.Sphere, "Crown", new Vector3(0f, 1.85f, 0f), new Vector3(1.6f, 1.35f, 1.6f), leaves);
                StripCollider(crown);
            }
        }

        private static void CreateFutureAreas(Transform parent, Material stone)
        {
            CreateCube(parent, "Future Memory Area Marker", new Vector3(-7.8f, 0.09f, 9.4f), new Vector3(3.2f, 0.18f, 3.2f), stone);
            CreateCube(parent, "Future Quest Area Marker", new Vector3(10.2f, 0.09f, 9.8f), new Vector3(3.2f, 0.18f, 3.2f), stone);
        }

        private static void CreateTestInteractable(Transform parent, Material glow)
        {
            GameObject root = new("Sprint 1 Test Interactable");
            root.transform.SetParent(parent, false);
            root.transform.position = new Vector3(1.8f, 0f, -8.2f);
            GameObject lantern = CreatePrimitive(root.transform, PrimitiveType.Sphere, "Toggle Lantern", new Vector3(0f, 0.55f, 0f), new Vector3(0.55f, 0.55f, 0.55f), glow);
            Light light = new GameObject("Feedback Light").AddComponent<Light>();
            light.transform.SetParent(root.transform, false);
            light.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            light.color = new Color(0.5f, 1f, 0.72f);
            light.range = 4f;
            light.intensity = 2f;
            light.enabled = false;

            TestInteractable interactable = root.AddComponent<TestInteractable>();
            SetObject(interactable, "targetRenderer", lantern.GetComponent<Renderer>());
            SetObject(interactable, "feedbackLight", light);
            SetString(interactable, "interactionLabel", "Toggle lantern");
        }

        private static GameObject CreateCube(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            return CreatePrimitive(parent, PrimitiveType.Cube, name, position, scale, material);
        }

        private static GameObject CreatePrimitiveChild(Transform parent, PrimitiveType primitiveType, string name, Vector3 position, Vector3 scale, Material material)
        {
            return CreatePrimitive(parent, primitiveType, name, position, scale, material);
        }

        private static GameObject CreatePrimitive(Transform parent, PrimitiveType primitiveType, string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject instance = GameObject.CreatePrimitive(primitiveType);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localScale = scale;
            if (material != null)
            {
                instance.GetComponent<Renderer>().sharedMaterial = material;
            }

            return instance;
        }

        private static Transform CreateChild(Transform parent, string name, Vector3 position)
        {
            GameObject child = new(name);
            child.transform.SetParent(parent, false);
            child.transform.localPosition = position;
            return child.transform;
        }

        private static Transform FindChild(Transform parent, string name)
        {
            Queue<Transform> queue = new();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                Transform current = queue.Dequeue();
                if (current.name == name)
                {
                    return current;
                }

                foreach (Transform child in current)
                {
                    queue.Enqueue(child);
                }
            }

            return null;
        }

        private static Material LoadMaterial(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Material>(path);
        }

        private static void StripCollider(GameObject instance)
        {
            Collider collider = instance.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }
        }

        private static void ConfigureMovement(FoxController controller)
        {
            SerializedObject serializedObject = new(controller);
            SerializedProperty movement = serializedObject.FindProperty("movement");
            movement.FindPropertyRelative("moveSpeed").floatValue = 3.6f;
            movement.FindPropertyRelative("sprintSpeed").floatValue = 6.1f;
            movement.FindPropertyRelative("rotationSpeed").floatValue = 15f;
            movement.FindPropertyRelative("jumpHeight").floatValue = 1.05f;
            movement.FindPropertyRelative("airControl").floatValue = 0.45f;
            movement.FindPropertyRelative("acceleration").floatValue = 18f;
            movement.FindPropertyRelative("deceleration").floatValue = 24f;
            movement.FindPropertyRelative("gravity").floatValue = -24f;
            movement.FindPropertyRelative("groundedStickVelocity").floatValue = -2f;
            movement.FindPropertyRelative("groundedProbeRadius").floatValue = 0.18f;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetBool(Object target, string propertyName, bool value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetString(Object target, string propertyName, string value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).stringValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
