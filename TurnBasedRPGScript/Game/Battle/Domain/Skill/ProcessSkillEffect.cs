using BehaviorDesigner.Runtime.Tasks.Unity.UnityLight;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;
using static UnityEngine.Rendering.DebugUI;

namespace S7
{
    public static class ProcessSkillEffect
    {        
        //데미지 계산전에 넣어준다
        public static void ApplyBeforeEffect(ActionContext ctx, HitResult hitResult)
        {
            var skill = ctx.unitSkill.skillData;
            foreach (var effectId in skill.EffectId)
            {
                var effectData = T_SkillEffectData.Get(effectId);
                if (effectData == null)
                {
                    continue;
                }

                if (!effectData.IsInstant)
                {
                    continue;
                }

                foreach (var target in ctx.targets)
                {
                    target.BuffManager.AddBuff(ctx.caster, effectData);
                }                
            }
        }

        //데미지 계산후
        public static void ApplyAfterEffect(ActionContext ctx, HitResult hitResult)
        {
            var skill = ctx.unitSkill.skillData;

            foreach (var effectId in skill.EffectId)
            {
                var effectData = T_SkillEffectData.Get(effectId);
                if (effectData == null)
                {
                    continue;
                }

                if (effectData.IsInstant)
                    continue;

                foreach (var target in ctx.targets)
                {
                    target.BuffManager.AddBuff(ctx.caster, effectData);
                }
            }           
        }
    }
}
