using UnityEngine;

namespace S7
{
    public class DamageReductionBuff : BaseBuff
    {
        public DamageReductionBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data) { }

        public override float ModifyDamageTaken(float value)
        {
            return value * (1f - (_effectData.EffectValue * Stack / 100f));
        }
    }
}

