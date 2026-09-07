using UnityEngine;

namespace S7.Game.Field.Enemy
{
    public class KiteStrategy : IUnitActionStrategy
    {
        private FieldEnemyAI _ai;
        private float _optimalRange;
        private Transform _target;

        public KiteStrategy(FieldEnemyAI ai, float optimalRange)
        {
            _ai = ai;
            _optimalRange = optimalRange;
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
            FieldEnemy fieldEnemy = _ai.GetComponentInParent<FieldEnemy>();
            fieldEnemy.Launcher.SetTarget(_target);
        }
        
        public void Exit()
        {
            _ai.Controller.MoveDirection = Vector3.zero;
            FieldEnemy fieldEnemy = _ai.GetComponentInParent<FieldEnemy>();
            fieldEnemy.Launcher.SetTarget(null);
        }

        public void Tick()
        {
            Vector3 minusValue = _target.position - _ai.transform.position;
            minusValue.y = 0;
            Vector3 dir = -minusValue.normalized; // 반대방향으로
            float dist = minusValue.magnitude;
            
            if (dist > _ai.Detector.LostRange)
            {
                _ai.ChangeState(ENEMY_AI_STATE.RETURN);
                return;
            }
            
            if (_optimalRange < dist)
            {
                // 공격
                _ai.Controller.MoveDirection = Vector3.zero;
                _ai.Controller.ChangeAction(ActionState.Attack);
            }
            else
            {
                _ai.Controller.MoveDirection = dir;
            }
        }
    }
}
