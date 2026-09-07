using BehaviorDesigner.Runtime;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class MoveNode : BasePresentationNode
    {

        public MoveNode(PresentationNodeData presentationData) : base(presentationData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            var casterView = ctx.caster;
            var targetView = ctx.targets[0];

            Vector3 movePos = ctx.caster.OriginPos;

            if (_data.param2 == "Target")
            {
                movePos = CommonUtil.GetAttackPosition(
                casterView.transform,
                targetView.Collider, 2.0f);
            }
            else if (_data.param2 == "Origin")
            {
                movePos = ctx.caster.OriginPos;
            }

            if (int.TryParse(_data.param1, out int time) == true)
            {

            }

            await ctx.caster.MoveToAsync(movePos, time);
        }
    }
}
