using System.Collections.Generic;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    /// <summary>Small project-owned food silhouettes; no external food pack or save changes.</summary>
    public sealed class WaffleIngredientVisual : MonoBehaviour
    {
        private readonly List<Material> materials = new();
        private MemoryCollectible collectible;
        private GameObject indicator;

        public Renderer[] Build(MemoryCollectible owner)
        {
            collectible = owner;
            foreach (var r in owner.GetComponentsInChildren<Renderer>(true)) r.enabled = false;
            var root = new GameObject("Waffle Ingredient").transform;
            root.SetParent(transform, false);
            // Keep all five objects readable even if an old token had a tiny root scale.
            Vector3 s = transform.lossyScale;
            root.localScale = new Vector3(1 / Mathf.Max(.01f, Mathf.Abs(s.x)), 1 / Mathf.Max(.01f, Mathf.Abs(s.y)), 1 / Mathf.Max(.01f, Mathf.Abs(s.z)));
            root.localPosition = Vector3.up * .35f;
            var cream = Material(new Color(.96f, .89f, .69f));
            var white = Material(new Color(.98f, .97f, .88f));
            var red = Material(new Color(.85f, .12f, .15f));
            var green = Material(new Color(.23f, .49f, .20f));
            var gold = Material(new Color(1f, .72f, .19f));
            switch (owner.Memory.Id)
            {
                case "memory_01":
                    Part(root, "Flour sack", PrimitiveType.Cube, new(0, .45f, 0), new(.8f, 1.05f, .55f), cream);
                    Part(root, "Folded sack top", PrimitiveType.Cube, new(0, 1f, 0), new(.82f, .15f, .4f), gold);
                    Part(root, "Flour label", PrimitiveType.Cube, new(0, .48f, -.285f), new(.5f, .38f, .035f), white);
                    break;
                case "memory_02":
                    Part(root, "Milk bottle", PrimitiveType.Cylinder, new(0, .4f, 0), new(.60f, .5f, .6f), white);
                    Part(root, "Milk neck", PrimitiveType.Cylinder, new(0, .96f, 0), new(.32f, .15f, .32f), white);
                    Part(root, "Red bottle cap", PrimitiveType.Cylinder, new(0, 1.14f, 0), new(.36f, .06f, .36f), red);
                    break;
                case "memory_03":
                    Part(root, "Egg", PrimitiveType.Sphere, new(0, .48f, 0), new(.73f, 1.05f, .73f), white);
                    Part(root, "Egg cup", PrimitiveType.Cylinder, new(0, .07f, 0), new(.65f, .12f, .65f), cream);
                    break;
                case "memory_04":
                    Part(root, "Butter wrapper", PrimitiveType.Cube, new(0, .08f, 0), new(1.10f, .12f, .72f), white);
                    Part(root, "Butter", PrimitiveType.Cube, new(0, .32f, 0), new(.87f, .43f, .56f), gold);
                    break;
                case "memory_05":
                    Part(root, "Strawberry", PrimitiveType.Sphere, new(0, .45f, 0), new(.83f, .96f, .83f), red);
                    for (int i = 0; i < 5; i++)
                    {
                        float angle = i * Mathf.PI * 2 / 5;
                        var leaf = Part(root, "Strawberry leaf", PrimitiveType.Sphere, new(Mathf.Cos(angle) * .20f, .91f, Mathf.Sin(angle) * .20f), new(.45f, .09f, .2f), green);
                        leaf.localEulerAngles = new(0, -i * 72, 0);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        float a = i * 2.39996f;
                        Part(root, "Seed", PrimitiveType.Sphere, new(Mathf.Cos(a) * .38f, .25f + (i % 3) * .18f, Mathf.Sin(a) * .38f), new(.045f, .075f, .045f), cream);
                    }
                    break;
            }
            var glow = Material(new Color(1f, .8f, .36f));
            glow.EnableKeyword("_EMISSION"); glow.SetColor("_EmissionColor", new Color(1f, .65f, .17f) * .65f);
            indicator = Part(root, "Active ingredient beacon", PrimitiveType.Sphere, new(0, 1.7f, 0), new(.20f, .32f, .20f), glow).gameObject;
            return root.GetComponentsInChildren<Renderer>();
        }

        private void LateUpdate()
        {
            if (indicator == null || collectible == null) return;
            indicator.SetActive(GameServices.HasCurrent && collectible.Memory != null
                && !GameServices.Current.Quest.HasCollectedMemory(collectible.Memory)
                && GameServices.Current.Quest.GetQuestStatus(collectible.Quest) == QuestStatus.Active);
        }

        private Material Material(Color color)
        {
            var result = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            result.SetColor("_BaseColor", color); result.SetFloat("_Smoothness", .22f);
            materials.Add(result); return result;
        }

        private static Transform Part(Transform root, string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
        {
            var go = GameObject.CreatePrimitive(type); go.name = name; go.transform.SetParent(root, false);
            go.transform.localPosition = position; go.transform.localScale = scale;
            var collider = go.GetComponent<Collider>(); collider.enabled = false; Destroy(collider);
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go.transform;
        }

        private void OnDestroy() { foreach (var material in materials) if (material != null) Destroy(material); }
    }
}
