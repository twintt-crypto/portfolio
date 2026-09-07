using Cysharp.Threading.Tasks;
using System.Threading;

namespace S7
{
    public class BranchNode : BasePresentationNode
    {
        public RuntimeNode trueNode;
        public RuntimeNode falseNode;

        public BranchNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            await UniTask.CompletedTask;
        }

        public RuntimeNode GetNext(PresentationContext context)
        {
            bool result = context.EvaluateCondition(
                _data.param1,
                _data.param2,
                _data.param3);

            return result ? trueNode : falseNode;
        }
    }
}
