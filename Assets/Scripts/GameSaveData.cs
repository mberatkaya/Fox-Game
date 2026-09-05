using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TilkiMacera
{
    [Serializable]
    public class GameSaveData
    {
        public int currentQuestIndex;
        public int currentQuestProgress;
        public bool finalSeen;
        public List<string> collectedMemoryIds = new List<string>();
    }

    public static class SimpleSaveSystem
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "tilki-macera-save.json");

        public static GameSaveData Load()
        {
            if (!File.Exists(SavePath))
            {
                return new GameSaveData();
            }

            try
            {
                return JsonUtility.FromJson<GameSaveData>(File.ReadAllText(SavePath)) ?? new GameSaveData();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Save dosyasi okunamadi, yeni kayit aciliyor: {exception.Message}");
                return new GameSaveData();
            }
        }

        public static void Save(GameSaveData data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        }

        public static void Clear()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
        }
    }
}
