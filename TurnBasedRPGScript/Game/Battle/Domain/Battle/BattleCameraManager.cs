using Cysharp.Threading.Tasks;
using GameEventSystem;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace S7
{
    public enum BattleCameraType
    {
        EnemySingle,            //나를 팔로우하고 적을 보고 있는 카메라
        FllowEnemySingle,       //적을 보고 쫒아가는 카메라
        TargetAlly,             //아군 전체를 보는 카메라
        EnemyWide               //적 전체를 보는 카메라
    }

    public class BattleCameraManager : MonoBehaviour
    {
        [System.Serializable]
        public class CameraEntry
        {
            public BattleCameraType type;
            public CinemachineCamera camera;
        }

        public CameraEntry CurrentCamera { get; private set; }

        [SerializeField] private List<CameraEntry> cameras;

        private Dictionary<BattleCameraType, CameraEntry> _map;

        public void Awake()
        {
            _map = new Dictionary<BattleCameraType, CameraEntry>();
            foreach (var entry in cameras)
            {
                _map[entry.type] = entry;
                entry.camera.Priority = -1; // 초기 비활성
            }
        }

        public CameraEntry Get(BattleCameraType type)
        {
            return _map[type];
        }

        public async UniTask SetCamera(BattleCameraType type)
        {
            if (CurrentCamera != null)
            {
                CurrentCamera.camera.SetActive(false);
                await UniTask.Yield();
            }

            var camera = Get(type);
            if (camera == null)
            {
                return;
            }

            camera.camera.gameObject.SetActive(true);
            CurrentCamera = camera;
        }

        public async UniTask SetCamera(BattleCameraType battleCameraType, UnitView follow, UnitView lookAt, Vector2 damping)
        {
            await SetCamera(battleCameraType);
            var camera = Get(battleCameraType);
            if (camera == null)
            {
                return;
            }

            camera.camera.SetActive(true);
            CinemachineCamera cc = camera.camera.GetComponent<CinemachineCamera>();

            if (cc == null)
            {
                return;
            }

            cc.Follow = follow.TargetPoint;
            cc.LookAt = lookAt.TargetPoint;

            var composer = cc.GetComponent<CinemachineRotationComposer>();
            if (composer == null)
            {
                return;
            }

            composer.Damping = damping;
        }

        public async UniTask SetAttackCamera(UnitView caster, UnitView target)
        {
            var camera = Get(BattleCameraType.FllowEnemySingle);
            if (camera == null)
            {
                return;
            }

            camera.camera.SetActive(true);
            CinemachineCamera cc = camera.camera.GetComponent<CinemachineCamera>();

            if (cc == null)
            {
                return;
            }

            cc.Follow = caster.transform;
            cc.LookAt = target.transform;

            await UniTask.Yield();

            CurrentCamera.camera.SetActive(false);
            CurrentCamera = camera;
        }

        public void SetCameraLookAt(BattleCameraType battleCameraType, UnitView lookAt, Vector2 damping)
        {
            var camera = Get(battleCameraType);
            if (camera == null)
            {
                return;
            }

            CinemachineCamera cc = camera.camera.GetComponent<CinemachineCamera>();

            if (cc == null)
            {
                return;
            }

            cc.LookAt = lookAt.TargetPoint;
            var composer = cc.GetComponent<CinemachineRotationComposer>();
            if (composer == null)
            {
                return;
            }

            composer.Damping = damping;
        }

        public void SetVcamDamping(BattleCameraType battleCameraType, Vector2 damping)
        {
            var camera = Get(battleCameraType);
            if (camera == null)
            {
                return;
            }

            var composer = camera.camera.GetComponent<CinemachineRotationComposer>();
            if (composer == null)
            {
                return;
            }

            composer.Damping = damping;
        }
    }

}
