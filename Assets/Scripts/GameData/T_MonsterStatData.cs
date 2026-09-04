using GameData;
using System;
using System.Collections.Generic;


public class T_MonsterStatData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Hp,
        Atk,
        Def,
        Speed,
        Resist,
        CritRate,
        CritDamage,
        CritResist,
        MaxBreakGauge,
        NavSpeed,
        SearchRange,
        AIStateInitial,
        Accuracy,
        Evasion,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual long Hp => _Hp;
    public virtual int Atk => _Atk;
    public virtual int Def => _Def;
    public virtual int Speed => _Speed;
    public virtual int Resist => _Resist;
    public virtual int CritRate => _CritRate;
    public virtual int CritDamage => _CritDamage;
    public virtual int CritResist => _CritResist;
    public virtual int MaxBreakGauge => _MaxBreakGauge;
    public virtual int NavSpeed => _NavSpeed;
    public virtual int SearchRange => _SearchRange;
    public virtual int AIStateInitial => _AIStateInitial;
    public virtual int Accuracy => _Accuracy;
    public virtual int Evasion => _Evasion;

    #region Repositories
    private int _tid;
    private long _Hp;
    private int _Atk;
    private int _Def;
    private int _Speed;
    private int _Resist;
    private int _CritRate;
    private int _CritDamage;
    private int _CritResist;
    private int _MaxBreakGauge;
    private int _NavSpeed;
    private int _SearchRange;
    private int _AIStateInitial;
    private int _Accuracy;
    private int _Evasion;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_MonsterStatData(){}
    public T_MonsterStatData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Hp = longTable[0];
        _Atk = intTable[1];
        _Def = intTable[2];
        _Speed = intTable[3];
        _Resist = intTable[4];
        _CritRate = intTable[5];
        _CritDamage = intTable[6];
        _CritResist = intTable[7];
        _MaxBreakGauge = intTable[8];
        _NavSpeed = intTable[9];
        _SearchRange = intTable[10];
        _AIStateInitial = intTable[11];
        _Accuracy = intTable[12];
        _Evasion = intTable[13];
        #endregion
    }

    public static T_MonsterStatData Get(int tid) { return Excel.GetRow(SheetName.T_MonsterStatData, (int)tid) as T_MonsterStatData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_MonsterStatData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_MonsterStatData> GetAll()
    {        
        return  Excel.GetList<T_MonsterStatData>(SheetName.T_MonsterStatData);
    }    
	
	public static T_MonsterStatData GetRandom()
    {        
		return Excel.GetRandom<T_MonsterStatData>(SheetName.T_MonsterStatData);
    }    
}
