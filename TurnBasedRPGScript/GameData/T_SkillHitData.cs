using GameData;
using System;
using System.Collections.Generic;


public class T_SkillHitData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        HitIndex,
        Multiplier,
        AttackEffectId,
        HitEffectId,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual int HitIndex => _HitIndex;
    public virtual int Multiplier => _Multiplier;
    public virtual int AttackEffectId => _AttackEffectId;
    public virtual int HitEffectId => _HitEffectId;

    #region Repositories
    private int _tid;
    private int _HitIndex;
    private int _Multiplier;
    private int _AttackEffectId;
    private int _HitEffectId;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_SkillHitData(){}
    public T_SkillHitData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _HitIndex = intTable[1];
        _Multiplier = intTable[2];
        _AttackEffectId = intTable[3];
        _HitEffectId = intTable[4];
        #endregion
    }

    public static List<T_SkillHitData> Get(int tid) { return Excel.GetRows<T_SkillHitData>(SheetName.T_SkillHitData, tid); }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_SkillHitData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_SkillHitData> GetAll()
    {        
        return  Excel.GetList<T_SkillHitData>(SheetName.T_SkillHitData);
    }    
	
	public static T_SkillHitData GetRandom()
    {        
		return Excel.GetRandom<T_SkillHitData>(SheetName.T_SkillHitData);
    }    
}
