using GameData;
using System;
using System.Collections.Generic;


public class T_DialogueScriptData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        Group_ID,
        Type,
        Speaker_ID,
        Coundition_Loop,
        Next_ID,
        From_ID,
        Text_ID,
        Text_Attribute,
        Anim_ID,
        Item_ID,
    }
    #endregion

    public virtual int TID => _tid;
    public virtual int Group_ID => _Group_ID;
    public virtual ScriptType Type =>  (ScriptType)System.Enum.Parse(typeof(ScriptType),stringTable[_Type]);
    public virtual int Speaker_ID => _Speaker_ID;
    public virtual int Coundition_Loop => _Coundition_Loop;
    public virtual int Next_ID => _Next_ID;
    public virtual int From_ID => _From_ID;
    public virtual int Text_ID => _Text_ID;
    public virtual int Text_Attribute => _Text_Attribute;
    public virtual int Anim_ID => _Anim_ID;
    public virtual int Item_ID => _Item_ID;

    #region Repositories
    private int _tid;
    private int _Group_ID;
    private int _Type;
    private int _Speaker_ID;
    private int _Coundition_Loop;
    private int _Next_ID;
    private int _From_ID;
    private int _Text_ID;
    private int _Text_Attribute;
    private int _Anim_ID;
    private int _Item_ID;
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_DialogueScriptData(){}
    public T_DialogueScriptData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _Group_ID = intTable[1];
        _Type = intTable[2];
        _Speaker_ID = intTable[3];
        _Coundition_Loop = intTable[4];
        _Next_ID = intTable[5];
        _From_ID = intTable[6];
        _Text_ID = intTable[7];
        _Text_Attribute = intTable[8];
        _Anim_ID = intTable[9];
        _Item_ID = intTable[10];
        #endregion
    }

    public static T_DialogueScriptData Get(int tid) { return Excel.GetRow(SheetName.T_DialogueScriptData, (int)tid) as T_DialogueScriptData; }
    public static List<int> GetTIDs()
    {
        List<int> list = new List<int>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_DialogueScriptData))
        {
             list.Add((int)iter);
        }
        return list;
    }
    
    public static List<T_DialogueScriptData> GetAll()
    {        
        return  Excel.GetList<T_DialogueScriptData>(SheetName.T_DialogueScriptData);
    }    
	
	public static T_DialogueScriptData GetRandom()
    {        
		return Excel.GetRandom<T_DialogueScriptData>(SheetName.T_DialogueScriptData);
    }    
}
