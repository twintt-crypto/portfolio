using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class WaitPlayAnimationNode : BasePresentationNode
    {
        private readonly string _anim;
        private readonly int _animHash;

        public WaitPlayAnimationNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
            _anim = presentationNodeData.param1;
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {            
            await ctx.caster.PlayAnimationAsync(Animator.StringToHash(_anim), token);
        }
    }
}
