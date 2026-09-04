using GameData;
using System;
using System.Collections.Generic;


public class T_MonsterSpawnData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        MonsterGroupId,
        BattleStageId,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual int MonsterGroupId => _MonsterGroupId;
    public virtual int BattleStageId => _BattleStageId;

    #region Repositories
    private int _tid;
    private int _MonsterGroupId;
    private int _BattleStageId;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_MonsterSpawnData(){}
    public T_MonsterSpawnData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _MonsterGroupId = intTable[1];
        _BattleStageId = intTable[2];
        #endregion
    }

    public static T_MonsterSpawnData Get(int tid) { return Excel.GetRow(SheetName.T_MonsterSpawnData, (int)tid) as T_MonsterSpawnData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_MonsterSpawnData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_MonsterSpawnData> GetAll()
    {        
        return  Excel.GetList<T_MonsterSpawnData>(SheetName.T_MonsterSpawnData);
    }    
	
	public static T_MonsterSpawnData GetRandom()
    {        
		return Excel.GetRandom<T_MonsterSpawnData>(SheetName.T_MonsterSpawnData);
    }    
}
