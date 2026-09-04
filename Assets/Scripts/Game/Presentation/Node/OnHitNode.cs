using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class OnHitNode : BasePresentationNode
    {
        public OnHitNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            var receiver = ctx.caster.animationEventReceiver;
            var animator = ctx.caster.Animator;

            var tcs = new UniTaskCompletionSource();

            void OnHit(AnimationEventData eventData)
            {
                ctx.onHit?.Invoke(eventData.intParam);

                tcs.TrySetResult();
            }

            receiver.Register(AnimationEventType.OnHit, OnHit);

            try
            {
                // onHit까지 대기
                await tcs.Task.AttachExternalCancellation(token);

                // 현재 애니 끝까지 대기
                await UniTask.WaitUntil(() =>
                {
                    var state = animator.GetCurrentAnimatorStateInfo(0);

                    // transition 중이면 아직 끝 아님
                    if (animator.IsInTransition(0))
                        return false;

                    return state.normalizedTime >= 1f;
                }, cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                receiver.Unregister(AnimationEventType.OnHit, OnHit);
            }
        }
    }
}
