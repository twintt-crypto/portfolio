using System;
using System.Collections.Generic;
using UnityEngine;

namespace S7.Game.Field
{
    public enum DetectionMode
    {
        FIRST_ENTER,
        NEAREST,
    }

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public partial class Detector : MonoBehaviour
    {
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private float _detectionRange = 5f;
        [SerializeField] private float _lostRange = 8f;
        [SerializeField, Range(0f, 360f)] private float _detectionAngle = 360f;
        [SerializeField] private DetectionMode _detectionMode = DetectionMode.FIRST_ENTER;

        // IsVisible 이 false → true 될 때 발동
        public event Action<Transform> OnDetected;
        // IsVisible 이 true → false 될 때 발동
        public event Action<Transform> OnLost;

        public bool IsVisible { get; private set; }
        public Transform CurrentTarget { get; private set; }
        public float LostRange => _lostRange;

        protected readonly List<Transform> _candidates = new List<Transform>();

        private void Awake()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
            if (col is SphereCollider sphere) sphere.radius = _detectionRange;
            else if (col is CapsuleCollider capsule) capsule.radius = _detectionRange;

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        private void FixedUpdate()
        {
            switch (_detectionMode)
            {
                case DetectionMode.FIRST_ENTER: FixedUpdateFirstEnter(); break;
                case DetectionMode.NEAREST:     FixedUpdateNearest();    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsTargetLayer(other.gameObject.layer)) return;
            if (_candidates.Contains(other.transform)) return;
            _candidates.Add(other.transform);
        }

        public void Clear()
        {
            _candidates.Clear();
            HandleLost();
        }

        // 후보 제거는 Update/FixedUpdate의 거리 체크에서 처리
        // lostRange <= detectionRange 여부와 무관하게 동일 로직으로 동작

        protected void HandleLost()
        {
            if (IsVisible)
            {
                IsVisible = false;
                OnLost?.Invoke(CurrentTarget);
            }
            CurrentTarget = null;
        }

        // 타겟 전환 시 기존 타겟 OnLost 처리 후 교체
        protected void SetTarget(Transform newTarget)
        {
            if (CurrentTarget == newTarget) return;

            if (IsVisible)
            {
                IsVisible = false;
                OnLost?.Invoke(CurrentTarget);
            }
            CurrentTarget = newTarget;
        }

        // CurrentTarget의 시야 여부 변화 시 이벤트 발생
        protected void UpdateVisibility()
        {
            if (CurrentTarget == null) return;

            bool nowVisible = CheckVision(CurrentTarget);
            if (nowVisible == IsVisible) return;

            IsVisible = nowVisible;
            if (IsVisible) OnDetected?.Invoke(CurrentTarget);
            else OnLost?.Invoke(CurrentTarget);
        }

        private bool CheckVision(Transform target)
        {
            if (_detectionAngle >= 360f) return true;

            Vector3 dir = target.position - transform.position;
            dir.y = 0f;
            dir.Normalize();

            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();

            float angle = Vector3.Angle(forward, dir);
            return angle <= _detectionAngle * 0.5f;
        }

        private bool IsTargetLayer(int layer)
        {
            return (_targetLayer & (1 << layer)) != 0;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            if (_detectionAngle < 360f)
            {
                float halfAngle = _detectionAngle * 0.5f;
                Vector3 leftDir = Quaternion.Euler(0f, -halfAngle, 0f) * transform.forward;
                Vector3 rightDir = Quaternion.Euler(0f, halfAngle, 0f) * transform.forward;
                Gizmos.DrawRay(transform.position, leftDir * _detectionRange);
                Gizmos.DrawRay(transform.position, rightDir * _detectionRange);
            }
            Gizmos.DrawWireSphere(transform.position, _detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _lostRange);
        }
#endif
    }
}
