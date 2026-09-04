using BehaviorDesigner.Runtime.Tasks.Unity.UnityLight;
using S7;
using System;

public static class EffectCalculator
{
    public static int CalculateValue(
        UnitController caster,
        UnitController target,
        T_SkillEffectData effect)
    {
        if (effect == null)
            return 0;

        int RATE_BASE = T_GlobalValueData.Get(GlobalValueType.RateBase).ValueInt;

        int baseValue = GetBaseStatValue(caster, target, effect);
        long value = 0;
        if (effect.EffectValueType == EffectValueType.Flat)
        {
            value = baseValue + effect.EffectValue;
        }
        else
        {
            value = baseValue * effect.EffectValue / RATE_BASE;
        }

        value = ApplyFinalModifier(caster, target, effect, value);

        return (int)Math.Max(0, value);
    }

    /* public static int CalculateValueRate(
         UnitController caster,
         UnitController target,
         T_SkillEffectData effect)
     {
         if (effect == null)
             return 0;

         int baseValue = GetBaseStatValue(caster, target, effect);




         / * // 1. 기준 스탯 가져오기
         baseValue = GetBaseStatValue(caster, target, effect);

         // 2. 고정값 추가
         baseValue += effect.FlatValue;

         // 3. 퍼센트 적용
         baseValue *= (1f + effect.PercentValue);

         // 4. 버프/디버프 반영
         baseValue = ApplyBuffModifier(caster, target, effect, baseValue);

         // 5. 랜덤값
         baseValue = ApplyRandom(effect, baseValue);* /

         return baseValue;
     }*/

    private static int GetBaseStatValue(
    UnitController caster,
    UnitController target,
    T_SkillEffectData effect)
    {
        switch (effect.EffectType)
        {
            case EffectType.Attack:
                return caster.data._stat.GetStat(effect.EffectType);

            case EffectType.Defense:
                return target.data._stat.GetStat(effect.EffectType);

            case EffectType.Speed:
                return target.data._stat.GetStat(effect.EffectType);
            default:
                return 0;
        }
    }

    private static long ApplyFinalModifier(
    UnitController caster,
    UnitController target,
    T_SkillEffectData effect,
    long value)
    {
        if (effect == null)
            return value;

        int RATE_BASE = T_GlobalValueData.Get(GlobalValueType.RateBase).ValueInt;

        int damageIncrease =
            caster?.BuffManager?.GetStat(EffectType.DamageIncrease, EffectValueType.Percent) ?? 0;

        int damageReduction =
            target?.BuffManager?.GetStat(EffectType.DamageReduction, EffectValueType.Percent) ?? 0;

        value = value * (RATE_BASE + damageIncrease) / RATE_BASE;
        value = value * (RATE_BASE - damageReduction) / RATE_BASE;

        if (value < 0)
            value = 0;

        if (value > int.MaxValue)
            value = int.MaxValue;

        return value;
    }
}