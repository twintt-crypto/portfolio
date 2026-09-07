using UnityEngine;

namespace S7.Game.Field.Enemy
{
    public class DeathStrategy : IUnitActionStrategy
    {
        private FieldEnemyAI _ai;

        public DeathStrategy(FieldEnemyAI ai)
        {
            _ai = ai;
        }

        public void Enter()
        {
            _ai.Controller.MoveDirection = Vector3.zero;
            _ai.Controller.RotateDirection = Vector3.zero;
            if (_ai.NavAgent != null) _ai.NavAgent.isStopped = true;
        }

        public void Exit() { }
        public void Tick() { }
    }
}
