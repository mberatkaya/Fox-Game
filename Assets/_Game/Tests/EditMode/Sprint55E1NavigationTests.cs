using System;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint55E1NavigationTests
    {
        private string directory;
        private SaveService save;
        private QuestService quests;
        private GameObject target;
        private MapMarker marker;
        private QuestDefinition memories, lights, cards;

        [SetUp]
        public void Setup()
        {
            directory = Path.Combine(Path.GetTempPath(), "TilkiE1", Guid.NewGuid().ToString("N"));
            save = new SaveService(Path.Combine(directory, "save.json"));
            quests = new QuestService(save, null);
            memories = AssetDatabase.LoadAssetAtPath<QuestDefinition>("Assets/_Game/Data/Quests/01_AnilariTopla.asset");
            lights = AssetDatabase.LoadAssetAtPath<QuestDefinition>("Assets/_Game/Data/Quests/Quest_LightPath.asset");
            cards = AssetDatabase.LoadAssetAtPath<QuestDefinition>("Assets/_Game/Data/Quests/Quest_CardMatching.asset");
            target = new GameObject("Map target test"); marker = target.AddComponent<MapMarker>();
        }

        [TearDown]
        public void Cleanup()
        {
            UnityEngine.Object.DestroyImmediate(target);
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }

        private bool Visible(MapMarkerType type, string id = "")
        {
            marker.Configure(type, "Target", id);
            return marker.IsVisible(quests, true, null);
        }

        [Test]
        public void FreshSaveShowsGuideAndHidesFutureObjectives()
        {
            Assert.That(Visible(MapMarkerType.NPC), Is.True);
            Assert.That(Visible(MapMarkerType.QuestObjective, "memory_01"), Is.False);
            Assert.That(Visible(MapMarkerType.LightPath), Is.False);
            Assert.That(Visible(MapMarkerType.CardQuest), Is.False);
            Assert.That(Visible(MapMarkerType.FinalCamp), Is.False);
        }

        [Test]
        public void IngredientsDisappearOnceAndStayGoneOnReloadThenGuideIsHighlighted()
        {
            Assert.That(memories.Id, Is.EqualTo("collect_memories"));
            Assert.That(memories.Title, Is.EqualTo("Bir Waffle Anısı"));
            Assert.That(quests.StartQuest(memories), Is.True);
            Assert.That(Visible(MapMarkerType.NPC), Is.False);
            string[] names = { "Un", "Süt", "Yumurta", "Tereyağı", "Çilek" };
            for (int i = 1; i <= 5; i++)
            {
                var item = AssetDatabase.LoadAssetAtPath<MemoryDefinition>($"Assets/_Game/Data/Memories/Memory_{i:00}.asset");
                Assert.That(item.Id, Is.EqualTo($"memory_{i:00}"));
                Assert.That(item.DisplayName, Is.EqualTo(names[i - 1]));
                Assert.That(Visible(MapMarkerType.QuestObjective, item.Id), Is.True);
                Assert.That(quests.RecordMemoryCollected(item, memories), Is.True);
                Assert.That(quests.RecordMemoryCollected(item, memories), Is.False);
                quests = new QuestService(save, null);
                Assert.That(Visible(MapMarkerType.QuestObjective, item.Id), Is.False);
                Assert.That(quests.CollectedMemoryIds.Count, Is.EqualTo(i));
            }
            Assert.That(Visible(MapMarkerType.NPC), Is.True);
            Assert.That(marker.IsHighlighted(quests, null), Is.True);
        }

        [Test]
        public void EachStageReconstructsAndFinalRequiresAllTurnIns()
        {
            quests.StartQuest(memories); quests.AddProgress(memories, 5); quests.TurnInQuest(memories);
            Assert.That(Visible(MapMarkerType.NPC), Is.True);
            quests.StartQuest(lights);
            Assert.That(Visible(MapMarkerType.LightPath), Is.True);
            Assert.That(Visible(MapMarkerType.CardQuest), Is.False);
            quests.RecordLightPathCompleted(lights);
            Assert.That(Visible(MapMarkerType.LightPath), Is.False);
            Assert.That(Visible(MapMarkerType.NPC), Is.True);
            quests.TurnInQuest(lights); quests.StartQuest(cards);
            quests = new QuestService(save, null);
            Assert.That(Visible(MapMarkerType.CardQuest), Is.True);
            Assert.That(Visible(MapMarkerType.FinalCamp), Is.False);
            quests.RecordCardMatchingCompleted(cards);
            Assert.That(Visible(MapMarkerType.CardQuest), Is.False);
            Assert.That(Visible(MapMarkerType.FinalCamp), Is.False);
            quests.TurnInQuest(cards); quests = new QuestService(save, null);
            Assert.That(Visible(MapMarkerType.FinalCamp), Is.True);
            Assert.That(Visible(MapMarkerType.NPC), Is.False);
            quests.SaveData.finalCompleted = true; save.Save(quests.SaveData);
            quests = new QuestService(save, null);
            Assert.That(Visible(MapMarkerType.FinalCamp), Is.False);
        }
    }
}
