using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace TilkiOyunu.Foundation.Editor
{
    // Deliberately separate from world builders: never rebuilds terrain, dressing or quests.
    public static class Sprint55AtmosphereBuilder
    {
        public const string Folder = "Assets/_Game/Art/Environment/Sprint55D";
        public const string ReportFolder = "Documentation/Sprint55D";
        public const string ProfilePath = Folder + "/ForestAtmosphere.asset";
        private const string PipelinePath = "Assets/_Game/Settings/TilkiURPAsset.asset";

        [MenuItem("Tilki Oyunu/Sprint 5.5 D/Audit Forest")]
        public static void AuditBaseline()
        {
            OpenForest();
            Audit(ReportFolder + "/Baseline/audit.txt");
            Capture(ReportFolder + "/Baseline/Screenshots");
        }

        private static void OpenForest()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before authoring atmosphere.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Save current scene edits before running the builder.");
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
        }

        [MenuItem("Tilki Oyunu/Sprint 5.5 D/Apply Presentation")]
        public static void Apply()
        {
            OpenForest();
            Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();
            Light sun = Object.FindObjectsByType<Light>(FindObjectsSortMode.None)
                .Single(l => l.type == LightType.Directional);
            sun.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
            sun.color = new Color(1f, 0.955f, 0.87f);
            sun.intensity = 0.9f; // Preserve C.2's glare correction.
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = 0.72f;
            RenderSettings.sun = sun;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.47f, 0.55f, 0.62f);
            RenderSettings.ambientEquatorColor = new Color(0.30f, 0.36f, 0.39f);
            RenderSettings.ambientGroundColor = new Color(0.19f, 0.23f, 0.22f);
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.reflectionIntensity = 0.35f;
            // 512m terrain: preserve the first 75m, soften long vistas toward the perimeter.
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.57f, 0.69f, 0.72f);
            RenderSettings.fogStartDistance = 75f;
            RenderSettings.fogEndDistance = 310f;
            Material sky = AssetDatabase.LoadAssetAtPath<Material>(Folder + "/ForestSky.mat");
            if (sky == null)
            {
                sky = new Material(RenderSettings.skybox) { name = "ForestSky" };
                AssetDatabase.CreateAsset(sky, Folder + "/ForestSky.mat");
            }
            sky.SetFloat("_Exposure", 0.9f);
            sky.SetColor("_GroundColor", new Color(0.57f, 0.69f, 0.72f));
            RenderSettings.skybox = sky;
            EditorUtility.SetDirty(sky);

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            pipeline.shadowDistance = 65f;
            pipeline.shadowCascadeCount = 2;
            pipeline.cascade2Split = 0.35f;
            var pipelineSettings = new SerializedObject(pipeline);
            pipelineSettings.FindProperty("m_SoftShadowsSupported").boolValue = true;
            pipelineSettings.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(pipeline);
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/_Game/Settings/TilkiUniversalRenderer.asset");
            var rendererSettings = new SerializedObject(renderer);
            rendererSettings.FindProperty("postProcessData").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<PostProcessData>("Packages/com.unity.render-pipelines.universal/Runtime/Data/PostProcessData.asset");
            rendererSettings.ApplyModifiedPropertiesWithoutUndo();

            VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(ProfilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, ProfilePath);
            }
            var color = Effect<ColorAdjustments>(profile);
            color.postExposure.Override(0f);
            color.contrast.Override(4f);
            color.saturation.Override(-3f);
            var bloom = Effect<Bloom>(profile);
            bloom.threshold.Override(1.2f);
            bloom.intensity.Override(0.16f);
            bloom.scatter.Override(0.55f);
            bloom.highQualityFiltering.Override(false);
            var vignette = Effect<Vignette>(profile);
            vignette.intensity.Override(0.08f);
            vignette.smoothness.Override(0.4f);
            var tone = Effect<Tonemapping>(profile);
            tone.mode.Override(TonemappingMode.Neutral);
            EditorUtility.SetDirty(profile);
            GameObject volumeObject = GameObject.Find("Forest Atmosphere Volume") ?? new GameObject("Forest Atmosphere Volume");
            Volume volume = volumeObject.GetComponent<Volume>() ?? volumeObject.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 10f;
            volume.sharedProfile = profile;
            foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
                camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;

            foreach (Terrain terrain in Object.FindObjectsByType<Terrain>(FindObjectsSortMode.None))
            {
                foreach (TerrainLayer layer in terrain.terrainData.terrainLayers)
                {
                    // RGBA diffuse alpha is 1. URP otherwise treats that as mirror-smooth,
                    // overriding C.2's matte scalar, especially on distant basemap terrain.
                    layer.smoothnessSource = TerrainLayerSmoothnessSource.Constant;
                    EditorUtility.SetDirty(layer);
                }
                terrain.terrainData.SetBaseMapDirty();
            }

            ConfigureWater("GB_Lake_TempWater", "Lake", new Color(0.19f, 0.43f, 0.49f, 0.78f), new Vector2(0.008f, 0.004f));
            ConfigureWater("GB_Creek_TempWater", "Creek", new Color(0.23f, 0.48f, 0.51f, 0.64f), new Vector2(0.018f, 0.014f));
            ConfigureLoop("Music Loop", 0.12f);
            ConfigureLoop("Forest Ambience Loop", 0.15f);
            ConfigureMagicalMaterials();
            ConfigureCamp();
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Audit(ReportFolder + "/after-audit.txt");
        }

        private static T Effect<T>(VolumeProfile profile) where T : VolumeComponent
        {
            if (profile.TryGet(out T existing)) return existing;
            T effect = profile.Add<T>();
            AssetDatabase.AddObjectToAsset(effect, profile);
            return effect;
        }

        private static void ConfigureLoop(string name, float volume)
        {
            var loop = GameObject.Find(name).GetComponent<SceneLoopAudio>();
            var so = new SerializedObject(loop);
            so.FindProperty("volume").floatValue = volume;
            so.ApplyModifiedPropertiesWithoutUndo();
            loop.GetComponent<AudioSource>().volume = volume;
        }

        private static void ConfigureWater(string objectName, string label, Color color, Vector2 speed)
        {
            GameObject surface = GameObject.Find(objectName);
            var renderer = surface.GetComponent<MeshRenderer>();
            string materialPath = $"{Folder}/{label}Water.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                material = new Material(renderer.sharedMaterial) { name = label + "Water" };
                AssetDatabase.CreateAsset(material, materialPath);
            }
            material.SetColor("_BaseColor", color);
            material.SetColor("_Color", color);
            material.SetFloat("_Smoothness", 0.28f);
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_SpecularHighlights", 1f);
            material.DisableKeyword("_SPECULARHIGHLIGHTS_OFF");
            material.SetFloat("_EnvironmentReflections", 0f);
            material.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
            material.SetTexture("_BumpMap", EnsureWaterNormal());
            material.SetFloat("_BumpScale", 0.18f);
            material.EnableKeyword("_NORMALMAP");
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            // Existing mesh has no UVs. Clone only to add UVs; vertices/triangles/colliders stay intact.
            var filter = surface.GetComponent<MeshFilter>();
            string meshPath = $"{Folder}/{label}WaterUV.asset";
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            if (mesh == null)
            {
                mesh = Object.Instantiate(filter.sharedMesh);
                mesh.name = label + "WaterUV";
                mesh.uv = mesh.vertices.Select(v => new Vector2(v.x, v.z) / 8f).ToArray();
                AssetDatabase.CreateAsset(mesh, meshPath);
            }
            filter.sharedMesh = mesh;
            // The inherited lake fan winds downward, so URP backface-culls it from shore.
            // Correct face orientation without changing any vertex or shoreline position.
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                if (Vector3.Cross(vertices[triangles[i + 1]] - vertices[triangles[i]],
                    vertices[triangles[i + 2]] - vertices[triangles[i]]).y < 0f)
                    (triangles[i + 1], triangles[i + 2]) = (triangles[i + 2], triangles[i + 1]);
            }
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            EditorUtility.SetDirty(mesh);
            var motion = surface.GetComponent<CalmWaterMotion>() ?? surface.AddComponent<CalmWaterMotion>();
            var settings = new SerializedObject(motion);
            settings.FindProperty("uvVelocity").vector2Value = speed;
            settings.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(material);
        }

        private static Texture2D EnsureWaterNormal()
        {
            const string path = Folder + "/WaterRipples.png";
            if (!File.Exists(path))
            {
                const int size = 128;
                var pixels = new Texture2D(size, size, TextureFormat.RGB24, false, true);
                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float u = x * Mathf.PI * 2f / size, v = y * Mathf.PI * 2f / size;
                    Vector3 n = new Vector3(0.28f * Mathf.Cos(u * 3f + v * 2f),
                        0.20f * Mathf.Cos(v * 4f - u), 1f).normalized;
                    pixels.SetPixel(x, y, new Color(n.x * .5f + .5f, n.y * .5f + .5f, n.z * .5f + .5f));
                }
                pixels.Apply();
                File.WriteAllBytes(path, pixels.EncodeToPNG());
                Object.DestroyImmediate(pixels);
                AssetDatabase.ImportAsset(path);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.NormalMap;
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static void ConfigureCamp()
        {
            Transform camp = GameObject.Find("Camp Placeholder").transform;
            Light light = camp.Find("Warm Camp Light").GetComponent<Light>();
            light.intensity = 1.8f;
            light.range = 7f;
            light.color = new Color(1f, 0.64f, 0.34f);
            light.shadows = LightShadows.None;
            // Keep the existing unlock gate, but limit its tint to the fire instead of gray-washing the camp.
            var campState = new SerializedObject(camp.GetComponent<FinalCampController>());
            var stateRenderers = campState.FindProperty("renderers");
            stateRenderers.arraySize = 1;
            stateRenderers.GetArrayElementAtIndex(0).objectReferenceValue = camp.Find("Camp Fire Base").GetComponent<Renderer>();
            campState.FindProperty("lockedColor").colorValue = new Color(1f, .52f, .20f);
            campState.FindProperty("unlockedColor").colorValue = new Color(1f, .65f, .30f);
            campState.FindProperty("lockedLightIntensity").floatValue = 1.15f;
            campState.FindProperty("unlockedLightIntensity").floatValue = 1.8f;
            campState.ApplyModifiedPropertiesWithoutUndo();
            AudioSource fire = camp.Find("Campfire Loop").GetComponent<AudioSource>();
            fire.transform.position = camp.Find("Camp Fire Base").position;
            fire.volume = 0.20f;
            fire.spatialBlend = 1f;
            fire.minDistance = 2.5f;
            fire.maxDistance = 22f;
            fire.dopplerLevel = 0f;
            fire.rolloffMode = AudioRolloffMode.Custom;
            fire.SetCustomCurve(AudioSourceCurveType.CustomRolloff, new AnimationCurve(
                new Keyframe(0f, 1f, 0f, 0f), new Keyframe(0.12f, 1f, 0f, 0f),
                new Keyframe(0.4f, 0.36f, -1.2f, -1.2f), new Keyframe(1f, 0f, 0f, 0f)));
            Material ember = AssetDatabase.LoadAssetAtPath<Material>(Folder + "/CampEmbers.mat");
            if (ember == null)
            {
                ember = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(ember, Folder + "/CampEmbers.mat");
            }
            ember.SetColor("_BaseColor", new Color(.48f, .17f, .045f));
            ember.EnableKeyword("_EMISSION");
            ember.SetColor("_EmissionColor", new Color(2.5f, .7f, .12f));
            ember.SetFloat("_Smoothness", 0f);
            camp.Find("Camp Fire Base").GetComponent<Renderer>().sharedMaterial = ember;
            EditorUtility.SetDirty(ember);
            // Small fixed-budget particle flame, attached to the existing fire without colliders/lights.
            Transform flame = camp.Find("Camp Flame");
            if (flame == null)
            {
                GameObject go = new GameObject("Camp Flame");
                go.transform.SetParent(camp, false);
                go.transform.localPosition = camp.Find("Camp Fire Base").localPosition + Vector3.up * .25f;
                var particles = go.AddComponent<ParticleSystem>();
                particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var main = particles.main;
                main.duration = 2f; main.loop = true; main.startLifetime = .8f;
                main.startSpeed = .48f; main.startSize = new ParticleSystem.MinMaxCurve(.12f, .26f);
                main.startColor = new Color(2.6f, 1.1f, .18f, .65f); main.maxParticles = 24;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
                var emission = particles.emission; emission.rateOverTime = 14f;
                var shape = particles.shape; shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = 9f; shape.radius = .22f; shape.rotation = new Vector3(-90f, 0f, 0f);
                var size = particles.sizeOverLifetime; size.enabled = true;
                size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0f));
                var alpha = particles.colorOverLifetime; alpha.enabled = true;
                var gradient = new Gradient();
                gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(1f,.3f,.05f),1f) },
                    new[] { new GradientAlphaKey(0f,0f), new GradientAlphaKey(.8f,.2f), new GradientAlphaKey(0f,1f) });
                alpha.color = gradient;
                var material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
                material.SetFloat("_Surface", 1f); material.SetFloat("_Blend", 0f);
                material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0f); material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.renderQueue = 3000;
                AssetDatabase.CreateAsset(material, Folder + "/CampFlame.mat");
                var particleRenderer = go.GetComponent<ParticleSystemRenderer>();
                particleRenderer.sharedMaterial = material;
                particleRenderer.shadowCastingMode = ShadowCastingMode.Off;
                particles.Play();
            }
            var flameRenderer = camp.Find("Camp Flame").GetComponent<ParticleSystemRenderer>();
            var flameSystem = camp.Find("Camp Flame").GetComponent<ParticleSystem>();
            var flameMain = flameSystem.main;
            flameMain.startSpeed = 1.1f;
            flameMain.startLifetime = 1.1f;
            flameMain.startSize = new ParticleSystem.MinMaxCurve(.18f, .42f);
            Material flameMaterial = flameRenderer.sharedMaterial;
            const string flamePath = Folder + "/FlameSoft.png";
            if (!File.Exists(flamePath))
            {
                var texture = new Texture2D(32, 64, TextureFormat.RGBA32, false);
                for (int y = 0; y < 64; y++)
                for (int x = 0; x < 32; x++)
                {
                    float v = (y + .5f) / 64f;
                    float width = .45f * Mathf.Pow(Mathf.Sin(v * Mathf.PI), .7f) * (1f - .6f * v);
                    float alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((width - Mathf.Abs((x + .5f) / 32f - .5f)) * 18f));
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
                texture.Apply(); File.WriteAllBytes(flamePath, texture.EncodeToPNG()); Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(flamePath);
                var importer = (TextureImporter)AssetImporter.GetAtPath(flamePath);
                importer.alphaIsTransparency = true; importer.wrapMode = TextureWrapMode.Clamp;
                importer.SaveAndReimport();
            }
            flameMaterial.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(flamePath));
            EditorUtility.SetDirty(flameMaterial);
        }

        private static void ConfigureMagicalMaterials()
        {
            foreach (string name in new[] { "Sprint55C_LightStoneInactive", "Sprint55C_MemoryLeafGlow" })
            {
                Material source = AssetDatabase.LoadAssetAtPath<Material>($"Assets/_Game/Art/Environment/Sprint55C/{name}.mat");
                string path = $"{Folder}/{name.Replace("Sprint55C_", "")}.mat";
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null)
                {
                    material = new Material(source);
                    AssetDatabase.CreateAsset(material, path);
                }
                // Existing node property blocks already animate emission; enable its URP variant.
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", name.Contains("Memory") ? new Color(1.8f, .95f, .22f) : Color.black);
                foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    Material[] materials = renderer.sharedMaterials;
                    bool changed = false;
                    for (int i = 0; i < materials.Length; i++)
                        if (materials[i] == source) { materials[i] = material; changed = true; }
                    if (changed) renderer.sharedMaterials = materials;
                }
                EditorUtility.SetDirty(material);
            }
        }

        public static void Audit(string path)
        {
            var s = new StringBuilder();
            foreach (Light l in Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                s.AppendLine($"LIGHT {l.name}: active={l.isActiveAndEnabled}; type={l.type}; position={l.transform.position}; rotation={l.transform.eulerAngles}; color={l.color}; intensity={l.intensity}; shadows={l.shadows}; strength={l.shadowStrength}; resolution={l.shadowResolution}; range={l.range}");
            s.AppendLine($"AMBIENT {RenderSettings.ambientMode}; intensity={RenderSettings.ambientIntensity}; sky={RenderSettings.ambientSkyColor}; equator={RenderSettings.ambientEquatorColor}; ground={RenderSettings.ambientGroundColor}");
            s.AppendLine($"SKY {AssetDatabase.GetAssetPath(RenderSettings.skybox)}; exposure={(RenderSettings.skybox.HasProperty("_Exposure") ? RenderSettings.skybox.GetFloat("_Exposure") : -1f)}; reflections={RenderSettings.defaultReflectionMode}/{RenderSettings.defaultReflectionResolution}/{RenderSettings.reflectionIntensity}/{RenderSettings.reflectionBounces}");
            s.AppendLine($"FOG enabled={RenderSettings.fog}; {RenderSettings.fogMode}; color={RenderSettings.fogColor}; start={RenderSettings.fogStartDistance}; end={RenderSettings.fogEndDistance}; density={RenderSettings.fogDensity}");
            foreach (Volume volume in Object.FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                s.AppendLine($"VOLUME {volume.name}: global={volume.isGlobal}; profile={AssetDatabase.GetAssetPath(volume.sharedProfile)}; components={string.Join(",", volume.sharedProfile.components.Select(c => c.name + ":" + c.active))}");
            foreach (AudioSource a in Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                s.AppendLine($"AUDIO {a.name}: group={a.outputAudioMixerGroup?.name ?? "UNROUTED"}; clip={a.clip?.name}; volume={a.volume}; spatial={a.spatialBlend}; rolloff={a.rolloffMode}; min/max={a.minDistance}/{a.maxDistance}; position={a.transform.position}");
            foreach (SceneLoopAudio loop in Object.FindObjectsByType<SceneLoopAudio>(FindObjectsSortMode.None))
                s.AppendLine($"LOOP {loop.name}: {EditorJsonUtility.ToJson(loop)}");
            foreach (string name in new[] { "GB_Lake_TempWater", "GB_Creek_TempWater" })
            {
                var go = GameObject.Find(name); var m = go.GetComponent<Renderer>().sharedMaterial;
                s.AppendLine($"WATER {name}: bounds={go.GetComponent<Renderer>().bounds}; shader={m.shader.name}; supported={m.shader.isSupported}; material={AssetDatabase.GetAssetPath(m)}; color={m.GetColor("_BaseColor")}; smooth={m.GetFloat("_Smoothness")}; metallic={m.GetFloat("_Metallic")}; collider={go.GetComponent<Collider>() != null}");
            }
            foreach (Terrain t in Object.FindObjectsByType<Terrain>(FindObjectsSortMode.None))
                s.AppendLine($"TERRAIN {t.name}: size={t.terrainData.size}; material={AssetDatabase.GetAssetPath(t.materialTemplate)}; layers={string.Join(",", t.terrainData.terrainLayers.Select(l => l.name + ":metal=" + l.metallic + ":smooth=" + l.smoothness + ":source=" + l.smoothnessSource))}");
            s.AppendLine("PIPELINE " + EditorJsonUtility.ToJson(AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath)));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, s.ToString());
        }

        public static void CaptureReview()
        {
            OpenForest();
            Capture(ReportFolder + "/Screenshots");
        }

        public static void Capture(string folder)
        {
            Directory.CreateDirectory(folder);
            Camera camera = Camera.main;
            Vector3 position = camera.transform.position;
            Quaternion rotation = camera.transform.rotation;
            RenderTexture previous = camera.targetTexture;
            RenderTexture previousActive = RenderTexture.active;
            var rt = new RenderTexture(1600, 900, 24, RenderTextureFormat.ARGBHalf);
            var pixels = new Texture2D(1600, 900, TextureFormat.RGB24, false);
            var poses = ReviewPoses();
            try
            {
                camera.targetTexture = rt;
                foreach (var pose in poses)
                {
                    camera.transform.position = pose.position;
                    camera.transform.LookAt(pose.target);
                    VolumeManager.instance.Update(camera.transform, camera.GetUniversalAdditionalCameraData().volumeLayerMask);
                    camera.Render();
                    RenderTexture.active = rt;
                    pixels.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0);
                    pixels.Apply();
                    File.WriteAllBytes(Path.Combine(folder, pose.name + ".png"), pixels.EncodeToPNG());
                }
            }
            finally
            {
                camera.targetTexture = previous;
                camera.transform.SetPositionAndRotation(position, rotation);
                RenderTexture.active = previousActive;
                Object.DestroyImmediate(rt); Object.DestroyImmediate(pixels);
            }
        }

        public static (string name, Vector3 position, Vector3 target)[] ReviewPoses()
        {
            Vector3 camp = GameObject.Find("Camp Placeholder").transform.position;
            Vector3 npc = GameObject.Find("NPC_Guide").transform.position;
            Terrain terrain = Object.FindFirstObjectByType<Terrain>();
            Vector3 Ground(float x, float z, float height) => new Vector3(x, terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y + height, z);
            return new[]
            {
                ("01_spawn_atmosphere", Ground(0,-176,4), Ground(0,-142,2)),
                ("02_forest_depth", Ground(66,-60,3), Ground(66,-60,0) + new Vector3(-25,0,0)),
                ("03_lake_water", Ground(88,5,7), Ground(139,60,1)),
                ("04_bridge_creek", Ground(3,-46,7), Ground(25,-17,1)),
                ("05_light_grove", Ground(-58,78,3), Ground(-58,78,0) + new Vector3(22,0,0)),
                ("06_heart_garden", Ground(-158,-52,3), Ground(-158,-52,0) + new Vector3(0,0,22)),
                ("07_final_hill_distance", Ground(-28,88,5), camp + Vector3.up),
                ("08_final_camp", camp + new Vector3(5,3,-7), camp + Vector3.up * .8f),
                ("09_npc_lighting", npc + new Vector3(5,3,-7), npc + Vector3.up * 2),
                ("10_fox_forest", Ground(65,-58,2.6f), Ground(65,-58,0) + new Vector3(15,0,-20))
            };
        }
    }
}
