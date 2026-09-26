using System;
using System.IO;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    /// <summary>User preferences, deliberately independent of quest/save-slot lifetime.</summary>
    public sealed class SettingsService
    {
        public const string FileName = "tilki-user-settings.json";
        private readonly string path;
        private readonly GameSettingsData defaults;
        private GameSettingsData current;
        public GameSettingsData Current => current.Copy();
        public GameSettingsData Defaults => defaults.Copy();
        public event Action Changed;

        public SettingsService(string path, GameSettingsData defaults)
        {
            this.path = path;
            this.defaults = defaults.Copy();
            current = Load();
        }

        private GameSettingsData Load()
        {
            try
            {
                if (!File.Exists(path)) return Defaults;
                string json = File.ReadAllText(path);
                if (!json.TrimStart().StartsWith("{") || !json.Contains("\"version\"")) return Defaults;
                var data = Defaults;
                JsonUtility.FromJsonOverwrite(json, data);
                if (data.version != 1) return Defaults;
                data.Normalize();
                return data;
            }
            catch (Exception e) when (e is IOException || e is ArgumentException || e is UnauthorizedAccessException)
            {
                return Defaults;
            }
        }

        public bool Commit(GameSettingsData draft, out string error)
        {
            var next = draft.Copy(); next.Normalize();
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
                string temporary = path + ".tmp";
                File.WriteAllText(temporary, JsonUtility.ToJson(next, true));
                if (File.Exists(path)) File.Replace(temporary, path, null);
                else File.Move(temporary, path);
                current = next;
                error = "";
                Changed?.Invoke();
                return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                error = "Ayarlar kaydedilemedi. " + e.Message;
                return false;
            }
        }

        public static FullScreenMode DisplayMode(int value) => value switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            2 => FullScreenMode.Windowed,
            _ => FullScreenMode.FullScreenWindow
        };

        public static float Decibels(float linear) =>
            float.IsNaN(linear) || linear <= .0001f ? -80 : 20 * Mathf.Log10(Mathf.Clamp01(linear));
    }
}
