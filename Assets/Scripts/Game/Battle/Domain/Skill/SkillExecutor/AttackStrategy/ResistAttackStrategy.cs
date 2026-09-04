
using VContainer;

namespace S7
{
    public static class ResistAttackStrategy
    {
        //전략 등록
        public static void Regist(IContainerBuilder builder)
        {
            builder.Register<IAttackStrategy, NormalAttackStrategy>(Lifetime.Singleton);
            builder.Register<IAttackStrategy, ProjectileAttackStrategy>(Lifetime.Singleton);
        }
    }
}
