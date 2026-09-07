using S7;

namespace S7
{
    public class ClearStageCondition : IQuestCondition
    {
        public int StageId { get; }

        private bool _complete;

        public bool IsComplete => _complete;

        public ClearStageCondition(int stageId)
        {
            StageId = stageId;
        }

        public void OnEvent(GameEvent e)
        {
            if (_complete)
                return;

            if (e is ClearStageEvent clearStageEvent)
            {
                if (clearStageEvent.type != GameEventType.ClearStage)
                    return;

                if (clearStageEvent.StageId != StageId)
                    return;

                _complete = true;
            }                
        }
    }
}
