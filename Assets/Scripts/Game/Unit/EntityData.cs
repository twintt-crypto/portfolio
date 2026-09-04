public class EntityData
{
    public virtual UnitType unitType => UnitType.None;

    public long unitKey;
    public int unitId;    

    public virtual void Initiaize(long unitKey, int unitId)
    {
        this.unitKey = unitKey;
        this.unitId = unitId; ;        
    }
}