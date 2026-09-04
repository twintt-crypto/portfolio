using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TimelineAddressableProvider
{
    private class TimelineInstance
    {
        public GameObject instance;
        public PlayableDirector director;
        public AsyncOperationHandle<GameObject> handle;
    }

    private readonly Dictionary<string, TimelineInstance> _timelineMap = new();

    public async UniTask<PlayableDirector> GetAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            return null;

        if (_timelineMap.TryGetValue(key, out var cached))
        {
            if (cached != null && cached.instance != null && cached.director != null)
                return cached.director;

            _timelineMap.Remove(key);
        }

        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key);
        GameObject instance = await handle.Task;

        if (instance == null)
            return null;

        PlayableDirector director = instance.GetComponent<PlayableDirector>();
        if (director == null)
            director = instance.GetComponentInChildren<PlayableDirector>(true);

        if (director == null)
        {
            Addressables.ReleaseInstance(instance);
            Debug.LogWarning($"TimelineAddressableProvider : PlayableDirector ¾øÀ½. key={key}");
            return null;
        }

        _timelineMap[key] = new TimelineInstance
        {
            instance = instance,
            director = director,
            handle = handle
        };

        return director;
    }

    public void ReleaseTimelineImpl(PlayableDirector director)
    {
        if (director == null)
            return;

        Addressables.ReleaseInstance(director.gameObject);
    }

    public void ReleaseAll()
    {
        foreach (var pair in _timelineMap)
        {
            var item = pair.Value;
            if (item != null && item.instance != null)
                Addressables.ReleaseInstance(item.instance);
        }

        _timelineMap.Clear();
    }
}