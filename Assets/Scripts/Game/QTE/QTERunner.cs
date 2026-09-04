using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UI.QTE;
using UnityEngine;

namespace Game.QTE
{
    public class QTERunner : MonoBehaviour
    {
        [SerializeField] private Transform qteViewWrap;
        [SerializeField] private GameObject defaultViewPrefab;

        private IReadOnlyList<QTEConfig> _currentConfigs;

        public async UniTask<QTE_RESULT[]> RunGroupAsync(
            IReadOnlyList<QTEConfig> configs,
            GameObject viewPrefab = null,
            Action<int, QTE_RESULT> onEachComplete = null,
            CancellationToken ct = default)
        {
            if (_currentConfigs != null)
            {
                Debug.LogError("[QTERunner] QTE is already playing");
                return null;
            }

            _currentConfigs = configs;

            try
            {
                UniTask<QTE_RESULT>[] tasks = new UniTask<QTE_RESULT>[configs.Count];
                for (int i = 0; i < configs.Count; i++)
                {
                    int index = i;
                    tasks[i] = RunSingleAsync(index, configs[index], viewPrefab, onEachComplete, ct);
                }
                return await UniTask.WhenAll(tasks);
            }
            finally
            {
                _currentConfigs = null;
            }
        }

        private async UniTask<QTE_RESULT> RunSingleAsync(
            int index,
            QTEConfig config,
            GameObject viewPrefab,
            Action<int, QTE_RESULT> onEachComplete,
            CancellationToken ct)
        {
            if (config.delay > 0f) await UniTask.Delay(TimeSpan.FromSeconds(config.delay), cancellationToken: ct);

            GameObject viewObject = Instantiate(viewPrefab != null ? viewPrefab : defaultViewPrefab, qteViewWrap);

            try
            {
                RectTransform rect = viewObject.transform as RectTransform;
                if (rect != null)
                {
                    rect.anchorMin = config.position;
                    rect.anchorMax = config.position;
                    rect.anchoredPosition = Vector2.zero;
                }

                IQTEView view = viewObject.GetComponent<IQTEView>();
                if (view == null) view = viewObject.AddComponent<QTEAnimatorView>();
                view.Setup(config);

                QTEJudge judge = QTEJudgeFactory.Create(config);
                AttachInput(viewObject, config, judge);

                float elapsed = 0f;
                while (elapsed < config.duration && !judge.IsComplete)
                {
                    await UniTask.NextFrame(cancellationToken: ct);
                    elapsed += Time.deltaTime;
                }
                
                // 
                float overTime = config.timingPoint + config.goodNegative;
                if (overTime > config.duration)
                {
                    while (elapsed < overTime && !judge.IsComplete)
                    {
                        await UniTask.NextFrame(cancellationToken: ct);
                        elapsed += Time.deltaTime;
                    }
                }
                
                

                QTE_RESULT result = judge.Judge();

                onEachComplete?.Invoke(index, result);
                await view.ShowResultAsync(result);
                return result;
            }
            finally
            {
                Destroy(viewObject);
            }
        }

        private void AttachInput(GameObject viewObject, QTEConfig config, QTEJudge judge)
        {
            switch (config.type)
            {
                case QTE_TYPE.TAP:
                case QTE_TYPE.MASH:
                    QTETapUI tapUI = viewObject.AddComponent<QTETapUI>();
                    tapUI.OnTap += judge.Feed;
                    return ;
                case QTE_TYPE.SWIPE:
                    QTESwipeUI swipeUI = viewObject.AddComponent<QTESwipeUI>();
                    swipeUI.OnSwipe += dir =>
                    {
                        if (dir == config.requiredDir) judge.Feed();
                        else judge.ForceComplete();
                    };
                    return ;
                case QTE_TYPE.RELEASE:
                    QTEReleaseInput releaseInput = viewObject.AddComponent<QTEReleaseInput>();
                    releaseInput.OnRelease += judge.Feed;
                    return ;
                default:
                    Debug.LogError($"[QTERunner] 지원하지 않는 QTE_TYPE: {config.type}");
                    return ;
            }
        }
    }
}
