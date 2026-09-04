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
    public enum GameState
    {
        None,
        Field,
        NightField,
        Story,
        Battle,
        UI,
        Loading
    }


    public partial class GameFlowManager : Singleton<GameFlowManager>
    {
        public GameState CurrentState { get; private set; }

        public event Action<GameState> OnStateChanged;

        // 각 state 가 켜질때 직접 호출하기
        public async UniTask ChangeStateAsync(GameState newState)
        {
            if (_isTransitioning)
                return;

            if (CurrentState == newState)
                return;

            _isTransitioning = true;

            try
            {
                await ExitState(CurrentState);

                CurrentState = newState;

                await EnterState(newState);

                OnStateChanged?.Invoke(newState);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        private async UniTask EnterState(GameState state)
        {            
            switch (state)
            {
                case GameState.Field:
                    FieldManager.Instance.ShowField();
                    break;

                case GameState.NightField:
                    FieldManager.Instance.ShowField();
                    break;

                case GameState.Story:
                    break;

                case GameState.Battle:
                    break;
            }

            await UniTask.CompletedTask;
        }

        private async UniTask ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Field:
                    FieldManager.Instance.HideField();
                    //InputLockSystem.Unlock();
                    break;

                case GameState.NightField:
                    FieldManager.Instance.HideField();
                    break;

                case GameState.Story:
                    //StorySystem.Skip();
                    break;

                case GameState.Battle:
                    if (_battleManager != null) await _battleManager.EndBattleAsync();
                    if (_currentBattleScene.IsValid() && _currentBattleScene.isLoaded)
                        await GameSceneManager.Instance.UnloadSceneAsync(_currentBattleScene);
                    if (_currentBattleBackgroundScene.IsValid() && _currentBattleBackgroundScene.isLoaded)
                        await GameSceneManager.Instance.UnloadSceneAsync(_currentBattleBackgroundScene);
                    break;
            }
        }
    }
}
