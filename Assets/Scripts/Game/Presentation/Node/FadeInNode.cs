using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class FadeInNode : BasePresentationNode
    {
        public FadeInNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            UIManager.Instance.FadeInAsync().Forget();
            await UniTask.CompletedTask;
        }
    }
}
