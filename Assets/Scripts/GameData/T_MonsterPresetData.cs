using GameData;
using System;
using System.Collections.Generic;


public class T_MonsterPresetData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Type,
        SkilID,
        SuperArmor,
        BreakResist,
        BreakDecayPerTurn,
        ParryReactionType,
        ActionDelayValue,
        PhaseControllerId,
        GimmickID,
        StunDuration,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual MonstatType Type =>  (MonstatType)System.Enum.Parse(typeof(MonstatType),stringTable[_Type]);
    public virtual List<int> SkilID => _SkilID;
    public virtual int RandomSkilID() {return 0 == (SkilID?.Count??0) ? 0 : SkilID?[new Random().Next(SkilID.Count)]??0;}
    public virtual int SuperArmor => _SuperArmor;
    public virtual int BreakResist => _BreakResist;
    public virtual int BreakDecayPerTurn => _BreakDecayPerTurn;
    public virtual ParryReactionType ParryReactionType =>  (ParryReactionType)System.Enum.Parse(typeof(ParryReactionType),stringTable[_ParryReactionType]);
    public virtual int ActionDelayValue => _ActionDelayValue;
    public virtual int PhaseControllerId => _PhaseControllerId;
    public virtual int GimmickID => _GimmickID;
    public virtual int StunDuration => _StunDuration;

    #region Repositories
    private int _tid;
    private int _Type;
    public List<int> _SkilID = new List<int>();
    private int _SuperArmor;
    private int _BreakResist;
    private int _BreakDecayPerTurn;
    private int _ParryReactionType;
    private int _ActionDelayValue;
    private int _PhaseControllerId;
    private int _GimmickID;
    private int _StunDuration;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_MonsterPresetData(){}
    public T_MonsterPresetData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Type = intTable[1];
        //
        var tempSkilID = intTable[2];
        if(string.IsNullOrEmpty( stringTable[tempSkilID]) == false )
        {
            string[] arraySkilID = stringTable[tempSkilID].Trim().Split(',');
            foreach (var iter in arraySkilID)
            {
               string name = iter;
               name = name.Replace("[", "");
               name = name.Replace("]", "").Trim();
               if(string.IsNullOrEmpty(name) == false)
                   SkilID.Add(int.Parse(name));
            }
        }
        //
        _SuperArmor = intTable[3];
        _BreakResist = intTable[4];
        _BreakDecayPerTurn = intTable[5];
        _ParryReactionType = intTable[6];
        _ActionDelayValue = intTable[7];
        _PhaseControllerId = intTable[8];
        _GimmickID = intTable[9];
        _StunDuration = intTable[10];
        #endregion
    }

    public static T_MonsterPresetData Get(int tid) { return Excel.GetRow(SheetName.T_MonsterPresetData, (int)tid) as T_MonsterPresetData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_MonsterPresetData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_MonsterPresetData> GetAll()
    {        
        return  Excel.GetList<T_MonsterPresetData>(SheetName.T_MonsterPresetData);
    }    
	
	public static T_MonsterPresetData GetRandom()
    {        
		return Excel.GetRandom<T_MonsterPresetData>(SheetName.T_MonsterPresetData);
    }    
}
