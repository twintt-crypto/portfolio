namespace S7
{
    public class SilenceBuff : BaseBuff
    {
        public SilenceBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data) { }

        public override bool CanUseSkill() => false;
    }
}
