using GameData;
using System;
using System.Collections.Generic;


public class T_FieldData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        FieldBgName,
        FieldAreaName,
        FieldEntryPoint,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string FieldBgName => stringTable[_FieldBgName];
    public virtual string FieldAreaName => stringTable[_FieldAreaName];
    public virtual int FieldEntryPoint => _FieldEntryPoint;

    #region Repositories
    private int _tid;
    private int _FieldBgName;
    private int _FieldAreaName;
    private int _FieldEntryPoint;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_FieldData(){}
    public T_FieldData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _FieldBgName = intTable[1];
        _FieldAreaName = intTable[2];
        _FieldEntryPoint = intTable[3];
        #endregion
    }

    public static T_FieldData Get(int tid) { return Excel.GetRow(SheetName.T_FieldData, (int)tid) as T_FieldData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_FieldData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_FieldData> GetAll()
    {        
        return  Excel.GetList<T_FieldData>(SheetName.T_FieldData);
    }    
	
	public static T_FieldData GetRandom()
    {        
		return Excel.GetRandom<T_FieldData>(SheetName.T_FieldData);
    }    
}
