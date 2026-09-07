using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public static class SkillGraphFinder
{
    private const string TARGET_PATH = "Assets/_RemoteData/Presentation/Skill";

    public static List<PresentationGraphAsset> LoadAll()
    {
        List<PresentationGraphAsset> list = new();

        // 해당 폴더에서 타입 검색
        string[] guids = AssetDatabase.FindAssets(
            "t:PresentationGraphAsset",
            new[] { TARGET_PATH });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            var asset = AssetDatabase.LoadAssetAtPath<PresentationGraphAsset>(path);
            if (asset != null)
                list.Add(asset);
        }

        return list;
    }
}