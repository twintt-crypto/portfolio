using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AddressableUtil.Addressable
{
    public class AddressableWrapper
    {
        public static string remotePath
        {
            get
            {                
                string path = "https://cdn.";

                Debug.Log("RemotePath : " + path);
                return path;
            }
        }
#if UNITY_EDITOR
        public static string catalogVersion
        {
            get
            {
                string version = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.OverridePlayerVersion;
                Debug.Log("[Debug] PlayerBuildVersion : " + version);
                return version;
            }
        }
#endif
    }
}