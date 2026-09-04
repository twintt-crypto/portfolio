using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;

namespace S7
{
    public class BuffManager
    {
        private readonly UnitController _owner;
        private readonly List<IBuff> _buffs = new();

        public IReadOnlyList<IBuff> Buffs => _buffs;

        public BuffManager(UnitController owner)
        {
            _owner = owner;
        }

        public void AddBuff(UnitController caster, T_SkillEffectData effectData)
        {
            if (caster == null)
                return;

            if (effectData == null)
                return;            

            // 있으면
            IBuff existing = _buffs.FirstOrDefault(x => x.BuffId == effectData.TID);
            if (existing != null)
            {
                if (effectData.IsStackable)
                {
                    existing.AddStack();
                }

                existing.RefreshDuration();
                return;
            }

            // 신규
            IBuff newBuff = BuffFactory.Create(caster, _owner, effectData);
            if(newBuff != null)
            {
                _buffs.Add(newBuff);
                newBuff.OnApply(_owner);
            }            
        }

        public void RemoveBuff(int buffId)
        {
            for (int i = _buffs.Count - 1; i >= 0; i--)
            {
                if (_buffs[i].BuffId != buffId)
                    continue;

                _buffs[i].OnRemove(_owner);
                _buffs.RemoveAt(i);
            }
        }

        public void RemoveDebuffs()
        {
            for (int i = _buffs.Count - 1; i >= 0; i--)
            {
                /*if (!_buffs[i].IsDebuff)
                    continue;*/

                _buffs[i].OnRemove(_owner);
                _buffs.RemoveAt(i);
            }
        }

        public void RemoveAllBuffs()
        {
            for (int i = _buffs.Count - 1; i >= 0; i--)
            {
                _buffs[i].OnRemove(_owner);
                _buffs.RemoveAt(i);
            }
        }

        public bool HasBuff(int buffId)
        {
            return _buffs.Any(x => x.BuffId == buffId);
        }

        /*public bool HasDebuff()
        {
            return _buffs.Any(x => x.IsDebuff);
        }*/

        public void OnTurnStart()
        {
            for (int i = 0; i < _buffs.Count; i++)
            {
                _buffs[i].OnTurnStart(_owner);
            }
        }

        public void OnTurnEnd()
        {
            for (int i = 0; i < _buffs.Count; i++)
            {
                _buffs[i].OnTurnEnd(_owner);
            }

            for (int i = _buffs.Count - 1; i >= 0; i--)
            {
                _buffs[i].DecreaseDuration();

                if (_buffs[i].IsExpired())
                {
                    _buffs[i].OnRemove(_owner);
                    _buffs.RemoveAt(i);
                }
            }
        }

        public int ApplyAttackModifiers(int baseValue)
        {
            int value = baseValue;

            for (int i = 0; i < _buffs.Count; i++)
            {
                value = _buffs[i].ModifyAttack(value);
            }

            return value;
        }

        public int ApplyDefenseModifiers(int baseValue)
        {
            int value = baseValue;

            for (int i = 0; i < _buffs.Count; i++)
            {
                value = _buffs[i].ModifyDefense(value);
            }

            return value;
        }

        public int ApplySpeedModifiers(int baseValue)
        {
            int value = baseValue;

            for (int i = 0; i < _buffs.Count; i++)
            {
                value = _buffs[i].ModifySpeed(value);
            }

            return value;
        }

        public float ApplyCritRateModifiers(float baseValue)
        {
            float value = baseValue;

            for (int i = 0; i < _buffs.Count; i++)
            {
                value = _buffs[i].ModifyCritRate(value);
            }

            return value;
        }

        public float ApplyCritDamageModifiers(float baseValue)
        {
            float value = baseValue;

            for (int i = 0; i < _buffs.Count; i++)
            {
                value = _buffs[i].ModifyCritDamage(value);
            }

            return value;
        }

        public int ApplyDamageDealtModifiers(int damage)
        {
            if (damage <= 0)
                return 0;

            const int RATE_BASE = 1000;

            int percent = 0;

            for (int i = 0; i < _buffs.Count; i++)
            {
                var buff = _buffs[i];

                if (buff.EffectType == EffectType.DamageIncrease)
                {
                    percent += buff.GetValue(); // 중요
                }
            }

            long result = damage;
            result = result * (RATE_BASE + percent) / RATE_BASE;

            return (int)result;
        }

        public int ApplyDamageTakenModifiers(int damage)
        {
            if (damage <= 0)
                return 0;

            const int RATE_BASE = 1000;

            int percent = 0;

            for (int i = 0; i < _buffs.Count; i++)
            {
                var buff = _buffs[i];

                if (buff.EffectType == EffectType.DamageReduction)
                {
                    percent += buff.GetValue();
                }
            }

            percent = Math.Min(percent, 900); // 최대 90% 제한

            long result = damage;
            result = result * (RATE_BASE - percent) / RATE_BASE;

            return (int)result;
        }

        public bool CanAct()
        {
            for (int i = 0; i < _buffs.Count; i++)
            {
                if (!_buffs[i].CanAct())
                    return false;
            }

            return true;
        }

        public bool CanUseSkill()
        {
            for (int i = 0; i < _buffs.Count; i++)
            {
                if (!_buffs[i].CanUseSkill())
                    return false;
            }

            return true;
        }


        public int GetStat(EffectType effectType, EffectValueType effectValueType)
        {
            int value = 0;
            for (int i = 0; i < _buffs.Count; i++)
            {
                if(_buffs[i].EffectType == effectType && _buffs[i].EffectValueType == effectValueType)
                {
                    T_SkillEffectData skillEffectData = T_SkillEffectData.Get(_buffs[i].BuffId);
                    if (skillEffectData == null)
                    {
                        continue;
                    }

                    value += skillEffectData.EffectValue;
                }                
            }

            return value;
        }
    }
}

