using GameData;
using System;
using System.Collections.Generic;


public class T_ProjectileData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Prefab,
        Speed,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string Prefab => stringTable[_Prefab];
    public virtual int Speed => _Speed;

    #region Repositories
    private int _tid;
    private int _Prefab;
    private int _Speed;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_ProjectileData(){}
    public T_ProjectileData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Prefab = intTable[1];
        _Speed = intTable[2];
        #endregion
    }

    public static T_ProjectileData Get(int tid) { return Excel.GetRow(SheetName.T_ProjectileData, (int)tid) as T_ProjectileData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_ProjectileData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_ProjectileData> GetAll()
    {        
        return  Excel.GetList<T_ProjectileData>(SheetName.T_ProjectileData);
    }    
	
	public static T_ProjectileData GetRandom()
    {        
		return Excel.GetRandom<T_ProjectileData>(SheetName.T_ProjectileData);
    }    
}
