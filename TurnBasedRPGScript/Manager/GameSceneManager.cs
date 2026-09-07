using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace S7
{
	public enum SceneType
	{
		None = 0,
		ScenePatch,
		SceneField,
		SceneBattle,
	}

	public partial class GameSceneManager : Singleton<GameSceneManager>
	{
		public SceneType currentSceneType = SceneType.None;

		private SceneBase currentSceneBase;

		private Scene currentScene;

		private bool isSceneLoading = false;

		// 씬로드
		public async UniTask LoadScene(SceneType nextScene, string loadingBg = "")
		{
			await TransitionAsync(
				async () => { await LoadSceneInternalAsync(nextScene); }
			);
		}

		// TransitionAsync 없이 씬 로드만 수행
		public async UniTask LoadSceneInternalAsync(SceneType nextScene)
		{
			//1. 현재 씬이 있으면 해지
			if (currentScene.isLoaded == true)
			{
				var unloadAO = SceneManager.UnloadSceneAsync(currentScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
				if (unloadAO != null)
					await unloadAO.ToUniTask();
				currentSceneBase = null;
			}

			//2. 새로운 씬 로드
			var ao = SceneManager.LoadSceneAsync(nextScene.ToString(), LoadSceneMode.Additive);
			//ao.allowSceneActivation = false;
			await UniTask.WaitUntil(() => ao.progress >= 0.9f);
			//ao.allowSceneActivation = true;
			await UniTask.WaitUntil(() => ao.isDone);
			if (currentSceneBase != null)
				await currentSceneBase.DisposeAsync();

			currentScene = SceneManager.GetSceneByName(nextScene.ToString());
			currentSceneType = nextScene;
			var roots = new List<GameObject>();
			currentScene.GetRootGameObjects(roots);
			currentSceneBase = roots
				.Find(x => x.CompareTag("Scene"))
				?.GetComponent<SceneBase>();

			if (currentSceneBase == null)
			{
				Debug.LogError("SceneBase not found (check tag Scene)");
				return;
			}

			await currentSceneBase.SceneInitializeAsync();
		}

		public async UniTask TransitionAsync(
			Func<UniTask> setup,
			Func<UniTask> onBeforeFadeIn = null,
			Func<UniTask> fadeOut = null,
			Func<UniTask> fadeIn = null)
		{
			if (isSceneLoading) return;
			isSceneLoading = true;

			try
			{
				if (fadeOut != null) await fadeOut();
				else await UIManager.Instance.FadeOutAsync();

				Time.timeScale = 0;
				UIManager.Instance.CloseAll();
				UIPopupManager.Instance.HidePopupAll();

				await setup();

				if (LoadDataCount() > 0)
				{
					UIPanelLoading loading = await UIManager.Instance.OpenPanelAsync("UIPanelLoading") as UIPanelLoading;
					if (loading == null)
						return;

                    await UIManager.Instance.FadeInAsync();
                    await loading.RegistResourceLoadAsync();
					await UIManager.Instance.FadeOutAsync();

					UIManager.Instance.CloseAll();
					UIPopupManager.Instance.HidePopupAll();
				}

				Time.timeScale = 1;
				currentSceneBase?.OnStart();
				if (onBeforeFadeIn != null) await onBeforeFadeIn();

				if (fadeIn != null) await fadeIn();
				else await UIManager.Instance.FadeInAsync();
			}
			finally
			{
				Time.timeScale = 1;
				isSceneLoading = false;
			}
		}

		public void RegistSceneLoadResource(string sceneName, Action<Scene> OnLoaded)
		{
			RegistLoadResource(new LoadResourceData<Scene>()
			{
				assetName = sceneName,
				loadType = ResourceLoadType.Scene,
				OnLoaded = OnLoaded
			});
		}

		public async UniTask<AsyncOperationHandle<SceneInstance>> LoadAdditiveSceneAsync(string sceneName)
		{
			var handle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			await handle.ToUniTask();
			return handle;
		}

		public async UniTask UnloadSceneAsync(Scene scene)
		{
			await SceneManager.UnloadSceneAsync(scene);
		}

		public void SetSceneActive(Scene scene, bool active)
        {
            if (!scene.isLoaded)
                return;

            foreach (var root in scene.GetRootGameObjects())
            {
                root.SetActive(active);
            }
        }
	}
}
