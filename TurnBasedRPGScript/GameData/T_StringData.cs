using GameData;
using System;
using System.Collections.Generic;


public class T_StringData : IRow
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

    public T_StringData(){}
    public T_StringData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Key = intTable[1];
        _KO = intTable[2];
        _EN = intTable[3];
        _JP = intTable[4];
        #endregion
    }

    public static T_StringData Get(int tid) { return Excel.GetRow(SheetName.T_StringData, (int)tid) as T_StringData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_StringData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_StringData> GetAll()
    {        
        return  Excel.GetList<T_StringData>(SheetName.T_StringData);
    }    
	
	public static T_StringData GetRandom()
    {        
		return Excel.GetRandom<T_StringData>(SheetName.T_StringData);
    }    
}
