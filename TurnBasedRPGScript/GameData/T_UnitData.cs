using GameData;
using System;
using System.Collections.Generic;


public class T_UnitData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Name,
        UnitType,
        Prefix,
        ModelPrefab,
        FieldAnimator,
        AnimationEvents,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string Name => stringTable[_Name];
    public virtual UnitType UnitType =>  (UnitType)System.Enum.Parse(typeof(UnitType),stringTable[_UnitType]);
    public virtual string Prefix => stringTable[_Prefix];
    public virtual string ModelPrefab => stringTable[_ModelPrefab];
    public virtual string FieldAnimator => stringTable[_FieldAnimator];
    public virtual string AnimationEvents => stringTable[_AnimationEvents];

    #region Repositories
    private int _tid;
    private int _Name;
    private int _UnitType;
    private int _Prefix;
    private int _ModelPrefab;
    private int _FieldAnimator;
    private int _AnimationEvents;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_UnitData(){}
    public T_UnitData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Name = intTable[1];
        _UnitType = intTable[2];
        _Prefix = intTable[3];
        _ModelPrefab = intTable[4];
        _FieldAnimator = intTable[5];
        _AnimationEvents = intTable[6];
        #endregion
    }

    public static T_UnitData Get(int tid) { return Excel.GetRow(SheetName.T_UnitData, (int)tid) as T_UnitData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_UnitData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_UnitData> GetAll()
    {        
        return  Excel.GetList<T_UnitData>(SheetName.T_UnitData);
    }    
	
	public static T_UnitData GetRandom()
    {        
		return Excel.GetRandom<T_UnitData>(SheetName.T_UnitData);
    }    
}
