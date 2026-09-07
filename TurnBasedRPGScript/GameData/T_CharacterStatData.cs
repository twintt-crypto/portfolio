using GameData;
using System;
using System.Collections.Generic;


public class T_CharacterStatData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Lv,
        Hp,
        Atk,
        Def,
        Speed,
        CritRate,
        CritDmg,
        BreakPower,
        FavorBonus,
        Accuracy,
        Evasion,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual int Lv => _Lv;
    public virtual long Hp => _Hp;
    public virtual int Atk => _Atk;
    public virtual int Def => _Def;
    public virtual int Speed => _Speed;
    public virtual int CritRate => _CritRate;
    public virtual int CritDmg => _CritDmg;
    public virtual int BreakPower => _BreakPower;
    public virtual int FavorBonus => _FavorBonus;
    public virtual int Accuracy => _Accuracy;
    public virtual int Evasion => _Evasion;

    #region Repositories
    private int _tid;
    private int _Lv;
    private long _Hp;
    private int _Atk;
    private int _Def;
    private int _Speed;
    private int _CritRate;
    private int _CritDmg;
    private int _BreakPower;
    private int _FavorBonus;
    private int _Accuracy;
    private int _Evasion;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_CharacterStatData(){}
    public T_CharacterStatData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Lv = intTable[1];
        _Hp = longTable[0];
        _Atk = intTable[2];
        _Def = intTable[3];
        _Speed = intTable[4];
        _CritRate = intTable[5];
        _CritDmg = intTable[6];
        _BreakPower = intTable[7];
        _FavorBonus = intTable[8];
        _Accuracy = intTable[9];
        _Evasion = intTable[10];
        #endregion
    }

    public static List<T_CharacterStatData> Get(int tid) { return Excel.GetRows<T_CharacterStatData>(SheetName.T_CharacterStatData, tid); }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_CharacterStatData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_CharacterStatData> GetAll()
    {        
        return  Excel.GetList<T_CharacterStatData>(SheetName.T_CharacterStatData);
    }    
	
	public static T_CharacterStatData GetRandom()
    {        
		return Excel.GetRandom<T_CharacterStatData>(SheetName.T_CharacterStatData);
    }    
}
