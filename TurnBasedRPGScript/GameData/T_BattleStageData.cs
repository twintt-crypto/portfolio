using GameData;
using System;
using System.Collections.Generic;


public class T_BattleStageData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        BattleStageType,
        BattleSceneID,
        ClearCondition,
        MaxTurn,
        EscapeAllowed,
        RewardId,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual BattleStageType BattleStageType =>  (BattleStageType)System.Enum.Parse(typeof(BattleStageType),stringTable[_BattleStageType]);
    public virtual int BattleSceneID => _BattleSceneID;
    public virtual int ClearCondition => _ClearCondition;
    public virtual int MaxTurn => _MaxTurn;
    public virtual bool EscapeAllowed => string.Equals(stringTable[_EscapeAllowed], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual int RewardId => _RewardId;

    #region Repositories
    private int _tid;
    private int _BattleStageType;
    private int _BattleSceneID;
    private int _ClearCondition;
    private int _MaxTurn;
    private int _EscapeAllowed;
    private int _RewardId;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_BattleStageData(){}
    public T_BattleStageData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _BattleStageType = intTable[1];
        _BattleSceneID = intTable[2];
        _ClearCondition = intTable[3];
        _MaxTurn = intTable[4];
        _EscapeAllowed = intTable[5];
        _RewardId = intTable[6];
        #endregion
    }

    public static T_BattleStageData Get(int tid) { return Excel.GetRow(SheetName.T_BattleStageData, (int)tid) as T_BattleStageData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_BattleStageData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_BattleStageData> GetAll()
    {        
        return  Excel.GetList<T_BattleStageData>(SheetName.T_BattleStageData);
    }    
	
	public static T_BattleStageData GetRandom()
    {        
		return Excel.GetRandom<T_BattleStageData>(SheetName.T_BattleStageData);
    }    
}
