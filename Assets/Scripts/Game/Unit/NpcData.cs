public class NpcData : EntityData
{
    public override UnitType unitType => UnitType.Npc;

    public override void Initiaize(long unitKey, int unitId)
    {
        base.Initiaize(unitKey, unitId);
    }
}