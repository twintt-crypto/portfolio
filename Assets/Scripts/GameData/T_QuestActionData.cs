using GameData;
using System;
using System.Collections.Generic;


public class T_QuestActionData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Stage,
        Order,
        QuestActionType,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual int Stage => _Stage;
    public virtual int Order => _Order;
    public virtual QuestActionType QuestActionType =>  (QuestActionType)System.Enum.Parse(typeof(QuestActionType),stringTable[_QuestActionType]);

    #region Repositories
    private int _tid;
    private int _Stage;
    private int _Order;
    private int _QuestActionType;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_QuestActionData(){}
    public T_QuestActionData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Stage = intTable[1];
        _Order = intTable[2];
        _QuestActionType = intTable[3];
        #endregion
    }

    public static List<T_QuestActionData> Get(int tid) { return Excel.GetRows<T_QuestActionData>(SheetName.T_QuestActionData, tid); }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_QuestActionData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_QuestActionData> GetAll()
    {        
        return  Excel.GetList<T_QuestActionData>(SheetName.T_QuestActionData);
    }    
	
	public static T_QuestActionData GetRandom()
    {        
		return Excel.GetRandom<T_QuestActionData>(SheetName.T_QuestActionData);
    }    
}
