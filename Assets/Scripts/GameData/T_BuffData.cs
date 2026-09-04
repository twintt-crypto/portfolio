using GameData;
using System;
using System.Collections.Generic;


public class T_BuffData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Name,
        EffectType,
        Value,
        TargetType,
        DurationTurn,
        IsStackable,
        MaxStack,
        IsDebuff,
        BuffTickType,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string Name => stringTable[_Name];
    public virtual EffectType EffectType =>  (EffectType)System.Enum.Parse(typeof(EffectType),stringTable[_EffectType]);
    public virtual int Value => _Value;
    public virtual TargetType TargetType =>  (TargetType)System.Enum.Parse(typeof(TargetType),stringTable[_TargetType]);
    public virtual int DurationTurn => _DurationTurn;
    public virtual bool IsStackable => string.Equals(stringTable[_IsStackable], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual int MaxStack => _MaxStack;
    public virtual bool IsDebuff => string.Equals(stringTable[_IsDebuff], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual BuffTickType BuffTickType =>  (BuffTickType)System.Enum.Parse(typeof(BuffTickType),stringTable[_BuffTickType]);

    #region Repositories
    private int _tid;
    private int _Name;
    private int _EffectType;
    private int _Value;
    private int _TargetType;
    private int _DurationTurn;
    private int _IsStackable;
    private int _MaxStack;
    private int _IsDebuff;
    private int _BuffTickType;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_BuffData(){}
    public T_BuffData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Name = intTable[1];
        _EffectType = intTable[2];
        _Value = intTable[3];
        _TargetType = intTable[4];
        _DurationTurn = intTable[5];
        _IsStackable = intTable[6];
        _MaxStack = intTable[7];
        _IsDebuff = intTable[8];
        _BuffTickType = intTable[9];
        #endregion
    }

    public static T_BuffData Get(int tid) { return Excel.GetRow(SheetName.T_BuffData, (int)tid) as T_BuffData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_BuffData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_BuffData> GetAll()
    {        
        return  Excel.GetList<T_BuffData>(SheetName.T_BuffData);
    }    
	
	public static T_BuffData GetRandom()
    {        
		return Excel.GetRandom<T_BuffData>(SheetName.T_BuffData);
    }    
}
