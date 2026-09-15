using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace TilkiOyunu.Foundation.Editor
{
    /// <summary>Opt-in, transient Editor Play Mode camera review. Never saves scene or quest state.</summary>
    [InitializeOnLoad]
    public static class Sprint55AtmospherePlayReview
    {
        private const string Key = "Sprint55D.PlayReview";
        private static Camera camera;
        private static RenderTexture target;
        private static (string name, Vector3 position, Vector3 target)[] poses;
        private static readonly List<float> times = new();
        private static int poseIndex, frame, lastFrame;
        private static bool effects = true;
        private static double deadline;
        private static int oldVsync, oldTargetFps;

        static Sprint55AtmospherePlayReview()
        {
            EditorApplication.playModeStateChanged += OnState;
        }

        [MenuItem("Tilki Oyunu/Sprint 5.5 D/Play Review and Profile")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Save the current scene and exit Play Mode before review.");
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            SessionState.SetBool(Key, true);
            EditorApplication.isPlaying = true;
        }

        private static void OnState(PlayModeStateChange state)
        {
            if (!SessionState.GetBool(Key, false)) return;
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                camera = Camera.main;
                target = new RenderTexture(1600, 900, 24, RenderTextureFormat.ARGBHalf);
                camera.targetTexture = target;
                poses = Sprint55AtmosphereBuilder.ReviewPoses();
                poseIndex = -1; effects = true;
                oldVsync = QualitySettings.vSyncCount; oldTargetFps = Application.targetFrameRate;
                QualitySettings.vSyncCount = 0; Application.targetFrameRate = -1;
                deadline = EditorApplication.timeSinceStartup + 300;
                Directory.CreateDirectory("Documentation/Sprint55D/PlayReview");
                File.WriteAllText("Documentation/Sprint55D/PlayReview/performance.csv", "view,postFX,meanMs,p95Ms,maxMs,meanFPS,samples\n");
                File.WriteAllText("Documentation/Sprint55D/PlayReview/device.txt", $"Editor Play Mode; {SystemInfo.graphicsDeviceName}; {SystemInfo.processorType}; 1600x900 HDR render target. Warmup 60 frames, 180 measured frames per view. Editor overhead included; no standalone player claim.\n");
                EditorApplication.update += Update;
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                SessionState.SetBool(Key, false);
                EditorApplication.update -= Update;
                if (Application.isBatchMode) EditorApplication.Exit(0);
            }
        }

        private static void Update()
        {
            if (EditorApplication.timeSinceStartup > deadline)
            {
                File.AppendAllText("Documentation/Sprint55D/PlayReview/device.txt", "TIMEOUT: incomplete review.\n");
                Finish(); return;
            }
            if (!EditorApplication.isPlaying || Time.frameCount == lastFrame) return;
            lastFrame = Time.frameCount;
            if (poseIndex < 0)
            {
                if (Time.timeSinceLevelLoad < 2f) return; // Allow gameplay bootstrap to place the production fox.
                Object.FindFirstObjectByType<ThirdPersonCameraController>().SetExternalControl(true);
                poseIndex = 0; SetPose(); return;
            }
            frame++;
            if (frame > 60) times.Add(Time.unscaledDeltaTime * 1000f);
            if (frame < 240) return;
            float[] sorted = times.OrderBy(t => t).ToArray();
            float mean = times.Average();
            File.AppendAllText("Documentation/Sprint55D/PlayReview/performance.csv",
                string.Format(CultureInfo.InvariantCulture, "{0},{1},{2:F3},{3:F3},{4:F3},{5:F1},{6}\n", poses[poseIndex].name, effects,
                mean, sorted[(int)(sorted.Length * .95f)], sorted.Last(), 1000f / mean, sorted.Length));
            if (effects) SaveFrame(poses[poseIndex].name);
            if (effects) { effects = false; SetPose(); }
            else
            {
                effects = true; poseIndex++;
                if (poseIndex == poses.Length) { Finish(); return; }
                SetPose();
            }
        }

        private static void SetPose()
        {
            camera.transform.position = poses[poseIndex].position;
            camera.transform.LookAt(poses[poseIndex].target);
            camera.GetUniversalAdditionalCameraData().renderPostProcessing = effects;
            if (effects)
            {
                // Only transient review positioning; no serialized scene or save writes.
                var fox = Object.FindFirstObjectByType<FoxController>();
                var marker = new GameObject("Transient Review Pose");
                Vector3 near = camera.transform.position + camera.transform.forward * 7f;
                Terrain terrain = Object.FindFirstObjectByType<Terrain>();
                near.y = terrain.SampleHeight(near) + terrain.transform.position.y + .2f;
                marker.transform.position = near;
                marker.transform.rotation = Quaternion.Euler(0f, camera.transform.eulerAngles.y + 150f, 0f);
                fox.TeleportTo(marker.transform);
                Object.Destroy(marker);
            }
            frame = 0; times.Clear();
        }

        private static void SaveFrame(string name)
        {
            RenderTexture previous = RenderTexture.active;
            var pixels = new Texture2D(1600, 900, TextureFormat.RGB24, false);
            try
            {
                RenderTexture.active = target;
                pixels.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); pixels.Apply();
                File.WriteAllBytes("Documentation/Sprint55D/PlayReview/" + name + ".png", pixels.EncodeToPNG());
            }
            finally { RenderTexture.active = previous; Object.DestroyImmediate(pixels); }
        }

        private static void Finish()
        {
            EditorApplication.update -= Update;
            if (camera != null) camera.targetTexture = null;
            if (target != null) Object.DestroyImmediate(target);
            QualitySettings.vSyncCount = oldVsync; Application.targetFrameRate = oldTargetFps;
            EditorApplication.isPlaying = false;
        }
    }
}
