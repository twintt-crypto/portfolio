using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S7
{
    public class QuestStep
    {
        public int step;

        public List<IQuestCondition> conditions = new();

        public List<IQuestAction> actions = new();

        public bool IsComplete => conditions.All(c => c.IsComplete);        

        public void OnEvent(GameEvent e)
        {
            foreach (var cond in conditions)
            {
                cond.OnEvent(e);
            }
        }

        public async UniTask ExecuteActions()
        {
            foreach (var action in actions)
            {
                await action.Execute();
            }
        }
    }
}

