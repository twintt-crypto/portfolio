using S7;

namespace S7
{
    public class TalkNpcCondition : IQuestCondition
    {
        private int _npcId;

        public bool IsComplete { get; private set; }

        public int NpcId { get => _npcId;}

        public TalkNpcCondition(int npcId)
        {
            _npcId = npcId;
        }        

        public void OnEvent(GameEvent e)
        {
            if (IsComplete)
                return;

            if (e is TalkNpcEvent talkNpcEvent)
            {
                if (talkNpcEvent.type != GameEventType.TalkNpc)
                    return;

                if (talkNpcEvent.npcId != _npcId)
                    return;

                IsComplete = true;
            }                
        }
    }
}
