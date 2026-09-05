using UnityEngine;
using Conditional = System.Diagnostics.ConditionalAttribute;

namespace TilkiOyunu.Foundation
{
    public enum LogCategory
    {
        Boot,
        Save,
        Quest,
        Audio,
        Scene,
        Input
    }

    public static class AppLog
    {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Info(LogCategory category, string message)
        {
            Debug.Log(Format(category, message));
        }

        public static void Warning(LogCategory category, string message)
        {
            Debug.LogWarning(Format(category, message));
        }

        public static void Error(LogCategory category, string message)
        {
            Debug.LogError(Format(category, message));
        }

        private static string Format(LogCategory category, string message)
        {
            return $"[{category.ToString().ToUpperInvariant()}] {message}";
        }
    }
}
