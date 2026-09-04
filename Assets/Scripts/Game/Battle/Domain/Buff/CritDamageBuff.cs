namespace S7
{
    public class CritDamageBuff : BaseBuff
    {
        public CritDamageBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data) { }

        public override float ModifyCritDamage(float value)
        {
            return value + (_effectData.EffectValue * Stack / 100f);
        }
    }
}
