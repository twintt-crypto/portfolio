using UnityEngine;

namespace S7.Game.Field
{
    [RequireComponent(typeof(Detector))]
    public class FieldAutoAim : MonoBehaviour
    {
        private Detector _detector;
        private UnitActionController _uaController;
        private UIPanelField _panelField;
        private UIPanelField PanelField
        {
            get
            {
                if (_panelField == null)
                    _panelField = UIManager.Instance.GetPanel("UIPanelField") as UIPanelField;
                return _panelField;
            }
        }
        private Collider _targetCollider;

        private void Awake()
        {
            _detector = GetComponent<Detector>();
        }

        public void Initialize(UnitActionController actionController)
        {
            _detector.OnDetected += HandleDetected;
            _detector.OnLost += HandleLost;
            
            _uaController = actionController;
            _uaController?.SubscribeOnEnter(ActionState.Attack, TrySnapToTarget);
        }

        public void ClearDetector()
        {
            _detector.Clear();
        }

        private void OnDestroy()
        {
            _detector.OnDetected -= HandleDetected;
            _detector.OnLost -= HandleLost;
            _uaController?.UnsubscribeOnEnter(ActionState.Attack, TrySnapToTarget);
        }

        private void Update()
        {
            if (_targetCollider == null) return;
            PanelField?.ShowTargetMarker(_targetCollider.bounds.center);
        }

        private void TrySnapToTarget()
        {
            if (_detector.CurrentTarget == null) return;
            Vector3 dir = _detector.CurrentTarget.position - _uaController.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            _uaController.SnapRotation(Quaternion.LookRotation(dir));
        }

        private void HandleDetected(Transform target)
        {
            _targetCollider = target.GetComponent<Collider>();
        }

        private void HandleLost(Transform target)
        {
            _targetCollider = null;
            PanelField?.HideTargetMarker();
        }
    }
}
