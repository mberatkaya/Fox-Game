using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace TilkiOyunu.Foundation.Editor
{
    [InitializeOnLoad]
    public static class Sprint55E1Review
    {
        private const string Key = "Sprint55E1.NormalInputReview";
        private const string Folder = "Documentation/Sprint55E1/";
        private static readonly HashSet<string> observed = new();
        private static double started, nextCapture;
        static Sprint55E1Review()
        {
            EditorApplication.playModeStateChanged += StateChanged;
            if (Environment.GetEnvironmentVariable("TILKI_E1_SCREENSHOTS") == "1")
                EditorApplication.update += CaptureTestMaps;
        }

        // Optional technical screenshots of input tests; never normal-input timing evidence.
        private static void CaptureTestMaps()
        {
            if (!EditorApplication.isPlaying || !GameServices.HasCurrent
                || GameServices.Current.Quest.GetQuestStatus("collect_memories") != QuestStatus.NotStarted) return;
            var map = UnityEngine.Object.FindFirstObjectByType<QuestMapUI>();
            if (map == null) return;
            string name = map.IsOpen ? "02_world_map" : "01_minimap";
            if (!observed.Add("test-" + name)) return;
            Directory.CreateDirectory(Folder + "Screenshots");
            ScreenCapture.CaptureScreenshot(Folder + "Screenshots/" + name + ".png");
        }

        [MenuItem("Tilki Oyunu/Sprint 5.5-E.1/Play production review")]
        public static void Play()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string path = new SaveService().SavePath;
            string backup = path + ".sprint55e1-backup";
            if (File.Exists(backup)) throw new InvalidOperationException("Restore the interrupted review backup first: " + backup);
            SessionState.SetBool(Key + ".hadSave", File.Exists(path));
            if (File.Exists(path)) File.Move(path, backup);
            SessionState.SetBool(Key, true);
            Directory.CreateDirectory(Folder + "Screenshots");
            EditorSceneManager.OpenScene(SceneIds.BootstrapPath);
            EditorApplication.isPlaying = true;
        }

        private static void StateChanged(PlayModeStateChange state)
        {
            if (!SessionState.GetBool(Key, false)) return;
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                observed.Clear(); started = 0; nextCapture = 0;
                File.WriteAllText(Folder + "NormalInputObservations.txt", "Read-only observer. No movement, teleport, interaction, or quest completion is performed by this helper.\n");
                EditorApplication.update += Observe;
            }
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.update -= Observe;
                string path = new SaveService().SavePath, backup = path + ".sprint55e1-backup";
                if (File.Exists(path)) File.Copy(path, Folder + "NormalInputReviewSave.json", true);
                if (SessionState.GetBool(Key + ".hadSave", false))
                {
                    File.Copy(backup, path, true); File.Delete(backup);
                }
                else if (File.Exists(path)) File.Delete(path);
                SessionState.SetBool(Key, false);
                Debug.Log("Normal-input review ended. Original save restored.");
            }
        }

        private static void Observe()
        {
            if (!GameServices.HasCurrent || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != SceneIds.Forest) return;
            var map = UnityEngine.Object.FindFirstObjectByType<QuestMapUI>();
            var player = UnityEngine.Object.FindFirstObjectByType<FoxController>();
            if (map == null || player == null || EditorApplication.timeSinceStartup < nextCapture) return;
            if (started == 0) { started = EditorApplication.timeSinceStartup; Record("Spawn"); }
            var quests = GameServices.Current.Quest;
            if (!map.IsOpen) Capture("01_minimap");
            if (map.IsOpen) Capture("02_world_map");
            var dialogue = UnityEngine.Object.FindFirstObjectByType<DialoguePanelUI>();
            if (dialogue != null && dialogue.IsOpen) Record("First NPC conversation");
            foreach (string id in QuestService.FinalCampRequiredQuestIds)
            {
                var status = quests.GetQuestStatus(id);
                if (status != QuestStatus.NotStarted) Record(id + ": " + status);
                if (status == QuestStatus.ReadyToTurnIn && map.IsOpen) Capture("09_npc_turnin");
            }
            if (quests.GetQuestStatus("collect_memories") == QuestStatus.Active)
            {
                if (map.IsOpen) Capture("04_waffle_map_markers");
                foreach (var item in UnityEngine.Object.FindObjectsByType<MemoryCollectible>(FindObjectsSortMode.None))
                    if (!map.IsOpen && !quests.HasCollectedMemory(item.Memory) && Vector3.Distance(player.transform.position, item.transform.position) < 6)
                        Capture("03_waffle_ingredient");
            }
            var light = UnityEngine.Object.FindFirstObjectByType<LightPathController>();
            if (quests.GetQuestStatus("light_path") == QuestStatus.Active && map.IsOpen) Capture("05_light_path_map");
            if (light != null && light.State == LightPathRunState.Running && !map.IsOpen) Capture("06_light_path_hud");
            if (quests.GetQuestStatus("card_matching") == QuestStatus.Active && map.IsOpen) Capture("07_card_quest_marker");
            if (quests.IsFinalCampUnlocked && map.IsOpen) Capture("08_final_camp_marker");
            if (quests.SaveData.finalCompleted) Record("finalCompleted");
        }

        private static void Record(string name)
        {
            if (observed.Add(name)) File.AppendAllText(Folder + "NormalInputObservations.txt", $"{EditorApplication.timeSinceStartup - started:F2}s · {name}\n");
        }

        private static void Capture(string name)
        {
            if (EditorApplication.timeSinceStartup < nextCapture || !observed.Add(name)) return;
            ScreenCapture.CaptureScreenshot(Folder + "Screenshots/" + name + ".png");
            nextCapture = EditorApplication.timeSinceStartup + 1;
        }
    }
}
