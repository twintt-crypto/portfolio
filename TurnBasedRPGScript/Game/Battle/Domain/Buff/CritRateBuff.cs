namespace S7
{ 
    public class CritRateBuff : BaseBuff
    {
        public CritRateBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data) { }

        public override float ModifyCritRate(float value)
        {
            return value + (_effectData.EffectValue * Stack / 100f);
        }
    }
}
