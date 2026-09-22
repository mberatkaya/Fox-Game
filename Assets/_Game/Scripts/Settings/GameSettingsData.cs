using System;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [Serializable]
    public sealed class GameSettingsData
    {
        public int version = 1;
        public bool minimap = true, objectiveMarkers = true, vsync = true, invertY;
        public int width = 1920, height = 1080, windowMode = 1, fps = 60, quality = 2;
        public float brightness = 1, master = 1, music = 1, ambience = 1, sfx = 1;
        public float sensitivity = 1, uiScale = 1, markerScale = 1, dialogueScale = 1;
        public string bindingOverrides = "";

        public static GameSettingsData Defaults(int width, int height) => new()
        {
            width = Mathf.Max(640, width), height = Mathf.Max(480, height)
        };

        public GameSettingsData Copy() => (GameSettingsData)MemberwiseClone();

        public void Normalize()
        {
            version = 1;
            width = Mathf.Clamp(width, 640, 16384); height = Mathf.Clamp(height, 480, 8640);
            windowMode = Mathf.Clamp(windowMode, 0, 2); quality = Mathf.Clamp(quality, 0, 3);
            if (fps != 30 && fps != 60 && fps != 120 && fps != 144 && fps != -1) fps = 60;
            master = Safe(master, 0, 1); music = Safe(music, 0, 1);
            ambience = Safe(ambience, 0, 1); sfx = Safe(sfx, 0, 1);
            brightness = Safe(brightness, .8f, 1.2f); sensitivity = Safe(sensitivity, .25f, 1.75f);
            uiScale = Safe(uiScale, .8f, 1.2f); markerScale = Safe(markerScale, .8f, 1.2f);
            dialogueScale = Safe(dialogueScale, .85f, 1.15f);
            bindingOverrides ??= "";
        }

        private static float Safe(float value, float min, float max) =>
            float.IsNaN(value) || float.IsInfinity(value) ? 1 : Mathf.Clamp(value, min, max);
    }
}
