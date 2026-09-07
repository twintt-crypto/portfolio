using System;
using Cysharp.Threading.Tasks;
using Game.QTE;
using UnityEngine;

namespace UI.QTE
{
    public class QTEAnimatorView : MonoBehaviour, IQTEView
    {
        private const float CrossFadeDuration = 0.1f;
        private static readonly int ProgressStateHash  = Animator.StringToHash("Progress");
        private static readonly int SuccessStateHash     = Animator.StringToHash("Success");
        private static readonly int FailStateHash    = Animator.StringToHash("Fail");

        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem[] _particles;
        [SerializeField] private float _resultDuration = 1f;

        // TODO: remove temp
        [SerializeField] private TMPro.TextMeshProUGUI _resultText;
        
        public void Setup(QTEConfig config)
        {
            if(_animator == null) _animator = GetComponent<Animator>();
            if(_resultText != null) _resultText.text = config.type.ToString();

            float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
            float speed = animLength > 0f ? animLength / config.duration : 1f;
            SetSpeed(speed);

            _animator.Play(ProgressStateHash);
        }

        public async UniTask ShowResultAsync(QTE_RESULT result)
        {
            // _animator.SetInteger(ResultIndexHash, (int)result);
            // _animator.SetTrigger(ShowResultHash);

            SetSpeed(1);

            switch (result)
            {
                case QTE_RESULT.PERFECT:
                case QTE_RESULT.GOOD:
                    _animator.Play(SuccessStateHash, 0, 0f);
                    break;
                case QTE_RESULT.FAIL:
                case QTE_RESULT.MISS:
                    _animator.Play(FailStateHash, 0, 0f);
                    break;
            }

            await UniTask.WaitForEndOfFrame();
            
            if(_resultText != null) _resultText.text = result.ToString();

            float currentAnimTime = _animator.GetCurrentAnimatorStateInfo(0).length;
            
            await UniTask.Delay(TimeSpan.FromSeconds(currentAnimTime));
        }

        private void SetSpeed(float speed)
        {
            _animator.speed = speed;
            
            if(_particles == null) return;
            for (int i = 0; i < _particles.Length; i++)
            {
                ParticleSystem.MainModule main = _particles[i].main;
                main.simulationSpeed = speed;
            }
        }
    }
}
