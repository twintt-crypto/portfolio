using System.Globalization;
using UnityEngine;


public static class LocalizeManager
{
    private static SystemLanguage _language = SystemLanguage.Unknown;
    private static CultureInfo _cultureInfo = null;
    
    public static bool IsChangeLanguage { get; set; } = false;

    private const string LANGS_KEY = "Language";         

    public static SystemLanguage Language
    {
        get
        {
            if (_language == SystemLanguage.Unknown)
            {
                // load
               _language = (SystemLanguage)PlayerPrefs.GetInt(LANGS_KEY, (int)_language);

                if (_language == SystemLanguage.Unknown)
                {
                    _language = Application.systemLanguage;
                    // save
                    PlayerPrefs.SetInt(LANGS_KEY, (int)_language);
                    System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo;
                }
            }

            return _language;
        }
        set
        {
            _language = value;
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo;
            // save
            PlayerPrefs.SetInt(LANGS_KEY, (int)_language);
            _cultureInfo = null;

            ChangeLanguage();
        }
    }

    public static CultureInfo CultureInfo
    {
        get
        {
            if (_cultureInfo == null)
            {
                _cultureInfo = CreateCultureInfo();
            }

            return _cultureInfo;
        }
    }

    private static CultureInfo CreateCultureInfo()
    {
        switch (Language)
        {
            case SystemLanguage.Korean: return new CultureInfo("ko-KR");
            case SystemLanguage.Japanese: return new CultureInfo("ja-JP");
            case SystemLanguage.Thai: return new CultureInfo("th-TR");
            case SystemLanguage.Indonesian: return new CultureInfo("id-ID");
            case SystemLanguage.Vietnamese: return new CultureInfo("vi-VN");
            default: return new CultureInfo("en-US");
        }
    }

    public static void ChangeLanguage()
    {        
        //
    }    
}
