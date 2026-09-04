using GameData;
using System;
using System.Collections.Generic;


public class T_MonsterGimmickData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Name,
        StatID,
        ModelPrefab,
        BaseAttack,
        SkillSetId,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string Name => stringTable[_Name];
    public virtual int StatID => _StatID;
    public virtual string ModelPrefab => stringTable[_ModelPrefab];
    public virtual int BaseAttack => _BaseAttack;
    public virtual int SkillSetId => _SkillSetId;

    #region Repositories
    private int _tid;
    private int _Name;
    private int _StatID;
    private int _ModelPrefab;
    private int _BaseAttack;
    private int _SkillSetId;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_MonsterGimmickData(){}
    public T_MonsterGimmickData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Name = intTable[1];
        _StatID = intTable[2];
        _ModelPrefab = intTable[3];
        _BaseAttack = intTable[4];
        _SkillSetId = intTable[5];
        #endregion
    }

    public static T_MonsterGimmickData Get(int tid) { return Excel.GetRow(SheetName.T_MonsterGimmickData, (int)tid) as T_MonsterGimmickData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_MonsterGimmickData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_MonsterGimmickData> GetAll()
    {        
        return  Excel.GetList<T_MonsterGimmickData>(SheetName.T_MonsterGimmickData);
    }    
	
	public static T_MonsterGimmickData GetRandom()
    {        
		return Excel.GetRandom<T_MonsterGimmickData>(SheetName.T_MonsterGimmickData);
    }    
}
