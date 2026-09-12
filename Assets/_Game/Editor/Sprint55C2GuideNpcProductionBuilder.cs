using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint55C2GuideNpcProductionBuilder
    {
        public const string BaseCharacterRoot = "Assets/ThirdParty/Quaternius/UniversalBaseCharacters";
        public const string GuideModelPath = BaseCharacterRoot + "/BaseCharacters/Superhero_Female_FullBody.fbx";
        public const string GuideHairPath = BaseCharacterRoot + "/Hairstyles/Hair_Buns.fbx";
        public const string GuideEyebrowsPath = BaseCharacterRoot + "/Hairstyles/Eyebrows_Female.fbx";
        public const string GuidePrefabPath = "Assets/_Game/Prefabs/NPC/NPC_Guide_Visual.prefab";
        public const string GuideControllerPath = "Assets/_Game/Animations/NPC/GuideNpcAnimator.controller";
        public const string GuideIdlePath = "Assets/_Game/Animations/NPC/GuideNpc_RelaxedIdle.anim";
        public const string GuideBodyMaterialPath = "Assets/_Game/Art/NPC/GuideNpc_Body_URP.mat";
        public const string GuideHairMaterialPath = "Assets/_Game/Art/NPC/GuideNpc_Hair_URP.mat";

        [MenuItem("Tilki Oyunu/Sprint 5.5/C.2 Apply Guide NPC Production Visual")]
        public static void ApplyGuideNpcProductionVisual()
        {
            EnsureFolders();
            AssetDatabase.ImportAsset(BaseCharacterRoot, ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceSynchronousImport);
            ConfigureModelImporter(GuideModelPath, ModelImporterAnimationType.Human);
            ConfigureModelImporter(GuideHairPath, ModelImporterAnimationType.Generic);
            ConfigureModelImporter(GuideEyebrowsPath, ModelImporterAnimationType.Generic);
            Material body = EnsureMaterial(GuideBodyMaterialPath, new Color(0.76f, 0.54f, 0.36f));
            Material hair = EnsureMaterial(GuideHairMaterialPath, new Color(0.26f, 0.16f, 0.09f));
            AnimationClip idle = EnsureIdleClip();
            AnimatorController controller = EnsureAnimatorController(idle);
            EnsureGuideVisualPrefab(controller, body, hair);
            ApplyForestGuideInstance(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ApplyGuideNpcProductionVisualFromCommandLine()
        {
            ApplyGuideNpcProductionVisual();
            Debug.Log(BuildGuideNpcReport());
            EditorApplication.Exit(0);
        }

        public static string BuildGuideNpcReport()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GuidePrefabPath);
            ModelImporter importer = AssetImporter.GetAtPath(GuideModelPath) as ModelImporter;
            Avatar avatar = AssetDatabase.LoadAssetAtPath<Avatar>(GuideModelPath);
            return "SPRINT55C2_GUIDE_NPC_REPORT_BEGIN\n"
                + $"SourceModel={GuideModelPath}\n"
                + $"ProductionPrefab={GuidePrefabPath}\n"
                + $"AnimatorController={GuideControllerPath}\n"
                + $"IdleClip={GuideIdlePath}\n"
                + $"ImporterAnimationType={(importer != null ? importer.animationType.ToString() : "MISSING")}\n"
                + $"Avatar={(avatar != null ? avatar.name : "MISSING")}\n"
                + $"AvatarValid={(avatar != null && avatar.isValid)}\n"
                + $"AvatarHuman={(avatar != null && avatar.isHuman)}\n"
                + $"Prefab={(prefab != null)}\n"
                + "SPRINT55C2_GUIDE_NPC_REPORT_END";
        }

        private static void ConfigureModelImporter(string assetPath, ModelImporterAnimationType animationType)
        {
            ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (importer == null)
            {
                throw new InvalidOperationException($"Missing imported Guide NPC asset at {assetPath}.");
            }

            importer.animationType = animationType;
            importer.avatarSetup = animationType == ModelImporterAnimationType.Human
                ? ModelImporterAvatarSetup.CreateFromThisModel
                : ModelImporterAvatarSetup.NoAvatar;
            importer.importAnimation = false;
            importer.optimizeGameObjects = false;
            importer.preserveHierarchy = true;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            importer.SaveAndReimport();
        }

        private static AnimationClip EnsureIdleClip()
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(GuideIdlePath);
            if (clip == null)
            {
                clip = new AnimationClip { name = "GuideNpc_RelaxedIdle", frameRate = 30f };
                AssetDatabase.CreateAsset(clip, GuideIdlePath);
            }

            clip.ClearCurves();
            AnimationCurve bob = AnimationCurve.EaseInOut(0f, 0f, 1.5f, 0.025f);
            bob.AddKey(new Keyframe(3f, 0f));
            AnimationCurve sway = AnimationCurve.EaseInOut(0f, -1.25f, 1.5f, 1.25f);
            sway.AddKey(new Keyframe(3f, -1.25f));
            clip.SetCurve("AnimatedRoot", typeof(Transform), "localPosition.y", bob);
            clip.SetCurve("AnimatedRoot", typeof(Transform), "localEulerAnglesRaw.y", sway);
            AnimationUtility.SetAnimationClipSettings(clip, new AnimationClipSettings { loopTime = true, loopBlend = true });
            EditorUtility.SetDirty(clip);
            return clip;
        }

        private static AnimatorController EnsureAnimatorController(AnimationClip idle)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(GuideControllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(GuideControllerPath);
            }

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            foreach (ChildAnimatorState state in stateMachine.states)
            {
                stateMachine.RemoveState(state.state);
            }

            AnimatorState idleState = stateMachine.AddState("Relaxed Idle");
            idleState.motion = idle;
            idleState.writeDefaultValues = true;
            stateMachine.defaultState = idleState;
            EditorUtility.SetDirty(controller);
            return controller;
        }

        private static void EnsureGuideVisualPrefab(RuntimeAnimatorController controller, Material body, Material hair)
        {
            GameObject root = new("NPC_Guide_Visual");
            try
            {
                Animator animator = root.AddComponent<Animator>();
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
                Avatar avatar = AssetDatabase.LoadAssetAtPath<Avatar>(GuideModelPath);
                if (avatar != null && avatar.isValid)
                {
                    animator.avatar = avatar;
                }

                Transform animatedRoot = new GameObject("AnimatedRoot").transform;
                animatedRoot.SetParent(root.transform, false);

                AddModelChild(animatedRoot, GuideModelPath, "QuaterniusGuideBody", body, new Vector3(0f, 0f, 0f), Vector3.one * 0.68f);
                AddModelChild(animatedRoot, GuideHairPath, "QuaterniusGuideHair", hair, new Vector3(0f, 0f, 0f), Vector3.one * 0.68f);
                AddModelChild(animatedRoot, GuideEyebrowsPath, "QuaterniusGuideEyebrows", hair, new Vector3(0f, 0f, 0f), Vector3.one * 0.68f);

                PrefabUtility.SaveAsPrefabAsset(root, GuidePrefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void AddModelChild(Transform parent, string path, string name, Material material, Vector3 localPosition, Vector3 localScale)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null)
            {
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset, parent);
            instance.name = name;
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = localScale;
            foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                renderer.sharedMaterial = material;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }
        }

        private static void ApplyForestGuideInstance(RuntimeAnimatorController controller)
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            GameObject npc = GameObject.Find("NPC_Guide");
            if (npc == null)
            {
                throw new InvalidOperationException("Forest scene is missing authoritative NPC_Guide gameplay object.");
            }

            DisablePlaceholderRenderer(npc.transform.Find("Guide Body"));
            DisablePlaceholderRenderer(npc.transform.Find("Guide Head"));

            Transform visualRoot = npc.transform.Find("VisualRoot");
            if (visualRoot == null)
            {
                visualRoot = new GameObject("VisualRoot").transform;
                visualRoot.SetParent(npc.transform, false);
            }

            visualRoot.localPosition = Vector3.zero;
            visualRoot.localRotation = Quaternion.identity;
            visualRoot.localScale = Vector3.one;
            for (int i = visualRoot.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(visualRoot.GetChild(i).gameObject);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GuidePrefabPath);
            GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(prefab, visualRoot);
            visual.name = "NPC_Guide_Visual";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;
            Animator animator = visual.GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
            }

            EnsurePhysicalBlocker(npc.transform);
            EditorUtility.SetDirty(npc);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void DisablePlaceholderRenderer(Transform transform)
        {
            if (transform == null)
            {
                return;
            }

            foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = false;
                EditorUtility.SetDirty(renderer);
            }
        }

        private static void EnsurePhysicalBlocker(Transform npc)
        {
            Transform blocker = npc.Find("NPC_Guide_PhysicalBlocker");
            if (blocker == null)
            {
                blocker = new GameObject("NPC_Guide_PhysicalBlocker").transform;
                blocker.SetParent(npc, false);
            }

            blocker.localPosition = Vector3.zero;
            blocker.localRotation = Quaternion.identity;
            blocker.localScale = Vector3.one;
            CapsuleCollider collider = blocker.GetComponent<CapsuleCollider>();
            if (collider == null)
            {
                collider = blocker.gameObject.AddComponent<CapsuleCollider>();
            }

            collider.isTrigger = false;
            collider.radius = 0.34f;
            collider.height = 1.62f;
            collider.center = new Vector3(0f, 0.81f, 0f);
        }

        private static Material EnsureMaterial(string path, Color color)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (material == null)
            {
                material = new Material(shader) { name = Path.GetFileNameWithoutExtension(path) };
                AssetDatabase.CreateAsset(material, path);
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

            SetFloatIfPresent(material, "_Metallic", 0f);
            SetFloatIfPresent(material, "_Smoothness", 0.18f);
            SetFloatIfPresent(material, "_SpecularHighlights", 0f);
            SetFloatIfPresent(material, "_EnvironmentReflections", 0.25f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void SetFloatIfPresent(Material material, string propertyName, float value)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                material.SetFloat(propertyName, value);
            }
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/_Game/Art/NPC");
            EnsureFolder("Assets/_Game/Prefabs/NPC");
            EnsureFolder("Assets/_Game/Animations/NPC");
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
    }
}
