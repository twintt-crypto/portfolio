using GameData;
using System;
using System.Collections.Generic;


public class T_BattleSceneData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        BattleSceneName,
        BattleGroundSceneName,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string BattleSceneName => stringTable[_BattleSceneName];
    public virtual string BattleGroundSceneName => stringTable[_BattleGroundSceneName];

    #region Repositories
    private int _tid;
    private int _BattleSceneName;
    private int _BattleGroundSceneName;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_BattleSceneData(){}
    public T_BattleSceneData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _BattleSceneName = intTable[1];
        _BattleGroundSceneName = intTable[2];
        #endregion
    }

    public static T_BattleSceneData Get(int tid) { return Excel.GetRow(SheetName.T_BattleSceneData, (int)tid) as T_BattleSceneData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_BattleSceneData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_BattleSceneData> GetAll()
    {        
        return  Excel.GetList<T_BattleSceneData>(SheetName.T_BattleSceneData);
    }    
	
	public static T_BattleSceneData GetRandom()
    {        
		return Excel.GetRandom<T_BattleSceneData>(SheetName.T_BattleSceneData);
    }    
}
