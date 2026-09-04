using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class UIDialogueChoiceButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _labelText;
        [SerializeField] private Button _button;

        public void Setup(string label, Action onClicked)
        {
            _labelText.text = label;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => onClicked?.Invoke());
        }
    }
}
