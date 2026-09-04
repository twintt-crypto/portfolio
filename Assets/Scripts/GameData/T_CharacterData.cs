using GameData;
using System;
using System.Collections.Generic;


public class T_CharacterData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        ClassType,
        AttributeType,
        Rarity,
        BaseStatID,
        BaseAggroRate,
        UIDispalyType,
        IsTrunVisible,
        PassiveSkillId,
        AttackSkillId,
        SpecialSkill,
        SkillAttackSkillId,
        UltimateSkillId,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual ClassType ClassType =>  (ClassType)System.Enum.Parse(typeof(ClassType),stringTable[_ClassType]);
    public virtual AttributeType AttributeType =>  (AttributeType)System.Enum.Parse(typeof(AttributeType),stringTable[_AttributeType]);
    public virtual Rarity Rarity =>  (Rarity)System.Enum.Parse(typeof(Rarity),stringTable[_Rarity]);
    public virtual int BaseStatID => _BaseStatID;
    public virtual int BaseAggroRate => _BaseAggroRate;
    public virtual UIDispalyType UIDispalyType =>  (UIDispalyType)System.Enum.Parse(typeof(UIDispalyType),stringTable[_UIDispalyType]);
    public virtual bool IsTrunVisible => string.Equals(stringTable[_IsTrunVisible], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual int PassiveSkillId => _PassiveSkillId;
    public virtual int AttackSkillId => _AttackSkillId;
    public virtual int SpecialSkill => _SpecialSkill;
    public virtual int SkillAttackSkillId => _SkillAttackSkillId;
    public virtual int UltimateSkillId => _UltimateSkillId;

    #region Repositories
    private int _tid;
    private int _ClassType;
    private int _AttributeType;
    private int _Rarity;
    private int _BaseStatID;
    private int _BaseAggroRate;
    private int _UIDispalyType;
    private int _IsTrunVisible;
    private int _PassiveSkillId;
    private int _AttackSkillId;
    private int _SpecialSkill;
    private int _SkillAttackSkillId;
    private int _UltimateSkillId;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_CharacterData(){}
    public T_CharacterData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _ClassType = intTable[1];
        _AttributeType = intTable[2];
        _Rarity = intTable[3];
        _BaseStatID = intTable[4];
        _BaseAggroRate = intTable[5];
        _UIDispalyType = intTable[6];
        _IsTrunVisible = intTable[7];
        _PassiveSkillId = intTable[8];
        _AttackSkillId = intTable[9];
        _SpecialSkill = intTable[10];
        _SkillAttackSkillId = intTable[11];
        _UltimateSkillId = intTable[12];
        #endregion
    }

    public static T_CharacterData Get(int tid) { return Excel.GetRow(SheetName.T_CharacterData, (int)tid) as T_CharacterData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_CharacterData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_CharacterData> GetAll()
    {        
        return  Excel.GetList<T_CharacterData>(SheetName.T_CharacterData);
    }    
	
	public static T_CharacterData GetRandom()
    {        
		return Excel.GetRandom<T_CharacterData>(SheetName.T_CharacterData);
    }    
}
