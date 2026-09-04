using S7;

public class AttackBuff : BaseBuff
{
    public AttackBuff(UnitController caster, UnitController target, T_SkillEffectData data) : base(caster,target,data)
    {
    }

    public override int ModifyAttack(int value)
    {
        int percent = _effectData.EffectValue * Stack;
        return value + (value * percent / 100);
    }
}