using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gpm.Ui;
using S7;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIPanelLoading : UIBase
{
    [SerializeField] private Slider _progress;
    [SerializeField] private TextMeshProUGUI _textProgress;

    protected override void Start()
    {

    }

    protected override void OnDestroy()
    {
    }

    public async UniTask RegistResourceLoadAsync()
    {
        int maxCount = GameSceneManager.Instance.LoadDataCount();

        int loadedIndex = 0;

        for (int i = 0; i < maxCount; i++)
        {
            var data = GameSceneManager.Instance.GetLoadData();

            await data.LoadAsync(progress =>
            {
                float totalProgress = (loadedIndex + progress) / maxCount;

                _progress.value = totalProgress;
                _textProgress.text = $"{totalProgress * 100f:0.00}%";
            });

            // 현재 리소스 로딩 완료
            loadedIndex++;
        }

        _progress.value = 1f;
        _textProgress.text = "100.00%";
    }
}

