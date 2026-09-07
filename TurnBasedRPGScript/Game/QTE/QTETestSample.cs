using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Game.QTE;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Test.QTE
{
    public class QTETestSample : MonoBehaviour
    {
        [SerializeField] private List<QTEConfig> _group = new();
        
        [SerializeField] private QTERunner _runner;
        [SerializeField] private GameObject _viewPrefab;

        [Header("Config Input Fields")]
        [SerializeField] private TMP_Dropdown   _typeDropdown;
        [SerializeField] private TMP_InputField _delayField;
        [SerializeField] private TMP_InputField _durationField;
        [SerializeField] private TMP_InputField _timingPointField;
        [SerializeField] private TMP_InputField _perfectNegativeField;
        [SerializeField] private TMP_InputField _perfectPositiveField;
        [SerializeField] private TMP_InputField _goodNegativeField;
        [SerializeField] private TMP_InputField _goodPositiveField;
        [SerializeField] private TMP_InputField _positionXField;
        [SerializeField] private TMP_InputField _positionYField;
        [SerializeField] private TMP_Dropdown   _requiredDirDropdown;

        [Header("Group")]
        [SerializeField] private Button          _addButton;
        [SerializeField] private Button          _clearButton;
        [SerializeField] private Button          _playButton;
        [SerializeField] private TextMeshProUGUI _groupListText;


        private void Awake()
        {
            _typeDropdown.AddOptions(Enum.GetNames(typeof(QTE_TYPE)).ToList());
            _typeDropdown.RefreshShownValue();

            _requiredDirDropdown.AddOptions(Enum.GetNames(typeof(QTE_SWIPE_DIR)).ToList());
            _requiredDirDropdown.RefreshShownValue();

            _delayField.text           = "0";
            _durationField.text        = "3";
            _timingPointField.text     = "1.5";
            _perfectNegativeField.text = "0.2";
            _perfectPositiveField.text = "0.2";
            _goodNegativeField.text    = "0.5";
            _goodPositiveField.text    = "0.5";
            _positionXField.text       = "0.5";
            _positionYField.text       = "0.5";

            _addButton.onClick.AddListener(AddConfig);
            _clearButton.onClick.AddListener(ClearGroup);
            _playButton.onClick.AddListener(PlayGroup);

            RefreshList();
        }

        private void AddConfig()
        {
            _group.Add(new QTEConfig
            {
                type            = (QTE_TYPE)_typeDropdown.value,
                delay           = float.Parse(_delayField.text),
                duration        = float.Parse(_durationField.text),
                timingPoint     = float.Parse(_timingPointField.text),
                perfectNegative = float.Parse(_perfectNegativeField.text),
                perfectPositive = float.Parse(_perfectPositiveField.text),
                goodNegative    = float.Parse(_goodNegativeField.text),
                goodPositive    = float.Parse(_goodPositiveField.text),
                position        = new Vector2(float.Parse(_positionXField.text), float.Parse(_positionYField.text)),
                requiredDir     = (QTE_SWIPE_DIR)_requiredDirDropdown.value,
            });

            RefreshList();
        }

        private void ClearGroup()
        {
            _group.Clear();
            RefreshList();
        }

        private void PlayGroup()
        {
            if (_group.Count == 0) return;
            _runner.RunGroupAsync(_group, _viewPrefab, (i, r) => Debug.Log($"[QTE] #{i} → {r}")).Forget();
        }

        private void RefreshList()
        {
            if (_group.Count == 0)
            {
                _groupListText.text = "(empty)";
                return;
            }

            var lines = new string[_group.Count];
            for (int i = 0; i < _group.Count; i++)
            {
                QTEConfig c = _group[i];
                string dir = c.type == QTE_TYPE.SWIPE ? $" {c.requiredDir}" : "";
                lines[i] = $"[{i}] {c.type,-5} +{c.delay:F1}s  {c.duration:F1}s  ({c.position.x:F2},{c.position.y:F2}){dir}";
            }

            _groupListText.text = string.Join("\n", lines);
        }
    }
}
