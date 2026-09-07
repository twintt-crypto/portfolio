using GameData;
using System;
using System.Collections.Generic;


public class T_GlobalValueData : IRow
{
    #region Record Info
    public enum Records
    {
        tid,
        ValueInt,
        ValueFloat,
        ValueBool,
        ValueString,
        ValueIntArray,
        ValueFloatArray,
        ValueStringArray,
    }
    #endregion

    public virtual GlobalValueType TID => (GlobalValueType)_tid;
    public virtual int ValueInt => _ValueInt;
    public virtual float ValueFloat => _ValueFloat;
    public virtual bool ValueBool => string.Equals(stringTable[_ValueBool], "TRUE", StringComparison.OrdinalIgnoreCase);
    public virtual string ValueString => stringTable[_ValueString];
    public virtual List<int> ValueIntArray => _ValueIntArray;
    public virtual int RandomValueIntArray() {return 0 == (ValueIntArray?.Count??0) ? 0 : ValueIntArray?[new Random().Next(ValueIntArray.Count)]??0;}
    public virtual List<float> ValueFloatArray => _ValueFloatArray;
    public virtual float RandomValueFloatArray() {return 0 == (ValueFloatArray?.Count??0) ? 0 : ValueFloatArray?[new Random().Next(ValueFloatArray.Count)]??0;}
    public virtual List<string> ValueStringArray => _ValueStringArray;
    public virtual string RandomValueStringArray() {return 0 == (ValueStringArray?.Count??0) ? "" : ValueStringArray?[new Random().Next(ValueStringArray.Count)]??"";}

    #region Repositories
    private int _tid;
    private int _ValueInt;
    private float _ValueFloat;
    private int _ValueBool;
    private int _ValueString;
    public List<int> _ValueIntArray = new List<int>();
    public List<float> _ValueFloatArray = new List<float>();
    public List<string> _ValueStringArray = new List<string>();
    #endregion

#pragma warning disable 649
#pragma warning disable 169
    private static string[] stringTable;
#pragma warning restore 169
#pragma warning restore 649

    public T_GlobalValueData(){}
    public T_GlobalValueData(int[] intTable, float[] floatTable, long[] longTable)
    {
        #region Constructor
        _tid = intTable[0];
        _ValueInt = intTable[1];
        _ValueFloat = floatTable[0];
        _ValueBool = intTable[2];
        _ValueString = intTable[3];
        //
        var tempValueIntArray = intTable[4];
        if(string.IsNullOrEmpty( stringTable[tempValueIntArray]) == false )
        {
            string[] arrayValueIntArray = stringTable[tempValueIntArray].Trim().Split(',');
            foreach (var iter in arrayValueIntArray)
            {
               string name = iter;
               name = name.Replace("[", "");
               name = name.Replace("]", "").Trim();
               if(string.IsNullOrEmpty(name) == false)
                   ValueIntArray.Add(int.Parse(name));
            }
        }
        //
        //
        var tempValueFloatArray = intTable[5];
        if(string.IsNullOrEmpty( stringTable[tempValueFloatArray]) == false )
        {
            string[] arrayValueFloatArray = stringTable[tempValueFloatArray].Trim().Split(',');
            foreach (var iter in arrayValueFloatArray)
            {
               string name = iter;
               name = name.Replace("[", "");
               name = name.Replace("]", "").Trim();
               if(string.IsNullOrEmpty(name) == false)
                   ValueFloatArray.Add(float.Parse(name));
            }
        }
        //
        //
        var tempValueStringArray = intTable[6];
        if(string.IsNullOrEmpty( stringTable[tempValueStringArray]) == false )
        {
            string[] arrayValueStringArray = stringTable[tempValueStringArray].Trim().Split(',');
            foreach (var iter in arrayValueStringArray)
            {
               string name = iter;
               name = name.Replace("[", "");
               name = name.Replace("]", "").Trim();
               if(string.IsNullOrEmpty(name) == false)
                   _ValueStringArray.Add(name);
            }
        }
        //
        #endregion
    }

    public static T_GlobalValueData Get(GlobalValueType tid) { return Excel.GetRow(SheetName.T_GlobalValueData, (int)tid) as T_GlobalValueData; }
    public static List<GlobalValueType> GetTIDs()
    {
        List<GlobalValueType> list = new List<GlobalValueType>();
        foreach(var iter in Excel.GetKeyList(SheetName.T_GlobalValueData))
        {
             list.Add((GlobalValueType)iter);
        }
        return list;
    }
    
    public static List<T_GlobalValueData> GetAll()
    {        
        return  Excel.GetList<T_GlobalValueData>(SheetName.T_GlobalValueData);
    }    
	
	public static T_GlobalValueData GetRandom()
    {        
		return Excel.GetRandom<T_GlobalValueData>(SheetName.T_GlobalValueData);
    }    
}
