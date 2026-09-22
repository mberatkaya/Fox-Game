using System.IO;
using UnityEditor;
using UnityEngine;

namespace TilkiOyunu.Foundation.Editor
{
    public static class WorldScaleReview
    {
        [MenuItem("Tilki Oyunu/Sprint 5.5/Review/NPC Scale")]
        public static void CaptureNpc()
        {
            Transform npc = GameObject.Find("NPC_Guide").transform;
            Capture(npc.position + npc.forward * 7f + npc.right * 4f + Vector3.up * 3.2f,
                npc.position - npc.right * 0.7f + Vector3.up * 1.5f, "10_npc_scale");
        }

        [MenuItem("Tilki Oyunu/Sprint 5.5/Review/Bridge Banks")]
        public static void CaptureBridge()
        {
            Transform bridge = GameObject.Find("Small Bridge").transform;
            Capture(bridge.position + new Vector3(-22f, 15f, -26f), bridge.position, "11_bridge_banks");
        }

        [MenuItem("Tilki Oyunu/Sprint 5.5/Review/NPC Outfit")]
        public static void CaptureOutfit()
        {
            Transform npc = GameObject.Find("NPC_Guide").transform;
            Capture(npc.position + npc.forward * 5.8f + npc.right * 3.4f + Vector3.up * 3.5f,
                npc.position + Vector3.up * 2.2f, "12_npc_outfit_front");
            Capture(npc.position - npc.forward * 5.8f - npc.right * 2.8f + Vector3.up * 3.2f,
                npc.position + Vector3.up * 1.95f, "13_npc_outfit_back");
            Capture(npc.position + npc.forward * 2.5f + npc.right * 2.4f + Vector3.up * 2.8f,
                npc.position + npc.right * 0.72f + npc.forward * 0.3f + Vector3.up * 2.3f, "14_npc_staff_grip");
        }

        private static void Capture(Vector3 position, Vector3 target, string name)
        {
            GameObject cameraObject = new GameObject("Scale Review Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            RenderTexture texture = new RenderTexture(1280, 800, 24);
            Texture2D pixels = new Texture2D(1280, 800, TextureFormat.RGB24, false);
            RenderTexture previous = RenderTexture.active;
            try
            {
                camera.transform.position = position;
                camera.transform.LookAt(target);
                camera.fieldOfView = 45f;
                camera.farClipPlane = 800f;
                camera.targetTexture = texture;
                camera.Render();
                RenderTexture.active = texture;
                pixels.ReadPixels(new Rect(0f, 0f, 1280, 800), 0, 0);
                pixels.Apply();
                Directory.CreateDirectory(Sprint55EnvironmentDressingBuilder.ScreenshotFolder);
                File.WriteAllBytes(Path.Combine(Sprint55EnvironmentDressingBuilder.ScreenshotFolder, $"{name}.png"), pixels.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active = previous;
                camera.targetTexture = null;
                Object.DestroyImmediate(pixels);
                Object.DestroyImmediate(texture);
                Object.DestroyImmediate(cameraObject);
            }
        }
    }
}
