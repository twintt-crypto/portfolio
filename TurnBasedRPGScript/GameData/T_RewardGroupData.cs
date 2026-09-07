using GameData;
using System;
using System.Collections.Generic;


public class T_RewardGroupData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        RewardIds,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual List<int> RewardIds => _RewardIds;
    public virtual int RandomRewardIds() {return 0 == (RewardIds?.Count??0) ? 0 : RewardIds?[new Random().Next(RewardIds.Count)]??0;}

    #region Repositories
    private int _tid;
    public List<int> _RewardIds = new List<int>();
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_RewardGroupData(){}
    public T_RewardGroupData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        //
        var tempRewardIds = intTable[1];
        if(string.IsNullOrEmpty( stringTable[tempRewardIds]) == false )
        {
            string[] arrayRewardIds = stringTable[tempRewardIds].Trim().Split(',');
            foreach (var iter in arrayRewardIds)
            {
               string name = iter;
               name = name.Replace("[", "");
               name = name.Replace("]", "").Trim();
               if(string.IsNullOrEmpty(name) == false)
                   RewardIds.Add(int.Parse(name));
            }
        }
        //
        #endregion
    }

    public static T_RewardGroupData Get(int tid) { return Excel.GetRow(SheetName.T_RewardGroupData, (int)tid) as T_RewardGroupData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_RewardGroupData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_RewardGroupData> GetAll()
    {        
        return  Excel.GetList<T_RewardGroupData>(SheetName.T_RewardGroupData);
    }    
	
	public static T_RewardGroupData GetRandom()
    {        
		return Excel.GetRandom<T_RewardGroupData>(SheetName.T_RewardGroupData);
    }    
}
