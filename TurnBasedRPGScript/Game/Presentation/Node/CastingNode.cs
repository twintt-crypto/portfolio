using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class CastingNode : BasePresentationNode
    {
        public CastingNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            if (ctx.caster == null)
                throw new InvalidOperationException("CastingNode failed. ctx.caster is null.");

            await UniTask.CompletedTask;
        }
    }
}
