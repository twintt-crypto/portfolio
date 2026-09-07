using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameEventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Video;

namespace S7
{
    public partial class BattleManager : MonoBehaviour
    {
        private async UniTask RunBattle(CancellationToken token)
        {
            // 처음 적을 보는 연출
            await UniTask.Delay(2000);

            foreach (var iter in _unit.Allies)
            {
                iter.data.Reset();
            };

            foreach (var iter in _unit.Enemies)
            {
                iter.data.Reset();
            }

            while (!IsBattleEnd())
            {
                token.ThrowIfCancellationRequested();

                await PlayTurn();
                _turn.NextTurn();
            }
        }

        private bool IsBattleEnd()
        {
            return _unit.Enemies.All(x => x.data.IsDead);
        }

        //한탄의 대한 플로우
        private async UniTask PlayTurn()
        {
            //현재의 유닛
            var unit = _turn.Current;

            //턴정보 생성
            _turn.CreateTurnContext(unit);
            if (unit == null || unit.IsDead())
            {
                return;
            }

            await _turn.ChangeState(TurnState.StartBattle);
            
            // 전투 시작            
            unit.BuffManager.OnTurnStart();

            // 현재 턴 실행
            if (unit.data.unitType == UnitType.Character)
            {
                //아군턴
                await ExecuteAllyTurn(unit, _battleCts.Token);
            }
            else
            {
                //적군턴
                await ExecuteEnemyTurn(unit, _battleCts.Token);
            }

            unit.BuffManager.OnTurnEnd();

            await _turn.ChangeState(TurnState.EndTurn);
        }

        //아군턴
        private async UniTask ExecuteAllyTurn(UnitController unit, CancellationToken token)
        {
            await _turn.ChangeState(TurnState.PlayerTurn);

            token.ThrowIfCancellationRequested();

            // 현재턴 unit 선택
            SelectPlayer(unit.view);
            List<UnitController> targets = new List<UnitController>();
            if (_turn.turnContext.targets == null || _turn.turnContext.targets.Count == 0)
            {
                var enemy = _unit.Enemies[0];
                enemy.view.SetSelected(true);
                _turn.turnContext.targets.Add(enemy);
            }

            targets = _turn.turnContext.targets;

            LookAtTarget(unit.view, _unit.Enemies);

            token.ThrowIfCancellationRequested();

            await _battleCameraManager.SetCamera(BattleCameraType.EnemySingle, unit.view, targets[0].view, Vector2.zero);

            token.ThrowIfCancellationRequested();

            await SelectTarget(targets[0].view);

            // 스킬 선택 대기
            await _turn.ChangeState(TurnState.SelectSkill);   
            
            UnitSkill skill = await WaitPlaySkill(unit);
            _turn.SetSelectSkill(skill);

            token.ThrowIfCancellationRequested();

            targets = _turn.turnContext.targets;

            await _battleCameraManager.SetAttackCamera(unit.view, targets[0].view);

            token.ThrowIfCancellationRequested();

            await _turn.ChangeState(TurnState.Attack);

            token.ThrowIfCancellationRequested();

            Vector3 oriPos = unit.view.transform.position;
            
            // 스킬 사용
            await _skill.UseSkill(unit, skill, targets, token);

            _battleCameraManager.CurrentCamera.camera.SetActive(false);

            token.ThrowIfCancellationRequested();

            await unit.view.MoveToAsync(oriPos, 300, Ease.OutQuad, token);

            unit.view.transform.DOLocalRotate(Vector3.zero, 0.0f);
            unit.view.OnlyPlayAnimationAsync(Animator.StringToHash("Battle_Idle"), token).Forget();

        }

        //적군턴
        private async UniTask ExecuteEnemyTurn(UnitController enemy, CancellationToken token)
        {
            await _turn.ChangeState(TurnState.EnemyTurn);
            token.ThrowIfCancellationRequested();

            // 적이 사용할 스킬 선택
            UnitSkill skill = null;
            try
            {
                await _turn.ChangeState(TurnState.EnemySelectSkill);
                skill = await WaitEnemyPlaySkill(enemy);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }            

            token.ThrowIfCancellationRequested();

            // play 캐릭터 선택
            await _turn.ChangeState(TurnState.EnemySelectTarget);            

            var targets = waitEnemyPlayTarget(enemy);

            _turn.SetTargets(targets);
            _turn.SetSelectSkill(skill);            

            ShowAllyUnit(targets[0].view);

            token.ThrowIfCancellationRequested();
            await _battleCameraManager.SetCamera(BattleCameraType.EnemySingle, targets[0].view, enemy.view, Vector2.zero);

            token.ThrowIfCancellationRequested();

            Vector3 oriPos = enemy.view.transform.position;

            await _turn.ChangeState(TurnState.EnemyAttack);

            token.ThrowIfCancellationRequested();

            await _skill.UseSkill(enemy, skill, targets, token);

            token.ThrowIfCancellationRequested();

            await enemy.view.MoveToAsync(oriPos, 300, Ease.OutQuad, token);            

            enemy.view.transform.DOLocalRotate(Vector3.zero, 0.0f);
            enemy.view.OnlyPlayAnimationAsync(Animator.StringToHash("Battle_Idle"), token).Forget();
        }
    }

}
