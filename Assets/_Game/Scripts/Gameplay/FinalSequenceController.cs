using System.Collections;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class FinalSequenceController : MonoBehaviour
    {
        [SerializeField] private FinalMessageDefinition finalMessage;
        [SerializeField] private FinalMessagePanelUI finalPanel;
        [SerializeField] private GameplayInputLock inputLock;
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private Transform cameraFocus;
        [SerializeField] private Light campLight;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip completionClip;
        [SerializeField, Min(0.1f)] private float cameraTransitionSeconds = 1.4f;
        [SerializeField, Min(0.1f)] private float lightTransitionSeconds = 1.4f;
        [SerializeField, Min(0f)] private float finalLightIntensity = 2.4f;

        private Coroutine sequence;
        private bool inputLocked;

        public bool IsRunning => sequence != null;
        public FinalMessageDefinition FinalMessage => finalMessage;

        public bool CanBegin
        {
            get
            {
                return GameServices.HasCurrent
                    && GameServices.Current.Quest.IsFinalCampUnlocked
                    && finalMessage != null
                    && finalPanel != null;
            }
        }

        private void OnDisable()
        {
            ReleaseInput();
            sequence = null;
        }

        public bool Begin()
        {
            if (IsRunning || !CanBegin)
            {
                return false;
            }

            sequence = StartCoroutine(RunSequence());
            return true;
        }

        public void CompleteAndClose()
        {
            PersistCompletion();
            finalPanel?.Hide();
            ReleaseInput();
            sequence = null;
        }

        private IEnumerator RunSequence()
        {
            AcquireInput();

            Vector3 cameraStartPosition = gameplayCamera != null ? gameplayCamera.transform.position : Vector3.zero;
            Quaternion cameraStartRotation = gameplayCamera != null ? gameplayCamera.transform.rotation : Quaternion.identity;
            Vector3 targetPosition = cameraStartPosition;
            Quaternion targetRotation = cameraStartRotation;

            if (gameplayCamera != null && cameraFocus != null)
            {
                Vector3 focus = cameraFocus.position + Vector3.up * 0.65f;
                targetPosition = focus + new Vector3(0f, 1.15f, -3.2f);
                targetRotation = Quaternion.LookRotation(focus - targetPosition, Vector3.up);
            }

            float initialIntensity = campLight != null ? campLight.intensity : 0f;
            float elapsed = 0f;
            while (elapsed < cameraTransitionSeconds || elapsed < lightTransitionSeconds)
            {
                elapsed += Time.deltaTime;
                if (gameplayCamera != null)
                {
                    float cameraT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / cameraTransitionSeconds));
                    gameplayCamera.transform.SetPositionAndRotation(
                        Vector3.Lerp(cameraStartPosition, targetPosition, cameraT),
                        Quaternion.Slerp(cameraStartRotation, targetRotation, cameraT));
                }

                if (campLight != null)
                {
                    float lightT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / lightTransitionSeconds));
                    campLight.intensity = Mathf.Lerp(initialIntensity, finalLightIntensity, lightT);
                }

                yield return null;
            }

            if (audioSource != null && completionClip != null)
            {
                audioSource.PlayOneShot(completionClip, 0.55f);
            }

            finalPanel.Show(finalMessage, CompleteAndClose);
        }

        private void PersistCompletion()
        {
            if (!GameServices.HasCurrent)
            {
                return;
            }

            SaveGameData saveData = GameServices.Current.Quest.SaveData;
            saveData.finalCompleted = true;
            saveData.gameState = GameState.Completed;
            GameServices.Current.Save.Save(saveData);
        }

        private void AcquireInput()
        {
            if (inputLock != null && !inputLocked)
            {
                inputLock.Acquire();
                inputLocked = true;
            }
        }

        private void ReleaseInput()
        {
            if (inputLock != null && inputLocked)
            {
                inputLock.Release();
                inputLocked = false;
            }
        }
    }
}
