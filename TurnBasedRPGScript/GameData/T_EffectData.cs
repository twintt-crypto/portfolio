using GameData;
using System;
using System.Collections.Generic;


public class T_EffectData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Prefab,
        AttachSocket,
        Duration,
        UsePooling,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string Prefab => stringTable[_Prefab];
    public virtual string AttachSocket => stringTable[_AttachSocket];
    public virtual int Duration => _Duration;
    public virtual bool UsePooling => string.Equals(stringTable[_UsePooling], "TRUE", StringComparison.OrdinalIgnoreCase);

    #region Repositories
    private int _tid;
    private int _Prefab;
    private int _AttachSocket;
    private int _Duration;
    private int _UsePooling;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_EffectData(){}
    public T_EffectData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Prefab = intTable[1];
        _AttachSocket = intTable[2];
        _Duration = intTable[3];
        _UsePooling = intTable[4];
        #endregion
    }

    public static T_EffectData Get(int tid) { return Excel.GetRow(SheetName.T_EffectData, (int)tid) as T_EffectData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_EffectData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_EffectData> GetAll()
    {        
        return  Excel.GetList<T_EffectData>(SheetName.T_EffectData);
    }    
	
	public static T_EffectData GetRandom()
    {        
		return Excel.GetRandom<T_EffectData>(SheetName.T_EffectData);
    }    
}
