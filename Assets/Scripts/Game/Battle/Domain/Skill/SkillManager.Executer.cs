using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace S7
{
    public partial class SkillManager
    {
        private Dictionary<SkillActionType, ISkillActionExecutor> _executorMap = new();

        private void InitializeSkillAction(IEnumerable<UnitController> units)
        {            
            //Register(new AttackExecutor(units));
            Register(new AttackExecutor());
            Register(new HealExecutor());
            Register(new BuffExecutor());
            Register(new SummonExecutor());
        }

        private void Register(ISkillActionExecutor actionExecutor)
        {
            _executorMap.Add(actionExecutor.ActionType, actionExecutor);
        }

        private SkillResult Executer(ActionContext context)
        {
            if (!_executorMap.TryGetValue(context.unitSkill.skillData.ActionType, out var executor))
            {
                Debug.LogError($"No Executor for {context.unitSkill.skillData.ActionType}");
                return null;
            }

            return executor.Execute(context);
        }
    }
}

