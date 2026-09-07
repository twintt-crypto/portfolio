using Cysharp.Threading.Tasks;
using S7;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TextCore.Text;

public class PreviewUnitController
{
    private T_UnitData _UnitData;
    public UnitView View;

    private bool _initialize = false;

    public bool isInitialize { get => _initialize;}

    public async UniTask Initialize(T_UnitData data, Transform parent)
    {
        _UnitData = data;
        await PreviewCharacter(parent);
    }
   
    public async UniTask PreviewCharacter(Transform parent)
    {
        var go = await Addressables.InstantiateAsync("UnitView", parent);

        View = go.GetComponent<UnitView>();

        await View.Initialize(_UnitData, parent);

        _initialize = true;
    }

    public void Release()
    {
        if (View != null)
        {
            Addressables.ReleaseInstance(View.Model);
            Addressables.ReleaseInstance(View.gameObject);
        }
    }
}
