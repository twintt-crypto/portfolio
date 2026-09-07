using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameEventSystem;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace S7
{
    public class UnitView : MonoBehaviour
    {        
        float _parryingTime = 0;
        float _dodgeTIme = 0;

        // ================================
        // Inspector
        // ================================
        [Header("View Anchors")]
        [SerializeField] private Transform _modelRoot;   // 
        [SerializeField] private Transform _hitPoint;    //


        [Header("UI Anchors")]
        [SerializeField] private UnitUI _unitUi;
        [SerializeField] private Transform _topPoint;

        [SerializeField] private Collider _collider;

        public CharacterAnimationSet AnimationSet { get; private set; }

        public CombatColliderController CombatColliderController { get; private set; }

        public AnimationEventReceiver animationEventReceiver;        

        // ================================
        // Runtime
        // ================================
        public UnitData Data { get; private set; }

        public Transform TargetPoint { get; private set; }

        private GameObject _model;
        private Animator _animator;
        public Animator Animator { get => _animator; }

        private int _unitLayerMask;

        private GameObject _selectEffect;
        public Vector3 OriginPos { get; private set; }
        public Transform HitPoint { get => _hitPoint; }
        public GameObject Model { get => _model;}
        public Collider Collider { get => _collider; }

        // Sockets
        private Dictionary<string, Transform> socketMap = new Dictionary<string, Transform>();

        // ================================
        // Bind
        // ================================
        public void Bind(UnitController controller)
        {
            Data = controller.data;
        }

        // ================================
        // Model Loading
        // ================================
        public async UniTask LoadModelAsync(string modelAddress)
        {
            if (_model != null)
            {
                ResourceManager.Free(_model);
                _model = null;
                _animator = null;
            }

            // Addressables / ResourceManager 
            _model = await ResourceManager.NewAsync(modelAddress, _modelRoot);

            _model.transform.localPosition = Vector3.zero;
            _model.transform.localRotation = Quaternion.identity;

            // Animator 
            _animator = _model.GetComponentInChildren<Animator>();
            CombatColliderController = _model.GetComponentInChildren<CombatColliderController>();
            animationEventReceiver = _model.SafeGetComponent<AnimationEventReceiver>();
            
            UpdateData();

            OriginPos = transform.position;
        }

        public void Initialize(UnitData unitData, Transform  target)
        {
            TargetPoint = target;

            if (unitData.unitType == UnitType.Character == true)
            {
                _unitUi.SetActive(false);                
            }

            if( unitData.unitType == UnitType.Monster)
            {
                Utility.ChangeLayersRecursively(transform, "Monster");
            }            

            LoadCharacterAnimationSet(T_UnitData.Get(unitData.unitId));
            InitSocket();
        }

#if UNITY_EDITOR
        public async UniTask Initialize(T_UnitData unitData, Transform parent)
        {
            _unitUi.SetActive(false);

            await LoadCharacterAnimationSet(unitData.AnimationEvents);

            var handle = Addressables.InstantiateAsync(unitData.ModelPrefab, _modelRoot);

            var obj = await handle.Task;

            if (obj == null)
            {
                Debug.LogError($"Addressable 로드 실패: {unitData.ModelPrefab}");
                return;
            }

            _model = obj;

            _animator = _model.GetComponentInChildren<Animator>();
            animationEventReceiver = _model.SafeGetComponent<AnimationEventReceiver>();

        }
#endif 

        public void LoadCharacterAnimationSet(T_UnitData unitData)
        {
            if (unitData == null)
            {
                return;
            }

            if (unitData.AnimationEvents.IsNullOrEmpty() == true)
            {
                return;
            }

            if (!unitData.AnimationEvents.IsNullOrEmpty()) 
                LoadCharacterAnimationSet(unitData.AnimationEvents).Forget();
        }

        private async UniTask LoadCharacterAnimationSet(string AnimationEventName)
        {
            
            Debug.Log($"Loadint AnimationEventName : {AnimationEventName}");
            AnimationSet = await ResourceManager.LoadAssetAsync<CharacterAnimationSet>(AnimationEventName);
        }


        public void InitSocket()
        {
            socketMap.Clear();

            var markers = GetComponentsInChildren<SocketMarker>(true);

            foreach (var marker in markers)
            {
                socketMap[marker.SocketName] = marker.transform;
            }
        }

        void UpdateData()
        {
            var renderers = _model.GetComponentsInChildren<Renderer>();

            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            float forwardOffset = bounds.extents.z;
            float height = bounds.max.y;

            _topPoint.position = new Vector3(bounds.center.x, height, bounds.center.z);
            _hitPoint.position = bounds.center; 

            var capsule = _collider as CapsuleCollider;

            if (capsule != null)
            {
                // center는 로컬 기준이라 변환 필요
                Vector3 localCenter = _collider.transform.InverseTransformPoint(bounds.center);
                capsule.center = localCenter;

                // radius: X,Z 중 큰값 기준
                float radius = Mathf.Max(bounds.extents.x, bounds.extents.z);

                // height: 전체 높이
                height = bounds.size.y;

                // 캡슐은 height >= radius * 2 이어야 정상
                height = Mathf.Max(height, radius * 2f);

                capsule.radius = radius;
                capsule.height = height;
            }
        }

        // ================================
        // Animation API 
        // ================================        
        public async UniTask PlayAttackAsync(Action<int> onHit, CancellationToken token)
        {
            await UniTask.CompletedTask;
        }            

        public void PlayDeath()
        {
            //Animator.StringToHash
            _animator.CrossFadeInFixedTime(Animator.StringToHash("Death"), 0.1f);
        }

        public void SetSelected(bool on)
        {
            _unitUi.SetTargetiing(on, GetHitPoint());
        }

        // ================================
        // Utility
        // ================================
        public Transform GetHitPoint()
        {
            return _hitPoint != null ? _hitPoint : transform;
        }

        public async UniTask LookAtAsync(Vector3 targetPos, float duration = 0.4f, Ease ease = Ease.OutQuad)
        {
            if (this == null) return;

            Vector3 dir = targetPos - transform.position;
            dir.y = 0f;

            if (dir == Vector3.zero)
                return;

            Quaternion targetRot = Quaternion.LookRotation(dir);

            var tween = transform
                .DORotateQuaternion(targetRot, duration)
                .SetEase(ease);

            try
            {
                await tween.ToUniTask(
                    cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            catch (OperationCanceledException)
            {
                // View Destroy 
            }
        }

        bool isParrying = false;
        public async Task PlayParry()
        {
            CancellationTokenSource cancelToken = new CancellationTokenSource();

            if(isParrying == true)
            {
                return;
            }

            if(_animator == null)
            {
                return;
            }

            var state = _animator.GetCurrentAnimatorStateInfo(0);
            if (state.shortNameHash == Animator.StringToHash("Parry") && !_animator.IsInTransition(0))
                return;
            
            isParrying = true;
            await PlayAnimationAsync(Animator.StringToHash("Parry"), cancelToken.Token);
            isParrying = false;

            OnlyPlayAnimationAsync(Animator.StringToHash("Battle_Idle"), cancelToken.Token).Forget();
        }

        public async UniTask PlayAnimationAsync(int animHash, CancellationToken token)
        {
            if (_animator == null || !gameObject.activeInHierarchy)
                return;

            var current = _animator.GetCurrentAnimatorStateInfo(0);

            if (current.shortNameHash == animHash && !_animator.IsInTransition(0))
                return;

            var next = _animator.GetNextAnimatorStateInfo(0);

            if ((current.shortNameHash == animHash || next.shortNameHash == animHash)
                && !_animator.IsInTransition(0))
                return;

            // 애니 실행
            _animator.CrossFadeInFixedTime(animHash, 0.15f);

            // 상태 진입 대기
            await UniTask.WaitUntil(() =>
            {
                if (_animator == null)
                    return true;

                var state = _animator.GetCurrentAnimatorStateInfo(0);

                return state.shortNameHash == animHash &&
                       !_animator.IsInTransition(0);
            }, cancellationToken: token);

            // ======================
            // 여기서 이벤트 시작
            // ======================
            RunEventAsync(animHash, token).Forget();

            token.ThrowIfCancellationRequested();


            // ======================
            // 애니 종료 대기
            // ======================
            await UniTask.WaitUntil(() =>
            {
                if (_animator == null)
                    return true;

                var state = _animator.GetCurrentAnimatorStateInfo(0);

                // 상태 바뀌면 종료
                if (state.shortNameHash != animHash)
                    return true;

                // transition 중이면 아직 끝 아님
                if (_animator.IsInTransition(0))
                    return false;

                return state.normalizedTime >= 1f;

            }, cancellationToken: token).SuppressCancellationThrow();
        }

        private async UniTask HandleEvents(AnimationStateEventData data, int animHash, CancellationToken token)
        {
            if (_animator == null || data == null || data.events == null || data.events.Count == 0)
                return;

            var events = data.events;

            // 이벤트 실행 상태 (데이터 오염 방지)
            bool[] triggered = new bool[events.Count];

            while (true)
            {
                if (_animator == null)
                    return;

                var state = _animator.GetCurrentAnimatorStateInfo(0);

                if (state.shortNameHash == animHash)
                    break;

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            while (true)
            {
                if (_animator == null)
                    return;

                var state = _animator.GetCurrentAnimatorStateInfo(0);

                // 애니 변경되면 종료
                if (state.shortNameHash != animHash)
                {               
                    break;
                }                    

                // 현재 시간 (초)
                float currentTime = state.normalizedTime * state.length;

                for (int i = 0; i < events.Count; i++)
                {
                    if (triggered[i])
                        continue;

                    var evt = events[i];

                    // 시간 도달
                    if (currentTime >= evt.time)
                    {
                        triggered[i] = true;
                        ExecuteEvent(evt);
                    }
                    else
                    {
                        // 정렬되어 있으므로 이후는 체크 불필요
                        break;
                    }
                }

                // 애니 끝
                if (state.normalizedTime >= 1f)
                    break;

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        private void ExecuteEvent(AnimationEventData evt)
        {
            switch (evt.type)
            {
                case AnimationEventType.SpawnEffect:
                    PlayFx(evt).Forget();
                    break;

                case AnimationEventType.OnHit:
                    animationEventReceiver.OnAnimationEvent(evt.type, evt);
                    break;
                case AnimationEventType.BeginParry:                    
                    _parryingTime = Time.unscaledTime;
                    break;
                case AnimationEventType.EndParry:
                    _parryingTime = 0;
                    _successParryCount = 0;
                    break;
                case AnimationEventType.BeginDodge:
                    _dodgeTIme = Time.unscaledTime;
                    break;
                case AnimationEventType.EndDodge:
                    _dodgeTIme = 0;
                    break;
                case AnimationEventType.Sound:
                    //PlaySound(evt);
                    break;
            }
        }

        private async UniTask PlayFx(AnimationEventData evt)
        {
            if (evt.attachSocket == null)
                return;

            Transform anchor = GetSocket(evt.attachSocket);

            if (anchor == null)
                anchor = transform;

            var effect = evt.attachSocket;                        

             await EffectSpawner.Spawn(evt.effectAddressKey, transform.position, _modelRoot.GetChild(0), evt.duration);
        }

        public async UniTask OnlyPlayAnimationAsync(int animHash, CancellationToken token)
        {
            if (_animator == null || !gameObject.activeInHierarchy)
                return;            

            // 애니 시작
            _animator.CrossFadeInFixedTime(animHash, 0.15f);            
            RunEventAsync(animHash, token).Forget();
            await UniTask.CompletedTask;
        }

        private async UniTaskVoid RunEventAsync(int animHash, CancellationToken token)
        {           
            if (_animator == null)
                return;

            var data = AnimationSet?.Get(animHash);

            if (data == null || data.events == null || data.events.Count == 0)
                return;

            // 이벤트 실행 (독립적으로)
            await HandleEvents(data, animHash, token);
        }

        public async UniTask MoveToAsync(
        Vector3 pos,
        int duration = 200,
        Ease ease = Ease.Linear,
        CancellationToken token = default)
        {
            if (this == null) return;

            var tween = transform
                .DOMove(pos, (float)duration / 1000)
                .SetEase(ease);

            await tween.ToUniTask(cancellationToken: token);
        }

        public void OnDestroy()
        {
            transform.DOKill();
        }

        public Transform GetSocket(string socketName)
        {
            if (string.IsNullOrEmpty(socketName))
                return transform;

            if (socketMap.TryGetValue(socketName, out var socket))
                return socket;

            Debug.LogWarning($"Socket not found: {socketName}");
            return transform;
        }

        private int _successParryCount = 0;
        public async UniTask OnHit(HitResult hit, List<T_SkillHitData> hitDatas)
        {
            if(hitDatas == null)
            {
                return;
            }

            if(hitDatas.Count == 0)
            {
                return;
            }
             
            var (reactiveType, success) = CheckReactive(hit);
            if (success == true)
            {
                switch(reactiveType)
                {
                    case ReactiveType.Parry:
                        {
                            Debug.Log("Success Parrying");
                            _successParryCount++;
                            if(hit.hitIndex == hitDatas.Count - 1)
                            {
                                if (_successParryCount == hitDatas.Count)
                                {
                                    //전체 패링 성공
                                }
                            }                            
                        }
                        break;
                    case ReactiveType.Dodge:
                        {
                            Debug.Log("Success Dodge");

                        }
                        break;
                }                                                
                return;
            }

            T_SkillHitData hitData = hitDatas[hit.hitIndex];
            if(hitData == null)
            {
                return;
            }

            T_EffectData effectData = T_EffectData.Get(hitData.HitEffectId);
            if (effectData != null)
            {
                EffectSpawner.Spawn(effectData, hit.target.view.GetHitPoint(), hit.target.view.transform).Forget();
            }            

            await BattleUIManager.Instance.ShowDamage(hit.target.view.transform.position, hit.damage);
            
            if( Data.IsDead == true)
            {
                PlayDeath();
            }            
        }

        private (ReactiveType, bool) CheckReactive(HitResult hit)
        {
            if (Data.unitType != UnitType.Character)
            {
                return (ReactiveType.None,false);
            }

            if (_parryingTime > 0)
            {
                if (hit.hitTime - _parryingTime < T_GlobalValueData.Get(GlobalValueType.ParryTime).ValueFloat)
                {
                    
                    return (ReactiveType.Parry, true);
                }
            }

            if( _dodgeTIme > 0)
            {
                if (hit.hitTime - _parryingTime < T_GlobalValueData.Get(GlobalValueType.ParryTime).ValueFloat)
                {

                    return (ReactiveType.Dodge, true);
                }
            }
            return (ReactiveType.None, false);
        }
    }   
}


