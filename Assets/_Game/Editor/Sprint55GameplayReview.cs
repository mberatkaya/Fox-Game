using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TilkiOyunu.Foundation.Editor
{
    /// <summary>Opt-in automated Game View evidence. Teleports are review-only, never manual timing evidence.</summary>
    [InitializeOnLoad]
    public static class Sprint55GameplayReview
    {
        private const string Key = "Sprint55E.Review";
        private const string Folder = "Documentation/Sprint55E/Screenshots/";
        private static int step;
        private static double next;
        private static string capture;
        private static int recordedFrame;
        private static double nextFrame;
        static Sprint55GameplayReview() { EditorApplication.playModeStateChanged += State; }

        [MenuItem("Tilki Oyunu/Sprint 5.5 E/Automated Game View Evidence")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Save the scene and exit Play Mode before review.");
            string path = new SaveService().SavePath;
            string backup = path + ".sprint55e-backup";
            if (File.Exists(backup)) throw new InvalidOperationException("Restore interrupted review backup first: " + backup);
            SessionState.SetBool(Key + ".hadSave", File.Exists(path));
            if (File.Exists(path)) { File.Copy(path, backup); File.Delete(path); }
            Directory.CreateDirectory(Folder);
            Directory.CreateDirectory("Documentation/Sprint55E/RecordingFrames");
            SessionState.SetBool(Key, true);
            EditorSceneManager.OpenScene(SceneIds.BootstrapPath);
            EditorApplication.isPlaying = true;
        }

        private static T Find<T>() where T : Object => Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        private static InteractionContext Context => new(Find<FoxController>().gameObject);
        private static void State(PlayModeStateChange state)
        {
            if (!SessionState.GetBool(Key, false)) return;
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                step = 0; capture = null; recordedFrame = 0; nextFrame = 0; next = EditorApplication.timeSinceStartup + 5;
                EditorApplication.update += Tick;
            }
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.update -= Tick;
                string path = new SaveService().SavePath;
                string backup = path + ".sprint55e-backup";
                if (SessionState.GetBool(Key + ".hadSave", false)) { File.Copy(backup, path, true); File.Delete(backup); }
                else if (File.Exists(path)) File.Delete(path);
                SessionState.SetBool(Key, false);
                Debug.Log("Sprint 5.5 E automated evidence finished; original save restored. Manual E2E not measured.");
            }
        }

        private static void Pose(Transform target, float distance = 3.5f)
        {
            var marker = new GameObject("Transient review player pose");
            var terrain = Find<Terrain>();
            Vector3 p = target.position + Vector3.back * distance;
            p.y = terrain.SampleHeight(p) + terrain.transform.position.y + .25f;
            marker.transform.position = p;
            Vector3 look = target.position - p; look.y = 0;
            marker.transform.rotation = Quaternion.LookRotation(look);
            Find<FoxController>().TeleportTo(marker.transform);
            Find<ThirdPersonCameraController>().SnapToTarget();
            Object.DestroyImmediate(marker);
        }
        private static void Talk()
        {
            Find<GuideNpc>().Interact(Context);
            var dialogue = Find<DialoguePanelUI>();
            for (int i = 0; i < 30 && dialogue.IsOpen; i++) dialogue.Advance();
        }

        private static void Tick()
        {
            if (!EditorApplication.isPlaying || Find<FoxController>() == null) return;
            if (EditorApplication.timeSinceStartup >= nextFrame)
            {
                ScreenCapture.CaptureScreenshot($"Documentation/Sprint55E/RecordingFrames/{recordedFrame++:D4}.png");
                nextFrame = EditorApplication.timeSinceStartup + 1;
            }
            if (EditorApplication.timeSinceStartup < next) return;
            try
            {
                if (capture != null)
                {
                    ScreenCapture.CaptureScreenshot(Folder + capture + ".png");
                    capture = null; next = EditorApplication.timeSinceStartup + 2; return;
                }
                switch (step++)
                {
                    case 0: capture = "01_spawn"; break;
                    case 1: Pose(Find<GuideNpc>().transform); Find<GuideNpc>().Interact(Context); capture = "02_npc_guide"; break;
                    case 2:
                        while (Find<DialoguePanelUI>().IsOpen) Find<DialoguePanelUI>().Advance();
                        Pose(Object.FindObjectsByType<MemoryCollectible>(FindObjectsSortMode.None).Single(m => m.Memory.Id == "memory_01").transform);
                        capture = "03_memory_route"; break;
                    case 3: Pose(Find<FinalCampController>().transform, 5); capture = "06_final_camp_locked"; break;
                    case 4:
                        foreach (var m in Object.FindObjectsByType<MemoryCollectible>(FindObjectsSortMode.None)) m.Interact(Context);
                        Talk(); Talk(); Pose(Find<LightPathStart>().transform, 4);
                        Find<LightPathStart>().Interact(Context); capture = "04_light_path"; break;
                    case 5:
                        foreach (var node in Object.FindObjectsByType<LightPathNode>(FindObjectsSortMode.None).OrderBy(n => n.SequenceIndex)) node.Activate();
                        Talk(); Talk(); Pose(Find<CardMatchingStart>().transform, 4); capture = "05_heart_garden"; break;
                    case 6: Find<CardMatchingStart>().Interact(Context); capture = "05b_card_ui"; break;
                    case 7:
                        var cards = Find<CardMatchingController>();
                        foreach (var group in Enumerable.Range(0, 8).GroupBy(i => cards.GetCard(i).PairId).Select(g => g.ToArray()).ToArray())
                        { cards.TryReveal(group[0]); cards.TryReveal(group[1]); }
                        capture = "05c_cards_matched"; break;
                    case 8: Find<CardMatchingPanelUI>().Close(); Talk(); Pose(Find<FinalCampController>().transform, 5); capture = "07_final_camp_unlocked"; break;
                    case 9: Find<FinalCampController>().Interact(Context); capture = "08_final_sequence"; break;
                    default: Find<FinalSequenceController>().CompleteAndClose(); EditorApplication.isPlaying = false; return;
                }
                next = EditorApplication.timeSinceStartup + 3;
            }
            catch (Exception error) { Debug.LogException(error); EditorApplication.isPlaying = false; }
        }
    }
}
