using Cysharp.Threading.Tasks;
using GameEventSystem;
using NUnit.Framework.Internal;
using System.Collections.Generic;

namespace S7
{
    public partial class TurnManager
    {
        public TurnState State { get; private set; }

        public UnitController Current => _order[_index];

        public async UniTask ChangeState(TurnState newState)
        {
            State = newState;
            EventManager.BroadCasting(new EventTarget(GameEventSystem.EventType.TurnStateChange), State, turnContext);
            switch (State)
            {
                case TurnState.StartBattle:
                    {

                    }
                    break;
                case TurnState.SelectSkill:
                    {

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

                    }
                    break;
                case TurnState.EndTurn:
                    {

                    }
                    break;                
                case TurnState.PlayerTurn:
                    {

                    }
                    break;
                case TurnState.Attack:
                    {
                        foreach(var target in turnContext.targets)
                        {
                            target.view.SetSelected(false);
                        }
                    }
                    break;
                case TurnState.EnemyTurn:
                    {

                    }
                    break;
            }

            await UniTask.CompletedTask;
        }

        public void RequestSkill(UnitSkill skill)
        {
            turnContext.selectSkill = skill;
        }

        public void RequestTarget(UnitController unitController)
        {
            turnContext.targets.Add(unitController);
        }
    }



}
