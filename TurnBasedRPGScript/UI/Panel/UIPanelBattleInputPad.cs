using Cysharp.Threading.Tasks;
using GameEventSystem;
using Gpm.Ui;
using JetBrains.Annotations;
using Newtonsoft.Json.Serialization;
using NUnit.Framework.Internal;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

namespace S7
{
    public class UIPanelBattleInputPad : UIPanelBattleBase
    {
        //¸â¹ö ±³Ã¼
        [SerializeField] Button _btnMemberChange;

        [Header("Attack")]
        [SerializeField] Button _btnAttack;
        [SerializeField] Button _btnSkillAttack;
        [SerializeField] Button _btnSpecialAttack;

        [Header("Defance")]
        [SerializeField] Button _Parrying;
        //[SerializeField] Button _dodge;        

        protected override void Awake()
        {
            _btnAttack.onClick.AddListener(OnClickAttack);
            _btnSkillAttack.onClick.AddListener(OnClickSkillAttack);
            _btnSpecialAttack.onClick.AddListener(OnClickSpecialAttack);

            _Parrying.onClick.AddListener(OnClickParrying);
            //_dodge.onClick.AddListener(OnClickDodge);

            EventManager.AddEventReceiver<TurnState, TurnContext>(new EventTarget(GameEventSystem.EventType.TurnStateChange), ChangeTurn);
            EventManager.AddEventReceiver<int>(new EventTarget(GameEventSystem.EventType.UpdateAp), UpdateAp);
        }

        public void OnClickAttack()
        {
            _battleManager?.HandleInput(BattleInputAction.Attack);
        }

        public void OnClickSkillAttack()
        {
            _battleManager?.HandleInput(BattleInputAction.SkillAttack);
        }

        public void OnClickSpecialAttack()
        {
            _battleManager?.HandleInput(BattleInputAction.SpecialAttack);
        }
        
        public void OnClickParrying()
        {
            _battleManager?.HandleInput(BattleInputAction.Parry);
        }
        public void OnClickDodge()
        {
            _battleManager?.HandleInput(BattleInputAction.Dodge);
        }

        protected override void OnDestroy()
        {
            EventManager.RemoveEventReceiver<TurnState, TurnContext>(new EventTarget(GameEventSystem.EventType.TurnStateChange), ChangeTurn);
        }

        private void ChangeTurn(TurnState state, TurnContext ctx)
        {
            Debug.Log($"Change Turn : {state}");

            switch (state)
            {
                case TurnState.StartBattle:
                    {

                    }
                    break;
                case TurnState.PlayerTurn:
                    {
                        _btnAttack.SetActive(true);
                        _btnSkillAttack.SetActive(true);
                        _btnSpecialAttack.SetActive(true);

                        _Parrying.SetActive(false);                                                
                    }
                    break;
                case TurnState.SelectSkill:
                    {

                    }
                    break;
                case TurnState.Attack:
                    {

                    }
                    break;
                case TurnState.EnemyTurn:
                    {
                        _btnAttack.SetActive(false);
                        _btnSkillAttack.SetActive(false);
                        _btnSpecialAttack.SetActive(false);

                        _Parrying.SetActive(false);
                    }
                    break;
                case TurnState.EnemySelectSkill:
                    {
                        
                    }

                    break;
                case TurnState.EnemySelectTarget:
                    {

                    }
                    break;
                case TurnState.EnemyAttack:
                    {
                        if(ctx.selectSkill.skillData.ReactiveType == ReactiveType.Parry)
                        {
                            _Parrying.SetActive(true);
                        }
                        else if(ctx.selectSkill.skillData.ReactiveType == ReactiveType.Dodge)
                        {

                        }                                                
                    }
                    break;
                case TurnState.EndTurn:
                    {

                    }
                    break;
            }
        }

        public void UpdateAp(int ap)
        {

        }
    }
}
