using UnityEngine;

namespace S7
{
    public class DamageIncreaseBuff : BaseBuff
    {
        public DamageIncreaseBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data) { }

        public override float ModifyDamageDealt(float value)
        {
            return value * (1f + (_effectData.EffectValue * Stack / 100f));
        }
    }
}

