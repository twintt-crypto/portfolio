using GameData;
using System;
using System.Collections.Generic;


public class T_RewardData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        RewardType,
        TargetId,
        Amount,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual RewardType RewardType =>  (RewardType)System.Enum.Parse(typeof(RewardType),stringTable[_RewardType]);
    public virtual int TargetId => _TargetId;
    public virtual int Amount => _Amount;

    #region Repositories
    private int _tid;
    private int _RewardType;
    private int _TargetId;
    private int _Amount;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_RewardData(){}
    public T_RewardData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _RewardType = intTable[1];
        _TargetId = intTable[2];
        _Amount = intTable[3];
        #endregion
    }

    public static T_RewardData Get(int tid) { return Excel.GetRow(SheetName.T_RewardData, (int)tid) as T_RewardData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_RewardData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_RewardData> GetAll()
    {        
        return  Excel.GetList<T_RewardData>(SheetName.T_RewardData);
    }    
	
	public static T_RewardData GetRandom()
    {        
		return Excel.GetRandom<T_RewardData>(SheetName.T_RewardData);
    }    
}
