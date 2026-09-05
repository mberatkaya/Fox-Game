using System;
using System.Collections.Generic;
using UnityEngine;

namespace TilkiMacera
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Show(IEnumerable<string> lines, Action onComplete = null)
        {
            UIController.Instance.ShowDialogue(lines, onComplete);
        }
    }
}
