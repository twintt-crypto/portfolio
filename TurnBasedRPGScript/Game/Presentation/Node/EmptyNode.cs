using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class EmptyNode : BasePresentationNode
    {
        public EmptyNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            await UniTask.CompletedTask;
        }
    }
}
