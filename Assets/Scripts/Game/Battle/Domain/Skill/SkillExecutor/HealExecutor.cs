using UnityEngine;
namespace S7
{
    public class HealExecutor : ISkillActionExecutor
    {
        public SkillActionType ActionType => SkillActionType.Heal;
        public SkillResult Execute(ActionContext ctx)
        {
            SkillResult result = new SkillResult();
            return result;
        }
    }
}
