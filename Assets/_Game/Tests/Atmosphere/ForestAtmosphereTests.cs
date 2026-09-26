using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class ForestAtmosphereTests
    {
        [SetUp]
        public void OpenForest() => EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);

        [Test]
        public void SunIsTheOnlyRealtimeShadowLight()
        {
            var lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            Assert.That(lights.Count(l => l.type == LightType.Directional), Is.EqualTo(1));
            Assert.That(RenderSettings.sun, Is.Not.Null);
            Assert.That(RenderSettings.sun.shadows, Is.Not.EqualTo(LightShadows.None));
            Assert.That(lights.Where(l => l.type != LightType.Directional).All(l => l.shadows == LightShadows.None), Is.True);
        }

        [Test]
        public void ProductionVolumeIsConnectedAndKeepsGameplayClean()
        {
            Volume volume = Object.FindObjectsByType<Volume>(FindObjectsSortMode.None).Single(v => v.isGlobal);
            VolumeProfile profile = volume.sharedProfile;
            Assert.That(profile, Is.Not.Null);
            Assert.That(profile.TryGet(out ColorAdjustments color) && color.active, Is.True);
            Assert.That(profile.TryGet(out Bloom bloom) && bloom.active, Is.True);
            Assert.That(bloom.threshold.value, Is.GreaterThan(1f), "Normal LDR surfaces should not bloom.");
            Assert.That(!profile.TryGet(out MotionBlur motion) || !motion.active || motion.intensity.value == 0f, Is.True);
            Assert.That(!profile.TryGet(out ChromaticAberration chroma) || !chroma.active || chroma.intensity.value == 0f, Is.True);
            Assert.That(!profile.TryGet(out DepthOfField dof) || !dof.active || dof.mode.value == DepthOfFieldMode.Off, Is.True);
            if (profile.TryGet(out Vignette vignette)) Assert.That(vignette.intensity.value, Is.LessThanOrEqualTo(.15f));
            Assert.That(Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing, Is.True);
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/_Game/Settings/TilkiUniversalRenderer.asset");
            Assert.That(new SerializedObject(renderer).FindProperty("postProcessData").objectReferenceValue, Is.Not.Null);
        }

        [TestCase("GB_Lake_TempWater")]
        [TestCase("GB_Creek_TempWater")]
        public void WaterHasVisibleUpwardFacesAndValidUrpMaterial(string name)
        {
            GameObject go = GameObject.Find(name);
            Assert.That(go, Is.Not.Null);
            Material material = go.GetComponent<Renderer>().sharedMaterial;
            Assert.That(material, Is.Not.Null);
            Assert.That(material.shader.name, Does.StartWith("Universal Render Pipeline/"));
            Assert.That(ShaderUtil.ShaderHasError(material.shader), Is.False);
            Assert.That(go.GetComponent<Collider>(), Is.Null, "Presentation water must not add a traversal blocker.");
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            Assert.That(mesh.uv.Length, Is.EqualTo(mesh.vertexCount));
            Assert.That(mesh.tangents.Length, Is.EqualTo(mesh.vertexCount));
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
                Assert.That(Vector3.Cross(vertices[triangles[i+1]] - vertices[triangles[i]],
                    vertices[triangles[i+2]] - vertices[triangles[i]]).y, Is.GreaterThan(0f), "Water must be visible from above.");
        }

        [Test]
        public void AudioRoutesThroughExistingMixerAndCampfireFadesToSilence()
        {
            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/_Game/Audio/Mixers/TilkiAudioMixer.mixer");
            Assert.That(mixer, Is.Not.Null);
            foreach (string name in new[] { "Master", "Music", "Ambience", "SFX" })
                Assert.That(mixer.FindMatchingGroups(name).Any(g => g.name == name), Is.True);
            foreach (AudioSource source in Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Assert.That(source.outputAudioMixerGroup, Is.Not.Null, source.name);
                Assert.That(source.outputAudioMixerGroup.audioMixer, Is.EqualTo(mixer), source.name);
                string expected = source.name == "Music Loop" ? "Music" :
                    source.name == "Forest Ambience Loop" || source.name == "Campfire Loop" ? "Ambience" : "SFX";
                Assert.That(source.outputAudioMixerGroup.name, Is.EqualTo(expected), source.name);
            }
            var fire = GameObject.Find("Campfire Loop").GetComponent<AudioSource>();
            Assert.That(fire.clip, Is.Not.Null);
            Assert.That(fire.spatialBlend, Is.GreaterThan(0f));
            AnimationCurve curve = fire.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
            Assert.That(curve.Evaluate(1f), Is.EqualTo(0f).Within(.001f));
            for (int i = 1; i <= 100; i++)
                Assert.That(curve.Evaluate(i / 100f), Is.LessThanOrEqualTo(curve.Evaluate((i-1) / 100f) + .001f));
            foreach (string name in new[] { "Music Loop", "Forest Ambience Loop" })
                Assert.That(GameObject.Find(name).GetComponent<AudioSource>().spatialBlend, Is.Zero);
        }

        [Test]
        public void DistanceFogLeavesNearGameplayClearAndTerrainRemainsMatte()
        {
            Assert.That(RenderSettings.fog, Is.True);
            Assert.That(RenderSettings.fogMode, Is.EqualTo(FogMode.Linear));
            Assert.That(RenderSettings.fogStartDistance, Is.GreaterThan(Camera.main.nearClipPlane + 20f));
            Assert.That(RenderSettings.fogEndDistance, Is.GreaterThan(RenderSettings.fogStartDistance));
            foreach (Terrain terrain in Object.FindObjectsByType<Terrain>(FindObjectsSortMode.None))
            foreach (TerrainLayer layer in terrain.terrainData.terrainLayers)
            {
                Assert.That(layer.metallic, Is.LessThanOrEqualTo(.05f));
                Assert.That(layer.smoothness, Is.LessThanOrEqualTo(.1f));
                Assert.That(layer.smoothnessSource, Is.EqualTo(TerrainLayerSmoothnessSource.Constant),
                    "Opaque diffuse alpha must not override matte smoothness and cause distant glare.");
            }
        }
    }
}
