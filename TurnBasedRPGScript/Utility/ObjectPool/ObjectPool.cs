using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPool : MonoBehaviour
{
    private readonly ConcurrentDictionary<string, Queue<GameObject>> objectPoolList
        = new ConcurrentDictionary<string, Queue<GameObject>>();

    private Transform objectPoolContainer;

    private readonly Queue<ObjectPoolData> loadList = new Queue<ObjectPoolData>();
    private bool isLoading;

    // ===============================
    // Initialize
    // ===============================
    public async UniTask InitializeAsync()
    {
        if (objectPoolContainer == null)
        {
            var container = new GameObject("objectPoolContainer");
            container.SetActive(false);
            container.transform.position = Vector3.zero;

            objectPoolContainer = container.transform;
            objectPoolContainer.SetParent(ObjectPoolManager.Instance.transform);
        }

        if (!isLoading)
        {
            isLoading = true;
            LoadPoolingLoopAsync().Forget();
        }

        await UniTask.CompletedTask;
    }

    // ===============================
    // New
    // ===============================
    public async UniTask<GameObject> NewAsync(
        string name,
        Transform parent,
        bool usePooling)
    {
        if (usePooling &&
            objectPoolList.TryGetValue(name, out var queue) &&
            queue.Count > 0)
        {
            var pooled = queue.Dequeue();
            pooled.transform.SetParent(parent);
            pooled.SetActive(true);
            pooled.transform.localPosition = Vector3.zero;
            return pooled;
        }

        var go = await CreateAsync(name, parent);

        // 기본 생성 개수만큼 백그라운드 preload
        EnqueuePreload(name, ObjectPoolManager.defaultCreateCount - 1);

        return go;
    }

    // ===============================
    // PreLoad (외부 호출)
    // ===============================
    public async UniTask PreLoadAsync(string name, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var go = await CreateAsync(name, objectPoolContainer);
            go.SetActive(false);

            var queue = objectPoolList.GetOrAdd(name, _ => new Queue<GameObject>());
            queue.Enqueue(go);
        }
    }

    // ===============================
    // Internal Create
    // ===============================
    private async UniTask<GameObject> CreateAsync(string name, Transform parent)
    {
        var handle = Addressables.InstantiateAsync(
            name,
            new Vector3(0, -100, 0),
            Quaternion.identity,
            parent
        );

        await handle.ToUniTask();

        var go = handle.Result;
        go.name = name;
        go.transform.localPosition = Vector3.zero;
        go.SetActive(true);

        return go;
    }

    // ===============================
    // Background Preload Loop
    // ===============================
    private async UniTaskVoid LoadPoolingLoopAsync()
    {
        while (true)
        {
            await UniTask.Yield();

            if (loadList.Count == 0)
                continue;

            var data = loadList.Dequeue();

            for (int i = 0; i < data.count; i++)
            {
                var go = await CreateAsync(data.name, objectPoolContainer);
                go.SetActive(false);

                var queue = objectPoolList.GetOrAdd(data.name, _ => new Queue<GameObject>());
                queue.Enqueue(go);
            }
        }
    }

    private void EnqueuePreload(string name, int count)
    {
        if (count <= 0)
            return;

        loadList.Enqueue(new ObjectPoolData
        {
            name = name,
            count = count
        });
    }

    // ===============================
    // Free
    // ===============================
    public void Free(GameObject go)
    {
        if (go == null)
            return;

        if (objectPoolList.TryGetValue(go.name, out var list))
        {
            go.SetActive(false);
            go.transform.SetParent(objectPoolContainer);
            list.Enqueue(go);
        }
        else
        {
            Destroy(go);
        }
    }

    // ===============================
    // Clear
    // ===============================
    public void Clear()
    {
        foreach (var iter in objectPoolList)
        {
            foreach (var obj in iter.Value)
            {
                Destroy(obj);
            }

            iter.Value.Clear();
        }

        objectPoolList.Clear();
    }
}
