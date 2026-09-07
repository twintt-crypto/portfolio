using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.QTE;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QTE
{
    public class QTEProgressImageView : MonoBehaviour, IQTEView
    {
        [SerializeField] private Image _progressImage;

        [Header("Swipe Arrow")]
        [SerializeField] private Image _arrowImage;
        [SerializeField] private Sprite[] _arrowSprites; // LEFT/RIGHT/UP/DOWN 순

        [Header("Result")]
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private CanvasGroup _resultGroup;
        [SerializeField] private float _resultDuration = 1f;

        public void Setup(QTEConfig config)
        {
            _progressImage.fillAmount = 0f;
        }

        public void SetProgress(float t)
        {
            _progressImage.fillAmount = t;
        }

        public async UniTask ShowResultAsync(QTE_RESULT result)
        {
            _resultText.text = result.ToString();
            _resultGroup.alpha = 1f;

            await _resultGroup.DOFade(0f, _resultDuration).AsyncWaitForCompletion().AsUniTask();
        }

        private void OnDestroy()
        {
            _resultGroup.DOKill();
        }
    }
}
