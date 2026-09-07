using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;

namespace S7
{
    public class PlayTimelineNode : BasePresentationNode
    {
        private readonly string _timelineName;

        public PlayTimelineNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
            _timelineName = presentationNodeData.param1;
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            if (ctx == null)
                return;

            if (string.IsNullOrEmpty(_data.param1))
            {
                Debug.LogWarning("PlayTimelineNode : _data.param1 이 비어있습니다.");
                return;
            }

            await UIManager.Instance.FadeOutAsync();

            var director = await ctx.GetTimelineAsync(_data.param1);
            if (director == null)
            {
                Debug.LogWarning($"PlayTimelineNode : 타임라인 로드 실패. key={_data.param1}");
                return;
            }

            UIManager.Instance.FadeInAsync().Forget();

            token.ThrowIfCancellationRequested();

            bool isStopped = false;

            void OnStopped(PlayableDirector d)
            {
                if (d == director)
                    isStopped = true;
            }

            director.stopped += OnStopped;

            try
            {
                director.time = 0;
                director.Evaluate();
                director.Play();

                // 끝날 때까지 대기
                await UniTask.WaitUntil(
                    () => isStopped || director.state != PlayState.Playing,
                    cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                // 취소 시 타임라인 정지
                director.Stop();
            }
            finally
            {
                await UIManager.Instance.FadeOutAsync();

                director.stopped -= OnStopped;

                // 필요하면 여기서 해제
                ctx.ReleaseTimeline(director);

                UIManager.Instance.FadeInAsync().Forget();
            }
        }
    }
}
