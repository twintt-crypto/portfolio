using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [SerializeField] private DialogueView _view;

        public bool IsPlaying { get; private set; } = false;

        public UniTask PlayAsync(DialogueSet set) => PlayAsync(set, null);

        public async UniTask PlayAsync(DialogueSet set, Action<int> onChoiceSelected)
        {
            if (IsPlaying) return;
            IsPlaying = true;

            await _view.PlayDialogueSetAsync(set, onChoiceSelected);

            IsPlaying = false;
        }

#if UNITY_EDITOR
        [Header("Test")]
        [SerializeField] private int _testDialogueId;

        [ContextMenu("Test Play Dialogue")]
        private void TestPlayDialogue()
        {
            var set = DialogueSet.GetRowById(_testDialogueId);
            if (set == null)
            {
                Debug.LogWarning($"[DialogueManager] DialogueSet not found for id: {_testDialogueId}");
                return;
            }
            PlayAsync(set).Forget();
        }
#endif
    }
}
