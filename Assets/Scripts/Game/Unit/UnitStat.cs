using System;

namespace S7
{
    public class UnitStat
    {
        int baseRate => T_GlobalValueData.Get(GlobalValueType.RateBase).ValueInt;
        public UnitStat(UnitBaseData baseData)
        {
            _baseData = baseData;
        }

        private readonly UnitBaseData _baseData;
        private BuffManager _buffManager;

        public long hp;

        public int Attack => _baseData.baseAttack;
        public int Defence => _baseData.baseDefence;
        public int Speed => _baseData.baseSpeed;
        public int CritRate => _baseData.baseCritRate;
        public int CritDamage => _baseData.baseCritDamage;
        public int Accuracy => _baseData.baseAccuracy;
        public int Evasion => _baseData.baseEvasion;        

        public BuffManager BuffManager { set => _buffManager = value; }

        public int GetStat(EffectType effectType)
        {
            int baseValue = GetBaseStat(effectType);
            int flat = _buffManager.GetStat(effectType, EffectValueType.Flat);
            int percent = _buffManager.GetStat(effectType, EffectValueType.Percent);

            long value = baseValue + flat;
            value = value * (baseRate + percent) / baseRate;

            return (int)Math.Max(0, value);
        }

        private int GetBaseStat(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.Attack:
                    return _baseData.baseAttack;

                case EffectType.Defense:
                    return _baseData.baseDefence;

                case EffectType.Speed:
                    return _baseData.baseSpeed;

                default:
                    return 0;
            }
        }

        /* public int GetDefence()
         {
             int flat = _buffManager.GetStat(EffectType.Defense, EffectValueType.Flat);
             int percent = _buffManager.GetStat(EffectType.Defense, EffectValueType.Percent);

             long value = _baseData.baseAttack + flat;
             value = value * (baseRate + percent) / baseRate;
             return (int)Math.Max(0, value);
         }

         public int GetSpeed()
         {
             int flat = _buffManager.GetStat(EffectType.Speed, EffectValueType.Flat);
             int percent = _buffManager.GetStat(EffectType.Speed, EffectValueType.Percent);

             long value = _baseData.baseAttack + flat;
             value = value * (baseRate + percent) / baseRate;
             return (int)Math.Max(0, value);
         }*/
    }
}
