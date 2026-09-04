using S7;
using System;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class BaseBuff : IBuff
{
    protected UnitController _caster;
    protected UnitController _owner;

    protected readonly T_SkillEffectData _effectData;

    public int BuffId => _effectData.TID;
    public string Name => _effectData.Name;
    public EffectType EffectType => _effectData.EffectType;
    public EffectValueType EffectValueType => _effectData.EffectValueType;

    public int RemainingTurn { get; protected set; }
    public int Stack { get; protected set; }

    public virtual int GetValue()
    {
        return _effectData.EffectValue * Stack;
    }

    protected BaseBuff(UnitController caster, UnitController owner, T_SkillEffectData effectData)
    {
        _caster = caster;
        _owner = owner;

        _effectData = effectData;
        RemainingTurn = effectData.DurationTurn;
        Stack = 1;
    }

    public virtual void OnApply(UnitController owner)
    {
    }

    public virtual void OnRemove(UnitController owner)
    {
    }

    public virtual void OnTurnStart(UnitController owner)
    {
    }

    public virtual void OnTurnEnd(UnitController owner)
    {
    }

    public virtual void AddStack()
    {
        int maxStack = Math.Max(1, _effectData.StackMax);
        Stack = Math.Min(Stack + 1, maxStack);
    }

    public virtual void RefreshDuration()
    {
        RemainingTurn = _effectData.DurationTurn;
    }

    public virtual void DecreaseDuration()
    {
        RemainingTurn--;
    }

    public virtual bool IsExpired()
    {
        return RemainingTurn <= 0;
    }

    public virtual int ModifyAttack(int value)
    {
        return value;
    }

    public virtual int ModifyDefense(int value)
    {
        return value;
    }

    public virtual int ModifySpeed(int value)
    {
        return value;
    }

    public virtual float ModifyCritRate(float value)
    {
        return value;
    }

    public virtual float ModifyCritDamage(float value)
    {
        return value;
    }

    public virtual float ModifyDamageDealt(float value)
    {
        return value;
    }

    public virtual float ModifyDamageTaken(float value)
    {
        return value;
    }

    public virtual bool CanAct()
    {
        return true;
    }

    public virtual bool CanUseSkill()
    {
        return true;
    }
}
