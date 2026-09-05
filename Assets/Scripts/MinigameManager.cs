using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TilkiMacera
{
    public class MinigameManager : MonoBehaviour
    {
        public static MinigameManager Instance { get; private set; }

        public PlayerController player;
        public float lightPathTimeLimit = 32f;

        private List<LightPathNode> lightNodes = new List<LightPathNode>();
        private int currentLightIndex;
        private float lightTimer;
        private bool lightPathRunning;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>();
            }

            lightNodes = FindObjectsOfType<LightPathNode>(true)
                .OrderBy(node => node.sequenceIndex)
                .ToList();

            SetLightNodesVisible(false);
        }

        private void Update()
        {
            if (!lightPathRunning)
            {
                return;
            }

            lightTimer -= Time.deltaTime;
            UIController.Instance.SetMinigameStatus($"Isik Yolu: {currentLightIndex + 1}/{lightNodes.Count}  Sure: {Mathf.CeilToInt(lightTimer)}");

            if (lightTimer <= 0f)
            {
                StopLightPath(false, "Isiklar sondu. Bir kez daha deneyelim.");
            }
        }

        public void StartLightPath()
        {
            QuestData quest = QuestManager.Instance.ActiveQuest;
            if (quest == null || quest.kind != QuestKind.LightPath)
            {
                QuestManager.Instance.ShowActiveQuestIntro();
                return;
            }

            if (lightNodes.Count == 0)
            {
                UIController.Instance.ShowResult("Isik yolu dugumleri bulunamadi.", 3f);
                return;
            }

            currentLightIndex = 0;
            lightTimer = lightPathTimeLimit;
            lightPathRunning = true;
            SetLightNodesVisible(true);
            RefreshLightNodes();
            UIController.Instance.ShowResult("Isiklari sirayla takip et.", 2f);
        }

        public void TouchLightNode(LightPathNode node)
        {
            if (!lightPathRunning || node.sequenceIndex != currentLightIndex)
            {
                return;
            }

            currentLightIndex++;
            if (currentLightIndex >= lightNodes.Count)
            {
                StopLightPath(true, "Isik yolu tamamlandi.");
                return;
            }

            RefreshLightNodes();
        }

        public void StartHeartGarden()
        {
            QuestData quest = QuestManager.Instance.ActiveQuest;
            if (quest == null || quest.kind != QuestKind.HeartGarden)
            {
                QuestManager.Instance.ShowActiveQuestIntro();
                return;
            }

            if (player != null)
            {
                player.SetCanMove(false);
            }

            UIController.Instance.ShowHeartGarden(new List<string>
            {
                "Gunes",
                "Ay",
                "Cay",
                "Sarki"
            }, success =>
            {
                if (player != null)
                {
                    player.SetCanMove(true);
                }

                if (success)
                {
                    QuestManager.Instance.RegisterProgress(QuestKind.HeartGarden, 1);
                }
                else
                {
                    UIController.Instance.ShowResult("Bahce kapanmadan once yeniden deneyebilirsin.", 3f);
                }
            });
        }

        private void StopLightPath(bool success, string resultMessage)
        {
            lightPathRunning = false;
            SetLightNodesVisible(false);
            UIController.Instance.SetMinigameStatus(string.Empty);
            UIController.Instance.ShowResult(resultMessage, 3f);

            if (success)
            {
                QuestManager.Instance.RegisterProgress(QuestKind.LightPath, 1);
            }
        }

        private void RefreshLightNodes()
        {
            for (int i = 0; i < lightNodes.Count; i++)
            {
                lightNodes[i].SetState(i == currentLightIndex, i < currentLightIndex);
            }
        }

        private void SetLightNodesVisible(bool visible)
        {
            foreach (LightPathNode node in lightNodes)
            {
                node.gameObject.SetActive(visible);
            }
        }
    }
}
