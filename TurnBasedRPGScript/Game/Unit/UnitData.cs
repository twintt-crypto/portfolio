namespace S7
{
    public class UnitData : EntityData
    {
        public int fomationIndex;

        // ����    
        protected UnitBaseData _baseData;
        public UnitStat _stat;
        

        public virtual void BindBuffManager(BuffManager buffManager)
        {
            _stat.BuffManager = buffManager;
        }

        public long MaxHp => _baseData.baseHp;
        public bool IsDead => _stat.hp <= 0;

        public virtual void Reset()
        {

        }
    }
}
