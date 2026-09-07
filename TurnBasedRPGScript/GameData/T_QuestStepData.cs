using GameData;
using System;
using System.Collections.Generic;


public class T_QuestStepData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Step,
        ConditionGroupId,
        ComplateDialogueId,
        NextStepId,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual int Step => _Step;
    public virtual int ConditionGroupId => _ConditionGroupId;
    public virtual int ComplateDialogueId => _ComplateDialogueId;
    public virtual int NextStepId => _NextStepId;

    #region Repositories
    private int _tid;
    private int _Step;
    private int _ConditionGroupId;
    private int _ComplateDialogueId;
    private int _NextStepId;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_QuestStepData(){}
    public T_QuestStepData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Step = intTable[1];
        _ConditionGroupId = intTable[2];
        _ComplateDialogueId = intTable[3];
        _NextStepId = intTable[4];
        #endregion
    }

    public static List<T_QuestStepData> Get(int tid) { return Excel.GetRows<T_QuestStepData>(SheetName.T_QuestStepData, tid); }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_QuestStepData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_QuestStepData> GetAll()
    {        
        return  Excel.GetList<T_QuestStepData>(SheetName.T_QuestStepData);
    }    
	
	public static T_QuestStepData GetRandom()
    {        
		return Excel.GetRandom<T_QuestStepData>(SheetName.T_QuestStepData);
    }    
}
