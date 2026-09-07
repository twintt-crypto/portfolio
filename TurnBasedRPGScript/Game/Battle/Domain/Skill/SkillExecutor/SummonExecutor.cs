using UnityEngine;

namespace S7
{
    public class SummonExecutor : ISkillActionExecutor
    {
        public SkillActionType ActionType => SkillActionType.Summon;

        public SkillResult Execute(ActionContext ctx)
        {
            SkillResult result = new SkillResult();
            return result;
        }
    }

}
