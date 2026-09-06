using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint3Validation
    {
        private const string LightPathQuestPath = "Assets/_Game/Data/Quests/Quest_LightPath.asset";

        [MenuItem("Tilki Oyunu/Sprint 3/Validate")]
        public static void ValidateFromMenu()
        {
            if (ValidateProject(out List<string> errors))
            {
                Debug.Log("Sprint 3 validation passed.");
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

            QuestDefinition lightPathQuest = AssetDatabase.LoadAssetAtPath<QuestDefinition>(LightPathQuestPath);
            if (lightPathQuest == null)
            {
                errors.Add("Quest_LightPath asset is missing.");
                return;
            }

            if (lightPathQuest.Id != QuestService.LightPathQuestId)
            {
                errors.Add("Light Path quest id must be light_path.");
            }

            if (lightPathQuest.ObjectiveType != QuestObjectiveType.CompleteLightPath)
            {
                errors.Add("Light Path quest objective type must be CompleteLightPath.");
            }

            if (lightPathQuest.RequiredAmount != 5)
            {
                errors.Add("Light Path quest must require exactly 5 nodes.");
            }

            if (lightPathQuest.PrerequisiteQuestIds == null || lightPathQuest.PrerequisiteQuestIds.Count != 1 || lightPathQuest.PrerequisiteQuestIds[0] != QuestService.CollectMemoriesQuestId)
            {
                errors.Add("Light Path quest must require collect_memories as prerequisite.");
            }

            if (!ContainsQuest(config, QuestService.CollectMemoriesQuestId))
            {
                errors.Add("collect_memories quest is missing from GameContentConfig.");
            }

            if (!ContainsQuest(config, QuestService.LightPathQuestId))
            {
                errors.Add("light_path quest is missing from GameContentConfig.");
            }
        }

        private static void ValidateForest(List<string> errors)
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);

            LightPathController controller = Object.FindFirstObjectByType<LightPathController>(FindObjectsInactive.Include);
            if (controller == null)
            {
                errors.Add("Forest scene is missing LightPathController.");
                return;
            }

            SerializedObject controllerObject = new(controller);
            SerializedProperty questProperty = controllerObject.FindProperty("quest");
            QuestDefinition quest = questProperty.objectReferenceValue as QuestDefinition;
            if (quest == null || quest.Id != QuestService.LightPathQuestId)
            {
                errors.Add("LightPathController must reference the light_path quest.");
            }

            SerializedProperty nodesProperty = controllerObject.FindProperty("nodes");
            if (nodesProperty == null || nodesProperty.arraySize != 5)
            {
                errors.Add("LightPathController must reference exactly 5 nodes.");
            }

            HashSet<int> nodeIndices = new();
            LightPathNode[] sceneNodes = Object.FindObjectsByType<LightPathNode>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (sceneNodes.Length != 5)
            {
                errors.Add($"Forest scene must contain 5 LightPathNode instances, found {sceneNodes.Length}.");
            }

            foreach (LightPathNode node in sceneNodes)
            {
                if (!nodeIndices.Add(node.SequenceIndex))
                {
                    errors.Add($"Duplicate LightPathNode index found: {node.SequenceIndex}.");
                }
            }

            for (int i = 0; i < 5; i++)
            {
                if (!nodeIndices.Contains(i))
                {
                    errors.Add($"LightPathNode index {i} is missing.");
                }
            }

            if (Object.FindFirstObjectByType<LightPathStart>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest scene is missing LightPathStart.");
            }

            if (Object.FindFirstObjectByType<LightPathHUD>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest scene is missing LightPathHUD.");
            }

            if (Object.FindFirstObjectByType<QuestHUD>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest scene is missing QuestHUD.");
            }

            GuideNpc guide = Object.FindFirstObjectByType<GuideNpc>(FindObjectsInactive.Include);
            if (guide == null)
            {
                errors.Add("Forest scene is missing GuideNpc.");
            }
            else
            {
                SerializedObject guideObject = new(guide);
                QuestDefinition lightPathQuest = guideObject.FindProperty("lightPathQuest").objectReferenceValue as QuestDefinition;
                if (lightPathQuest == null || lightPathQuest.Id != QuestService.LightPathQuestId)
                {
                    errors.Add("GuideNpc must reference the light_path quest.");
                }
            }
        }

        private static bool ContainsQuest(GameContentConfig config, string questId)
        {
            if (config.Quests == null)
            {
                return false;
            }

            foreach (QuestDefinition quest in config.Quests)
            {
                if (quest != null && quest.Id == questId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
