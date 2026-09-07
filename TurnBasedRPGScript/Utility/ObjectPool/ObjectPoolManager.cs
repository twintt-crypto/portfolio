using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public const int defaultCreateCount = 5;

    private ObjectPool pool;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // ===============================
    // Initialize
    // ===============================

    public void Initialize()
    {
        InitializeAsync().Forget();
    }

    private async UniTask InitializeAsync()
    {
        if (pool != null)
            return;

        pool = gameObject.AddComponent<ObjectPool>();
        await pool.InitializeAsync();
    }

    // ===============================
    // New (async / return)
    // ===============================
    public async UniTask<GameObject> NewAsync(
        string name,
        Transform parent,
        bool usePooling = false)
    {
        if (pool == null)
        {
            Debug.LogError("ObjectPoolManager not initialized");
            return null;
        }

        return await pool.NewAsync(name, parent, usePooling);
    }

    // ===============================
    // PreLoad
    // ===============================
    public async UniTask PreLoadAsync(string name, int count)
    {
        if (pool == null)
        {
            Debug.LogError("ObjectPoolManager not initialized");
            return;
        }

        await pool.PreLoadAsync(name, count);
    }

    // ===============================
    // Free
    // ===============================
    public void Free(GameObject go)
    {
        pool?.Free(go);
    }

    public void Clear()
    {
        pool?.Clear();
    }

    private void OnDestroy()
    {
        pool?.Clear();
    }
}
