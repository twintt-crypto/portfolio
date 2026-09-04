using GameData;
using System;
using System.Collections.Generic;


public class T_NpcData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
    }
    #endregion

    public virtual int TID => _tid;

    #region Repositories
    private int _tid;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_NpcData(){}
    public T_NpcData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        #endregion
    }

    public static T_NpcData Get(int tid) { return Excel.GetRow(SheetName.T_NpcData, (int)tid) as T_NpcData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_NpcData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_NpcData> GetAll()
    {        
        return  Excel.GetList<T_NpcData>(SheetName.T_NpcData);
    }    
	
	public static T_NpcData GetRandom()
    {        
		return Excel.GetRandom<T_NpcData>(SheetName.T_NpcData);
    }    
}
