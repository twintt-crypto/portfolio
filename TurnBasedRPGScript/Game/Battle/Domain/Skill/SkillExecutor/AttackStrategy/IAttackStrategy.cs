using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace S7
{
    public interface IAttackStrategy
    {
        AttackType Type { get; }

        SkillResult Execute(ActionContext ctx);
    }
}

