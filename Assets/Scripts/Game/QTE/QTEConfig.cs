using System;
using UnityEngine;

namespace Game.QTE
{
    [Serializable]
    public struct QTEConfig
    {
        public QTE_TYPE type;

        public float delay;
        public float duration;
        public float timingPoint;

        public float perfectNegative;
        public float perfectPositive;

        public float goodNegative;
        public float goodPositive;

        public Vector2 position;

        // SWIPE 전용
        public QTE_SWIPE_DIR requiredDir;

        // MASH 전용
        public int mashThreshold;
        public int mashGoodThreshold;
    }
}
