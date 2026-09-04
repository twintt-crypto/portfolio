using UnityEngine;

namespace S7.Game.Field.Enemy
{
    public class ChaseStrategy : IUnitActionStrategy
    {
        private FieldEnemyAI _ai;
        
        private float _lostTime; // 체크하는 시간
        private float _lostGauge = 0; // 현재 체킹 시간 == target_lost_time

        private Transform _target;
        private Vector3 _lastPos;

        private bool _isLost;

        public ChaseStrategy(FieldEnemyAI ai, float lostTime)
        {
            _ai = ai;
            _lostTime = lostTime;
        }

        public void Enter()
        {
            if (_ai.Detector == null || _ai.Detector.CurrentTarget == null)
            {
                Debug.LogWarning("[ChaseStrategy] detector error");
                _ai.ChangeState(ENEMY_AI_STATE.IDLE);
                return;
            }
            
            _ai.Detector.OnDetected += OnDetect;
            _ai.Detector.OnLost += OnLost;
            
            _target = _ai.Detector.CurrentTarget;
            _lostGauge = 0f;
            _isLost = false;
        }
        
        public void Exit()
        {
            _ai.Controller.MoveDirection = Vector3.zero;
            _ai.Detector.OnDetected -= OnDetect;
            _ai.Detector.OnLost -= OnLost;           
        }

        public void Tick()
        {
            if (_isLost)
            {
                bool arrived = !_ai.NavAgent.pathPending
                            && _ai.NavAgent.remainingDistance <= _ai.NavAgent.stoppingDistance;
                if (!arrived)
                {
                    _ai.Controller.MoveDirection = GetNavDirection(_lastPos);
                }
                else
                {
                    _ai.Controller.MoveDirection = Vector3.zero;
                    _lostGauge = Mathf.Min(_lostGauge + Time.deltaTime, _lostTime);
                    if (_lostGauge >= _lostTime) _ai.ChangeState(ENEMY_AI_STATE.RETURN);
                }
                return;
            }

            _lastPos = _target.position;
            _ai.Controller.MoveDirection = GetNavDirection(_lastPos);
        }

        private Vector3 GetNavDirection(Vector3 destination)
        {
            if (_ai.NavAgent == null) return Vector3.zero;
            _ai.NavAgent.SetDestination(destination);
            Vector3 vel = _ai.NavAgent.desiredVelocity;
            return vel != Vector3.zero ? vel.normalized : Vector3.zero;
        }

        private void OnDetect(Transform transform)
        {
            _isLost = false;
            _lostGauge = 0;
        }

        private void OnLost(Transform transform)
        {
            _isLost = true;
        }
    }
}
