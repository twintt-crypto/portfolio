using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

namespace S7
{
    public class UIPanelPatch : UIBase
    {
        private long _patchSize = 0;
        private Dictionary<string, (bool, long)> _progress = new Dictionary<string, (bool, long)>();

        [SerializeField] TextMeshProUGUI _version;

        [SerializeField] Slider _slider;
        [SerializeField] TextMeshProUGUI _textProgress;
        [SerializeField] TextMeshProUGUI _textDownload;

        [SerializeField] Button _btnLogin;

        protected override void Start()
        {
            _btnLogin.SetActive(false);
        }

        protected override void OnDestroy()
        {
        }

        public async UniTask Patch()
        {
            await UniTask.Delay(1000);
            /*
            #if !UNITY_EDITOR && UNITY_ANDROID && GAME_REAL
                await CheckForUpdate().ToUniTask();
            #endif*/

            await LoadLocalGameData();

            var labels = new List<string>() { "GameData", "Sprite", "Font", "UI", "Unit", "Presentation","Fx", "Event" };
            await CheckUpdate(labels);

            if (_patchSize > decimal.Zero)
            {
                await ProcessDownload(labels);
            }
            else
            {
                DOTween.To(() => 0.0f, x =>
                {
                    _textDownload.SetActive(true);
                    _textDownload.text = StringManager.Get("UI_LOAD_DATA");
                    _slider.value = x;
                    _textProgress.text = $"{x * 100:F2}%"; ;
                }, 0.8f, 0.5f).SetEase(Ease.Linear);

                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                await AddressableManager.LoadGameDataAsync(
                    _slider,
                    _textProgress,
                    0.8f,
                    1f);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            _slider.SetActive(false);
            _btnLogin.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            // TODO: remove temp
            UnitDataManager.Instance.InitializeTestParty();
            
            GameFlowManager.Instance.Initialize();
            QuestManager.Instance.Initialize();

            _btnLogin.onClick.AddListener(() =>
            {
                GameFlowManager.Instance.RequestMoveDayField(8);
            });
        }

        private async UniTask LoadLocalGameData()
        {
            AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>("GameData/T_LocalStringData_Client.Bytes");

            await handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                TextAsset textAsset = handle.Result;

                // byte ������ ����
                byte[] bytes = textAsset.bytes;

                GameData.Excel.LoadGameData(bytes, SheetName.T_LocalStringData);
                StringManager.Instance.LoadLocalData();
            }
            else
            {
                Debug.LogError("T_LocalStringData_Client.byte");
            }
        }

        private async UniTask CheckUpdate(List<string> labels)
        {
            _patchSize = 0;
            foreach (var label in labels)
            {                
                AsyncOperationHandle<IList<IResourceLocation>> locationHandle =
                    Addressables.LoadResourceLocationsAsync(label);

                await locationHandle;

                if (locationHandle.Status == AsyncOperationStatus.Succeeded &&
                    locationHandle.Result.Count > 0)
                {
                    AsyncOperationHandle<long> sizeHandle =
                        Addressables.GetDownloadSizeAsync(label);

                    await sizeHandle;

                    if (sizeHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        _patchSize += sizeHandle.Result;
                    }

                    Addressables.Release(sizeHandle);
                }

                Addressables.Release(locationHandle);
            }
        }


        /*#if !UNITY_EDITOR && UNITY_ANDROID && GAME_REAL
            private async UniTask CheckForUpdate()
            {
                Debug.Log("1");

                PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
                    appUpdateManager.GetAppUpdateInfo();

                Debug.Log("2");

                // PlayAsyncOperation �� await ����
                await appUpdateInfoOperation;

                Debug.Log("3");

                if (!appUpdateInfoOperation.IsSuccessful)
                {
                    Debug.LogError($"CheckForUpdate error : {appUpdateInfoOperation.Error}");
                    return;
                }

                var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

                Debug.Log($"AvailableVersionCode : {appUpdateInfoResult.AvailableVersionCode}");
                Debug.Log($"bundleVersionCode : {GetAndroidVersionCode()}");

                if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
                {
                    var appUpdateOption = AppUpdateOptions.ImmediateAppUpdateOptions();

                    // ��� ������Ʈ ����
                    await StartImmediateUpdate(appUpdateInfoResult, appUpdateOption);
                }
            }
        #endif*/

        private async UniTask ProcessDownload(List<string> labels)
        {
            List<UniTask> downloadTasks = new List<UniTask>();

            foreach (var label in labels)
            {
                AsyncOperationHandle<long> sizeHandle =
                    Addressables.GetDownloadSizeAsync(label);

                await sizeHandle;

                if (sizeHandle.Status == AsyncOperationStatus.Succeeded &&
                    sizeHandle.Result > 0)
                {
                    // label ���� �ٿ�ε� ����
                    downloadTasks.Add(Download(label));
                }

                Addressables.Release(sizeHandle);
            }

            await UniTask.WhenAll(downloadTasks);
            
            await CheckDownload();

            _textDownload.text = StringManager.Get("UI_LOAD_DATA");


            await AddressableManager.LoadGameDataAsync(
                _slider,
                _textProgress,
                0.8f,
                1f);
        }

        private async UniTask Download(string label)
        {
            _progress.Add(label, (false, 0));

            AsyncOperationHandle handle =
                Addressables.DownloadDependenciesAsync(label, false);

            while (!handle.IsDone)
            {
                var status = handle.GetDownloadStatus();
                _progress[label] = (false, status.DownloadedBytes);

                // yield return null �� ����
                await UniTask.Yield();
            }

            var finalStatus = handle.GetDownloadStatus();
            _progress[label] = (true, finalStatus.TotalBytes);

            Addressables.Release(handle);
        }

        private string FormatSize(long sizeInBytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (sizeInBytes >= GB)
            {
                return $"{(sizeInBytes / (float)GB):F2} GB";
            }
            else if (sizeInBytes >= MB)
            {
                return $"{(sizeInBytes / (float)MB):F2} MB";
            }
            else if (sizeInBytes >= KB)
            {
                return $"{(sizeInBytes / (float)KB):F2} KB";
            }
            else
            {
                return $"{sizeInBytes} Bytes";
            }
        }

        private async UniTask CheckDownload()
        {
            long total = 0;

            _slider.value = 0.0f;
            _textProgress.text = "0%";
            _textDownload.text = $"({FormatSize(total)}/{FormatSize(_patchSize)})";
            _textDownload.SetActive(true);

            while (true)
            {
                total += _progress.Sum(tmp => tmp.Value.Item2);

                float ratio = (float)total / _patchSize * 0.5f;

                _slider.value = ratio;
                _textProgress.text = $"{ratio * 100:0.00}%";
                _textDownload.text = $"({FormatSize(total)}/{FormatSize(_patchSize)})";

                // ��� �ٿ�ε� �Ϸ�
                if (_progress.Values.Count(x => x.Item1 == false) == 0)
                {
                    break;
                }

                total = 0;

                // yield return null �� ���� (���� ������)
                await UniTask.Yield();
            }
        }
    }

}


