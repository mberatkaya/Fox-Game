using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class SaveService
    {
        public const int CurrentSaveVersion = 1;
        public const string DefaultFileName = "tilki-oyunu-save.json";

        private readonly string savePath;

        public SaveService()
            : this(Path.Combine(Application.persistentDataPath, DefaultFileName))
        {
        }

        public SaveService(string savePath)
        {
            this.savePath = savePath;
        }

        public string SavePath => savePath;

        public bool HasSave()
        {
            return File.Exists(savePath);
        }

        public SaveGameData Load()
        {
            if (!HasSave())
            {
                return CreateNewSave();
            }

            try
            {
                string json = File.ReadAllText(savePath);
                SaveGameData data = JsonUtility.FromJson<SaveGameData>(json);
                Normalize(data);
                return data ?? CreateNewSave();
            }
            catch (Exception exception)
            {
                AppLog.Warning(LogCategory.Save, $"Could not load save file. Starting a new save. {exception.Message}");
                return CreateNewSave();
            }
        }

        public bool Save(SaveGameData data)
        {
            if (data == null)
            {
                AppLog.Warning(LogCategory.Save, "Ignored save request with null data.");
                return false;
            }

            try
            {
                Normalize(data);
                data.saveVersion = CurrentSaveVersion;
                string directory = Path.GetDirectoryName(savePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(savePath, JsonUtility.ToJson(data, true));
                return true;
            }
            catch (Exception exception)
            {
                AppLog.Error(LogCategory.Save, $"Could not write save file. {exception.Message}");
                return false;
            }
        }

        public bool DeleteSave()
        {
            try
            {
                if (HasSave())
                {
                    File.Delete(savePath);
                }

                return true;
            }
            catch (Exception exception)
            {
                AppLog.Warning(LogCategory.Save, $"Could not delete save file. {exception.Message}");
                return false;
            }
        }

        public SaveGameData CreateNewSave()
        {
            SaveGameData data = new SaveGameData
            {
                saveVersion = CurrentSaveVersion
            };
            Normalize(data);
            return data;
        }

        public static void Normalize(SaveGameData data)
        {
            if (data == null)
            {
                return;
            }

            data.quests ??= new System.Collections.Generic.List<QuestProgress>();
            data.collectedMemoryIds ??= new System.Collections.Generic.List<string>();

            for (int i = data.quests.Count - 1; i >= 0; i--)
            {
                QuestProgress progress = data.quests[i];
                if (progress == null || string.IsNullOrWhiteSpace(progress.questId))
                {
                    data.quests.RemoveAt(i);
                    continue;
                }

                if (progress.completed)
                {
                    progress.status = QuestStatus.Completed;
                }

                if (progress.status == QuestStatus.Completed)
                {
                    progress.completed = true;
                }

                progress.currentAmount = Math.Max(0, progress.currentAmount);
            }

            HashSet<string> seenMemoryIds = new();
            for (int i = data.collectedMemoryIds.Count - 1; i >= 0; i--)
            {
                string memoryId = data.collectedMemoryIds[i];
                if (string.IsNullOrWhiteSpace(memoryId) || !seenMemoryIds.Add(memoryId))
                {
                    data.collectedMemoryIds.RemoveAt(i);
                }
            }
        }
    }
}
