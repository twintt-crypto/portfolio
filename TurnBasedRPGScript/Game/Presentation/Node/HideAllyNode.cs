using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class HideAllyNode : BasePresentationNode
    {
        public HideAllyNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            foreach (var unit in ctx.ally)
            {
                unit.SetActive(false);
            }

            return UniTask.CompletedTask;
        }
    }
}
