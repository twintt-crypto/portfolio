using UnityEngine;

namespace S7.Game.Field
{
    [RequireComponent(typeof(InteractableObject))]
    public class FieldPortal : MonoBehaviour
    {
        [SerializeField] private int fieldId = 0;
        [SerializeField] private bool isNightPortal = false;
        [SerializeField] private bool isActive = true;

        private InteractableObject _interactable;
        private Collider _collider;

        private void Awake()
        {
            _interactable = GetComponent<InteractableObject>();
            _collider = GetComponent<Collider>();
            _interactable.interactAction.AddListener(OnInteract);
            Refresh();
        }

        public void Setup(int fieldId)
        {
            this.fieldId = fieldId;
            Refresh();
        }

        private void Refresh()
        {
            bool active = fieldId != 0;
            if (_collider != null) _collider.enabled = active;
        }

        private void OnInteract()
        {
            if (fieldId == 0 || !isActive) return;

            if (isNightPortal) GameFlowManager.Instance.RequestMoveNightField(fieldId);
            else GameFlowManager.Instance.RequestMoveDayField(fieldId);
        }
    }
}
