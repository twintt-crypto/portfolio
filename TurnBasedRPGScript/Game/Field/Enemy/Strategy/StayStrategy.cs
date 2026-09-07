using UnityEngine;

namespace S7.Game.Field.Enemy
{
    public class StayStrategy : IUnitActionStrategy
    {
        private FieldEnemyAI _ai;
        private float _leashRange;
        private Transform _target;

        public StayStrategy(FieldEnemyAI ai, float leashRange)
        {
            _ai = ai;
            _leashRange = leashRange;
        }

        public void Enter()
        {
            if (_ai.Detector == null || _ai.Detector.CurrentTarget == null)
            {
                Debug.LogWarning("[StayStrategy] detector error");
                _ai.ChangeState(ENEMY_AI_STATE.IDLE);
                return;
            }
            
            _target = _ai.Detector.CurrentTarget;
            
            _ai.Detector.OnLost += OnLost;     
        }
        
        public void Exit()
        {
            _ai.Controller.MoveDirection = Vector3.zero;

            _ai.Detector.OnLost -= OnLost;
        }

        public void Tick()
        {
            Vector3 dir = (_target.position - _ai.transform.position).normalized;

            float targetWithStartPos = (_ai.StartPos - _target.position).magnitude;
            if (targetWithStartPos > _leashRange)
            {
                // _ai.ChangeState(ENEMY_AI_STATE.ALERT);
                _ai.Controller.MoveDirection = Vector3.zero;
                _ai.Controller.RotateDirection = dir;
                return;
            }

            // target 쫒아가는 중
            _ai.Controller.MoveDirection = dir;
        }

        public void OnLost(Transform transform)
        {
            _ai.ChangeState(ENEMY_AI_STATE.IDLE);
        }
    }
}
