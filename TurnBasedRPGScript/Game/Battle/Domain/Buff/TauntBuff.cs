namespace S7
{
    public class TauntBuff : BaseBuff
    {
        public TauntBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data) { }

        public UnitController GetTauntTarget()
        {
            return _caster;
        }
    }
}
