using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Effect Data")]
public class EffectData : ScriptableObject
{
    public GameObject prefab;
    public string address;
    public string AttachSocket;
    public float duration = 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (prefab == null)
        {
            address = null;
            return;
        }

        address = GetOrCreateAddress(prefab);
        EditorUtility.SetDirty(this);
    }

    private string GetOrCreateAddress(GameObject prefab)
    {
        string path = AssetDatabase.GetAssetPath(prefab);
        string guid = AssetDatabase.AssetPathToGUID(path);

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            return null;

        var entry = settings.FindAssetEntry(guid);

        if (entry == null)
        {
            var group = settings.DefaultGroup;
            entry = settings.CreateOrMoveEntry(guid, group);
        }

        string address = path
            .Replace("Assets/_RemoteData/", "");            

        entry.address = address;

        EditorUtility.SetDirty(settings);

        return address;
    }
#endif
}