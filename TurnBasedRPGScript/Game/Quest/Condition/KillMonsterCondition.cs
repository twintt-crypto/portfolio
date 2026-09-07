using S7;

namespace S7
{
    public class KillMonsterCondition : IQuestCondition
    {
        private int _monsterId;
        private int _needCount;
        private int _current;

        public int MonsterId { get => _monsterId; }

        public bool IsComplete { get; private set; }


        public KillMonsterCondition(int monsterId, int needCount)
        {
            _monsterId = monsterId;
            _needCount = needCount;
        }
        
        public void OnEvent(GameEvent e)
        {
            if (IsComplete)
                return;

            if (e is KillMonsterEvent killMonsterEvent)
            {
                if (killMonsterEvent.type != GameEventType.KillMonster)
                    return;

                if (killMonsterEvent.monsterId!= _monsterId)
                    return;

                _current++;
            }                
        }
    }
}
