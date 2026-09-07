using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class RegisterOnHitNode : BasePresentationNode
    {
        public RegisterOnHitNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            var receiver = ctx.caster.animationEventReceiver;

            void OnHit(AnimationEventData eventData)
            {
                ctx.onHit?.Invoke(eventData.intParam);
            }
            
            receiver.Register(AnimationEventType.OnHit, OnHit);
            return UniTask.CompletedTask;
        }
    }
}
