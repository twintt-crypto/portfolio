using TMPro;
using UnityEngine;

/// <summary>
/// maxLines 이하면 실제 줄 수 높이만 차지하고, 초과하면 maxLines 높이 고정 + Ellipsis 처리
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(TextMeshProUGUI))]
public class AutoLineText : MonoBehaviour
{
    [SerializeField, Min(1)] private int _maxLines = 2;

    private TextMeshProUGUI _text;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
    }

#if UNITY_EDITOR
    private string _prevText;
    private int _prevMaxLines;

    private void Update()
    {
        if (Application.isPlaying) return;
        if (_text == null) _text = GetComponent<TextMeshProUGUI>();
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        if (_text.text == _prevText && _maxLines == _prevMaxLines) return;

        _prevText = _text.text;
        _prevMaxLines = _maxLines;
        Apply();
    }
#endif

    public void SetText(string value)
    {
        if (_text == null) _text = GetComponent<TextMeshProUGUI>();
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        _text.text = value;
        Apply();
    }

    private void Apply()
    {
        _text.overflowMode = TextOverflowModes.Overflow;
        _text.ForceMeshUpdate();

        int lineCount = _text.textInfo.lineCount;

        if (lineCount <= _maxLines)
        {
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _text.preferredHeight);
            _text.overflowMode = TextOverflowModes.Overflow;
        }
        else
        {
            float height = 0f;
            for (int i = 0; i < _maxLines; i++)
                height += _text.textInfo.lineInfo[i].lineHeight;

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _text.overflowMode = TextOverflowModes.Ellipsis;
            _text.ForceMeshUpdate();
        }
    }
}
