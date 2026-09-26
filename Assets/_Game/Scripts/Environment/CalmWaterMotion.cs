using UnityEngine;

namespace TilkiOyunu.Foundation
{
    /// <summary>Presentation only: scrolls the existing URP Lit normal-map UVs.</summary>
    [RequireComponent(typeof(Renderer))]
    public sealed class CalmWaterMotion : MonoBehaviour
    {
        [SerializeField] private Vector2 uvVelocity = new(0.008f, 0.004f);
        private static readonly int BaseMapST = Shader.PropertyToID("_BaseMap_ST");
        private Renderer surface;
        private MaterialPropertyBlock properties;
        private Vector4 originalST;

        private void OnEnable()
        {
            surface = GetComponent<Renderer>();
            properties ??= new MaterialPropertyBlock();
            Material material = surface.sharedMaterial;
            Vector2 scale = material != null ? material.mainTextureScale : Vector2.one;
            Vector2 offset = material != null ? material.mainTextureOffset : Vector2.zero;
            originalST = new Vector4(scale.x, scale.y, offset.x, offset.y);
        }

        private void Update()
        {
            surface.GetPropertyBlock(properties);
            properties.SetVector(BaseMapST, new Vector4(originalST.x, originalST.y,
                Mathf.Repeat(originalST.z + Time.time * uvVelocity.x, 1f),
                Mathf.Repeat(originalST.w + Time.time * uvVelocity.y, 1f)));
            surface.SetPropertyBlock(properties);
        }

        private void OnDisable()
        {
            if (surface == null || properties == null) return;
            surface.GetPropertyBlock(properties);
            properties.SetVector(BaseMapST, originalST);
            surface.SetPropertyBlock(properties);
        }
    }
}
