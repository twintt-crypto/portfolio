using Cysharp.Threading.Tasks;
using GameEventSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace S7
{
    public class UIPanelBattle : UIPanelBattleBase
    {
        [SerializeField] Transform _subPanel;

        [SerializeField] Button _exit;

        [Header("Target Marker")]
        [SerializeField] private RectTransform _targetMarkerPrefab;

        private TurnState _currentTurnState;
        private TurnContext _currentTurnContext;        
        private RectTransform _canvasRect;

        //UI
        private UIPanelBattleInputPad _battleInputPad;
        private UIPanelBattleUnitSlots _battleUnitSlots;
        private UIBattleTurnOrderBar _battleTurnOrderBar;

        protected override void Awake()
        {
            _canvasRect = UIManager.Instance.Canvas.GetComponent<RectTransform>();
        }

        public override async UniTask Initialize(BattleManager battleManager)
        {
            await base.Initialize(battleManager);

            _exit.onClick.AddListener(() =>
            {
                GameFlowManager.Instance.ExitBattle();

            });

            EventManager.AddEventReceiver<UnitView>(
                new EventTarget(GameEventSystem.EventType.BattleSelectTarget), OnSelectTarget);

            await LoadBattleUI();
        }

        private async UniTask LoadBattleUI()
        {
            _battleInputPad = await UIManager.Instance.OpenPanelAsync("UIPanelBattleInputPad", _subPanel) as UIPanelBattleInputPad;
            await _battleInputPad.Initialize(_battleManager);

            _battleUnitSlots = await UIManager.Instance.OpenPanelAsync("UIPanelBattleUnitSlots", _subPanel) as UIPanelBattleUnitSlots;
            await _battleUnitSlots.Initialize(_battleManager);

            _battleTurnOrderBar = await UIManager.Instance.OpenPanelAsync("UIPanelBattleTurnOrderBar", _subPanel) as UIBattleTurnOrderBar;

            #if UNITY_EDITOR
            // TODO: Remove
            await InitializeDummy();
            #endif
        }

        public void SetupBattleUnits()
        {
            List<BattleUnitInfo> allies = new List<BattleUnitInfo>();
            foreach (UnitController unit in _battleManager.Allies)
            {
                allies.Add(CreateUnitInfo(unit));
            }
            _battleUnitSlots.SetUnits(allies);

            List<BattleUnitInfo> turnOrder = new List<BattleUnitInfo>();
            foreach (UnitController unit in _battleManager._turn.Order)
            {
                turnOrder.Add(CreateUnitInfo(unit));
            }
            _battleTurnOrderBar.Build(turnOrder);

            /*EventManager.BroadCasting<long>(
                new EventTarget(GameEventSystem.EventType.TurnStateChange),
                _battleManager._turn.Current.data.unitKey);*/
        }

        private BattleUnitInfo CreateUnitInfo(UnitController unit)
        {
            T_UnitData unitData = T_UnitData.Get(unit.data.unitId);
            BattleUnitInfo info = new BattleUnitInfo();
            info.unitKey = unit.data.unitKey;
            info.unitId = unit.data.unitId;
            info.name = unitData != null ? unitData.Name : "";
            info.hp = (int)unit.data._stat.hp;
            info.maxHp = (int)unit.data.MaxHp;
            info.speed = unit.data._stat.Speed;
            info.isPlayer = unit.data.unitType == UnitType.Character;
            return info;
        }

        private void BattleTurnState(TurnState state, TurnContext context)
        {
            _currentTurnState = state;
            _currentTurnContext = context;
        }

        private void OnSelectTarget(EventTarget target, UnitView view)
        {

        }        
        
        public void HideTargetMarkers()
        {
         
        }

        protected override void OnDestroy()
        {
            EventManager.RemoveEventReceiver<UnitView>(
                new EventTarget(GameEventSystem.EventType.BattleSelectTarget), OnSelectTarget);

            UIManager.Instance.ClosePanel(_battleUnitSlots);
            UIManager.Instance.ClosePanel(_battleTurnOrderBar);
            UIManager.Instance.ClosePanel(_battleInputPad);
        }

#if UNITY_EDITOR
        public async UniTask InitializeDummy()
        {
            _exit.onClick.AddListener(() => UIManager.Instance.ClosePanel(this));

            var allies = CreateDefaultAllies();
            var enemies = CreateDefaultEnemies();
            var all = new List<BattleUnitInfo>(allies);
            all.AddRange(enemies);
            all.Sort((a, b) => b.speed.CompareTo(a.speed));

            _battleUnitSlots.SetUnits(allies);
            _battleTurnOrderBar.Build(all);
        }

        [ContextMenu("Dummy/Initialize")]
        void ContextInitializeDummy() => InitializeDummy().Forget();

        [ContextMenu("SetupBattleUnits")]
        void ContextSetupBattleUnits() => SetupBattleUnits();

        static List<BattleUnitInfo> CreateDefaultAllies() => new List<BattleUnitInfo>
    {
        new BattleUnitInfo { unitId=1, name="전사",   hp=120, maxHp=120, speed=80, isPlayer=true },
        new BattleUnitInfo { unitId=2, name="마법사", hp=80,  maxHp=80,  speed=60, isPlayer=true },
        new BattleUnitInfo { unitId=3, name="궁수",   hp=90,  maxHp=100, speed=90, isPlayer=true },
        new BattleUnitInfo { unitId=4, name="성직자", hp=70,  maxHp=70,  speed=50, isPlayer=true },
    };

        static List<BattleUnitInfo> CreateDefaultEnemies() => new List<BattleUnitInfo>
    {
        new BattleUnitInfo { unitId=5, name="고블린A", hp=60,  maxHp=60,  speed=70, isPlayer=false },
        new BattleUnitInfo { unitId=6, name="고블린B", hp=60,  maxHp=60,  speed=55, isPlayer=false },
        new BattleUnitInfo { unitId=7, name="오크",    hp=150, maxHp=150, speed=40, isPlayer=false },
        new BattleUnitInfo { unitId=8, name="마법사E", hp=70,  maxHp=70,  speed=85, isPlayer=false },
    };
#endif
    }

}
