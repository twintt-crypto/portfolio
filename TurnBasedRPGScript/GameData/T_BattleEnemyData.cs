using GameData;
using System;
using System.Collections.Generic;


public class T_BattleEnemyData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        EnemyId,
        EnemyLv,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual List<int> EnemyId => _EnemyId;
    public virtual int RandomEnemyId() {return 0 == (EnemyId?.Count??0) ? 0 : EnemyId?[new Random().Next(EnemyId.Count)]??0;}
    public virtual List<int> EnemyLv => _EnemyLv;
    public virtual int RandomEnemyLv() {return 0 == (EnemyLv?.Count??0) ? 0 : EnemyLv?[new Random().Next(EnemyLv.Count)]??0;}

    #region Repositories
    private int _tid;
    public List<int> _EnemyId = new List<int>();
    public List<int> _EnemyLv = new List<int>();
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_BattleEnemyData(){}
    public T_BattleEnemyData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        //
        var tempEnemyId = intTable[1];
        if(string.IsNullOrEmpty( stringTable[tempEnemyId]) == false )
        {
            string[] arrayEnemyId = stringTable[tempEnemyId].Trim().Split(',');
            foreach (var iter in arrayEnemyId)
            {
               string name = iter;
               name = name.Replace("[", "");
               name = name.Replace("]", "").Trim();
               if(string.IsNullOrEmpty(name) == false)
                   EnemyId.Add(int.Parse(name));
            }
        }
        //
        //
        var tempEnemyLv = intTable[2];
        if(string.IsNullOrEmpty( stringTable[tempEnemyLv]) == false )
        {
            string[] arrayEnemyLv = stringTable[tempEnemyLv].Trim().Split(',');
            foreach (var iter in arrayEnemyLv)
            {
               string name = iter;
               name = name.Replace("[", "");
               name = name.Replace("]", "").Trim();
               if(string.IsNullOrEmpty(name) == false)
                   EnemyLv.Add(int.Parse(name));
            }
        }
        //
        #endregion
    }

    public static T_BattleEnemyData Get(int tid) { return Excel.GetRow(SheetName.T_BattleEnemyData, (int)tid) as T_BattleEnemyData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_BattleEnemyData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_BattleEnemyData> GetAll()
    {        
        return  Excel.GetList<T_BattleEnemyData>(SheetName.T_BattleEnemyData);
    }    
	
	public static T_BattleEnemyData GetRandom()
    {        
		return Excel.GetRandom<T_BattleEnemyData>(SheetName.T_BattleEnemyData);
    }    
}
