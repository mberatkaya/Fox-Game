using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint55GameplayPlacementTests
    {
        [SetUp] public void Open() => EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
        private static T[] All<T>() where T : Object => Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        private static T One<T>() where T : Object { Assert.That(All<T>().Length, Is.EqualTo(1), typeof(T).Name); return All<T>().Single(); }

        [Test]
        public void ProductionInstancesAndPersistentIdsAreUnique()
        {
            One<PlayerSpawnPoint>(); One<FoxController>(); One<GuideNpc>(); One<LightPathController>();
            One<LightPathStart>(); One<CardMatchingController>(); One<CardMatchingStart>(); One<FinalCampController>(); One<FinalSequenceController>();
            var memories = All<MemoryCollectible>();
            Assert.That(memories.Select(m => m.Memory.Id), Is.EquivalentTo(new[] { "memory_01", "memory_02", "memory_03", "memory_04", "memory_05" }));
            Assert.That(memories.All(m => m.Quest.Id == "collect_memories"), Is.True);
            Assert.That(One<LightPathController>().Quest.Id, Is.EqualTo("light_path"));
            Assert.That(One<CardMatchingController>().Quest.Id, Is.EqualTo("card_matching"));
            var guide = new SerializedObject(One<GuideNpc>());
            foreach (string field in new[] { "collectMemoriesQuest", "lightPathQuest", "cardMatchingQuest", "dialogueUI", "introDialogue", "readyToTurnInDialogue" })
                Assert.That(guide.FindProperty(field).objectReferenceValue, Is.Not.Null, field);
        }

        [Test]
        public void MinigamesAndFinaleKeepTheirSerializedReferences()
        {
            var light = One<LightPathController>();
            var nodes = new SerializedObject(light).FindProperty("nodes");
            Assert.That(nodes.arraySize, Is.EqualTo(All<LightPathNode>().Length));
            Assert.That(nodes.arraySize, Is.GreaterThan(1));
            var seen = new System.Collections.Generic.HashSet<LightPathNode>();
            for (int i = 0; i < nodes.arraySize; i++)
            {
                var node = nodes.GetArrayElementAtIndex(i).objectReferenceValue as LightPathNode;
                Assert.That(node, Is.Not.Null);
                Assert.That(seen.Add(node), Is.True);
                Assert.That(node.SequenceIndex, Is.EqualTo(i));
                Assert.That(new SerializedObject(node).FindProperty("controller").objectReferenceValue, Is.EqualTo(light));
            }
            Assert.That(One<CardMatchingController>().PairCount, Is.EqualTo(4));
            Assert.That(One<CardMatchingController>().CardCount, Is.EqualTo(8));
            foreach (Component c in new Component[] { One<LightPathStart>(), One<CardMatchingStart>() })
                Assert.That(new SerializedObject(c).FindProperty("controller").objectReferenceValue, Is.Not.Null);
            Assert.That(new SerializedObject(One<CardMatchingController>()).FindProperty("panel").objectReferenceValue, Is.Not.Null);
            var finale = One<FinalSequenceController>();
            Assert.That(finale.FinalMessage, Is.Not.Null);
            Assert.That(finale.GameplayCamera, Is.EqualTo(Camera.main));
            Assert.That(finale.CameraController, Is.EqualTo(Camera.main.GetComponent<ThirdPersonCameraController>()));
            Assert.That(finale.CameraFocus.IsChildOf(One<FinalCampController>().transform), Is.True);
            Assert.That(new SerializedObject(One<FinalCampController>()).FindProperty("finalSequence").objectReferenceValue, Is.EqualTo(finale));
        }

        [Test]
        public void InteractionLocationsAreGroundedAndHaveOpenApproaches()
        {
            var terrain = One<Terrain>();
            Physics.SyncTransforms();
            var objects = All<MemoryCollectible>().Cast<Component>().Concat(All<LightPathNode>()).Concat(new Component[] {
                One<PlayerSpawnPoint>(), One<GuideNpc>(), One<LightPathStart>(), One<CardMatchingStart>(), One<FinalCampController>() });
            foreach (var item in objects)
            {
                Vector3 p = item.transform.position;
                float ground = terrain.SampleHeight(p) + terrain.transform.position.y;
                Assert.That(p.y - ground, Is.InRange(-.05f, 1.5f), item.name + " terrain clearance");
                Vector3 uv = p - terrain.transform.position;
                Assert.That(uv.x, Is.InRange(2, terrain.terrainData.size.x - 2), item.name);
                Assert.That(uv.z, Is.InRange(2, terrain.terrainData.size.z - 2), item.name);
                // NPC/camp have intentional solid centers. At least one nearby interaction approach must be open.
                bool open = Enumerable.Range(0, 8).Any(i =>
                {
                    Vector3 q = p + Quaternion.Euler(0, i * 45, 0) * Vector3.forward * 1.5f;
                    q.y = terrain.SampleHeight(q) + terrain.transform.position.y;
                    // Spawn has an intentional raised walkable platform; test clearance above its surface.
                    var supports = Physics.RaycastAll(q + Vector3.up * .6f, Vector3.down, .8f, ~0, QueryTriggerInteraction.Ignore)
                        .Where(h => h.normal.y > .7f && !(h.collider is CharacterController));
                    if (supports.Any()) q.y = Mathf.Max(q.y, supports.Max(h => h.point.y));
                    return !Physics.OverlapCapsule(q + Vector3.up * .55f, q + Vector3.up * 1.4f, .45f, ~0, QueryTriggerInteraction.Ignore)
                        .Any(c => !(c is TerrainCollider) && !c.transform.IsChildOf(item.transform) && !(c is CharacterController));
                });
                Assert.That(open, Is.True, item.name + " has no clear approach");
            }
        }

        [Test]
        public void OrderedLightRouteHasClearWalkableSegments()
        {
            var terrain = One<Terrain>();
            Physics.SyncTransforms();
            var route = new[] { One<LightPathStart>().transform.position }.Concat(All<LightPathNode>().OrderBy(n => n.SequenceIndex).Select(n => n.transform.position)).ToArray();
            for (int segment = 1; segment < route.Length; segment++)
            {
                Assert.That(Vector3.Distance(route[segment - 1], route[segment]), Is.InRange(4, 20), "node spacing");
                int samples = Mathf.CeilToInt(Vector3.Distance(route[segment - 1], route[segment]));
                for (int sample = 0; sample <= samples; sample++)
                {
                    Vector3 p = Vector3.Lerp(route[segment - 1], route[segment], sample / (float)samples);
                    p.y = terrain.SampleHeight(p) + terrain.transform.position.y;
                    var hits = Physics.OverlapCapsule(p + Vector3.up * .6f, p + Vector3.up * 1.4f, .45f, ~0, QueryTriggerInteraction.Ignore)
                        .Where(c => !(c is TerrainCollider)).ToArray();
                    Assert.That(hits, Is.Empty, $"Light segment {segment}, sample {sample}: {string.Join(", ", hits.Select(c => c.name))}");
                    Vector3 uv = p - terrain.transform.position;
                    Assert.That(terrain.terrainData.GetSteepness(uv.x / terrain.terrainData.size.x, uv.z / terrain.terrainData.size.z), Is.LessThan(35), "Light route slope");
                }
            }
        }

        [Test]
        public void ActivitiesUseLandmarksAndMemoriesUseDistinctRegions()
        {
            void Near(Component c, string landmark, float radius) => Assert.That(Vector2.Distance(
                new Vector2(c.transform.position.x, c.transform.position.z),
                new Vector2(GameObject.Find(landmark).transform.position.x, GameObject.Find(landmark).transform.position.z)), Is.LessThan(radius), c.name);
            Near(One<PlayerSpawnPoint>(), "LM_SpawnMeadow", 15);
            Near(One<GuideNpc>(), "LM_NPCGrove", 15);
            Near(One<LightPathStart>(), "LM_LightGrove", 25);
            foreach (var node in All<LightPathNode>()) Near(node, "LM_LightGrove", 30);
            Near(One<CardMatchingStart>(), "LM_HeartGarden", 20);
            Near(One<FinalCampController>(), "LM_FinalHill", 20);
            var memories = All<MemoryCollectible>();
            for (int i = 0; i < memories.Length; i++)
            for (int j = i + 1; j < memories.Length; j++)
                Assert.That(Vector3.Distance(memories[i].transform.position, memories[j].transform.position), Is.GreaterThan(25), "Memories cluster together");
        }
    }
}
