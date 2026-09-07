using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using S7.Game.Field;

namespace S7
{
    public class DayFieldSnapshot
    {
        public int fieldId;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
    }

    public partial class GameFlowManager : Singleton<GameFlowManager>
    {
        private DayFieldSnapshot _dayFieldSnapshot;

        public void RequestMoveNightField(int fieldId)
        {
            if (fieldId == 0) return;

            T_FieldData nightFieldData = T_FieldData.Get(fieldId);
            Debug.Log($"[GameFlowManager] RequestMoveNightField: fieldId={fieldId}");
            EnterNightField(nightFieldData).Forget();
        }

        private async UniTask EnterNightField(T_FieldData nightFieldData)
        {
            if (!IsFieldSceneLoaded) return;

            // 낮 → 밤: 스냅샷 저장 (밤 → 밤이면 기존 스냅샷 유지)
            if (_dayFieldSnapshot == null)
            {
                _dayFieldSnapshot = new DayFieldSnapshot
                {
                    fieldId = fieldContext.fieldData != null ? fieldContext.fieldData.TID : 0,
                    playerPosition = FieldManager.Instance.PlayerTransform != null ? FieldManager.Instance.PlayerTransform.position : Vector3.zero,
                    playerRotation = FieldManager.Instance.PlayerTransform != null ? FieldManager.Instance.PlayerTransform.rotation : Quaternion.identity,
                };
            }

            fieldContext.fieldData = nightFieldData;

            await GameSceneManager.Instance.TransitionAsync(
                async () =>
                {
                    await ChangeStateAsync(GameState.NightField);

                    // 낮 Area 씬 Unload
                    if (CurrentFieldAreaScene.IsValid()) await GameSceneManager.Instance.UnloadSceneAsync(CurrentFieldAreaScene);

                    // 배경 교체 (다를 경우)
                    if (prevBgName != nightFieldData.FieldBgName)
                    {
                        if (CurrentFieldBgScene.IsValid()) await GameSceneManager.Instance.UnloadSceneAsync(CurrentFieldBgScene);
                        prevBgName = nightFieldData.FieldBgName;

                        RegistFieldScene(FIELD_BG_PATH + nightFieldData.FieldBgName + ".unity", (scene) =>
                        {
                            CurrentFieldBgScene = scene;
                            SceneManager.SetActiveScene(scene);
                        });
                    }

                    // 밤 Area 씬 Load
                    RegistFieldScene(FIELD_AREA_PATH + nightFieldData.FieldAreaName + ".unity", (scene) =>
                    {
                        CurrentFieldAreaScene = scene;

                        foreach (GameObject go in scene.GetRootGameObjects())
                        {
                            AreaSceneData areaData = go.GetComponentInChildren<AreaSceneData>();
                            if (areaData != null)
                            {
                                FieldManager.Instance.SetArea(areaData);
                                break;
                            }
                        }
                    });

                    NightManager.Instance.Initialize();
                },
                async () =>
                {
                    var panel = await UIManager.Instance.OpenPanelAsync("UIPanelField") as UIPanelField;
                    if (panel != null) panel.SetField(true, UnitDataManager.Instance.PartyUnits);
                    FieldManager.Instance.SetAreaObjects();
                }
            );
        }

        public async UniTask ExitNightField()
        {
            if (_dayFieldSnapshot == null) return;

            int fieldId = _dayFieldSnapshot.fieldId;
            T_FieldData dayFieldData = T_FieldData.Get(fieldId);
            if (dayFieldData == null) return;

            await GameSceneManager.Instance.TransitionAsync(
                async () =>
                {
                    NightManager.Instance.Clear();

                    // 밤 Area 씬 Unload
                    if (CurrentFieldAreaScene.IsValid()) await GameSceneManager.Instance.UnloadSceneAsync(CurrentFieldAreaScene);

                    // 배경 교체 (다를 경우)
                    if (prevBgName != dayFieldData.FieldBgName)
                    {
                        if (CurrentFieldBgScene.IsValid()) await GameSceneManager.Instance.UnloadSceneAsync(CurrentFieldBgScene);
                        prevBgName = dayFieldData.FieldBgName;

                        RegistFieldScene(FIELD_BG_PATH + dayFieldData.FieldBgName + ".unity", (scene) =>
                        {
                            CurrentFieldBgScene = scene;
                            SceneManager.SetActiveScene(scene);
                        });
                    }

                    // 낮 Area 씬 Load
                    fieldContext.fieldData = dayFieldData;
                    RegistFieldScene(FIELD_AREA_PATH + dayFieldData.FieldAreaName + ".unity", (scene) =>
                    {
                        CurrentFieldAreaScene = scene;

                        foreach (GameObject go in scene.GetRootGameObjects())
                        {
                            AreaSceneData areaData = go.GetComponentInChildren<AreaSceneData>();
                            if (areaData != null)
                            {
                                FieldManager.Instance.SetArea(areaData);
                                break;
                            }
                        }
                    });

                    await ChangeStateAsync(GameState.Field);
                },
                async () =>
                {
                    var panel = await UIManager.Instance.OpenPanelAsync("UIPanelField") as UIPanelField;
                    if (panel != null) panel.SetField(false);
                    FieldManager.Instance.SetAreaObjects();

                    // 낮 필드 저장 위치로 복원
                    if (FieldManager.Instance.PlayerTransform != null)
                    {
                        FieldManager.Instance.PlayerTransform.position = _dayFieldSnapshot.playerPosition;
                        FieldManager.Instance.PlayerTransform.rotation = _dayFieldSnapshot.playerRotation;
                    }

                    _dayFieldSnapshot = null;
                }
            );
        }
    }
}
