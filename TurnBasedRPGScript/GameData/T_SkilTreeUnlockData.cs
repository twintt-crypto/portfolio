using GameData;
using System;
using System.Collections.Generic;


public class T_SkilTreeUnlockData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        ItemId,
        Count,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual int ItemId => _ItemId;
    public virtual int Count => _Count;

    #region Repositories
    private int _tid;
    private int _ItemId;
    private int _Count;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_SkilTreeUnlockData(){}
    public T_SkilTreeUnlockData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _ItemId = intTable[1];
        _Count = intTable[2];
        #endregion
    }

    public static T_SkilTreeUnlockData Get(int tid) { return Excel.GetRow(SheetName.T_SkilTreeUnlockData, (int)tid) as T_SkilTreeUnlockData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_SkilTreeUnlockData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_SkilTreeUnlockData> GetAll()
    {        
        return  Excel.GetList<T_SkilTreeUnlockData>(SheetName.T_SkilTreeUnlockData);
    }    
	
	public static T_SkilTreeUnlockData GetRandom()
    {        
		return Excel.GetRandom<T_SkilTreeUnlockData>(SheetName.T_SkilTreeUnlockData);
    }    
}
