using GameData;
using System;
using System.Collections.Generic;


public class T_QuestData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Type,
        Title,
        Description,
        StartNpcId,
        EndNpcId,
        UnlockCondition,
        RewardGroupId,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual QuestType Type =>  (QuestType)System.Enum.Parse(typeof(QuestType),stringTable[_Type]);
    public virtual string Title => stringTable[_Title];
    public virtual string Description => stringTable[_Description];
    public virtual int StartNpcId => _StartNpcId;
    public virtual int EndNpcId => _EndNpcId;
    public virtual string UnlockCondition => stringTable[_UnlockCondition];
    public virtual int RewardGroupId => _RewardGroupId;

    #region Repositories
    private int _tid;
    private int _Type;
    private int _Title;
    private int _Description;
    private int _StartNpcId;
    private int _EndNpcId;
    private int _UnlockCondition;
    private int _RewardGroupId;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_QuestData(){}
    public T_QuestData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Type = intTable[1];
        _Title = intTable[2];
        _Description = intTable[3];
        _StartNpcId = intTable[4];
        _EndNpcId = intTable[5];
        _UnlockCondition = intTable[6];
        _RewardGroupId = intTable[7];
        #endregion
    }

    public static T_QuestData Get(int tid) { return Excel.GetRow(SheetName.T_QuestData, (int)tid) as T_QuestData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_QuestData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_QuestData> GetAll()
    {        
        return  Excel.GetList<T_QuestData>(SheetName.T_QuestData);
    }    
	
	public static T_QuestData GetRandom()
    {        
		return Excel.GetRandom<T_QuestData>(SheetName.T_QuestData);
    }    
}
