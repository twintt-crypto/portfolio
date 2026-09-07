using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace S7
{
    public class SpawnArrowNode : BasePresentationNode
    {
        private GameObject _arrow;

        public SpawnArrowNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            var caster = ctx.caster;

            T_ProjectileData projectileData = T_ProjectileData.Get(ctx.unitSkill.skillData.ProjectileId);

            ctx.runtimeArrow = await ResourceManager.NewAsync(projectileData.Prefab, caster.GetSocket("ArrowSocket"), true);
            ctx.runtimeArrowSpeed = projectileData.Speed;
        }
    }
}
