using UnityEngine;

namespace S7.Game.Field
{
    public abstract class FieldUnit : MonoBehaviour, IStateSaveable
    {
        [SerializeField] protected AttackType _attackType;
        [SerializeField] protected int _projectileId;

        private ProjectileLauncher _launcher;
        protected UnitActionController _actionController;
        protected CombatColliderController _ccController;
        protected AnimationEventReceiver _receiver;

        public GameObject UnitObject { get; protected set; }
        public AttackType AttackType { get => _attackType; set => _attackType = value; }
        public int ProjectileId { get => _projectileId; set => _projectileId = value; }
        public ProjectileLauncher Launcher => _launcher;
        public bool IsDead { get; private set; }
        public UnitActionController ActionController => _actionController;

        protected virtual void Awake()
        {
            if(!TryGetComponent<UnitActionController>(out _actionController)) _actionController = gameObject.AddComponent<UnitActionController>();
        }

        // CombatCollider가 unitObject에 붙어있음
        protected virtual void SetUnitObject()
        {
            _ccController?.DisableAll();

            Animator animator = GetComponentInChildren<Animator>();

            _ccController = GetComponentInChildren<CombatColliderController>();
            if(_ccController == null)
            {
                GameObject unitObj = animator != null ? animator.gameObject : gameObject;
                _ccController = unitObj.AddComponent<CombatColliderController>();
            }
            UnitObject = _ccController.gameObject;
            
            Collider existingCol = UnitObject.GetComponent<Collider>();
            if(existingCol != null)
            {
                existingCol.gameObject.layer = LayerMask.NameToLayer("HurtBox");
                _ccController.HurtCollider = existingCol;
            }
            else
            {
                CapsuleCollider col = UnitObject.AddComponent<CapsuleCollider>();
                col.isTrigger = true;
                col.center = new Vector3(0f, 0.8f, 0f);
                col.radius = 0.3f;
                col.height = 1.6f;
                col.gameObject.layer = LayerMask.NameToLayer("HurtBox");
                _ccController.HurtCollider = col;
            }

            _receiver = animator.GetComponent<AnimationEventReceiver>();
            if(_receiver == null) _receiver = animator.gameObject.AddComponent<AnimationEventReceiver>();

            _actionController.ChangeAnimator(animator);
            SetAttackType();
        }

        protected void SetAttackType(AttackType attackType)
        {
            if (_attackType == attackType) return;

            _attackType = attackType;
            SetAttackType();
        }
        
        protected void SetAttackType()
        {
            _launcher?.Dispose();
            _launcher = null;

            switch (_attackType)
            {
                case AttackType.Normal:
                    _receiver.Register("HitColliderOn", _ccController.HitColliderOn);
                    _receiver.Register("HitColliderOff", _ccController.HitColliderOff);
                    break;
                case AttackType.Projectile:
                    Animator animator = GetComponentInChildren<Animator>();
                    if (animator == null) break;

                    Transform arrowSocket = null;
                    foreach (Transform child in animator.GetComponentsInChildren<Transform>())
                    {
                        if (child.name == "ArrowSocket")
                        {
                            arrowSocket = child;
                            break;
                        }
                    }
                    if (arrowSocket == null) Debug.LogError($"[FieldUnit] {gameObject.name} has no ArrowSocket");

                    _launcher = new ProjectileLauncher(_projectileId, _receiver, arrowSocket, _ccController);
                    break;
            }
        }
        
        public virtual void Die()
        {
            if (IsDead) return;
            IsDead = true;

            if (_ccController != null) _ccController.DisableAll();

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                Collider selfCollider = rb.GetComponent<Collider>();
                if (selfCollider != null) selfCollider.enabled = false;
            }

            _actionController.ChangeAction(ActionState.Die);
        }

#if UNITY_EDITOR
        [ContextMenu("Test Die")]
        private void TestDie()
        {
            if (!Application.isPlaying) { Debug.LogWarning("[FieldUnit] 플레이 모드에서만 사용 가능합니다."); return; }
            Die();
        }
#endif

        protected virtual void OnDestroy()
        {
            _launcher?.Dispose();
        }

        public virtual void CaptureState(StateSnapshot snapshot)
        {
            _actionController.CaptureAnimatorState();
        }

        public virtual void RestoreState(StateSnapshot snapshot)
        {
            _actionController.RestoreAnimatorState();
        }
    }
}
