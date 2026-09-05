using UnityEngine.SceneManagement;

namespace TilkiOyunu.Foundation
{
    public sealed class SceneService
    {
        public string ActiveSceneName => SceneManager.GetActiveScene().name;

        public void LoadForest()
        {
            Load(SceneIds.Forest);
        }

        public void Load(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                AppLog.Warning(LogCategory.Scene, "Ignored scene load with an empty scene name.");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
