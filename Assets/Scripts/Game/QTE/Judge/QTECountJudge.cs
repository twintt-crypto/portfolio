namespace Game.QTE
{
    public class QTECountJudge : QTEJudge
    {
        private readonly QTEConfig _config;
        private int _count;

        public QTECountJudge(QTEConfig config) => _config = config;

        public override void Feed()
        {
            if(IsComplete) return;
            
            _count++;
            IsComplete = _count >= _config.mashThreshold;
            
            Debug.Log($"[CountJudge] {_count} {_config.mashThreshold}");
        }

        public override QTE_RESULT Judge()
        {
            if (_count >= _config.mashThreshold)    return QTE_RESULT.PERFECT;
            if (_count >= _config.mashGoodThreshold) return QTE_RESULT.GOOD;
            
            return QTE_RESULT.FAIL;
        }
    }
}
