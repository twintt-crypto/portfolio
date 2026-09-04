using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace S7
{
    public class BattleInputHandler : MonoBehaviour
    {
        [SerializeField] private BattleManager _battleManager;
        private Camera _camera;

        private Vector2 _dragStart;
        private bool _dragging;

        private void Awake()
        {
            _camera = Camera.main;            
        }

        private void Update()
        {
            if (_battleManager == null)
                return;

            if (!Input.GetMouseButtonDown(0))
                return;

            if (_camera == null)
                return;

            // 클릭 시작 프레임
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // UI 위 클릭 무시
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject())
                    return;

                Vector2 mousePos = Mouse.current.position.ReadValue();

                Ray ray = _camera.ScreenPointToRay(mousePos);

                var _unitLayerMask = LayerMask.GetMask("Monster");

                if (Physics.Raycast(ray, out RaycastHit hit, 100f, _unitLayerMask))
                {
                    UnitView unit = hit.collider.GetComponentInParent<UnitView>();

                    if (unit != null)
                    {
                        _battleManager.SelectTarget(unit).Forget();
                    }
                }
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _dragStart = Mouse.current.position.ReadValue();
                _dragging = true;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && _dragging)
            {
                _dragging = false;

                Vector2 end = Mouse.current.position.ReadValue();
                float deltaX = end.x - _dragStart.x;

                if (Mathf.Abs(deltaX) > 40f)
                {
                    if (deltaX < 0)
                        SelectLeftCharacter();
                    else
                        SelectRightCharacter();
                }
            }
        }

        private void SelectLeftCharacter()
        {
            var units = _battleManager.GetSelectableUnits();
            if (units == null || units.Count == 0)
                return;

            UnitView leftMost = units
                .OrderBy(u => _camera.WorldToScreenPoint(u.transform.position).x)
                .First();

            _battleManager.SelectTarget(leftMost).Forget();
        }

        private void SelectRightCharacter()
        {
            var units = _battleManager.GetSelectableUnits();
            if (units == null || units.Count == 0)
                return;

            UnitView rightMost = units
                .OrderByDescending(u => _camera.WorldToScreenPoint(u.transform.position).x)
                .First();

            _battleManager.SelectTarget(rightMost).Forget();
        }
    }
}
