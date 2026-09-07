using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;


namespace S7
{
    public partial class GameFlowManager : Singleton<GameFlowManager>
    {
        Scene _currentBattleScene;
        public Scene CurrentBattleScene { get => _currentBattleScene; }

        Scene _currentBattleBackgroundScene;
        public Scene CurrentBattleBackgroundScene { get => _currentBattleBackgroundScene; }        
        
        public BattleContext BattleContext;
        
        BattleManager _battleManager;        

        public async UniTask EnterBattle()
        {
            if(BattleContext == null)
            {
                return;
            }

            T_BattleSceneData sceneData = T_BattleSceneData.Get(BattleContext.stageId);
            if (sceneData == null)
                return;

            // TODO: remove temp
            Time.timeScale = 0.1f;
            
            await GameSceneManager.Instance.TransitionAsync(
                async () =>
                {
                    await ChangeStateAsync(GameState.Battle);

                    //씬
                    await AddBattleSceneAsync(sceneData.BattleSceneName);

                    //배경
                    await AddBattleBackgroundSceneAsync(sceneData.BattleGroundSceneName);

                    _battleManager = FindFirstObjectByType<BattleManager>();
                    if (_battleManager == null) return;

                    _battleManager.SetPresentationCore(PresentationCore);

                    //UI Hud
                    var panel = await UIManager.Instance.OpenPanelAsync("UIPanelBattle", showImmediately: false) as UIPanelBattle;
                    if (panel == null) return;

                    await panel.Initialize(_battleManager);
                    await _battleManager.ReadyBattle(BattleContext);
                },
                async () =>
                {
                    Time.timeScale = 1f;
                    UIManager.Instance.ShowPanel("UIPanelBattle");
                    _battleManager?.StartTurn();
                    await UniTask.CompletedTask;
                }
            );
        }

        public void ExitBattle(BattleResultType result = BattleResultType.ESCAPE)
        {
            RequestReturnField(result, BattleContext);
        }
        
        public async UniTask RequestBattle(BattleContext battleContext)
        {
            if (CurrentState == GameState.Battle) return;
            
            BattleContext = battleContext;
            await EnterBattle();
        }

        public async UniTask RequestBattle(int stageId = 1, List<int> battleIds = null, List<int> fieldEnemyIds = null)
        {
            BattleContext battleContext = new BattleContext();

            // ally
            battleContext.allyUnit = new List<UnitData>(UnitDataManager.Instance.PartyUnits);

            // enemy
            // TODO: change to load by battleIds
            battleContext.enemyUnit = UnitDataManager.CreateTestEnemies();

            battleContext.stageId = stageId;
            battleContext.battleIds = battleIds;
            battleContext.battleEnemyFields = fieldEnemyIds;

            await RequestBattle(battleContext);
        }

        public async UniTask LoadTestBattle(int stageId = 1, List<int> battleIds = null, List<int> fieldEnemyIds = null)
        {

        }

        private async UniTask AddBattleSceneAsync(string sceneName)
        {
            var handle = await GameSceneManager.Instance.LoadAdditiveSceneAsync(sceneName);
            _currentBattleScene = handle.Result.Scene;
        }

        private async UniTask AddBattleBackgroundSceneAsync(string sceneName)
        {
            var handle = await GameSceneManager.Instance.LoadAdditiveSceneAsync(sceneName);
            _currentBattleBackgroundScene = handle.Result.Scene;
            SceneManager.SetActiveScene(_currentBattleBackgroundScene);
        }
    }
}
