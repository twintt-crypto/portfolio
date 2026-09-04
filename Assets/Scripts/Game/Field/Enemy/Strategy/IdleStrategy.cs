using UnityEngine;

namespace S7.Game.Field.Enemy
{
    public class IdleStrategy : IUnitActionStrategy
    {
        private FieldEnemyAI _ai;

        public IdleStrategy(FieldEnemyAI ai)
        {
            _ai = ai;
        }

        public void Enter()
        {
            _ai.Controller.MoveDirection = Vector3.zero;
            _ai.Controller.RotateDirection = Vector3.zero;
            _ai.Detector.OnDetected += OnDetected;
        }
        
        public void Exit()
        {
            _ai.Detector.OnDetected -= OnDetected;
        }

        public void Tick()
        {
            _ai.ReduceAlertGauge();
        }

        private void OnDetected(Transform transform)
        {
            _ai.ChangeState(ENEMY_AI_STATE.ALERT);
        }
    }
}
