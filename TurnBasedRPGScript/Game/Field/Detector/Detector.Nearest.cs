using UnityEngine;

namespace S7.Game.Field
{
    public partial class Detector
    {
        private void FixedUpdateNearest()
        {
            _candidates.RemoveAll(t => t == null);

            Transform nearest = null;
            float minDist = float.MaxValue;

            for (int i = _candidates.Count - 1; i >= 0; i--)
            {
                float dist = Vector3.Distance(transform.position, _candidates[i].position);
                if (dist > _lostRange)
                {
                    _candidates.RemoveAt(i);
                    continue;
                }

                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = _candidates[i];
                }
            }

            if (nearest == null)
            {
                if (CurrentTarget != null) HandleLost();
                return;
            }

            SetTarget(nearest);
            UpdateVisibility();
        }
    }
}
