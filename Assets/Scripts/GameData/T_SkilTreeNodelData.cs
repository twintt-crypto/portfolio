using GameData;
using System;
using System.Collections.Generic;


public class T_SkilTreeNodelData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        NodeId,
        SkillId,
        RequiredNodeId,
        Enable,
        UnlockId,
    }
    #endregion

    public virtual ClassType TID => (ClassType)_tid;
    public virtual int NodeId => _NodeId;
    public virtual int SkillId => _SkillId;
    public virtual int RequiredNodeId => _RequiredNodeId;
    public virtual bool Enable => string.Equals(stringTable[_Enable], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual int UnlockId => _UnlockId;

    #region Repositories
    private int _tid;
    private int _NodeId;
    private int _SkillId;
    private int _RequiredNodeId;
    private int _Enable;
    private int _UnlockId;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_SkilTreeNodelData(){}
    public T_SkilTreeNodelData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _NodeId = intTable[1];
        _SkillId = intTable[2];
        _RequiredNodeId = intTable[3];
        _Enable = intTable[4];
        _UnlockId = intTable[5];
        #endregion
    }

    public static T_SkilTreeNodelData Get(ClassType tid) { return Excel.GetRow(SheetName.T_SkilTreeNodelData, (int)tid) as T_SkilTreeNodelData; }
    public static List<ClassType> GetTIDs()
    {
        List<ClassType> list = new List<ClassType>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_SkilTreeNodelData))
        {
             list.Add((ClassType)iter);
        }
        return list;
    }
    
    public static List<T_SkilTreeNodelData> GetAll()
    {        
        return  Excel.GetList<T_SkilTreeNodelData>(SheetName.T_SkilTreeNodelData);
    }    
	
	public static T_SkilTreeNodelData GetRandom()
    {        
		return Excel.GetRandom<T_SkilTreeNodelData>(SheetName.T_SkilTreeNodelData);
    }    
}
