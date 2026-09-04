using GameData;
using System;
using System.Collections.Generic;


public class T_SkillData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Name,
        Explain,
        Enable,
        SkillType,
        ActionType,
        AttackType,
        ReactiveType,
        TargetType,
        targetScope,
        Ap,
        SplashEffectRate,
        StatRate,
        BreakRate,
        ActivationCondition,
        PresentationGraph,
        ProjectileId,
        EffectId,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string Name => stringTable[_Name];
    public virtual string Explain => stringTable[_Explain];
    public virtual bool Enable => string.Equals(stringTable[_Enable], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual SkillType SkillType =>  (SkillType)System.Enum.Parse(typeof(SkillType),stringTable[_SkillType]);
    public virtual SkillActionType ActionType =>  (SkillActionType)System.Enum.Parse(typeof(SkillActionType),stringTable[_ActionType]);
    public virtual AttackType AttackType =>  (AttackType)System.Enum.Parse(typeof(AttackType),stringTable[_AttackType]);
    public virtual ReactiveType ReactiveType =>  (ReactiveType)System.Enum.Parse(typeof(ReactiveType),stringTable[_ReactiveType]);
    public virtual TargetType TargetType =>  (TargetType)System.Enum.Parse(typeof(TargetType),stringTable[_TargetType]);
    public virtual TargetScope targetScope =>  (TargetScope)System.Enum.Parse(typeof(TargetScope),stringTable[_targetScope]);
    public virtual int Ap => _Ap;
    public virtual int SplashEffectRate => _SplashEffectRate;
    public virtual int StatRate => _StatRate;
    public virtual int BreakRate => _BreakRate;
    public virtual ActivationCondition ActivationCondition =>  (ActivationCondition)System.Enum.Parse(typeof(ActivationCondition),stringTable[_ActivationCondition]);
    public virtual string PresentationGraph => stringTable[_PresentationGraph];
    public virtual int ProjectileId => _ProjectileId;
    public virtual List<int> EffectId => _EffectId;
    public virtual int RandomEffectId() {return 0 == (EffectId?.Count??0) ? 0 : EffectId?[new Random().Next(EffectId.Count)]??0;}

    #region Repositories
    private int _tid;
    private int _Name;
    private int _Explain;
    private int _Enable;
    private int _SkillType;
    private int _ActionType;
    private int _AttackType;
    private int _ReactiveType;
    private int _TargetType;
    private int _targetScope;
    private int _Ap;
    private int _SplashEffectRate;
    private int _StatRate;
    private int _BreakRate;
    private int _ActivationCondition;
    private int _PresentationGraph;
    private int _ProjectileId;
    public List<int> _EffectId = new List<int>();
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_SkillData(){}
    public T_SkillData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Name = intTable[1];
        _Explain = intTable[2];
        _Enable = intTable[3];
        _SkillType = intTable[4];
        _ActionType = intTable[5];
        _AttackType = intTable[6];
        _ReactiveType = intTable[7];
        _TargetType = intTable[8];
        _targetScope = intTable[9];
        _Ap = intTable[10];
        _SplashEffectRate = intTable[11];
        _StatRate = intTable[12];
        _BreakRate = intTable[13];
        _ActivationCondition = intTable[14];
        _PresentationGraph = intTable[15];
        _ProjectileId = intTable[16];
        //
        var tempEffectId = intTable[17];
        if(string.IsNullOrEmpty( stringTable[tempEffectId]) == false )
        {
            string[] arrayEffectId = stringTable[tempEffectId].Trim().Split(',');
            foreach (var iter in arrayEffectId)
            {
               string name = iter;
               name = name.Replace("[", "");
               name = name.Replace("]", "").Trim();
               if(string.IsNullOrEmpty(name) == false)
                   EffectId.Add(int.Parse(name));
            }
        }
        //
        #endregion
    }

    public static T_SkillData Get(int tid) { return Excel.GetRow(SheetName.T_SkillData, (int)tid) as T_SkillData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_SkillData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_SkillData> GetAll()
    {        
        return  Excel.GetList<T_SkillData>(SheetName.T_SkillData);
    }    
	
	public static T_SkillData GetRandom()
    {        
		return Excel.GetRandom<T_SkillData>(SheetName.T_SkillData);
    }    
}
