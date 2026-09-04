using UnityEngine;

namespace S7.Game.Field
{
    public partial class Detector
    {
        private void FixedUpdateFirstEnter()
        {
            _candidates.RemoveAll(t => t == null);

            for (int i = _candidates.Count - 1; i >= 0; i--)
            {
                if (Vector3.Distance(transform.position, _candidates[i].position) > _lostRange)
                    _candidates.RemoveAt(i);
            }

            if (_candidates.Count == 0)
            {
                if (CurrentTarget != null) HandleLost();
                return;
            }

            SetTarget(_candidates[0]);
            UpdateVisibility();
        }
    }
}
