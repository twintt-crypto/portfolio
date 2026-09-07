using System;

namespace S7
{
    public class SpeedBuff : BaseBuff
    {
        public SpeedBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data) { }

        public override int ModifySpeed(int value)
        {
            return value + (_effectData.EffectValue * Stack);
        }
    }
}
