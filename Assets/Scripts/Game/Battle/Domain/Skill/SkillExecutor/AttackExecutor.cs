using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S7
{
    public class AttackExecutor : ISkillActionExecutor
    {
        public SkillActionType ActionType => SkillActionType.Attack;

        private Dictionary<AttackType, IAttackStrategy> _strategyMap = new();

        public AttackExecutor()
        {
        }

        /*public AttackExecutor(IEnumerable<UnitController> units)
        {
            var registered = new HashSet<AttackType>();

            foreach (var unit in units)
            {
                foreach (var skill in unit.skills)
                {
                    var attackType = skill.skillData.AttackType;

                    if (registered.Add(attackType))
                        Register(attackType);
                }
            }
        }

        private void Register(AttackType attackType)
        {
            IAttackStrategy attackStrategy = attackType switch
            {
                AttackType.Normal => new NormalAttackStrategy(),
                AttackType.Projectile => new ProjectileAttackStrategy(),
                AttackType.Ultimate => new ProjectileAttackStrategy(),
                _ => null
            };

            if (attackStrategy != null)
                _strategyMap.TryAdd(attackStrategy.Type, attackStrategy);
        }*/

        public SkillResult Execute(ActionContext ctx)
        {
            SkillResult result = new SkillResult();
            result.hitResults = new List<HitResult>();
            result.skillData = ctx.unitSkill.skillData;
            result.ap += result.skillData.Ap;

            var skillHits = T_SkillHitData.Get(ctx.unitSkill.skillData.TID);
            if (skillHits == null)
                return null;

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
                        //계산전 버프
                        ProcessSkillEffect.ApplyBeforeEffect(ctx, hitResult);
                        var damageResult = DamageCalculator.Calculate(ctx.caster, target, hitData);

                        hitResult.damage = damageResult.damage;
                        hitResult.isCritical = damageResult.isCritical;

                        //계산후 버프
                        ProcessSkillEffect.ApplyAfterEffect(ctx, hitResult);
                    }
                    result.hitResults.Add(hitResult);
                }
            }

            /*if (_strategyMap.TryGetValue(skill.skillData.AttackType, out var strategy) == false)
            {
                return null;                        
            }

            SkillResult skillResult = strategy.Execute(ctx);
            if (skillResult == null)
            {
                return null;
            }

            for( int i = 0; i < skill.skillData.EffectId.Count; i++)
            {
                var id = skill.skillData.EffectId[i];
                T_SkillEffectData skillEffectData = T_SkillEffectData.Get(id);
                if (skillEffectData == null)
                    continue;

                foreach(var target in ctx.targets)
                {
                    skillResult.buffResults.Add(new BuffApplyResult()
                    {
                        target = target,
                        effectData = skillEffectData,
                    });
                }                
            }
*/

            return result;
        }
    }
}

