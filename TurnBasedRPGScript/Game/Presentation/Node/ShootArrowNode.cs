using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class ShootArrowNode : BasePresentationNode
    {
        public ShootArrowNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            var arrow = ctx.runtimeArrow;
            arrow.transform.SetParent(null);

            var projectile = arrow.AddComponent<Projectile>();
            if (int.TryParse(_data.param1, out int index) == false)
            {
                return;
            }

            projectile.Init(ctx.targets[0].GetHitPoint(), ctx.runtimeArrowSpeed, index, ctx.onHit);
            await UniTask.Yield();
        }
    }
}
