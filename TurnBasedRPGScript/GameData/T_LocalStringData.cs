using GameData;
using System;
using System.Collections.Generic;


public class T_LocalStringData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Key,
        KO,
        EN,
        JP,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string Key => stringTable[_Key];
    public virtual string KO => stringTable[_KO];
    public virtual string EN => stringTable[_EN];
    public virtual string JP => stringTable[_JP];

    #region Repositories
    private int _tid;
    private int _Key;
    private int _KO;
    private int _EN;
    private int _JP;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_LocalStringData(){}
    public T_LocalStringData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Key = intTable[1];
        _KO = intTable[2];
        _EN = intTable[3];
        _JP = intTable[4];
        #endregion
    }

    public static List<T_LocalStringData> Get(int tid) { return Excel.GetRows<T_LocalStringData>(SheetName.T_LocalStringData, tid); }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_LocalStringData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_LocalStringData> GetAll()
    {        
        return  Excel.GetList<T_LocalStringData>(SheetName.T_LocalStringData);
    }    
	
	public static T_LocalStringData GetRandom()
    {        
		return Excel.GetRandom<T_LocalStringData>(SheetName.T_LocalStringData);
    }    
}
