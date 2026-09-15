using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TilkiOyunu.Foundation.Editor
{
    /// <summary>Placement audit only; never runs a world/environment rebuild.</summary>
    public static class Sprint55GameplayPlacement
    {
        public static void ApplyFromCommandLine()
        {
            var scene = EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            var terrain = Object.FindFirstObjectByType<Terrain>();
            Physics.SyncTransforms();
            var memories = Object.FindObjectsByType<MemoryCollectible>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            string[] ids = { "memory_01", "memory_02", "memory_03", "memory_04", "memory_05" };
            Vector2[] positions = { new(18,-86), new(60,-62), new(96,40), new(-76,18), new(8,112) };
            for (int i = 0; i < ids.Length; i++)
            {
                var memory = memories.Single(m => m.Memory != null && m.Memory.Id == ids[i]);
                Place(memory.transform, positions[i], terrain, .55f);
                var anchor = GameObject.Find("MemoryVisualAnchor_" + memory.name);
                if (anchor != null) Place(anchor.transform, new Vector2(memory.transform.position.x, memory.transform.position.z), terrain, .02f, false);
            }
            var light = Object.FindFirstObjectByType<LightPathController>();
            Vector3 grove = GameObject.Find("LM_LightGrove").transform.position;
            light.transform.position = new Vector3(grove.x, terrain.SampleHeight(grove) + terrain.transform.position.y, grove.z);
            var lightData = new SerializedObject(light);
            var nodes = lightData.FindProperty("nodes");
            Vector2[] offsets = { new(-7,-7), new(-10,2), new(-5,10), new(5,10), new(11,2) };
            if (nodes.arraySize != offsets.Length) throw new InvalidOperationException("Unexpected Light Path node count; no scene saved.");
            for (int i = 0; i < nodes.arraySize; i++)
                Place(((LightPathNode)nodes.GetArrayElementAtIndex(i).objectReferenceValue).transform,
                    new Vector2(grove.x, grove.z) + offsets[i], terrain, .65f);
            Place(Object.FindFirstObjectByType<LightPathStart>().transform, new Vector2(grove.x, grove.z - 14), terrain, .55f);
            var cards = Object.FindFirstObjectByType<CardMatchingController>();
            Vector3 garden = GameObject.Find("LM_HeartGarden").transform.position;
            Place(cards.transform, new Vector2(garden.x, garden.z), terrain, .52f);
            var cardStart = Object.FindFirstObjectByType<CardMatchingStart>();
            Place(cardStart.transform, new Vector2(cardStart.transform.position.x, cardStart.transform.position.z), terrain, .7f);
            var spawn = Object.FindFirstObjectByType<PlayerSpawnPoint>();
            Vector3 direction = Object.FindFirstObjectByType<GuideNpc>().transform.position - spawn.transform.position;
            direction.y = 0;
            spawn.transform.rotation = Quaternion.LookRotation(direction);
            Object.FindFirstObjectByType<FoxController>().transform.rotation = spawn.transform.rotation;
            foreach (var t in memories.Select(m => m.transform).Concat(new[] { spawn.transform, Object.FindFirstObjectByType<FoxController>().transform }))
                PrefabUtility.RecordPrefabInstancePropertyModifications(t);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            WriteAudit("Documentation/Sprint55E/placement-after.txt");
            EditorApplication.Exit(0);
        }

        private static void Place(Transform target, Vector2 requested, Terrain terrain, float height, bool check = true)
        {
            // Search a small local clearing only. Never remove environment collision to fit an object.
            for (int attempt = 0; attempt < (check ? 81 : 1); attempt++)
            {
                float angle = attempt * 2.399963f;
                float radius = attempt == 0 ? 0 : .65f * Mathf.Sqrt(attempt);
                Vector3 p = new(requested.x + Mathf.Cos(angle) * radius, 0, requested.y + Mathf.Sin(angle) * radius);
                p.y = terrain.SampleHeight(p) + terrain.transform.position.y;
                Vector3 uv = p - terrain.transform.position;
                float slope = terrain.terrainData.GetSteepness(uv.x / terrain.terrainData.size.x, uv.z / terrain.terrainData.size.z);
                bool blocked = Physics.OverlapCapsule(p + Vector3.up * .65f, p + Vector3.up * 1.6f, .65f, ~0, QueryTriggerInteraction.Ignore)
                    .Any(c => !(c is TerrainCollider) && !c.transform.IsChildOf(target) && c.enabled);
                if (check && (blocked || slope > 32)) continue;
                target.position = p + Vector3.up * height;
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                Physics.SyncTransforms();
                return;
            }
            throw new InvalidOperationException("No clear terrain candidate for " + target.name + "; no scene saved.");
        }

        public static void AuditFromCommandLine()
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            WriteAudit("Documentation/Sprint55E/Baseline/placement.txt");
            if (Sprint5Validation.ValidateProject(out var errors)) Debug.Log("Sprint 5 validation passed.");
            else foreach (string error in errors) Debug.LogError(error);
            EditorApplication.Exit(errors.Count == 0 ? 0 : 1);
        }

        public static void WriteAudit(string path)
        {
            var report = new StringBuilder();
            var terrain = Object.FindFirstObjectByType<Terrain>();
            var types = new[] { typeof(PlayerSpawnPoint), typeof(FoxController), typeof(GuideNpc), typeof(MemoryCollectible),
                typeof(LightPathController), typeof(LightPathStart), typeof(LightPathNode), typeof(CardMatchingController),
                typeof(CardMatchingStart), typeof(FinalCampController), typeof(FinalSequenceController), typeof(ForestGameplayBootstrap) };
            foreach (var type in types)
            foreach (Component component in Object.FindObjectsByType(type, FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Vector3 p = component.transform.position;
                report.AppendLine($"{type.Name} | {Hierarchy(component.transform)} | world={p:F3} | terrainY={terrain.SampleHeight(p) + terrain.transform.position.y:F3}");
                var serialized = new SerializedObject(component);
                var property = serialized.GetIterator();
                while (property.Next(true))
                {
                    if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                    Object reference = property.objectReferenceValue;
                    string value = reference == null ? "NULL" : reference is Component c ? Hierarchy(c.transform) + "/" + c.GetType().Name
                        : reference is GameObject go ? Hierarchy(go.transform) : AssetDatabase.GetAssetPath(reference) + " (" + reference.name + ")";
                    report.AppendLine($"  {property.propertyPath} -> {value}");
                }
            }
            report.AppendLine("LANDMARKS");
            foreach (Transform t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(t => t.name.StartsWith("LM_")))
                report.AppendLine($"{Hierarchy(t)} | {t.position:F3}");
            File.WriteAllText(path, report.ToString());
        }

        private static string Hierarchy(Transform t) => t.parent == null ? t.name : Hierarchy(t.parent) + "/" + t.name;
    }
}
