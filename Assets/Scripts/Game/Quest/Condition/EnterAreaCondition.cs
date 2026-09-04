using S7;

namespace S7
{
    public class EnterAreaCondition : IQuestCondition
    {
        private int _areaId;

        public bool IsComplete { get; private set; }

        public int AreaId { get => _areaId; }

        public EnterAreaCondition(int areaId)
        {
            _areaId = areaId;
        }        

        public void OnEvent(GameEvent e)
        {
            if (IsComplete)
                return;

            if (e is EnterAreaEvent enterAreaEvent)
            {                
                if (enterAreaEvent.type != GameEventType.EnterArea)
                    return;

                if (enterAreaEvent.areaId != _areaId)
                    return;

                IsComplete = true;

            }                
        }        
    }
}
