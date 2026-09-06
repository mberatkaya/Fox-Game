using System.IO;
using UnityEditor;
using UnityEngine;

namespace TilkiOyunu.Foundation.Editor
{
    public static class TmpEssentialsImport
    {
        [MenuItem("Tilki Oyunu/Tools/Import TMP Essential Resources")]
        public static void ImportFromMenu()
        {
            ImportEssentials();
        }

        public static void ImportEssentials()
        {
            UnityEditor.PackageManager.PackageInfo packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(TMPro.TMP_Text).Assembly);
            string packageRoot = packageInfo?.resolvedPath;
            string packagePath = string.IsNullOrWhiteSpace(packageRoot)
                ? string.Empty
                : Path.Combine(packageRoot, "Package Resources", "TMP Essential Resources.unitypackage");

            if (string.IsNullOrWhiteSpace(packagePath) || !File.Exists(packagePath))
            {
                Debug.LogError("Could not find the official TMP Essential Resources package.");
                return;
            }

            AssetDatabase.ImportPackage(packagePath, false);
            AssetDatabase.Refresh();
            Debug.Log($"TMP Essential Resources import requested from {packagePath}.");
        }
    }
}
