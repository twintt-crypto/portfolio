using UnityEngine;
using System;
using UnityEngine.Events;

namespace S7.Game.Field
{
    public class InteractableObject : MonoBehaviour, IStateSaveable
    {
        private struct InteractableState
        {
            public bool fired;
        }
        
        [SerializeField] private bool showConfirmUI = true;
        [SerializeField] private bool useFire = true;
        
        private bool _fired = false;

        private bool CanInteract => !useFire || !_fired;

        public UnityEvent interactAction;
        
        public void Fire()
        {
            _fired = true;
        }

        public void TryInteract()
        {
            if (!CanInteract) return;
            
            if (showConfirmUI)
            {
                UIPanelField fieldPanel = UIManager.Instance.GetPanel("UIPanelField") as UIPanelField;
                // TODO: change text to real text
                if (fieldPanel) fieldPanel.ShowInteractUI("Interact~", Interact);
            }
            else
            {
                Interact();
            }
        }

        private void Interact()
        {
            if (!CanInteract) return;
            
            Fire();
            interactAction?.Invoke();
        }

#if UNITY_EDITOR
        private const string InteractableLayer = "InteractableObject";

        private void Reset()
        {
            int layer = LayerMask.NameToLayer(InteractableLayer);
            if (layer == -1)
            {
                Debug.LogError($"[InteractableObject] '{InteractableLayer}' 레이어가 존재하지 않습니다.", this);
                return;
            }
            gameObject.layer = layer;
        }

        private void OnValidate()
        {
            if (GetComponent<Collider>() == null)
                Debug.LogError("[InteractableObject] Collider가 없습니다.", this);
        }
#endif

        public void CaptureState(StateSnapshot snapshot)
        {
            snapshot.Set(GetInstanceID(), new InteractableState
            {
                fired = _fired,
            });
        }

        public void RestoreState(StateSnapshot snapshot)
        {
            if (!snapshot.TryGet<InteractableState>(GetInstanceID(), out InteractableState state)) return;

            _fired = state.fired;
        }
    }
}