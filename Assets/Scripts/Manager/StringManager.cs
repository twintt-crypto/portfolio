using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringManager : Singleton<StringManager>
{
    Dictionary<string, string> dicString = new Dictionary<string, string>();
    
    void Start()
    {
        DontDestroyOnLoad(this);        
    }    

    public void Initialize()
    {
        dicString.Clear();
        LoadLocalData();

        var data = T_StringData.GetAll();        
        foreach (var iter in data)
        {            
            switch (LocalizeManager.Language)
            {
                case SystemLanguage.Korean:
                    {
                        if (dicString.ContainsKey(iter.Key) == false)
                            dicString.Add(iter.Key, iter.KO.Replace("\\n", "\n"));
                    }
                    break;
                case SystemLanguage.Japanese:
                    {
                        if (dicString.ContainsKey(iter.Key) == false)
                            dicString.Add(iter.Key, iter.JP.Replace("\\n", "\n"));
                    }
                    break;
                default:
                    {
                        if (dicString.ContainsKey(iter.Key) == false)
                            dicString.Add(iter.Key, iter.EN.Replace("\\n", "\n"));
                    }
                    break;
            }
        }
    }

    public void LoadLocalData()
    {
        var data = T_LocalStringData.GetAll();
        foreach (var iter in data)
        {
            switch (LocalizeManager.Language)
            {
                case SystemLanguage.Korean:
                    {
                        if (dicString.ContainsKey(iter.Key) == false)
                            dicString.Add(iter.Key, iter.KO.Replace("\\n", "\n"));
                    }
                    break;
                case SystemLanguage.Japanese:
                    {
                        if (dicString.ContainsKey(iter.Key) == false)
                            dicString.Add(iter.Key, iter.JP.Replace("\\n", "\n"));
                    }
                    break;
                default:
                    {
                        if (dicString.ContainsKey(iter.Key) == false)
                            dicString.Add(iter.Key, iter.EN.Replace("\\n", "\n"));
                    }
                    break;
            }
        }    
    }

    public string GetString(string id)
    {
        if(dicString.TryGetValue(id, out string value) == false)
        {
            return "";
        }

        return value;
    }

    static public string Get(string id)
    {
        return StringManager.Instance.GetString(id);
    }    
}
