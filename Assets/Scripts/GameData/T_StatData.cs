using GameData;
using System;
using System.Collections.Generic;


public class T_StatData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        MaxHp,
        Attack,
        Defense,
        Crit,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual int MaxHp => _MaxHp;
    public virtual int Attack => _Attack;
    public virtual int Defense => _Defense;
    public virtual int Crit => _Crit;

    #region Repositories
    private int _tid;
    private int _MaxHp;
    private int _Attack;
    private int _Defense;
    private int _Crit;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_StatData(){}
    public T_StatData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _MaxHp = intTable[1];
        _Attack = intTable[2];
        _Defense = intTable[3];
        _Crit = intTable[4];
        #endregion
    }

    public static T_StatData Get(int tid) { return Excel.GetRow(SheetName.T_StatData, (int)tid) as T_StatData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_StatData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_StatData> GetAll()
    {        
        return  Excel.GetList<T_StatData>(SheetName.T_StatData);
    }    
	
	public static T_StatData GetRandom()
    {        
		return Excel.GetRandom<T_StatData>(SheetName.T_StatData);
    }    
}
