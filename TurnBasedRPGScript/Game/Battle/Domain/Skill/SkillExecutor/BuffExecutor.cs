using UnityEngine;

namespace S7
{
    public class BuffExecutor : ISkillActionExecutor
    {
        public SkillActionType ActionType => SkillActionType.Buff;

        public SkillResult Execute(ActionContext ctx)
        {
            SkillResult result = new SkillResult();
            return result;
        }
    }

}
