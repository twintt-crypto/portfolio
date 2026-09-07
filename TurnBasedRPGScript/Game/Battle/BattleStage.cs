using System;
using System.Collections.Generic;

public class BattleStage
{
    Dictionary<BattleSide, BattleSlot[]> _slots;    

    public void Initialize(BattleAnchorProvider provider)
    {
        _slots = new()
        {
            { BattleSide.Ally, provider.allySlots },
            { BattleSide.Enemy, provider.enemySlots },
        };
    }

    public BattleSlot GetSlot(BattleSide side, int index)
    {
        return  _slots[side][index];
    }    
}