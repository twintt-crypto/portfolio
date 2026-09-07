using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.QTE
{
    public class QTEReleaseInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnRelease;

        private bool _pressing;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_pressing) return;
            _pressing = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_pressing) return;
            _pressing = false;
            OnRelease?.Invoke();
        }
    }
}
