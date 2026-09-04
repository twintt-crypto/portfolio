using UnityEngine;

namespace S7
{
    public class LifeStealBuff : BaseBuff
    {
        public LifeStealBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data) { }

        public float GetLifeStealRate()
        {
            return _effectData.EffectValue * Stack / 100f;
        }
    }
}

