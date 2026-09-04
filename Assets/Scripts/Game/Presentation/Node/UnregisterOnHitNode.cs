using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class UnregisterOnHitNode : BasePresentationNode
    {
        public UnregisterOnHitNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            var receiver = ctx.caster.animationEventReceiver;
            if (receiver != null)
            {
                receiver.Unregister(AnimationEventType.OnHit);
            }                        

            return UniTask.CompletedTask;
        }
    }
}
