using System;

namespace S7
{
    public class ShieldBuff : BaseBuff
    {
        private int _shieldValue;

        public ShieldBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data)
        {
            _shieldValue = data.EffectValue;
        }

        public int AbsorbDamage(int damage)
        {
            int absorbed = Math.Min(damage, _shieldValue);
            _shieldValue -= absorbed;
            return damage - absorbed;
        }
    }
}
