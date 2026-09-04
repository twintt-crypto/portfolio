using UnityEditor.EditorTools;
using UnityEngine;

namespace S7
{
    public class EffectAutoRelease : MonoBehaviour
    {
        private float _duration;
        private float _elapsed;
        private bool _useTimer;
        private bool _running;
        private ParticleSystem[] _particleSystems;

        public void Setup(float duration)
        {
            _duration = duration;
            _elapsed = 0f;
            _running = true;
            _useTimer = duration > 0f;

            if (_useTimer == false)
                _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        }

        private void OnEnable()
        {
            _elapsed = 0f;
        }

        private void Update()
        {
            if (_running == false)
                return;

            if (_useTimer)
            {
                _elapsed += Time.deltaTime;
                if (_elapsed >= _duration)
                    Release();
            }
            else
            {
                if (IsAlive() == false)
                    Release();
            }
        }

        private bool IsAlive()
        {
            if (_particleSystems == null || _particleSystems.Length == 0)
                return false;

            for (int i = 0; i < _particleSystems.Length; i++)
            {
                ParticleSystem ps = _particleSystems[i];
                if (ps != null && ps.IsAlive(true))
                    return true;
            }

            return false;
        }

        private void Release()
        {
            _running = false;
            ResourceManager.Free(gameObject);
        }
    }

}
