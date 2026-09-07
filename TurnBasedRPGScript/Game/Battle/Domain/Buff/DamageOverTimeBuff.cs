using UnityEngine;

namespace S7
{
    public class DamageOverTimeBuff : BaseBuff
    {
        public DamageOverTimeBuff(UnitController caster, UnitController owner, T_SkillEffectData data)
        : base(caster, owner, data) { }

        public override void OnTurnStart(UnitController owner)
        {
            int damage = _effectData.EffectValue * Stack;
            owner.ApplyDamage(damage);
        }
    }
}

