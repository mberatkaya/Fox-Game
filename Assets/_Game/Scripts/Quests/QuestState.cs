namespace TilkiOyunu.Foundation
{
    public readonly struct QuestState
    {
        public QuestState(string questId, string title, string description, QuestStatus status, int currentAmount, int requiredAmount)
        {
            QuestId = questId;
            Title = title;
            Description = description;
            Status = status;
            CurrentAmount = currentAmount;
            RequiredAmount = requiredAmount;
        }

        public string QuestId { get; }
        public string Title { get; }
        public string Description { get; }
        public QuestStatus Status { get; }
        public int CurrentAmount { get; }
        public int RequiredAmount { get; }
    }
}
