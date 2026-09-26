using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public interface ISettingsDisplay
    {
        GameSettingsData Capture();
        void Apply(GameSettingsData data);
    }

    public sealed class SettingsDisplay : ISettingsDisplay
    {
        public GameSettingsData Capture() => new()
        {
            width = Screen.width, height = Screen.height,
            windowMode = Screen.fullScreenMode == FullScreenMode.Windowed ? 2 :
                Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen ? 0 : 1
        };

        public void Apply(GameSettingsData data) => SettingsRuntime.ApplyDisplay(data);
    }
}
