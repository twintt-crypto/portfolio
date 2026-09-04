using UnityEngine;

namespace S7.Game.Field.Enemy
{
    public class AlertStrategy : IUnitActionStrategy
    {
        private FieldEnemyAI _ai;

        public AlertStrategy(FieldEnemyAI ai)
        {
            _ai = ai;
        }

        public void Enter()
        {
            _ai.Detector.OnLost += OnLost;
        }

        public void Tick()
        {
            if (!_ai.Detector.IsVisible)
            {
                _ai.ChangeState(ENEMY_AI_STATE.RETURN);
                return;
            }

            _ai.Controller.RotateDirection = (_ai.Detector.CurrentTarget.position - _ai.transform.position).normalized;
            
            _ai.AddAlertGauge();
            if (_ai.AlertRatio >= 1f) _ai.ChangeState(ENEMY_AI_STATE.COMBAT);
        }

        public void Exit()
        {
            _ai.Controller.RotateDirection = Vector3.zero;
            _ai.Detector.OnLost -= OnLost;
        }

        public void OnLost(Transform transform)
        {
            _ai.ChangeState(ENEMY_AI_STATE.RETURN);
        }
    }
}
