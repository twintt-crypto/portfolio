using System;

namespace Game.QTE
{
    public static class QTEJudgeFactory
    {
        public static QTEJudge Create(QTEConfig config) => config.type switch
        {
            QTE_TYPE.TAP     => new QTETimingJudge(config),
            QTE_TYPE.SWIPE   => new QTETimingJudge(config),
            QTE_TYPE.RELEASE => new QTETimingJudge(config),
            QTE_TYPE.MASH    => new QTECountJudge(config),
            _                => throw new ArgumentOutOfRangeException(nameof(config.type), config.type, null)
        };
    }
}
