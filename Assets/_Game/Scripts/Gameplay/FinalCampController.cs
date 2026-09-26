using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class FinalCampController : MonoBehaviour, IInteractable
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Light[] lights;
        [SerializeField] private Color lockedColor = new(0.22f, 0.22f, 0.24f);
        [SerializeField] private Color unlockedColor = new(0.95f, 0.72f, 0.36f);
        [SerializeField] private float lockedLightIntensity = 0.15f;
        [SerializeField] private float unlockedLightIntensity = 1.6f;
        [SerializeField] private string lockedInteractionLabel = "Henüz zamanı değil";
        [SerializeField] private string unlockedInteractionLabel = "Son anıyı aç";
        [SerializeField] private FinalSequenceController finalSequence;

        private bool unlocked;
        private MaterialPropertyBlock propertyBlock;

        public bool IsUnlocked => unlocked;
        public string InteractionLabel => unlocked ? unlockedInteractionLabel : lockedInteractionLabel;

        private void Awake()
        {
            CacheReferences();
            Refresh();
        }

        private void OnEnable()
        {
            if (GameServices.HasCurrent)
            {
                GameServices.Current.Quest.FinalCampUnlockChanged += HandleFinalCampUnlockChanged;
                GameServices.Current.Quest.QuestCompleted += HandleQuestCompleted;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (GameServices.HasCurrent)
            {
                GameServices.Current.Quest.FinalCampUnlockChanged -= HandleFinalCampUnlockChanged;
                GameServices.Current.Quest.QuestCompleted -= HandleQuestCompleted;
            }
        }

        public bool CanInteract(InteractionContext context)
        {
            return true;
        }

        public void Interact(InteractionContext context)
        {
            if (!unlocked)
            {
                AppLog.Info(LogCategory.Quest, "Final camp is still locked.");
                return;
            }

            if (finalSequence != null)
            {
                finalSequence.Begin();
            }
        }

        private void HandleFinalCampUnlockChanged(bool isUnlocked)
        {
            unlocked = isUnlocked;
            ApplyVisualState();
        }

        private void HandleQuestCompleted(QuestState state)
        {
            Refresh();
        }

        private void Refresh()
        {
            unlocked = GameServices.HasCurrent && GameServices.Current.Quest.IsFinalCampUnlocked;
            ApplyVisualState();
        }

        private void ApplyVisualState()
        {
            CacheReferences();
            Color color = unlocked ? unlockedColor : lockedColor;
            float intensity = unlocked ? unlockedLightIntensity : lockedLightIntensity;

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer campRenderer = renderers[i];
                if (campRenderer != null)
                {
                    propertyBlock ??= new MaterialPropertyBlock();
                    campRenderer.GetPropertyBlock(propertyBlock);
                    propertyBlock.SetColor("_BaseColor", color);
                    propertyBlock.SetColor("_Color", color);
                    propertyBlock.SetColor("_EmissionColor", color * intensity);
                    campRenderer.SetPropertyBlock(propertyBlock);
                }
            }

            for (int i = 0; i < lights.Length; i++)
            {
                Light campLight = lights[i];
                if (campLight != null)
                {
                    campLight.color = color;
                    campLight.intensity = intensity;
                    campLight.enabled = true;
                }
            }
        }

        private void CacheReferences()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }

            if (lights == null || lights.Length == 0)
            {
                lights = GetComponentsInChildren<Light>();
            }

            if (finalSequence == null)
            {
                finalSequence = GetComponent<FinalSequenceController>();
            }
        }
    }
}
