#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

public static class BoneCacheUtility
{
    public static string[] GetBoneNames(GameObject target)
    {
        if (target == null)
            return new[] { "Root" };

        var transforms = target.GetComponentsInChildren<Transform>(true);

        List<string> names = new List<string>();

        foreach (var t in transforms)
        {
            if (!names.Contains(t.name))
                names.Add(t.name);
        }

        // 기본값 보장
        if (!names.Contains("Root"))
            names.Insert(0, "Root");

        return names.ToArray();
    }
}
#endif