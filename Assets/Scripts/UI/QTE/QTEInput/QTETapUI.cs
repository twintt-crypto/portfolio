using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.QTE
{
    public class QTETapUI : MonoBehaviour, IPointerDownHandler
    {
        public event Action OnTap;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnTap?.Invoke();
        }
    }
}
