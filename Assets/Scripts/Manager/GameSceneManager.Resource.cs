using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace S7
{
    public interface ILoadResourceData
    {
        UniTask LoadAsync(Action<float> onProgress = null);
    }

    public class LoadResourceData<T> : ILoadResourceData
    {
        public string assetName;
        public ResourceLoadType loadType;

        public int count = 1;
        public bool isLoaded = false;
        public Action<T> OnLoaded;

        public async UniTask LoadAsync(Action<float> onProgress = null)
        {
            switch (loadType)
            {
                case ResourceLoadType.Scene:
                    {
                        var handle = Addressables.LoadSceneAsync(assetName, LoadSceneMode.Additive);

                        while (!handle.IsDone)
                        {
                            float progress = Mathf.Clamp01(handle.PercentComplete / 0.9f);

                            // ÁøÇà·ü Àü´Þ
                            onProgress?.Invoke(progress);

                            await UniTask.Yield();
                        }

                        var sceneInstance = handle.Result;

                        OnLoaded?.Invoke((T)(object)sceneInstance.Scene);
                        break;
                    }

                case ResourceLoadType.Prefab:
                    {
                        await ObjectPoolManager.Instance.PreLoadAsync(
                            assetName,
                            count);
                        break;
                    }                
            }

            isLoaded = true;
        }
    }

    public partial class GameSceneManager : Singleton<GameSceneManager>
    {
        private Queue<ILoadResourceData> _resourcesLoadDatas = new Queue<ILoadResourceData>();
        public void RegistLoadResource(ILoadResourceData data)
        {
            _resourcesLoadDatas.Enqueue(data);
        }
        public void ClearLoadData()
        {
            _resourcesLoadDatas.Clear();
        }

        private string _bgName = "";

        public string BgName { get => _bgName; set => _bgName = value; }

        public ILoadResourceData GetLoadData()
        {
            if (_resourcesLoadDatas.Count == 0)
            {
                return null;
            }

            return _resourcesLoadDatas.Dequeue();
        }

        public bool IsLoadDataEmpty()
        {
            return _resourcesLoadDatas.Count == 0 ? true : false;
        }

        public int LoadDataCount()
        {
            return _resourcesLoadDatas.Count;
        }

    }
}

