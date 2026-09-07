using Cysharp.Threading.Tasks;
using GameEventSystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace S7
{
    public partial class BattleManager : MonoBehaviour
    {
        private UniTaskCompletionSource<UnitSkill> _skillTcs;
        private UniTaskCompletionSource<UnitController> _unitTcs;

        public UniTask<UnitSkill> WaitPlaySkill(UnitController unit)
        {            
            _skillTcs = new UniTaskCompletionSource<UnitSkill>();

            return _skillTcs.Task;
        }

        public UniTask<UnitSkill> WaitEnemyPlaySkill(UnitController unit)
        {
            
            _skillTcs = new UniTaskCompletionSource<UnitSkill>();

            var skill = GetRandomSkill(unit.skills);
            _skillTcs?.TrySetResult(skill);
            return _skillTcs.Task;
        }

        public UnitSkill GetRandomSkill(Dictionary<int, UnitSkill> skills)
        {
            if (skills == null || skills.Count == 0)
                return null;

            var list = new List<UnitSkill>(skills.Values);
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        // 적이 
        public List<UnitController> waitEnemyPlayTarget(UnitController unit)
        {            
            List<UnitController> targets = new List<UnitController>();

            targets.Add(_unit.Allies[0]);
            return targets;
        }

        public async Task HandleInput(BattleInputAction InputAction)
        {
            switch (InputAction)
            {
                case BattleInputAction.Attack:
                    {
                        if (_turn.State == TurnState.SelectSkill)
                        {
                            _skillTcs?.TrySetResult(_turn.Current.GetAttackSkill());
                        }
                    }
                    break;
                case BattleInputAction.SkillAttack:
                    {

                    }
                    break;
                case BattleInputAction.SpecialAttack:
                    {

                    }
                    break;
                case BattleInputAction.UltimateAttack:
                    {

                    }
                    break;
                case BattleInputAction.Parry:
                    {                        
                        if (_turn.State == TurnState.EnemyAttack)
                        {
                            foreach (var unit in _turn.turnContext.targets)
                            {
                                await unit.view.PlayParry();
                            }
                        }
                    }
                    break;
            }
        }
    }
}


