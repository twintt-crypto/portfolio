using UnityEngine;

namespace S7.Game.Field
{
    public class FieldPlayer : FieldUnit
    {
        [SerializeField] private FieldAutoAim _autoAim;

        protected override void Awake()
        {
            base.Awake();
            
            if (!TryGetComponent<PlayerInputController>(out var controller)) controller = gameObject.AddComponent<PlayerInputController>();
            controller.Initialize(_actionController);
        }
        
        private void OnValidate()
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        protected override void SetUnitObject()
        {
            if(_ccController != null) _ccController.OnHurt -= RequestBattle;
            
            _actionController.Initialize(UnitActionFactory.PlayerActions());
            _autoAim.Initialize(_actionController);
            
            base.SetUnitObject();
            _ccController.OnHurt += RequestBattle;
        }

        public void SetPlayerObject()
        {
            SetUnitObject();
        }

        public void ChangeAttackType(AttackType attackType)
        {
            SetAttackType(attackType);
        }

        private struct PlayerState
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        public override void CaptureState(StateSnapshot snapshot)
        {
            base.CaptureState(snapshot);
            snapshot.Set(GetInstanceID(), new PlayerState
            {
                position = transform.position,
                rotation = transform.rotation,
            });
        }

        public override void RestoreState(StateSnapshot snapshot)
        {
            if (!snapshot.TryGet<PlayerState>(GetInstanceID(), out PlayerState state)) return;

            transform.SetPositionAndRotation(state.position, state.rotation);
            _autoAim.ClearDetector();
            base.RestoreState(snapshot);
        }

        private void RequestBattle(CombatColliderController enemy)
        {
            // TODO: add battle info
            GameFlowManager.Instance.RequestBattle();
        }

        public override void Die()
        {
            if(IsDead) return;

            base.Die();
        }
    }
}
