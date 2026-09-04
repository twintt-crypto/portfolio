using DG.Tweening;
using GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class AddressableManager : SingletonInMemory<AddressableManager>
{
    private Dictionary<string, IResourceLocation> _assetReferences = new();

    public Dictionary<string, IResourceLocation> AssetReferences
    {
        get => _assetReferences;
        set => _assetReferences = value;
    }

    // ===============================
    // GameData Load (UniTask)
    // ===============================
    public static async UniTask LoadGameDataAsync(
        Slider progress,
        TextMeshProUGUI textProgress,
        float min,
        float max)
    {
        var locationHandle = Addressables.LoadResourceLocationsAsync("GameData");
        await locationHandle.ToUniTask();

        if (locationHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load resource locations: " + locationHandle.DebugName);
            return;
        }

        var dotween = DOTween.To(
            () => min,
            x =>
            {
                progress.value = x;
                textProgress.text = $"{x * 100:F2}%";
            },
            max,
            0.2f
        ).SetEase(Ease.Linear);

        Dictionary<SheetName, bool> loaded = new();

        foreach (var location in locationHandle.Result)
        {
            Debug.Log("Resource location: " + location.PrimaryKey);

            int indexOfUnderscore = location.PrimaryKey.IndexOf("_Client");
            if (indexOfUnderscore == -1)
                continue;

            string result = location.PrimaryKey
            .Replace("GameData/", "")
            .Replace("_Client.bytes", "");

            if (!Enum.TryParse(result, out SheetName sheetName))
                continue;

            if (!loaded.ContainsKey(sheetName))
                loaded.Add(sheetName, false);

            LoadSheetAsync(location, sheetName, loaded).Forget();
        }

        await UniTask.WaitUntil(() => loaded.All(x => x.Value));

        dotween.Kill();
        progress.value = max;
        textProgress.text = $"{max * 100:F2}%";
    }

    private static async UniTaskVoid LoadSheetAsync(
        IResourceLocation location,
        SheetName sheetName,
        Dictionary<SheetName, bool> loaded)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(location);
        await handle.ToUniTask();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            byte[] bytes = handle.Result.bytes;
            Excel.LoadGameData(bytes, sheetName);
            loaded[sheetName] = true;
        }

        Addressables.Release(handle);
    }

    // ===============================
    // Instantiate
    // ===============================
    public static async UniTask<GameObject> InstantiateAsync(string assetName, Transform parent = null)
    {
        var handle = parent == null
            ? Addressables.InstantiateAsync(assetName)
            : Addressables.InstantiateAsync(assetName, parent);

        await handle.ToUniTask();

        handle.Result.name = assetName;
        return handle.Result;
    }

    // ===============================
    // Load Asset
    // ===============================
    public static async UniTask<T> LoadAssetAsync<T>(string assetName)
    {
        var handle = Addressables.LoadAssetAsync<T>(assetName);
        await handle.ToUniTask();
        return handle.Result;
    }

    public static T LoadAsset<T>(string assetName)
    {
        return Addressables.LoadAssetAsync<T>(assetName).WaitForCompletion();
    }

    // ===============================
    // Scene
    // ===============================
    public static async UniTask<SceneInstance> LoadSceneAsync(string name, LoadSceneMode mode)
    {
        var handle = Addressables.LoadSceneAsync(name, mode);
        await handle.ToUniTask();
        return handle.Result;
    }

    public static async UniTask UnloadSceneAsync(SceneInstance scene)
    {
        var handle = Addressables.UnloadSceneAsync(scene);
        await handle.ToUniTask();

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("언로드 실패");
        }
    }

    // ===============================
    // Cache
    // ===============================
    public async UniTask ClearCacheAsync()
    {
        foreach (var locator in Addressables.ResourceLocators)
        {
            var handle = Addressables.ClearDependencyCacheAsync(locator.Keys, false);
            await handle.ToUniTask();
            Addressables.Release(handle);
        }

        Caching.ClearCache();
        await Addressables.UpdateCatalogs();
    }

    public static async UniTask LoadGameDataAsync()
    {
        await Addressables.InitializeAsync();
        Excel.ClearGameData();

        var locationHandle = Addressables.LoadResourceLocationsAsync("GameData");
        await locationHandle.ToUniTask();

        if (locationHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load resource locations: " + locationHandle.DebugName);
            return;
        }        

        Dictionary<SheetName, bool> loaded = new();

        foreach (var location in locationHandle.Result)
        {
            Debug.Log("Resource location: " + location.PrimaryKey);

            int indexOfUnderscore = location.PrimaryKey.IndexOf("_Client");
            if (indexOfUnderscore == -1)
                continue;

            string result = location.PrimaryKey
            .Replace("GameData/", "")
            .Replace("_Client.bytes", "");

            if (!Enum.TryParse(result, out SheetName sheetName))
                continue;

            if (!loaded.ContainsKey(sheetName))
                loaded.Add(sheetName, false);

            LoadSheetAsync(location, sheetName, loaded).Forget();
        }

        await UniTask.WaitUntil(() => loaded.All(x => x.Value));
    }
}
