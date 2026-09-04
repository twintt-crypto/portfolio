using GameData;
using System;
using System.Collections.Generic;


public class T_MonsterGrroupMemberData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        MonsterId,
        SlotIndex,
        Level,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual int MonsterId => _MonsterId;
    public virtual int SlotIndex => _SlotIndex;
    public virtual int Level => _Level;

    #region Repositories
    private int _tid;
    private int _MonsterId;
    private int _SlotIndex;
    private int _Level;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_MonsterGrroupMemberData(){}
    public T_MonsterGrroupMemberData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _MonsterId = intTable[1];
        _SlotIndex = intTable[2];
        _Level = intTable[3];
        #endregion
    }

    public static List<T_MonsterGrroupMemberData> Get(int tid) { return Excel.GetRows<T_MonsterGrroupMemberData>(SheetName.T_MonsterGrroupMemberData, tid); }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_MonsterGrroupMemberData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_MonsterGrroupMemberData> GetAll()
    {        
        return  Excel.GetList<T_MonsterGrroupMemberData>(SheetName.T_MonsterGrroupMemberData);
    }    
	
	public static T_MonsterGrroupMemberData GetRandom()
    {        
		return Excel.GetRandom<T_MonsterGrroupMemberData>(SheetName.T_MonsterGrroupMemberData);
    }    
}
