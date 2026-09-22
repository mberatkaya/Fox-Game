using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static TilkiOyunu.Foundation.Editor.Sprint55C2GuideNpcProductionBuilder;

namespace TilkiOyunu.Foundation.Editor
{
    internal static class GuideNpcOutfitBuilder
    {
        private const string ArtFolder = "Assets/_Game/Art/NPC/";

        internal static void Build(Transform frame, Material cloth, Material gold, Material leather, string fernPath, string flowerPath)
        {
            Transform body = frame.Find("GuideCharacterRig/QuaterniusGuideBody");
            Dictionary<string, Transform> bones = body.GetComponentsInChildren<Transform>().ToDictionary(t => t.name);
            SkinnedMeshRenderer skin = body.GetComponentsInChildren<SkinnedMeshRenderer>().Single(r => r.name == "Superhero_Female");
            float height = MeasureHeight(frame, skin);
            PoseArm(frame, bones, "r", new Vector3(0.215f, 0.65f, 0.075f) * height, new Vector3(1f, -0.6f, 0.1f));
            PoseArm(frame, bones, "l", new Vector3(-0.17f, 0.566f, 0.025f) * height, new Vector3(-1f, -0.4f, 0.1f));

            Transform hand = bones["hand_r"];
            hand.rotation = Quaternion.LookRotation(frame.up, frame.forward);
            CurlFingers(frame, bones, "r", -50f, -60f, -50f);
            CurlFingers(frame, bones, "l", 12f, 18f, 12f);
            Vector3 grip = hand.position + hand.up * (height * 0.038f) + hand.right * (height * 0.02f);
            PoseThumb(bones, grip, frame.up);
            Transform gripSocket = new GameObject("GuideStaffGrip").transform;
            gripSocket.SetParent(hand, false);
            gripSocket.position = grip;

            DressBody(skin, cloth, leather);
            BuildCape(frame, bones["spine_03"], height, cloth);
            BuildBelt(frame, bones["pelvis"], skin, height, leather, gold);
            BuildShoulder(frame, bones["clavicle_l"], bones["upperarm_l"], fernPath, "GuideShoulderFern_Left", cloth, height, -1f);
            BuildShoulder(frame, bones["clavicle_r"], bones["upperarm_r"], fernPath, "GuideShoulderFern_Right", cloth, height, 1f);

            Vector3 chest = frame.InverseTransformPoint(bones["spine_03"].position);
            chest.z = BakedVertices(frame, skin).Where(v => Mathf.Abs(v.y - chest.y) < 0.06f && Mathf.Abs(v.x) < 0.055f).Max(v => v.z) + 0.025f;
            GameObject brooch = CreatePrimitiveChild(frame, PrimitiveType.Sphere, "GuideBeacon", chest,
                new Vector3(0.044f, 0.058f, 0.021f) * height, gold);
            brooch.transform.SetParent(bones["spine_03"], true);
            BuildStaff(frame, hand, grip, height, leather, gold, flowerPath);
            CreatePrimitiveChild(frame, PrimitiveType.Cylinder, "GuideVisibilityRing", new Vector3(0f, 0.014f, 0f),
                new Vector3(0.4f * height, 0.008f, 0.4f * height), gold);
        }

        private static void PoseArm(Transform frame, Dictionary<string, Transform> bones, string side, Vector3 localTarget, Vector3 bendHint)
        {
            Transform upper = bones[$"upperarm_{side}"];
            Transform lower = bones[$"lowerarm_{side}"];
            Transform hand = bones[$"hand_{side}"];
            Vector3 target = frame.TransformPoint(localTarget);
            Vector3 delta = target - upper.position;
            float upperLength = Vector3.Distance(upper.position, lower.position);
            float lowerLength = Vector3.Distance(lower.position, hand.position);
            float distance = Mathf.Clamp(delta.magnitude, Mathf.Abs(upperLength - lowerLength) + 0.001f, upperLength + lowerLength - 0.001f);
            Vector3 direction = delta.normalized;
            Vector3 bend = Vector3.ProjectOnPlane(frame.TransformDirection(bendHint), direction).normalized;
            float along = (upperLength * upperLength - lowerLength * lowerLength + distance * distance) / (2f * distance);
            Vector3 elbow = upper.position + direction * along + bend * Mathf.Sqrt(Mathf.Max(0f, upperLength * upperLength - along * along));
            upper.rotation = Quaternion.FromToRotation(lower.position - upper.position, elbow - upper.position) * upper.rotation;
            lower.rotation = Quaternion.FromToRotation(hand.position - lower.position, target - lower.position) * lower.rotation;
        }

        private static void CurlFingers(Transform frame, Dictionary<string, Transform> bones, string side, float first, float second, float third)
        {
            foreach (string finger in new[] { "index", "middle", "ring", "pinky" })
            {
                float[] angles = { first, second, third };
                for (int joint = 1; joint <= 3; joint++)
                {
                    Transform bone = bones[$"{finger}_{joint:00}_{side}"];
                    Vector3 axis = side == "r" ? frame.up : bones["hand_l"].forward;
                    bone.rotation = Quaternion.AngleAxis(angles[joint - 1], axis) * bone.rotation;
                }
            }
        }

        private static void PoseThumb(Dictionary<string, Transform> bones, Vector3 grip, Vector3 up)
        {
            Transform first = bones["thumb_01_r"];
            Transform second = bones["thumb_02_r"];
            Transform third = bones["thumb_03_r"];
            Vector3 target = grip + up * 0.035f;
            first.rotation = Quaternion.FromToRotation(second.position - first.position, target - first.position) * first.rotation;
            second.rotation = Quaternion.FromToRotation(third.position - second.position, target - second.position) * second.rotation;
        }

        private static void DressBody(SkinnedMeshRenderer skin, Material cloth, Material leather)
        {
            Mesh mesh = Object.Instantiate(skin.sharedMesh);
            mesh.name = "GuideNpc_FittedOutfit";
            BoneWeight[] weights = mesh.boneWeights;
            List<int>[] groups = { new(), new(), new() };
            int Region(int vertex)
            {
                BoneWeight weight = weights[vertex];
                int index = weight.boneIndex0;
                float strongest = weight.weight0;
                if (weight.weight1 > strongest) { index = weight.boneIndex1; strongest = weight.weight1; }
                if (weight.weight2 > strongest) { index = weight.boneIndex2; strongest = weight.weight2; }
                if (weight.weight3 > strongest) index = weight.boneIndex3;
                string bone = skin.bones[index].name;
                if (bone.StartsWith("calf") || bone.StartsWith("foot") || bone.StartsWith("ball")) return 2;
                if (bone.StartsWith("spine") || bone.StartsWith("pelvis") || bone.StartsWith("thigh") || bone.StartsWith("clavicle") || bone.StartsWith("upperarm")) return 1;
                return 0;
            }
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int a = Region(triangles[i]);
                int b = Region(triangles[i + 1]);
                int c = Region(triangles[i + 2]);
                int group = a == b || a == c ? a : b;
                groups[group].AddRange(new[] { triangles[i], triangles[i + 1], triangles[i + 2] });
            }
            mesh.subMeshCount = groups.Length;
            for (int i = 0; i < groups.Length; i++) mesh.SetTriangles(groups[i], i);
            skin.sharedMesh = SaveMesh(mesh);
            skin.sharedMaterials = new[] { skin.sharedMaterial, cloth, leather };
        }

        private static void BuildCape(Transform frame, Transform spine, float height, Material cloth)
        {
            const int columns = 11;
            float[] levels = { 0.84f, 0.8f, 0.68f, 0.55f, 0.36f, 0.17f };
            float[] widths = { 0.065f, 0.17f, 0.16f, 0.15f, 0.19f, 0.22f };
            float[] depths = { 0.075f, 0.088f, 0.1f, 0.105f, 0.115f, 0.125f };
            List<Vector3> vertices = new();
            List<int> triangles = new();
            for (int row = 0; row < levels.Length; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    float t = column / (columns - 1f) * 2f - 1f;
                    float angle = t * 1.25f;
                    float x = Mathf.Sin(angle) / Mathf.Sin(1.25f) * widths[row];
                    float y = levels[row] - (row == levels.Length - 1 && column % 2 == 1 ? 0.025f : 0f);
                    float z = -depths[row] * (0.55f + 0.45f * Mathf.Cos(angle));
                    vertices.Add(new Vector3(x, y, z) * height);
                    if (row == 0 || column == 0) continue;
                    int current = row * columns + column;
                    triangles.AddRange(new[] { current, current - columns, current - columns - 1, current, current - columns - 1, current - 1 });
                }
            }
            AddMesh(frame, spine, "GuideLeafCloak_Back", "GuideNpc_FittedCape", vertices, triangles, cloth, true);
        }

        private static void BuildBelt(Transform frame, Transform pelvis, SkinnedMeshRenderer skin, float height, Material leather, Material gold)
        {
            float y = frame.InverseTransformPoint(pelvis.position).y + height * 0.044f;
            Bounds section = TorsoSection(frame, skin, y);
            List<Vector3> vertices = new();
            List<int> triangles = new();
            const int segments = 24;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;
                for (int row = 0; row < 2; row++)
                    vertices.Add(new Vector3(Mathf.Sin(angle) * (section.extents.x + 0.022f), y + (row - 0.5f) * 0.032f * height,
                        section.center.z + Mathf.Cos(angle) * (section.extents.z + 0.022f)));
                if (i == 0) continue;
                int n = i * 2;
                triangles.AddRange(new[] { n, n - 2, n - 1, n, n - 1, n + 1 });
            }
            AddMesh(frame, pelvis, "GuideWaistBelt", "GuideNpc_FittedBelt", vertices, triangles, leather, true);
            GameObject buckle = CreatePrimitiveChild(frame, PrimitiveType.Cube, "GuideBeltClasp",
                new Vector3(0f, y, section.max.z + 0.036f), new Vector3(0.19f, 0.13f, 0.045f), gold);
            buckle.transform.SetParent(pelvis, true);
        }

        private static void BuildShoulder(Transform frame, Transform clavicle, Transform shoulder, string path, string name, Material cloth, float height, float side)
        {
            Vector3 position = frame.InverseTransformPoint(shoulder.position) + new Vector3(side * 0.01f, 0.035f, 0.005f);
            AddNatureChild(frame, path, name, cloth, position, Quaternion.Euler(0f, side * 45f, side * -15f), Vector3.one);
            Transform fern = frame.Find(name);
            Renderer[] renderers = fern.GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
            fern.localScale *= (height * 0.19f) / Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            fern.SetParent(clavicle, true);
        }

        private static void BuildStaff(Transform frame, Transform hand, Vector3 grip, float height, Material wood, Material gold, string flowerPath)
        {
            Transform staff = new GameObject("GuideStaff").transform;
            staff.SetParent(frame, false);
            staff.position = grip;
            staff.rotation = Quaternion.LookRotation(frame.forward, frame.up);
            float gripY = frame.InverseTransformPoint(grip).y;
            float length = height * 1.05f;
            CreatePrimitiveChild(staff, PrimitiveType.Cylinder, "GuideStaff_Shaft", Vector3.up * (length * 0.5f + 0.055f - gripY), new Vector3(0.085f, length * 0.5f, 0.085f), wood);
            CreatePrimitiveChild(staff, PrimitiveType.Cylinder, "GuideStaff_GripWrap", Vector3.zero, new Vector3(0.092f, 0.14f, 0.092f), wood);
            float top = length + 0.055f - gripY;
            CreatePrimitiveChild(staff, PrimitiveType.Sphere, "GuideStaff_GlowSeed", Vector3.up * top, Vector3.one * 0.2f, gold);
            AddNatureChild(staff, flowerPath, "GuideStaff_FlowerAccent", gold, Vector3.up * (top + 0.045f), Quaternion.identity, Vector3.one * 0.085f);
            staff.SetParent(hand, true);
        }

        private static float MeasureHeight(Transform frame, SkinnedMeshRenderer skin)
        {
            Vector3[] vertices = BakedVertices(frame, skin);
            return vertices.Max(v => v.y) - vertices.Min(v => v.y);
        }

        private static Bounds TorsoSection(Transform frame, SkinnedMeshRenderer skin, float height)
        {
            Vector3[] vertices = BakedVertices(frame, skin).Where(v => Mathf.Abs(v.y - height) < 0.045f && Mathf.Abs(v.x) < 0.5f).ToArray();
            if (vertices.Length == 0) throw new System.InvalidOperationException("No body surface found for guide outfit fitting.");
            Bounds bounds = new(vertices[0], Vector3.zero);
            foreach (Vector3 vertex in vertices) bounds.Encapsulate(vertex);
            return bounds;
        }

        private static Vector3[] BakedVertices(Transform frame, SkinnedMeshRenderer skin)
        {
            Mesh mesh = new();
            try
            {
                skin.BakeMesh(mesh, true);
                return mesh.vertices.Select(v => frame.InverseTransformPoint(skin.transform.TransformPoint(v))).ToArray();
            }
            finally { Object.DestroyImmediate(mesh); }
        }

        private static void AddMesh(Transform frame, Transform bone, string name, string meshName, List<Vector3> vertices, List<int> triangles, Material material, bool doubleSided)
        {
            if (doubleSided)
            {
                int count = vertices.Count;
                vertices.AddRange(vertices.ToArray());
                int[] front = triangles.ToArray();
                for (int i = 0; i < front.Length; i += 3) triangles.AddRange(new[] { front[i] + count, front[i + 2] + count, front[i + 1] + count });
            }
            Mesh mesh = new() { name = meshName };
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            GameObject garment = new(name, typeof(MeshFilter), typeof(MeshRenderer));
            garment.transform.SetParent(frame, false);
            garment.GetComponent<MeshFilter>().sharedMesh = SaveMesh(mesh);
            garment.GetComponent<MeshRenderer>().sharedMaterial = material;
            garment.transform.SetParent(bone, true);
        }

        private static Mesh SaveMesh(Mesh mesh)
        {
            string path = ArtFolder + mesh.name + ".asset";
            Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing == null) { AssetDatabase.CreateAsset(mesh, path); return mesh; }
            EditorUtility.CopySerialized(mesh, existing);
            EditorUtility.SetDirty(existing);
            Object.DestroyImmediate(mesh);
            return existing;
        }
    }
}
