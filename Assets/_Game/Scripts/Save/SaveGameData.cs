using System;
using System.Collections.Generic;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [Serializable]
    public sealed class SaveGameData
    {
        public int saveVersion = SaveService.CurrentSaveVersion;
        public GameState gameState = GameState.NotStarted;
        public List<QuestProgress> quests = new List<QuestProgress>();
        public List<string> collectedMemoryIds = new List<string>();
        public bool lightPathCompleted;
        public bool cardMatchingCompleted;
        public bool finalUnlocked;
        public bool finalCompleted;
        public bool hasPlayerPosition;
        public SerializableVector3 playerPosition;
    }

    [Serializable]
    public sealed class QuestProgress
    {
        public string questId;
        public int currentAmount;
        public QuestStatus status = QuestStatus.NotStarted;
        public bool completed;
    }

    [Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 value)
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
