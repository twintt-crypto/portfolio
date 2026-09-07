using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace  S7
{
    public class SceneBase : MonoBehaviour
    {
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void FirstLoad()
        {
            Application.targetFrameRate = 60;
            var sceneName = SceneManager.GetActiveScene().name;
            if (Enum.TryParse<SceneType>(sceneName, out SceneType name) == false)
            {
                return;
            }

            if (sceneName.Equals("SceneRoot") == false)
            {
                SceneManager.LoadScene("SceneRoot");
            }
        }
#endif


        //start ¥Ÿ¿Ω   

        public async UniTask SceneInitializeAsync()
        {
            Initialize();
            await PreLoadAsync();
        }

        public virtual void Initialize()
        {
            ObjectPoolManager.Instance.Clear();
        }

        public virtual void OnStart()
        {
            Resources.UnloadUnusedAssets();
        }

        public virtual UniTask DisposeAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask PreLoadAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual void Awake() { }
        protected virtual void OnDestroy() { }
    }
}
