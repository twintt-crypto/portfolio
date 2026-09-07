using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class SkillPreviewSpawner
{
    public static async UniTask<GameObject> Spawn(T_UnitData data, Transform parent)
    {
        if (data == null)
            return null;

        var handle = Addressables.InstantiateAsync(data.ModelPrefab, parent);

        var obj = await handle.Task;

        if (obj == null)
        {
            Debug.LogError($"Addressable 로드 실패: {data.ModelPrefab}");
            return null;
        }

        return obj;
    }
}