using System.Collections.Generic;
using UnityEngine;

namespace S7
{
    public class QuestState
    {
        public int QuestId;
        public QuestStatus Status;
        public Dictionary<int, int> ConditionProgress;

        // ConditionId -> 현재 진행도    
    }
}

