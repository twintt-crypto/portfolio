using UnityEngine;

namespace S7
{
    public class CharacterData : UnitData
    {
        public override UnitType unitType => UnitType.Character;
        public int lv;
        public long exp;

        public virtual void Initiaize(long unitKey, int unitId, int lv, long exp)
        {
            this.unitKey = unitKey;
            this.unitId = unitId;
            this.lv = lv;
            this.exp = exp;

            var stats = T_CharacterStatData.Get(unitId);
            if (stats.Count <= lv)
            {
                return;
            }

            T_CharacterStatData statData = stats[lv];

            _baseData = new UnitBaseData(statData);
            _stat = new UnitStat(_baseData)
            {
                hp = _baseData.baseHp
            };
        }

        public override void Reset()
        {
            base.Reset();            
        }
    }
}
