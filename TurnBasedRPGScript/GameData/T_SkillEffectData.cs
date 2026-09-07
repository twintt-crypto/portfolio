using GameData;
using System;
using System.Collections.Generic;


public class T_SkillEffectData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Name,
        EffectType,
        EffectValueType,
        EffectValue,
        IsInstant,
        DurationTurn,
        TickInterval,
        IsStackable,
        StackMax,
        VFX_ID,
        AggroChangeType,
        Aggro_Value,
        AggroDuration,
        IsForcedTaunt,
        StatusIcon,
        SpecialIconID,
        Display_Priority,
        AdjDamageReduceRate,
        IsProtectSide,
        LinkedEffectId,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string Name => stringTable[_Name];
    public virtual EffectType EffectType =>  (EffectType)System.Enum.Parse(typeof(EffectType),stringTable[_EffectType]);
    public virtual EffectValueType EffectValueType =>  (EffectValueType)System.Enum.Parse(typeof(EffectValueType),stringTable[_EffectValueType]);
    public virtual int EffectValue => _EffectValue;
    public virtual bool IsInstant => string.Equals(stringTable[_IsInstant], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual int DurationTurn => _DurationTurn;
    public virtual int TickInterval => _TickInterval;
    public virtual bool IsStackable => string.Equals(stringTable[_IsStackable], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual int StackMax => _StackMax;
    public virtual string VFX_ID => stringTable[_VFX_ID];
    public virtual AggroChangeType AggroChangeType =>  (AggroChangeType)System.Enum.Parse(typeof(AggroChangeType),stringTable[_AggroChangeType]);
    public virtual int Aggro_Value => _Aggro_Value;
    public virtual int AggroDuration => _AggroDuration;
    public virtual bool IsForcedTaunt => string.Equals(stringTable[_IsForcedTaunt], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual string StatusIcon => stringTable[_StatusIcon];
    public virtual string SpecialIconID => stringTable[_SpecialIconID];
    public virtual int Display_Priority => _Display_Priority;
    public virtual int AdjDamageReduceRate => _AdjDamageReduceRate;
    public virtual bool IsProtectSide => string.Equals(stringTable[_IsProtectSide], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual int LinkedEffectId => _LinkedEffectId;

    #region Repositories
    private int _tid;
    private int _Name;
    private int _EffectType;
    private int _EffectValueType;
    private int _EffectValue;
    private int _IsInstant;
    private int _DurationTurn;
    private int _TickInterval;
    private int _IsStackable;
    private int _StackMax;
    private int _VFX_ID;
    private int _AggroChangeType;
    private int _Aggro_Value;
    private int _AggroDuration;
    private int _IsForcedTaunt;
    private int _StatusIcon;
    private int _SpecialIconID;
    private int _Display_Priority;
    private int _AdjDamageReduceRate;
    private int _IsProtectSide;
    private int _LinkedEffectId;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_SkillEffectData(){}
    public T_SkillEffectData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Name = intTable[1];
        _EffectType = intTable[2];
        _EffectValueType = intTable[3];
        _EffectValue = intTable[4];
        _IsInstant = intTable[5];
        _DurationTurn = intTable[6];
        _TickInterval = intTable[7];
        _IsStackable = intTable[8];
        _StackMax = intTable[9];
        _VFX_ID = intTable[10];
        _AggroChangeType = intTable[11];
        _Aggro_Value = intTable[12];
        _AggroDuration = intTable[13];
        _IsForcedTaunt = intTable[14];
        _StatusIcon = intTable[15];
        _SpecialIconID = intTable[16];
        _Display_Priority = intTable[17];
        _AdjDamageReduceRate = intTable[18];
        _IsProtectSide = intTable[19];
        _LinkedEffectId = intTable[20];
        #endregion
    }

    public static T_SkillEffectData Get(int tid) { return Excel.GetRow(SheetName.T_SkillEffectData, (int)tid) as T_SkillEffectData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_SkillEffectData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_SkillEffectData> GetAll()
    {        
        return  Excel.GetList<T_SkillEffectData>(SheetName.T_SkillEffectData);
    }    
	
	public static T_SkillEffectData GetRandom()
    {        
		return Excel.GetRandom<T_SkillEffectData>(SheetName.T_SkillEffectData);
    }    
}
