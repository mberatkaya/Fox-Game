using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint2Validation
    {
        [MenuItem("Tilki Oyunu/Sprint 2/Validate")]
        public static void ValidateFromMenu()
        {
            if (ValidateProject(out List<string> errors))
            {
                Debug.Log("Sprint 2 validation passed.");
                return;
            }

            foreach (string error in errors)
            {
                Debug.LogError(error);
            }
        }

        public static bool ValidateProject(out List<string> errors)
        {
            errors = new List<string>();
            ValidateData(errors);
            ValidateForest(errors);
            return errors.Count == 0;
        }

        private static void ValidateData(List<string> errors)
        {
            GameContentConfig config = AssetDatabase.LoadAssetAtPath<GameContentConfig>("Assets/_Game/Data/Config/GameContentConfig.asset");
            if (config == null)
            {
                errors.Add("GameContentConfig asset is missing.");
                return;
            }

            QuestDefinition quest = null;
            foreach (QuestDefinition candidate in config.Quests)
            {
                if (candidate != null && candidate.Id == QuestService.CollectMemoriesQuestId)
                {
                    quest = candidate;
                    break;
                }
            }

            if (quest == null)
            {
                errors.Add("collect_memories quest is missing from GameContentConfig.");
            }
            else
            {
                if (quest.RequiredAmount != 5)
                {
                    errors.Add("collect_memories quest must require exactly 5 memories.");
                }

                if (quest.ObjectiveType != QuestObjectiveType.CollectMemory)
                {
                    errors.Add("collect_memories quest objective type must be CollectMemory.");
                }
            }

            if (config.Memories == null || config.Memories.Length != 5)
            {
                errors.Add("GameContentConfig must reference exactly 5 memory definitions.");
            }
            else
            {
                HashSet<string> memoryIds = new();
                foreach (MemoryDefinition memory in config.Memories)
                {
                    if (memory == null)
                    {
                        errors.Add("A memory definition reference is missing.");
                        continue;
                    }

                    if (!memoryIds.Add(memory.Id))
                    {
                        errors.Add($"Duplicate memory id found: {memory.Id}");
                    }
                }
            }

            if (config.Dialogues == null || config.Dialogues.Length < 4)
            {
                errors.Add("Guide dialogue definitions are missing.");
            }
        }

        private static void ValidateForest(List<string> errors)
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);

            GuideNpc guide = Object.FindFirstObjectByType<GuideNpc>();
            if (guide == null)
            {
                errors.Add("Forest scene is missing NPC_Guide with GuideNpc.");
            }

            MemoryCollectible[] collectibles = Object.FindObjectsByType<MemoryCollectible>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (collectibles.Length != 5)
            {
                errors.Add($"Forest scene must contain 5 MemoryCollectible instances, found {collectibles.Length}.");
            }

            HashSet<string> sceneMemoryIds = new();
            foreach (MemoryCollectible collectible in collectibles)
            {
                if (collectible.Memory == null)
                {
                    errors.Add($"{collectible.name} has no MemoryDefinition.");
                    continue;
                }

                if (!sceneMemoryIds.Add(collectible.Memory.Id))
                {
                    errors.Add($"Duplicate scene memory id found: {collectible.Memory.Id}");
                }
            }

            if (Object.FindFirstObjectByType<QuestHUD>() == null)
            {
                errors.Add("Forest scene is missing QuestHUD.");
            }

            if (Object.FindFirstObjectByType<DialoguePanelUI>() == null)
            {
                errors.Add("Forest scene is missing DialoguePanelUI.");
            }

            if (Object.FindFirstObjectByType<MemoryFeedbackUI>() == null)
            {
                errors.Add("Forest scene is missing MemoryFeedbackUI.");
            }
        }
    }
}
