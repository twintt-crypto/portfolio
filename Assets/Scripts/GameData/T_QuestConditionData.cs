using GameData;
using System;
using System.Collections.Generic;


public class T_QuestConditionData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        ConditionType,
        TargetId,
        RequiredCount,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual QuestConditionType ConditionType =>  (QuestConditionType)System.Enum.Parse(typeof(QuestConditionType),stringTable[_ConditionType]);
    public virtual int TargetId => _TargetId;
    public virtual int RequiredCount => _RequiredCount;

    #region Repositories
    private int _tid;
    private int _ConditionType;
    private int _TargetId;
    private int _RequiredCount;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_QuestConditionData(){}
    public T_QuestConditionData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _ConditionType = intTable[1];
        _TargetId = intTable[2];
        _RequiredCount = intTable[3];
        #endregion
    }

    public static List<T_QuestConditionData> Get(int tid) { return Excel.GetRows<T_QuestConditionData>(SheetName.T_QuestConditionData, tid); }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_QuestConditionData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_QuestConditionData> GetAll()
    {        
        return  Excel.GetList<T_QuestConditionData>(SheetName.T_QuestConditionData);
    }    
	
	public static T_QuestConditionData GetRandom()
    {        
		return Excel.GetRandom<T_QuestConditionData>(SheetName.T_QuestConditionData);
    }    
}
