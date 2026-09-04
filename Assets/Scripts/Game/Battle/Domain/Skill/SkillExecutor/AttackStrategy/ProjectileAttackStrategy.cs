using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace S7
{
    public class ProjectileAttackStrategy : IAttackStrategy
    {
        public AttackType Type => AttackType.Projectile;
        public SkillResult Execute(ActionContext ctx)
        {
            SkillResult result = new SkillResult();
            result.hitResults = new List<HitResult>();

            var skillHits = T_SkillHitData.Get(ctx.unitSkill.skillData.TID);
            if (skillHits == null)
                return result;

            for (int hitIndex = 0; hitIndex < skillHits.Count; hitIndex++)
            {
                var hitData = skillHits[hitIndex];

                // 그 다음 타겟
                foreach (var target in ctx.targets)
                {
                    var hitResult = new HitResult(ctx.caster, target);
                    hitResult.hitIndex = hitIndex;   // 중요

                    hitResult.isHit = DamageCalculator.RollHit(ctx.caster, target);

                    if (hitResult.isHit)
                    {
                        var damageResult =
                            DamageCalculator.Calculate(ctx.caster, target, hitData);

                        hitResult.damage = damageResult.damage;
                        hitResult.isCritical = damageResult.isCritical;
                    }

                    result.hitResults.Add(hitResult);
                }
            }

            return result;
        }
    }
}
