using Cysharp.Threading.Tasks;
using Gpm.Ui;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace S7
{
    public class UIPanelField : UIBase
    {
        [SerializeField] Joystick _joystick;

        [Header("Attack")]
        [SerializeField] private Button attackButton;

        [Header("Interact")]
        [SerializeField] private Button interactButton;
        [SerializeField] private TextMeshProUGUI interactText;

        [Header("Speech")]
        [SerializeField] private GameObject speechObject;
        [SerializeField] private TextMeshProUGUI speechText;

        [Header("Stamina")]
        [SerializeField] private GameObject staminaObject;

        [Header("Target Marker")]
        [SerializeField] private RectTransform _targetMarker;

        [Header("Minimap")]
        [SerializeField] private FieldMinimap _minimap;

        [Header("Unit Status Bar")]
        [SerializeField] private FieldUnitStatusBar _unitStatusBar;

        [Header("Quest Tracker")]
        [SerializeField] private UIFieldQuestTracker _questTracker;

        private Gamepad _virtualGamepad;
        private UnityAction interactButtonAction;
        private RectTransform _canvasRect;

        protected override void Awake()
        {
            base.Awake();
            _canvasRect = UIManager.Instance.Canvas.GetComponent<RectTransform>();
            
            HideInteractUI();
            HideSpeech();
            HideStamina();
            HideTargetMarker();
        }
        
        protected override void Start()
        {
            base.Start();
            _virtualGamepad = InputSystem.AddDevice<Gamepad>();
        }

        protected override void Initialize()
        {
            attackButton.onClick.AddListener(() =>
            {
                PressAttackAsync().Forget();
            });

            if (_minimap != null) _minimap.Initialize();
            if (_questTracker != null) _questTracker.Initialize();
        }

        private async UniTaskVoid PressAttackAsync()
        {
            if (_virtualGamepad == null) return;

            _virtualGamepad.buttonWest.QueueValueChange(1f);
            await UniTask.NextFrame();
            _virtualGamepad.buttonWest.QueueValueChange(0f);
        }

        private void Update()
        {
            if (_joystick != null && _virtualGamepad != null)
                InputState.Change(_virtualGamepad.leftStick, _joystick.Direction);

        }

        protected override void OnDestroy()
        {
            if (_minimap != null) _minimap.Release();
            if (_unitStatusBar != null) _unitStatusBar.Release();
            if (_questTracker != null) _questTracker.Release();

            if (_virtualGamepad != null)
                InputSystem.RemoveDevice(_virtualGamepad);
        }

        public void ResetInteract()
        {
            if (interactButtonAction != null) interactButton.onClick.RemoveListener(interactButtonAction);
            interactButtonAction = null;
        }

        public void ShowInteractUI(string interactTitle, UnityAction callback = null)
        {
            ResetInteract();

            interactButtonAction = callback;
            interactText.text = interactTitle;
            interactButton.onClick.AddListener(interactButtonAction);
            interactButton.gameObject.SetActive(true);
        }
        
        public void HideInteractUI()
        {
            ResetInteract();
            interactButton.gameObject.SetActive(false);
        }

        public void ShowSpeech(string text)
        {
            speechText.text = text;
            speechObject.SetActive(true);
        }

        public void HideSpeech()
        {
            speechObject.SetActive(false);
        }

        [ContextMenu("Show Stamina")]
        public void ShowStamina()
        {
            staminaObject.SetActive(true);
        }

        [ContextMenu("Hide Stamina")]
        public void HideStamina()
        {
            staminaObject.SetActive(false);
        }

        public void ShowTargetMarker(Vector3 worldPos)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            
            bool result = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect,
                screenPos,
                UIManager.Instance.Camera,
                out Vector2 localPoint);

            _targetMarker.localPosition = localPoint;
            _targetMarker.gameObject.SetActive(true);
        }

        public void HideTargetMarker()
        {
            _targetMarker.gameObject.SetActive(false);
        }

        public void SetField(bool isNight, IReadOnlyList<UnitData> units = null)
        {
            if (isNight && units != null)
            {
                if (_unitStatusBar == null) return;

                _unitStatusBar.Initialize(units);
                _unitStatusBar.Show();
            }
            else
            {
                if (_unitStatusBar != null) _unitStatusBar.Hide();
            }
        }

#if UNITY_EDITOR
        [GameButton("Test Night")]
        private void TestNight() => SetField(true, UnitDataManager.Instance.PartyUnits);

        [GameButton("Test Day")]
        private void TestDay() => SetField(false);

        [GameButton("Test Quest Tracker")]
        private void TestQuestTracker() => _questTracker?.Show("Test Quest title", "테스트 퀘스트 설명입니다.");
#endif
    }
}
