using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class FadeOutNode : BasePresentationNode
    {
        public FadeOutNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            await UIManager.Instance.FadeOutAsync();
        }
    }
}
