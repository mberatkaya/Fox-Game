using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class LightPathNode : MonoBehaviour
    {
        [SerializeField, Min(0)] private int sequenceIndex;
        [SerializeField] private LightPathController controller;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Light[] lights;
        [SerializeField] private Color inactiveColor = new(0.18f, 0.27f, 0.32f);
        [SerializeField] private Color availableColor = new(0.9f, 0.95f, 1f);
        [SerializeField] private Color completedColor = new(0.45f, 0.9f, 0.66f);
        [SerializeField] private float inactiveLightIntensity = 0.1f;
        [SerializeField] private float availableLightIntensity = 2.4f;
        [SerializeField] private float completedLightIntensity = 0.75f;
        [SerializeField] private Vector3 inactiveScale = Vector3.one * 0.85f;
        [SerializeField] private Vector3 availableScale = Vector3.one * 1.18f;
        [SerializeField] private Vector3 completedScale = Vector3.one;

        private LightPathVisualState visualState;

        public int SequenceIndex => sequenceIndex;
        public LightPathVisualState VisualState => visualState;

        private void Awake()
        {
            CacheSceneReferences();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != null && controller != null)
            {
                controller.HandleNodeTriggered(this);
            }
        }

        public void Configure(LightPathController owner)
        {
            controller = owner;
        }

        public bool Activate()
        {
            return controller != null && controller.HandleNodeTriggered(this);
        }

        public void SetVisualState(LightPathVisualState state)
        {
            visualState = state;
            Color color = GetColor(state);
            float intensity = GetLightIntensity(state);

            CacheSceneReferences();
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer != null)
                {
                    renderer.material.color = color;
                    renderer.material.SetColor("_EmissionColor", color * intensity);
                }
            }

            for (int i = 0; i < lights.Length; i++)
            {
                Light nodeLight = lights[i];
                if (nodeLight != null)
                {
                    nodeLight.color = color;
                    nodeLight.intensity = intensity;
                    nodeLight.enabled = state != LightPathVisualState.Inactive || intensity > 0f;
                }
            }

            Transform target = visualRoot != null ? visualRoot : transform;
            target.localScale = state switch
            {
                LightPathVisualState.Available => availableScale,
                LightPathVisualState.Completed => completedScale,
                _ => inactiveScale
            };
        }

        private void CacheSceneReferences()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }

            if (lights == null || lights.Length == 0)
            {
                lights = GetComponentsInChildren<Light>();
            }
        }

        private Color GetColor(LightPathVisualState state)
        {
            return state switch
            {
                LightPathVisualState.Available => availableColor,
                LightPathVisualState.Completed => completedColor,
                _ => inactiveColor
            };
        }

        private float GetLightIntensity(LightPathVisualState state)
        {
            return state switch
            {
                LightPathVisualState.Available => availableLightIntensity,
                LightPathVisualState.Completed => completedLightIntensity,
                _ => inactiveLightIntensity
            };
        }
    }
}
