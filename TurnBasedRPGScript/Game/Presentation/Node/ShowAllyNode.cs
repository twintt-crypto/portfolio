using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class ShowAllyNode : BasePresentationNode
    {
        public ShowAllyNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            foreach(var unit in ctx.ally)
            {
                unit.SetActive(true);
            }            

            return UniTask.CompletedTask;
        }
    }
}
