using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

static public class ResourceManager
{
    // ===============================
    // New (Pooling / Addressables 통합)
    // ===============================
    public static async UniTask<GameObject> NewAsync(string name, Transform parent = null, bool usePooling = false)
    {
        if (usePooling)
        {
            return await ObjectPoolManager.Instance.NewAsync(name, parent, usePooling);
        }

        return await AddressableManager.InstantiateAsync(name, parent);
    }

    // ===============================
    // Load Asset
    // ===============================
    public static async UniTask<T> LoadAssetAsync<T>(string assetName)
    {
        return await AddressableManager.LoadAssetAsync<T>(assetName);
    }

    // ===============================
    // Free
    // ===============================
    public static void Free(GameObject go)
    {
        ObjectPoolManager.Instance.Free(go);
    }

    // ===============================
    // Load Image
    // ===============================
    public static async UniTask<Sprite> LoadImageAsync(string name)
    {
        return await LoadAssetAsync<Sprite>(name);
    }

    // ===============================
    // Load TimeLine
    // ===============================
    public static async UniTask<PlayableDirector> LoadTimelineAsync(string key)
    {
        // 프리팹을 "씬에 생성"
        var go = await Addressables.InstantiateAsync(key);

        if (go == null)
            return null;

        var director = go.GetComponent<PlayableDirector>();

        if (director == null)
        {
            Addressables.ReleaseInstance(go);
            return null;
        }

        // 초기 상태 맞춤 (중요)
        director.time = 0;
        director.Evaluate();

        return director;
    }
}
