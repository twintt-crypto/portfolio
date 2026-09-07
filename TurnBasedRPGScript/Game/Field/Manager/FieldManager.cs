using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using S7.Game.Field;
using VContainer;
using System.Collections.Generic;

namespace S7
{
    public class FieldManager : Singleton<FieldManager>
    {
        [SerializeField] private FieldPlayer _player;
        
        public AreaSceneData CurrentArea { get; private set; }

        private PlayerManager _playerManager;
        private FieldCameraController _cameraController;

#if UNITY_EDITOR
        public void SwitchPlayerUnit(int unitId) => _playerManager.SwitchUnit(unitId);
        public void ChangePlayerAttackType(AttackType attackType) => _player.ChangeAttackType(attackType);
#endif

        private List<GameObject> hideObjects;
        private StateSnapshot _snapshot;

        public bool IsActive => CurrentArea != null;
        public Transform PlayerTransform => _player != null ? _player.transform : null;

        private int _entryIndex;

        [Inject]
        public void Construct(FieldCameraController cameraController)
        {
            _cameraController = cameraController;
            _playerManager = new PlayerManager(_player);
            _playerManager.RegistLoadResource();
            _playerManager.Initialize().Forget();
        }

        // scene preload 에서 동작. prefab 호출
        public void SetArea(AreaSceneData area, int entryIndex = 0)
        {
            CurrentArea = area;
            _entryIndex = entryIndex;
        }

        // scene onStart 에서 동작. object 세팅
        public void SetAreaObjects()
        {
            _playerManager.SwitchUnit();
            TeleportPlayerToEntry();
        }

        // 동적으로 포탈 목적지를 재설정. slotIndex 순서로 fieldId 배열 전달.
        public void ConfigurePortals(int[] fieldIds)
        {
            FieldPortal[] portals = CurrentArea.portals;
            if (portals == null) return;

            for (int i = 0; i < portals.Length; i++)
            {
                if (portals[i] == null) continue;
                int id = i < fieldIds.Length ? fieldIds[i] : 0;
                portals[i].Setup(id);
            }
        }

        private void CaptureAreaState()
        {
            _snapshot = new StateSnapshot();
            StateSaver.Capture(_snapshot, gameObject.scene, CurrentArea.gameObject.scene);
            Debug.Log($"[FieldManager] CaptureAreaState: {_snapshot.Count}개 저장");
        }

        private void RestoreAreaState()
        {
            if (_snapshot == null) return;
            StateSaver.Restore(_snapshot, gameObject.scene, CurrentArea.gameObject.scene);
            Debug.Log("[FieldManager] RestoreAreaState 완료");
        }

        public void ShowField()
        {
            if (hideObjects == null) return;

            foreach (GameObject go in hideObjects)
            {
                if (go == null) continue;
                go.SetActive(true);
            }

            hideObjects = null;
            RestoreAreaState();
        }
        
        public void HideField()
        {
            CaptureAreaState();
            hideObjects = new List<GameObject>();
            
            Scene bgScene = GameFlowManager.Instance.CurrentFieldBgScene;
            if (bgScene.IsValid() && bgScene.isLoaded)
            {
                foreach (GameObject go in bgScene.GetRootGameObjects())
                {
                    if (!go.activeSelf) continue;
                    go.SetActive(false);
                    hideObjects.Add(go);
                }
            }

            foreach (GameObject go in gameObject.scene.GetRootGameObjects())
            {
                // TODO: remove
                if (go.GetComponent<Camera>() != null) continue;

                if (!go.activeSelf) continue;
                go.SetActive(false);
                hideObjects.Add(go);
            }

            foreach (GameObject go in CurrentArea.gameObject.scene.GetRootGameObjects())
            {
                if (!go.activeSelf) continue;
                go.SetActive(false);
                hideObjects.Add(go);
            }
        }

        public void TeleportPlayerToEntry()
        {
            if (CurrentArea == null) return;

            _playerManager.MovePlayer(CurrentArea.GetEntry(_entryIndex));
            _cameraController.AlignToTarget();
        }

        public void ResetEnemies()
        {
            if (CurrentArea == null) return;

            foreach (FieldEnemy enemy in CurrentArea.Enemies)
                enemy.ResetToSpawn();
        }

        public void RemoveEnemy(List<int> enemyInstanceIds)
        {
            if (CurrentArea == null) return;

            foreach (FieldEnemy enemy in CurrentArea.Enemies)
            {
                if (enemy == null) continue;
                if (!enemyInstanceIds.Contains(enemy.GetInstanceID())) continue;
                enemy.Die();
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Test/Field State/Hide Field")]
        private static void TestHideField()
        {
            if (!Application.isPlaying) { Debug.LogWarning("[FieldState] 플레이 모드에서만 사용 가능합니다."); return; }
            Instance.HideField();
        }

        [UnityEditor.MenuItem("Test/Field State/Show Field")]
        private static void TestShowField()
        {
            if (!Application.isPlaying) { Debug.LogWarning("[FieldState] 플레이 모드에서만 사용 가능합니다."); return; }
            Instance.ShowField();
        }
#endif
    }
}
