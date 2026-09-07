using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextKeySetup : MonoBehaviour
{
    public string _stringID;
    public string _outConvertText;
    private TextMeshProUGUI _text;        
    
    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        LoadData(LocalizeManager.Language);
    }

    // Update is called once per frame
    private void LoadData(SystemLanguage language)
    {
        _text = GetComponent<TextMeshProUGUI>();

        if (_stringID.Equals(""))
        {
            return;
        }

        string convertText = StringManager.Get(_stringID.Trim());
        if (_text == null)
        {
            _text = GetComponent<TextMeshProUGUI>();
        }        

        if (convertText == null)
        {
            convertText = $"#{_stringID}";
        }
        else
        {
            _outConvertText = convertText;
        }

        if (_text != null)
        {
           _text.text = convertText;
        }
    }    


    private void OnDestroy()
    {
       
    }
}
