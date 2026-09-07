namespace S7
{
    public class StunBuff : BaseBuff
    {
        public StunBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data) { }

        public override bool CanAct() => false;
        public override bool CanUseSkill() => false;
    }
}
