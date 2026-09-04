using UnityEngine;
using S7.Game.Field.Enemy;
using System.Collections.Generic;

namespace S7.Game.Field
{
    public class FieldEnemy : FieldUnit
    {
        private struct EnemyState
        {
            public Vector3 position;
            public Quaternion rotation;
            public bool isActive;
            public ENEMY_AI_STATE aiState;
            public float alertGauge;
        }

        [SerializeField] private Detector _detector;
        [SerializeField] private int _battleId = 1;
        [SerializeField] private ENEMY_ENCOUNTER_TYPE _encounterType = ENEMY_ENCOUNTER_TYPE.PASSIVE;
        // [SerializeField] private bool isAutoActive = true; 
        private bool _isAutoActive = true; 

        private FieldEnemyAI _ai;
        private bool _isBattleTriggered;
        
        private int BattleId => _battleId;

        public override void Die()
        {
            if (IsDead) return;
            base.Die();

            _ai.ChangeState(ENEMY_AI_STATE.DEATH);
            if (_detector != null) _detector.gameObject.SetActive(false);
        }

        public override void CaptureState(StateSnapshot snapshot)
        {
            base.CaptureState(snapshot);
            snapshot.Set(GetInstanceID(), new EnemyState
            {
                position = transform.position,
                rotation = transform.rotation,
                isActive = gameObject.activeSelf,
                aiState = _ai.State,
                alertGauge = _ai.AlertGauge,
            });
        }

        public override void RestoreState(StateSnapshot snapshot)
        {
            if (!snapshot.TryGet<EnemyState>(GetInstanceID(), out EnemyState state)) return;

            gameObject.SetActive(state.isActive);
            transform.position = state.position;
            transform.rotation = state.rotation;
            _ai.AlertGauge = state.alertGauge;
            _ai.ChangeState(state.aiState);
            _isBattleTriggered = false;
            base.RestoreState(snapshot);
        }

        public void ResetToSpawn()
        {
            if (!gameObject.activeSelf) return;

            transform.position = _ai.StartPos;
            transform.rotation = Quaternion.LookRotation(_ai.StartDir);
            _isBattleTriggered = false;
            _ai.Reset();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            if (_encounterType != ENEMY_ENCOUNTER_TYPE.AGGRESSIVE) return;
            if (_ai.State != ENEMY_AI_STATE.COMBAT) return;
            RequestBattle();
        }

        private void RequestBattle()
        {
            if (_isBattleTriggered) return;
            _isBattleTriggered = true;
            // TODO: change to unitId
            GameFlowManager.Instance.RequestBattle(1, null, new List<int>(){ GetInstanceID() });
        }

        private void OnValidate()
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }

        private void Start()
        {
            if (_isAutoActive)
            {
                SetUnitObject();
                
                _ccController.OnHurt += (x) => RequestBattle();
            
                _actionController.Initialize(UnitActionFactory.EnemyActions());

                _ai = GetComponent<FieldEnemyAI>();
                _ai.Initialize(_actionController);
            }
        }
    }
}
