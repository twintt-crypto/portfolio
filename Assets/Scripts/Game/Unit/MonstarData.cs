using UnityEngine;

namespace S7
{
    public class MonstarData : UnitData
    {
        public override UnitType unitType => UnitType.Monster;

        public override void Initiaize(long unitKey, int unitId)
        {
            base.Initiaize(unitKey, unitId);

            T_MonsterStatData statData = T_MonsterStatData.Get(unitId);
            if (statData == null)
            {
                return;
            }

            _baseData = new UnitBaseData(statData);
            _stat = new UnitStat(_baseData)
            {
                hp = _baseData.baseHp
            };
        }
    }
}
