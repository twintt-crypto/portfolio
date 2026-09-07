using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using S7.Game.Field;

namespace S7.Game.Field.Enemy
{
    public partial class FieldEnemyAI : MonoBehaviour
    {
        [Header("Alert")]
        [SerializeField] private float _alertTime = 2f;
       
        // TODO: social 구현
        /*
        [Header("Social")]
        [SerializeField] private ENEMY_SOCIAL_TYPE _social_type = ENEMY_SOCIAL_TYPE.INDIVIDUAL;
        [SerializeField] private List<FieldEnemy> _social_enemies;
        */
        
        [Header("Combat")]
        [SerializeField] private ENEMY_COMBAT_STYLE _combatStyle = ENEMY_COMBAT_STYLE.CHASE;
        [SerializeField] private float _leashRange = 10f;
        [SerializeField] private float _targetLostTime = 3f;
        [SerializeField] private float _optimalRange = 5f;
        
        private Detector _detector;
        private UnitActionController _controller;
        private NavMeshAgent _navAgent;
        private Dictionary<ENEMY_AI_STATE, IUnitActionStrategy> _strategies = new();
        private ENEMY_AI_STATE _state = ENEMY_AI_STATE.NONE;
        private float _alertGauge;

        public Vector3 StartPos { get; private set; }
        public Vector3 StartDir { get; private set; }
        public float AlertRatio => _alertGauge / _alertTime;
        public float AlertGauge { get => _alertGauge; set => _alertGauge = value; }
        public ENEMY_AI_STATE State => _state;
        public Detector Detector => _detector;
        public UnitActionController Controller => _controller;
        public NavMeshAgent NavAgent => _navAgent;
        private IUnitActionStrategy CurrentStrategy => _strategies[_state];
        
        public event Action<float> OnGaugeChanged;
        public event Action<ENEMY_AI_STATE> OnStateChanged;

        public void Initialize(UnitActionController controller)
        {
            StartPos = transform.position;
            StartDir = transform.forward;         
            
            _detector = GetComponentInChildren<Detector>();
            _controller = controller;
            _navAgent = GetComponent<NavMeshAgent>();
            if (_navAgent != null)
            {
                _navAgent.updatePosition = false;
                _navAgent.updateRotation = false;
                _navAgent.stoppingDistance = controller.Mover.MoveStepValue;
            }
            
            _strategies[ENEMY_AI_STATE.IDLE] = new IdleStrategy(this);
            _strategies[ENEMY_AI_STATE.ALERT] = new AlertStrategy(this);
            _strategies[ENEMY_AI_STATE.RETURN] = new ReturnStrategy(this);
            _strategies[ENEMY_AI_STATE.DEATH] = new DeathStrategy(this);

            switch (_combatStyle)
            {
                case ENEMY_COMBAT_STYLE.STAY:
                    _strategies[ENEMY_AI_STATE.COMBAT] = new StayStrategy(this, _leashRange);
                    break;
                case ENEMY_COMBAT_STYLE.CHASE:
                    _strategies[ENEMY_AI_STATE.COMBAT] = new ChaseStrategy(this, _targetLostTime);
                    break;
                case ENEMY_COMBAT_STYLE.KITE:
                    _strategies[ENEMY_AI_STATE.COMBAT] = new KiteStrategy(this, _optimalRange);
                    break;
                // case ENEMY_COMBAT_STYLE.SUPPORT:
                //     break;
                default:
                    Debug.LogWarning($"Unknown combat style: {_combatStyle}");
                    break;
            }

            _controller.SubscribeOnEnter(ActionState.Attack, TrySnapToTarget);

            Reset();
        }

        private void TrySnapToTarget()
        {
            if (_detector.CurrentTarget == null) return;
            Vector3 dir = _detector.CurrentTarget.position - transform.position;
            dir.y = 0f;
            if (dir == Vector3.zero) return;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        private void Update()
        {
            CurrentStrategy.Tick();
        }

        private void FixedUpdate()
        {
            // navMehAgent sync 를 실제 position과 맞춰주기 위함
            if (_navAgent != null) _navAgent.nextPosition = _controller.Mover.Position;
        }

        public void AddAlertGauge()
        {
            if(_alertGauge == _alertTime) return;
            
            float prev = _alertGauge;
            _alertGauge = Mathf.Min(_alertGauge + Time.deltaTime, _alertTime);
            
            if (_alertGauge != prev) OnGaugeChanged?.Invoke(AlertRatio);
        }

        public void ReduceAlertGauge()
        {
            if (_alertGauge == 0) return;
            
            float prev = _alertGauge;
            _alertGauge = Mathf.Max(_alertGauge - Time.deltaTime, 0f);
            
            if (_alertGauge != prev) OnGaugeChanged?.Invoke(AlertRatio);
        }

        public void Reset()
        {
            _alertGauge = 0f;
            OnGaugeChanged?.Invoke(AlertRatio);
            ChangeState(ENEMY_AI_STATE.IDLE);
        }

        public void ChangeState(ENEMY_AI_STATE newState)
        {
            if (_state == newState) return;
            if (!_strategies.ContainsKey(newState)) return;

            if(_state != ENEMY_AI_STATE.NONE) CurrentStrategy.Exit();
            
            _state = newState;
            CurrentStrategy.Enter();
            OnStateChanged?.Invoke(newState);
        }
    }
}
