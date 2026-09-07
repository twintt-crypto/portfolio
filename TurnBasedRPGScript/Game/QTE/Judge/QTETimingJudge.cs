using UnityEngine;

namespace Game.QTE
{
    public class QTETimingJudge : QTEJudge
    {
        private readonly QTEConfig _config;
        private readonly float _startTime;
        private float _feedTime = int.MinValue;

        public QTETimingJudge(QTEConfig config)
        {
            _config    = config;
            _startTime = Time.time;
        }

        public override void Feed()
        {
            if (IsComplete) return;
            
            IsComplete  = true;
            _feedTime  = Time.time;

            Debug.Log($"[TimingJudge] {_feedTime}");
        }

        public override QTE_RESULT Judge()
        {
            if (!IsComplete) return QTE_RESULT.FAIL;
            
            float diff = (_feedTime - _startTime) - _config.timingPoint;

            Debug.Log($"[TimingJudge] {diff} {_feedTime} {_startTime}");
            
            if (diff >= -_config.perfectNegative && diff <= _config.perfectPositive) return QTE_RESULT.PERFECT;
            if (diff >= -_config.goodNegative    && diff <= _config.goodPositive)    return QTE_RESULT.GOOD;
            return QTE_RESULT.MISS;
        }
    }
}
