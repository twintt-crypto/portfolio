using S7;

namespace S7
{
    public class GetItemCondition : IQuestCondition
    {
        public int ItemId { get; }
        public int NeedCount { get; }

        private int _currentCount;

        public bool IsComplete => _currentCount >= NeedCount;

        public GetItemCondition(int itemId, int needCount)
        {
            ItemId = itemId;
            NeedCount = needCount;
        }

        public void OnEvent(GameEvent e)
        {
            if (IsComplete)
                return;

            if (e is GetItemEvent getItemEvent)
            {
                if (getItemEvent.type != GameEventType.GetItem)
                    return;

                if (getItemEvent.itemId != ItemId)
                    return;

                _currentCount += getItemEvent.count;
            }                            
        }
    }
}
