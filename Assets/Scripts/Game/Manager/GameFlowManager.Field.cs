using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using S7.Game.Field;

namespace S7
{
    public partial class GameFlowManager : Singleton<GameFlowManager>
    {
        private const string FIELD_AREA_PATH = "FieldArea/";
        private const string FIELD_BG_PATH = "FieldBg/";
        
        public Scene CurrentFieldAreaScene { get; private set; }
        public Scene CurrentFieldBgScene { get ; private set; }

        public FieldContext fieldContext = new FieldContext();

        public bool IsFieldSceneLoaded { get ; private set; }
        private string prevBgName;

        // field to field
        public void RequestMoveDayField(int fieldId)
        {
            if (fieldId == 0) return;

            fieldContext.fieldData = T_FieldData.Get(fieldId);
            Debug.Log($"[GameFlowManager] RequestMoveDayField: fieldId={fieldId}");
            MoveField(fieldContext.fieldData.FieldBgName, fieldContext.fieldData.FieldAreaName).Forget();
        }

        // battle to field
        public void RequestReturnField(BattleResultType battleResult, BattleContext battleContext = null)
        {
            MoveField(battleResult, battleContext).Forget();
        }

        #if UNITY_EDITOR
        // for cheat
        public void RequestMoveDayField(string bgName, string areaName)
        {
            MoveField(bgName, areaName).Forget();
        }
        #endif
        
        // end battle
        private async UniTask MoveField(BattleResultType battleResult, BattleContext battleContext)
        {
            if (!IsFieldSceneLoaded)
            {
                Debug.LogError("[GameFlowManager.Field] FieldScene is not loaded");
                return;
            }
            
            GameState returnState = _dayFieldSnapshot != null ? GameState.NightField : GameState.Field;

            await GameSceneManager.Instance.TransitionAsync(
                async () =>
                {
                    await ChangeStateAsync(returnState);

                    switch (battleResult)
                    {
                        case BattleResultType.VICTORY:
                            if(battleContext != null && battleContext.battleEnemyFields != null) FieldManager.Instance.RemoveEnemy(battleContext.battleEnemyFields);
                            break;
                        case BattleResultType.DEFEAT:
                            break;
                        case BattleResultType.ESCAPE:
                            FieldManager.Instance.TeleportPlayerToEntry();
                            FieldManager.Instance.ResetEnemies();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(battleResult), battleResult, null);
                    }
                },
                async () =>
                {
                    bool isNight = returnState == GameState.NightField;
                    var panel = await UIManager.Instance.OpenPanelAsync("UIPanelField", showImmediately: false) as UIPanelField;
                    if (panel != null) panel.SetField(isNight, isNight ? UnitDataManager.Instance.PartyUnits : null);
                    UIManager.Instance.ShowPanel("UIPanelField");
                }
            );
        }

        // field to field
        private async UniTask MoveField(string bgName, string areaName)
        {
            await GameSceneManager.Instance.TransitionAsync(
                async () =>
                {
                    // 1. FieldScene 로드 (최초 1회)
                    if (!IsFieldSceneLoaded)
                    {
                        await GameSceneManager.Instance.LoadSceneInternalAsync(SceneType.SceneField);
                        IsFieldSceneLoaded = true;
                    }

                    // 2. field bg/area regist
                    await ChangeStateAsync(GameState.Field);

                    // 배경 같을 경우 재사용
                    if (prevBgName != bgName)
                    {
                        if (CurrentFieldBgScene.IsValid()) await GameSceneManager.Instance.UnloadSceneAsync(CurrentFieldBgScene);
                        prevBgName = bgName;

                        RegistFieldScene(FIELD_BG_PATH + bgName + ".unity", (scene) =>
                        {
                            CurrentFieldBgScene = scene;
                            SceneManager.SetActiveScene(scene);
                        });
                    }

                    if (CurrentFieldAreaScene.IsValid()) await GameSceneManager.Instance.UnloadSceneAsync(CurrentFieldAreaScene);
                    RegistFieldScene(FIELD_AREA_PATH + areaName + ".unity", (scene) =>
                    {
                        CurrentFieldAreaScene = scene;

                        foreach (GameObject go in scene.GetRootGameObjects())
                        {
                            AreaSceneData areaData = go.GetComponentInChildren<AreaSceneData>();
                            if (areaData != null)
                            {
                                FieldManager.Instance.SetArea(areaData, fieldContext.fieldData == null ? 0 : fieldContext.fieldData.FieldEntryPoint);
                                break;
                            }
                        }
                    });
                },
                async () =>
                {
                    var panel = await UIManager.Instance.OpenPanelAsync("UIPanelField", showImmediately: false) as UIPanelField;
                    if (panel != null) panel.SetField(false);
                    UIManager.Instance.ShowPanel("UIPanelField");
                    FieldManager.Instance.SetAreaObjects();
                }
            );
        }

        public void RegistFieldScene(string sceneName, Action<Scene> onLoaded)
        {
            GameSceneManager.Instance.RegistSceneLoadResource(sceneName, onLoaded);
        }
    }
}
