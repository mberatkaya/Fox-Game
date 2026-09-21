using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TilkiOyunu.Foundation.Editor
{
    // Opt-in, read-only input diagnostics for the physical retest. Never injects input.
    [InitializeOnLoad]
    public static class Sprint55E11InputReview
    {
        private const string Key = "Sprint55E11.InputReview";
        private const string Path = "Documentation/Sprint55E11/NativeInput.txt";
        private static string lastState;
        private static double next;

        static Sprint55E11InputReview()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Key, false))
                {
                    InputSystem.onActionChange += ActionChanged;
                    EditorApplication.update += Observe;
                }
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    InputSystem.onActionChange -= ActionChanged;
                    EditorApplication.update -= Observe;
                }
                if (state == PlayModeStateChange.EnteredEditMode && SessionState.GetBool(Key, false))
                {
                    string save = new SaveService().SavePath, backup = save + ".sprint55e11-backup";
                    if (SessionState.GetBool(Key + ".hadSave", false))
                    {
                        File.Copy(backup, save, true);
                        File.Delete(backup);
                    }
                    else if (File.Exists(save)) File.Delete(save);
                    SessionState.SetBool(Key, false);
                    Debug.Log("E.11 input review ended; original save restored.");
                }
            };
        }

        [MenuItem("Tilki Oyunu/Sprint 5.5-E.11/Review physical input (preserves save)")]
        public static void Begin()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string save = new SaveService().SavePath, backup = save + ".sprint55e11-backup";
            if (File.Exists(backup)) throw new InvalidOperationException("Restore interrupted input-review backup first: " + backup);
            SessionState.SetBool(Key + ".hadSave", File.Exists(save));
            if (File.Exists(save)) File.Move(save, backup);
            Directory.CreateDirectory("Documentation/Sprint55E11");
            File.WriteAllText(Path, "Opt-in production input observation; no injected events or gameplay actions.\n");
            SessionState.SetBool(Key, true);
            EditorSceneManager.OpenScene(SceneIds.BootstrapPath);
            EditorApplication.isPlaying = true;
        }

        private static void ActionChanged(object obj, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed || obj is not InputAction action
                || (action.name != "WorldMap" && action.name != "Interact" && action.name != "Pause")) return;
            File.AppendAllText(Path, $"{Time.frameCount}: {action.name} performed; id={action.id}; control={action.activeControl?.path}\n");
        }

        private static void Observe()
        {
            if (EditorApplication.timeSinceStartup < next) return;
            next = EditorApplication.timeSinceStartup + .25;
            var map = UnityEngine.Object.FindFirstObjectByType<QuestMapUI>();
            var interactor = UnityEngine.Object.FindFirstObjectByType<PlayerInteractor>();
            var camera = UnityEngine.Object.FindFirstObjectByType<ThirdPersonCameraController>();
            var inputLock = UnityEngine.Object.FindFirstObjectByType<GameplayInputLock>();
            string state = $"services={GameServices.HasCurrent}; map={map?.IsOpen}; mapEnabled={map?.isActiveAndEnabled}; lock={inputLock?.IsLocked}; lookLock={camera?.IsLookInputLocked}; focus={interactor?.Focused}; update={InputSystem.settings.updateMode}";
            if (camera != null)
                foreach (string name in new[] { "Move", "Look", "Jump", "Interact", "WorldMap", "Pause" })
                    state += $"; {name}={camera.InputActions?.FindAction("Player/" + name)?.enabled}";
            if (map != null)
            {
                var action = typeof(QuestMapUI).GetField("toggleAction", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(map) as InputAction;
                state += $"; mapOwnerAction={action?.enabled}; mapControls={action?.controls.Count}";
            }
            if (state == lastState) return;
            lastState = state;
            File.AppendAllText(Path, $"{Time.frameCount}: {state}\n");
        }
    }
}
