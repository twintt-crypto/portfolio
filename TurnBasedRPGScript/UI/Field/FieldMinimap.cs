using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace S7
{
    /// <summary>
    /// 플레이어 중심 회전 미니맵.
    /// 플레이어 아이콘은 중앙 고정(항상 위 방향), 맵 이미지가 이동+회전.
    ///
    /// 프리팹 구조:
    ///   MinimapRoot (this, RectMask2D)
    ///     └ MapPivot (회전용)
    ///         └ MapImage (RawImage, 이동용)
    ///             └ 마커들 (MinimapIcon)
    ///     PlayerIcon (중앙 고정)
    /// </summary>
    public class FieldMinimap : MonoBehaviour
    {
        [Header("Map")]
        [SerializeField] private RectTransform _mapPivot;
        [SerializeField] private RectTransform _mapMoveRect;
        [SerializeField] private Image _mapImage;

        [Header("World Bounds")]
        [SerializeField] private Vector2 _worldCenter;
        [SerializeField] private Vector2 _worldSize = new Vector2(100f, 100f);

        private bool _isActive;
        [SerializeField] private List<MinimapIcon> _icons = new List<MinimapIcon>();
        [SerializeField] private RectTransform _cameraIcon;

        public void RegisterIcon(MinimapIcon icon)
        {
            _icons.Add(icon);
        }

        public void UnregisterIcon(MinimapIcon icon)
        {
            _icons.Remove(icon);
        }

        public void Initialize()
        {
            _isActive = true;
            gameObject.SetActive(true);
        }

        public void Initialize(SpriteRenderer mapImage)
        {
            _mapImage.sprite = mapImage.sprite;
            Initialize();
        }

        public void Release()
        {
            _isActive = false;
            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!_isActive) return;

            Transform playerTransform = FieldManager.Instance.PlayerTransform;
            if (playerTransform == null) return;

            float playerY = playerTransform.eulerAngles.y;
            UpdateMap(playerTransform.position, playerY);

            for (int i = 0; i < _icons.Count; i++)
            {
                _icons[i].ResetRotation();
            }

            if (_cameraIcon != null)
            {
                float cameraY = Camera.main.transform.eulerAngles.y;
                _cameraIcon.rotation = Quaternion.Euler(0f, 0f, -(cameraY - playerY));
            }
        }

        private void UpdateMap(Vector3 playerWorldPos, float playerYRotation)
        {
            // 월드 좌표 → 맵 이미지 내 픽셀 좌표
            Vector2 worldMin = _worldCenter - _worldSize * 0.5f;
            float normalizedX = (playerWorldPos.x - worldMin.x) / _worldSize.x;
            float normalizedZ = (playerWorldPos.z - worldMin.y) / _worldSize.y;

            Vector2 mapSize = _mapMoveRect.sizeDelta;
            Vector2 playerMapPos = new Vector2(normalizedX * mapSize.x, normalizedZ * mapSize.y);
            Vector2 mapCenter = mapSize * 0.5f;

            // 맵 이미지 이동: 플레이어 위치가 피봇 중앙에 오도록 오프셋
            // (피봇 회전 시 자식 오프셋도 함께 회전되므로 별도 보정 불필요)
            _mapMoveRect.anchoredPosition = mapCenter - playerMapPos;

            // 피봇 회전: 플레이어가 항상 화면 위를 향하도록
            _mapPivot.localRotation = Quaternion.Euler(0f, 0f, playerYRotation);
        }

    }
}
