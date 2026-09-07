using System.Collections.Generic;

[System.Serializable]
public class BattleUnitInfo
{
    public long unitKey;
    public int unitId;
    public string name;
    public int hp;
    public int maxHp;
    public int ultimateGuage;
    public int speed;
    public bool isPlayer;
    public List<string> buffs = new();
}
