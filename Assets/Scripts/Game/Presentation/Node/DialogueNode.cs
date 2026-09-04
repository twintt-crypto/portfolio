using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class DialogueNode : BasePresentationNode
    {
        public DialogueNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {

        }
    }
}
