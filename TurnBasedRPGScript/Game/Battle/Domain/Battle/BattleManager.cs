using Cysharp.Threading.Tasks;
using GameEventSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using VContainer;

namespace S7
{
    public partial class BattleManager : MonoBehaviour
    {
        private void Awake()
        {
            _sceneData = FindFirstObjectByType<BattleSceneData>();
            if (_sceneData == null)
            {
                Debug.LogError("BattleSceneData not found in BattleScene");
            }
        }

        private BattleUnitManager _unit;
        public TurnManager _turn;
        private SkillManager _skill;        


        public IReadOnlyList<UnitController> Allies => _unit.Allies;
        public IReadOnlyList<UnitController> Enemies => _unit.Enemies;
        public IEnumerable<UnitController> AllUnits => _unit.AllUnits;

        

        private BattleStage _battleStage;

        private PresentationCore _presentationCore;

        [SerializeField] private BattleCameraManager _battleCameraManager;

        private BattleSceneData _sceneData;

        [SerializeField] private BattleSlot[] allySlots;
        [SerializeField] private BattleSlot[] enemySlots;

        private CancellationTokenSource _battleCts;
        private UniTask _battleTask;

        public void SetPresentationCore(PresentationCore presentationCore)
        {
            _presentationCore = presentationCore;
        }

        public async UniTask ReadyBattle(BattleContext battleContext)
        {
            _battleStage = new BattleStage();
            _battleStage.Initialize(_sceneData.AnchorProvider);

            _unit = new BattleUnitManager(battleContext.allyUnit, battleContext.enemyUnit);
            await _unit.BuildUnits(_battleStage);

            //턴초기화
            _turn = new TurnManager();
            _turn.Initialize(battleContext, _unit.AllUnits);

            _skill = new SkillManager(_presentationCore);
            //스킬 초기화        
            _skill.Initialize(battleContext, _unit.AllUnits);

            //연출 초기화                
            await _battleCameraManager.SetCamera(BattleCameraType.EnemyWide);
        }

        public void StartTurn()
        {
            _battleCts = new CancellationTokenSource();
            //연출이 플레이 중이면 대기한다.        
            _battleTask = RunBattle(_battleCts.Token).Preserve();
        }

        bool isEnd = false;

        public async UniTask EndBattleAsync()
        {
            if (isEnd == true)
            {
                return;
            }

            isEnd = true;

            if (_battleCts == null)
                return;

            _battleCts.Cancel();

            await UniTask.Yield();
            _battleCts.Dispose();
            _battleCts = null;
        }

        public void OnSkillSelected(UnitSkill skill)
        {
            _turn.RequestSkill(skill);
        }

        public async UniTask SelectTarget(UnitView view)
        {
            if (_turn.State != TurnState.SelectSkill)
            {
                return;
            }

            await UniTask.NextFrame();
            foreach (var unit in _unit.Enemies)
            {
                if (view == unit.view)
                {
                    _turn.SetTarget(unit);
                    _battleCameraManager.SetCameraLookAt(BattleCameraType.EnemySingle, unit.view, new Vector2(1, 1));
                    unit.view.SetSelected(true);
                    EventManager.BroadCasting(new EventTarget(GameEventSystem.EventType.BattleSelectTarget), unit.view);                    
                }
                else
                {
                    unit.view.SetSelected(false);
                }
            }
        }

        public void SelectPlayer(UnitView view)
        {
            if (view == null)
            {
                return;
            }

            if (view.gameObject == null)
            {
                return;
            }

            //view.SetSelected(true);
            ShowAllyUnit(view);
        }

        public void ShowAllyUnit(UnitView view)
        {
            if (view == null)
            {
                return;
            }

            if (view.gameObject == null)
            {
                return;
            }

            bool isFind = false;
            foreach (var unit in _unit.Allies)
            {
                if (unit.view == view)
                {
                    isFind = true;
                    unit.view.SetActive(true);
                    continue;
                }

                if (isFind == true)
                {
                    unit.view.SetActive(false);
                }
                else
                {
                    unit.view.SetActive(true);
                }
            }
        }

        public void LookAtTarget(UnitView view, IReadOnlyList<UnitController> targets)
        {
            if (view == null)
            {
                return;
            }

            if (view.gameObject == null)
            {
                return;
            }

            view.LookAtAsync(GetTargetsCenter(targets)).Forget();
        }

        private Vector3 GetTargetsCenter(IReadOnlyList<UnitController> targets)
        {
            Vector3 sum = Vector3.zero;
            int count = 0;

            foreach (var t in targets)
            {
                if (t?.view == null)
                    continue;

                sum += t.view.transform.position;
                count++;
            }

            if (count == 0)
                return Vector3.zero;

            return sum / count;
        }

        public List<UnitView> GetSelectableUnits()
        {
            List<UnitView> list = new List<UnitView>();

            foreach (var unit in _unit.Enemies)
            {
                list.Add(unit.view);
            }

            return list;
        }



        private void OnDestroy()
        {
        }

#if UNITY_EDITOR
        [GameButton("Test Victory")]
        private void TestVictory()
        {
            if (!Application.isPlaying) { Debug.LogWarning("[BattleManager] 플레이 모드에서만 사용 가능합니다."); return; }
            GameFlowManager.Instance.ExitBattle(BattleResultType.VICTORY);
        }
#endif
    }

}
