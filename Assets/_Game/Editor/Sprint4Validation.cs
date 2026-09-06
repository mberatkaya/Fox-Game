using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint4Validation
    {
        private const string CardQuestPath = "Assets/_Game/Data/Quests/Quest_CardMatching.asset";

        [MenuItem("Tilki Oyunu/Sprint 4/Validate")]
        public static void ValidateFromMenu()
        {
            if (ValidateProject(out List<string> errors))
            {
                Debug.Log("Sprint 4 validation passed.");
                return;
            }

            foreach (string error in errors)
            {
                Debug.LogError(error);
            }
        }

        public static void ValidateFromCommandLine()
        {
            if (ValidateProject(out List<string> errors))
            {
                Debug.Log("Sprint 4 validation passed.");
                EditorApplication.Exit(0);
                return;
            }

            foreach (string error in errors)
            {
                Debug.LogError(error);
            }

            EditorApplication.Exit(1);
        }

        public static bool ValidateProject(out List<string> errors)
        {
            errors = new List<string>();
            ValidateData(errors);
            ValidateTmp(errors);
            ValidateFinalRequirement(errors);
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

            QuestDefinition cardQuest = AssetDatabase.LoadAssetAtPath<QuestDefinition>(CardQuestPath);
            if (cardQuest == null)
            {
                errors.Add("Quest_CardMatching asset is missing.");
                return;
            }

            if (cardQuest.Id != QuestService.CardMatchingQuestId)
            {
                errors.Add("Card Matching quest id must be card_matching.");
            }

            if (cardQuest.ObjectiveType != QuestObjectiveType.CompleteCardMatch)
            {
                errors.Add("Card Matching quest objective type must be CompleteCardMatch.");
            }

            if (cardQuest.RequiredAmount != 4)
            {
                errors.Add("Card Matching quest must require exactly 4 pairs.");
            }

            if (cardQuest.PrerequisiteQuestIds == null || cardQuest.PrerequisiteQuestIds.Count != 1 || cardQuest.PrerequisiteQuestIds[0] != QuestService.LightPathQuestId)
            {
                errors.Add("Card Matching quest must require light_path as prerequisite.");
            }

            if (!ContainsQuest(config, QuestService.CardMatchingQuestId))
            {
                errors.Add("card_matching quest is missing from GameContentConfig.");
            }
        }

        private static void ValidateTmp(List<string> errors)
        {
            if (AssetDatabase.LoadAssetAtPath<TMP_Settings>("Assets/TextMesh Pro/Resources/TMP Settings.asset") == null)
            {
                errors.Add("TMP Settings asset is missing from TMP Essential Resources.");
            }

            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset") == null)
            {
                errors.Add("TMP default LiberationSans SDF font asset is missing.");
            }
        }

        private static void ValidateFinalRequirement(List<string> errors)
        {
            HashSet<string> required = new(QuestService.FinalCampRequiredQuestIds);
            if (!required.Contains(QuestService.CollectMemoriesQuestId) || !required.Contains(QuestService.LightPathQuestId) || !required.Contains(QuestService.CardMatchingQuestId))
            {
                errors.Add("Final camp requirements must include collect_memories, light_path, and card_matching.");
            }
        }

        private static void ValidateForest(List<string> errors)
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);

            if (GameObject.Find("Heart Garden") == null)
            {
                errors.Add("Forest scene is missing Heart Garden.");
            }

            CardMatchingController controller = Object.FindFirstObjectByType<CardMatchingController>(FindObjectsInactive.Include);
            if (controller == null)
            {
                errors.Add("Forest scene is missing CardMatchingController.");
            }
            else
            {
                ValidateController(controller, errors);
            }

            if (Object.FindFirstObjectByType<CardMatchingStart>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest scene is missing CardMatchingStart.");
            }

            CardMatchingPanelUI panel = Object.FindFirstObjectByType<CardMatchingPanelUI>(FindObjectsInactive.Include);
            if (panel == null)
            {
                errors.Add("Forest scene is missing CardMatchingPanelUI.");
            }

            CardMatchingCardButton[] cardButtons = Object.FindObjectsByType<CardMatchingCardButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (cardButtons.Length != 8)
            {
                errors.Add($"Forest scene must contain 8 CardMatchingCardButton instances, found {cardButtons.Length}.");
            }

            if (Object.FindFirstObjectByType<GameplayInputLock>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest scene is missing GameplayInputLock.");
            }

            if (Object.FindFirstObjectByType<FinalCampController>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest scene is missing FinalCampController.");
            }

            GuideNpc guide = Object.FindFirstObjectByType<GuideNpc>(FindObjectsInactive.Include);
            if (guide == null)
            {
                errors.Add("Forest scene is missing GuideNpc.");
            }
            else
            {
                SerializedObject guideObject = new(guide);
                QuestDefinition cardQuest = guideObject.FindProperty("cardMatchingQuest").objectReferenceValue as QuestDefinition;
                if (cardQuest == null || cardQuest.Id != QuestService.CardMatchingQuestId)
                {
                    errors.Add("GuideNpc must reference the card_matching quest.");
                }
            }

            foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(root);
                if (missingCount > 0)
                {
                    errors.Add($"Scene root '{root.name}' has {missingCount} missing script reference(s).");
                }
            }
        }

        private static void ValidateController(CardMatchingController controller, List<string> errors)
        {
            SerializedObject controllerObject = new(controller);
            QuestDefinition quest = controllerObject.FindProperty("quest").objectReferenceValue as QuestDefinition;
            if (quest == null || quest.Id != QuestService.CardMatchingQuestId)
            {
                errors.Add("CardMatchingController must reference the card_matching quest.");
            }

            if (controller.CardCount != 8)
            {
                errors.Add($"CardMatchingController must create 8 cards, found {controller.CardCount}.");
            }

            SerializedProperty pairs = controllerObject.FindProperty("pairs");
            if (pairs == null || pairs.arraySize != 4)
            {
                errors.Add("CardMatchingController must define exactly 4 pairs.");
                return;
            }

            HashSet<string> pairIds = new();
            for (int i = 0; i < pairs.arraySize; i++)
            {
                SerializedProperty pair = pairs.GetArrayElementAtIndex(i);
                string pairId = pair.FindPropertyRelative("pairId").stringValue;
                if (string.IsNullOrWhiteSpace(pairId))
                {
                    errors.Add($"Card pair {i} is missing a pair ID.");
                }
                else if (!pairIds.Add(pairId))
                {
                    errors.Add($"Duplicate card pair ID found: {pairId}.");
                }

                if (string.IsNullOrWhiteSpace(pair.FindPropertyRelative("symbol").stringValue))
                {
                    errors.Add($"Card pair {i} is missing a symbol.");
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
