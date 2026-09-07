
using static MagicaCloth2.InertiaConstraint;

namespace S7
{
    public static class BuffFactory
    {
        public static IBuff Create(UnitController caster, UnitController owner, T_SkillEffectData effectData)
        {
            switch (effectData.EffectType)
            {
                // case EffectType.DefensePercent:
                //     return new DefensePercentBuff(caster, owner, effectData);
                //
                // case EffectType.DefenseFlat:
                //     return new DefenseFlatBuff(caster, owner, effectData);
                //
                // case EffectType.CritRate:
                //     return new CritRateBuff(caster, owner, effectData);
                //
                // case EffectType.HealOverTime:
                //     return new HealOverTimeBuff(caster, owner, effectData);
                //
                // case EffectType.Shield:
                //     return new ShieldBuff(caster, owner, effectData);
                //
                // case EffectType.SpeedPercent:
                //     return new SpeedPercentBuff(caster, owner, effectData);
                //
                // case EffectType.SpeedFlat:
                //     return new SpeedFlatBuff(caster, owner, effectData);
                //
                // case EffectType.CritDamage:
                //     return new CritDamageBuff(caster, owner, effectData);
                //
                // case EffectType.DamageOverTime:
                //     return new DamageOverTimeBuff(caster, owner, effectData);
                //
                // case EffectType.DamageIncrease:
                //     return new DamageIncreaseBuff(caster, owner, effectData);
                //
                // case EffectType.DamageReduction:
                //     return new DamageReductionBuff(caster, owner, effectData);
                //
                // case EffectType.LifeSteal:
                //     return new LifeStealBuff(caster, owner, effectData);
                //
                // case EffectType.Stun:
                //     return new StunBuff(caster, owner, effectData);
                //
                // case EffectType.Silence:
                //     return new SilenceBuff(caster, owner, effectData);
                //
                // case EffectType.Taunt:
                //     return new TauntBuff(caster, owner, effectData);
                //
                // case EffectType.Freeze:
                //     return new FreezeBuff(caster, owner, effectData);
            }

            return null;
        }        
    }
}
