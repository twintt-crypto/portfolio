using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject _characterImagePanel;
        [SerializeField] private Image _characterImage;
        [SerializeField] private TextMeshProUGUI _speakerNameText;
        [SerializeField] private TextMeshProUGUI _bodyText;
        [SerializeField] private GameObject _nextIndicator;
        [SerializeField] private Transform _choiceContainer;
        [SerializeField] private UIDialogueChoiceButton _choiceButtonPrefab;
        [SerializeField] private Button _clickArea;

        private bool _isTyping;
        private bool _skipRequested;
        private bool _nextRequested;
        private int _choiceResult;
        private bool _choiceSelected;

        // cancellationToken 은 전체 강제 종료용도
        private CancellationTokenSource _cts;
        private readonly List<UIDialogueChoiceButton> _choiceButtons = new();

        private void Awake()
        {
            _clickArea.onClick.AddListener(OnScreenClicked);
        }

        public virtual void Show() => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);

        public void ForceStop() => _cts?.Cancel(); // 대화 전체 강제 종료

        public async UniTask PlayDialogueSetAsync(DialogueSet set, Action<int> onChoiceSelected)
        {
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            Show();

            int currentId = set.startId;
            
            // 순차적으로 실행 + 클릭해서 문장 전체 보이기
            while (currentId != -1 && !_cts.Token.IsCancellationRequested)
            {
                if (!set.entryDic.TryGetValue(currentId, out var entry))
                {
                    Debug.LogWarning($"[DialogueView] Entry not found: {currentId}");
                    break;
                }
                currentId = await ShowEntryAsync(entry, onChoiceSelected, _cts.Token);
            }
            
            Hide();
        }

        // 현재 entry를 표시하고 다음으로 이동할 ID를 반환
        private async UniTask<int> ShowEntryAsync(
            DialogueEntry entry,
            Action<int> onChoiceSelected,
            CancellationToken ct)
        {
            _speakerNameText.text = entry.speakerName;

            _characterImagePanel.SetActive(entry.characterImage != null);
            if (entry.characterImage != null) _characterImage.sprite = entry.characterImage;

            _nextIndicator.SetActive(false);
            await RunTypingAsync(entry.bodyText, entry.typingSpeed, ct);

            if (ct.IsCancellationRequested) return -1;

            if (entry.type == DIALOGUE_TYPE.QUESTION && entry.choices?.Count > 0)
            {
                return await ShowChoicesAsync(entry.choices, onChoiceSelected, ct);
            }

            _nextIndicator.SetActive(true);
            await WaitForNextAsync(ct);
            _nextIndicator.SetActive(false);
            return entry.nextId;
        }

        private async UniTask RunTypingAsync(string fullText, float speed, CancellationToken ct)
        {
            _bodyText.text = fullText;
            _bodyText.maxVisibleCharacters = 0;
            _isTyping = true;
            _skipRequested = false;

            for (int i = 0; i <= fullText.Length; i++)
            {
                if (_skipRequested || ct.IsCancellationRequested)
                {
                    _bodyText.maxVisibleCharacters = fullText.Length;
                    break;
                }
                _bodyText.maxVisibleCharacters = i;
                await UniTask.Delay(TimeSpan.FromSeconds(speed), ignoreTimeScale: true, cancellationToken: ct)
                    .SuppressCancellationThrow(); // 예외 처리 안하게 하는 용도 
            }
            
            _isTyping = false;
            _skipRequested = false;
        }

        private async UniTask WaitForNextAsync(CancellationToken ct)
        {
            _nextRequested = false;
            
            while (!_nextRequested && !ct.IsCancellationRequested)
            {
                await UniTask.NextFrame(cancellationToken: ct).SuppressCancellationThrow();
            }
        }

        private void OnScreenClicked()
        {
            if (_isTyping) _skipRequested = true;
            else _nextRequested = true;
        }

        // 선택지를 표시하고 선택된 choice의 nextId를 반환
        private async UniTask<int> ShowChoicesAsync(
            List<DialogueChoice> choices,
            Action<int> onChoiceSelected,
            CancellationToken ct)
        {
            _clickArea.gameObject.SetActive(false);
            _choiceSelected = false;
            _choiceResult = -1;

            for (int i = 0; i < choices.Count; i++)
            {
                int capturedIndex = i;
                var btn = Instantiate(_choiceButtonPrefab, _choiceContainer);
                btn.Setup(choices[i].labelText, () =>
                {
                    _choiceResult = capturedIndex;
                    _choiceSelected = true;
                });
                _choiceButtons.Add(btn);
            }

            while (!_choiceSelected && !ct.IsCancellationRequested)
                await UniTask.NextFrame(cancellationToken: ct).SuppressCancellationThrow();

            ClearChoices();
            _clickArea.gameObject.SetActive(true);

            if (ct.IsCancellationRequested) return -1;

            onChoiceSelected?.Invoke(_choiceResult);
            return choices[_choiceResult].nextId;
        }

        private void ClearChoices()
        {
            foreach (var btn in _choiceButtons) Destroy(btn.gameObject);
            _choiceButtons.Clear();
        }
    }
}
