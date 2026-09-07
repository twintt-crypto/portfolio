using Cysharp.Threading.Tasks;
using UnityEngine;

namespace S7.Game.Field
{
    public class ProjectileLauncher
    {
        private static string SpawnEvent = "SpawnArrow";
        private static string ShootEvent = "ShootArrow";

        private readonly int _projectileId;
        private readonly AnimationEventReceiver _animEventReceiver;
        private readonly Transform _arrowSocket;
        private readonly CombatColliderController _shooter;

        private GameObject _spawnedArrow;
        private T_ProjectileData _data;

        private Transform _target = null;

        public ProjectileLauncher(int projectileId, AnimationEventReceiver receiver, Transform arrowSocket, CombatColliderController shooter)
        {
            _projectileId = projectileId;
            _animEventReceiver = receiver;
            _arrowSocket = arrowSocket;
            _shooter = shooter;
            _animEventReceiver?.Register(SpawnEvent, OnSpawnArrow);
            _animEventReceiver?.Register(ShootEvent, OnShootArrow);
            
            _data = T_ProjectileData.Get(_projectileId);
        }

        public void Dispose()
        {
            _animEventReceiver?.Unregister(SpawnEvent, OnSpawnArrow);
            _animEventReceiver?.Unregister(ShootEvent, OnShootArrow);
        }

        private void OnSpawnArrow()
        {
            SpawnArrowAsync().Forget();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private async UniTaskVoid SpawnArrowAsync()
        {
            if (_arrowSocket == null) return;

            _spawnedArrow = await ResourceManager.NewAsync(_data.Prefab, _arrowSocket, usePooling: true);
            if (_spawnedArrow == null) return;

            _spawnedArrow.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        private void OnShootArrow()
        {
            if (_spawnedArrow == null) return;
            
            // 없으면 앞으로
            Vector3 direction = _target == null ? 
                _shooter.transform.forward : 
                (_target.position - _spawnedArrow.transform.position + new Vector3(0, _spawnedArrow.transform.position.y, 0)) .normalized;
            
            _spawnedArrow.transform.SetParent(null);

            FieldArrow arrow = _spawnedArrow.AddComponent<FieldArrow>();
            arrow.Launch(direction, 10f, _shooter); // temp speed

            _spawnedArrow = null;
        }
    }
}
