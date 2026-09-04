using UnityEngine;

namespace S7.Game.Field.Enemy
{
    public class ReturnStrategy : IUnitActionStrategy
    {
        private FieldEnemyAI _ai;
        private bool _arrived = false;

        public ReturnStrategy(FieldEnemyAI ai)
        {
            _ai = ai;
        }

        public void Enter()
        {
            _ai.NavAgent.SetDestination(_ai.StartPos);
            
            _arrived = false;
            _ai.Detector.OnDetected += OnDetected;
        }

        public void Exit()
        {
            _ai.Controller.MoveDirection = Vector3.zero;
            _ai.Detector.OnDetected -= OnDetected;
        }

        public void Tick()
        {
            if (!_arrived)
            {
                // pathPending -> 경로 비동기 계산 중이면 true, 계산 끝났으면 false
                bool tickArrived = !_ai.NavAgent.pathPending && _ai.NavAgent.remainingDistance <= _ai.NavAgent.stoppingDistance;
                
                if (!tickArrived)
                {
                    _ai.Controller.MoveDirection = GetNavDirection();
                }
                else
                {
                    _arrived = true;
                    _ai.Controller.SnapTo(_ai.StartPos);
                    _ai.Controller.MoveDirection = Vector3.zero;
                }
            }
            else if (Vector3.Dot(_ai.transform.forward, _ai.StartDir) < 0.99f)
            {
                _ai.Controller.RotateDirection = _ai.StartDir;
            }
            else
            {
                _ai.ChangeState(ENEMY_AI_STATE.IDLE);
            }

            _ai.ReduceAlertGauge();
        }

        private Vector3 GetNavDirection()
        {
            if (_ai.NavAgent == null) return Vector3.zero;
            
            Vector3 vel = _ai.NavAgent.desiredVelocity;
            return vel != Vector3.zero ? vel.normalized : Vector3.zero;
        }

        private void OnDetected(Transform transform)
        {
            _ai.ChangeState(ENEMY_AI_STATE.ALERT);
        }
    }
}
