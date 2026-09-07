using System;
using System.Collections.Generic;
using Game.QTE;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.QTE
{
    public class QTESwipeUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private static readonly Dictionary<QTE_SWIPE_DIR, Vector2> Directions = new()
        {
            { QTE_SWIPE_DIR.LEFT,  new Vector2(-1f,  0f) },
            { QTE_SWIPE_DIR.RIGHT, new Vector2( 1f,  0f) },
            { QTE_SWIPE_DIR.UP,    new Vector2( 0f,  1f) },
            { QTE_SWIPE_DIR.DOWN,  new Vector2( 0f, -1f) },
        };

        [SerializeField] private float _swipeThreshold = 40f;
        [SerializeField, Range(-1f, 1f)] private float _degreeSensitivity = 0.87f;

        public event Action<QTE_SWIPE_DIR> OnSwipe;

        private Vector2 _pointerDownPos;
        private bool _fired;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_fired) return;
            _pointerDownPos = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_fired) return;

            Vector2 delta = eventData.position - _pointerDownPos;
            if (delta.magnitude < _swipeThreshold) return;

            _fired = true;
            OnSwipe?.Invoke(GetDirection(delta.normalized));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _fired = false;
        }

        private QTE_SWIPE_DIR GetDirection(Vector2 normalizedDelta)
        {
            foreach (var pair in Directions)
            {
                if (Vector2.Dot(normalizedDelta, pair.Value) >= _degreeSensitivity)
                    return pair.Key;
            }
            return QTE_SWIPE_DIR.NONE;
        }
    }
}
