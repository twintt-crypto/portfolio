using GameData;
using System;
using System.Collections.Generic;


public class T_DialogueSciprtTextData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Text_Key,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual string Text_Key => stringTable[_Text_Key];

    #region Repositories
    private int _tid;
    private int _Text_Key;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_DialogueSciprtTextData(){}
    public T_DialogueSciprtTextData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Text_Key = intTable[1];
        #endregion
    }

    public static T_DialogueSciprtTextData Get(int tid) { return Excel.GetRow(SheetName.T_DialogueSciprtTextData, (int)tid) as T_DialogueSciprtTextData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_DialogueSciprtTextData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_DialogueSciprtTextData> GetAll()
    {        
        return  Excel.GetList<T_DialogueSciprtTextData>(SheetName.T_DialogueSciprtTextData);
    }    
	
	public static T_DialogueSciprtTextData GetRandom()
    {        
		return Excel.GetRandom<T_DialogueSciprtTextData>(SheetName.T_DialogueSciprtTextData);
    }    
}
