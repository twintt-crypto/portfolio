namespace S7.Game.Field.Enemy
{
    public interface IUnitActionStrategy
    {
        void Tick();
        void Enter();
        void Exit();
    }
}
