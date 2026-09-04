namespace S7
{
    public interface ISkillActionExecutor
    {
        SkillActionType ActionType { get; }
        SkillResult Execute(ActionContext context);
    }
}
