using UnityEngine;

namespace S7
{
    public class QuestCondition
    {
        public QuestConditionType ConditionType;
        public int targetId;
        public int needCount;

        public int _current;
        public bool IsComplate => _current >= needCount;
        public void Check(S7.GameEvent e)
        {

        }
    }
}

