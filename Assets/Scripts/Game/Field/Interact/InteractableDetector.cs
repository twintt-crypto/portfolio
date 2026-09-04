using UnityEngine;

namespace S7.Game.Field
{
    [RequireComponent(typeof(Detector))]
    public class InteractableDetector : MonoBehaviour
    {
        private Detector _detector;

        private void Awake()
        {
            _detector = GetComponent<Detector>();
        }

        private void OnEnable()
        {
            _detector.OnDetected += HandleDetected;
            _detector.OnLost += HandleLost;
        }

        private void OnDisable()
        {
            _detector.OnDetected -= HandleDetected;
            _detector.OnLost -= HandleLost;
        }

        private void HandleDetected(Transform target)
        {
            InteractableObject interactable = target.GetComponent<InteractableObject>();
            if (interactable == null) return;

            interactable.TryInteract();
        }

        private void HandleLost(Transform target)
        {
            UIPanelField fieldPanel = UIManager.Instance.GetPanel("UIPanelField") as UIPanelField;
            if (fieldPanel) fieldPanel.HideInteractUI();
        }
    }
}
