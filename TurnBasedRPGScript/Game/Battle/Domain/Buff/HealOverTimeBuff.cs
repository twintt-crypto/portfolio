namespace S7
{
    public class HealOverTimeBuff : BaseBuff
    {
        public HealOverTimeBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
            : base(caster, owner, data) { }

        public override void OnTurnStart(UnitController owner)
        {
            int heal = _effectData.EffectValue * Stack;
            owner.Heal(heal);
        }
    }
}
