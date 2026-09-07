using Cysharp.Threading.Tasks;
using System.Threading;

namespace S7
{
    public class ProjectileAttackNode : BasePresentationNode
    {
        private readonly float _time;

        public ProjectileAttackNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
            _time = float.Parse(presentationNodeData.param1);
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            //ctx.caster
        }
    }
}
