using Cysharp.Threading.Tasks;
using UnityEditor.EditorTools;
using UnityEngine;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

namespace S7
{
    public static class EffectSpawner
    {
        public static async UniTask<GameObject> Spawn(T_EffectData effectData, Transform anchor, Transform forwardSource)
        {
            if (effectData == null)
                return null;

            if (string.IsNullOrEmpty(effectData.Prefab))
                return null;

            if (anchor == null)
                return null;

            var effect = await ResourceManager.NewAsync(effectData.Prefab, anchor, effectData.UsePooling);
            if (effect == null)
                return null;            

            effect.transform.SetParent(anchor);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.identity;
            effect.transform.localScale = Vector3.one;

            EffectAutoRelease autoRelease = effect.GetComponent<EffectAutoRelease>();
            if (autoRelease == null)
                autoRelease = effect.AddComponent<EffectAutoRelease>();

            autoRelease.Setup(effectData.Duration);

            return effect;
        }

        public static async UniTask<GameObject> Spawn(T_EffectData effectData, Vector3 position, Transform forwardSource)
        {
            if (effectData == null)
                return null;

            if (string.IsNullOrEmpty(effectData.Prefab))
                return null;

            var effect = await ResourceManager.NewAsync(effectData.Prefab, null, effectData.UsePooling);
            if (effect == null)
                return null;

            Quaternion rotation = Quaternion.LookRotation(forwardSource.forward);
            effect.transform.SetPositionAndRotation(position, rotation);

            EffectAutoRelease autoRelease = effect.GetComponent<EffectAutoRelease>();
            if (autoRelease == null)
                autoRelease = effect.AddComponent<EffectAutoRelease>();

            autoRelease.Setup(effectData.Duration);
            return effect;
        }

        public static async UniTask<GameObject> Spawn(string prefabKey, Vector3 position, Transform forwardSource, float duration)
        {
            var effect = await ResourceManager.NewAsync(prefabKey, null, true);
            if (effect == null)
                return null;

            Quaternion rotation = Quaternion.LookRotation(forwardSource.forward);
            effect.transform.SetPositionAndRotation(position, rotation);

            // Ãß°¡
            var particles = effect.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var p in particles)
            {
                p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                p.Play(true);
            }

            EffectAutoRelease autoRelease = effect.GetComponent<EffectAutoRelease>();
            if (autoRelease == null)
                autoRelease = effect.AddComponent<EffectAutoRelease>();

            autoRelease.Setup(duration);
            return effect;
        }
    }
}

