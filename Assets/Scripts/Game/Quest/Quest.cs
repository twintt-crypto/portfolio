using NUnit.Framework;
using System.Collections.Generic;

namespace S7
{
    public class Quest
    {
        public int questId;
        public QuestType type;

        public int currentStep;

        public List<QuestStep> steps = new();

        public bool IsComplete => currentStep >= steps.Count;

        public QuestStep CurrentStep => steps[currentStep];

        public void OnEvent(GameEvent e)
        {
            if (IsComplete)
                return;

            CurrentStep.OnEvent(e);

            if (CurrentStep.IsComplete)
            {
                CompleteStep();
            }
        }

        public void Start()
        {
            currentStep = 0;
        }

        private async void CompleteStep()
        {
            await CurrentStep.ExecuteActions();

            currentStep++;

            if (IsComplete)
            {
                CompleteQuest();
            }
        }

        private void CompleteQuest()
        {
            // 퀘스트 완료 처리
        }
    }

}
