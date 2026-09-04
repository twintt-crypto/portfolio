using UnityEngine;

namespace S7
{
    public interface IQuestCondition
    {
        bool IsComplete { get; }

        void OnEvent(GameEvent e);
    }
}