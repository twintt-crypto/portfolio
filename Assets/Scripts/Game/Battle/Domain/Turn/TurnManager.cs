using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace S7
{
    public partial class TurnManager
    {
        private BattleContext _context;

        private readonly List<UnitController> _order = new();
        public IReadOnlyList<UnitController> Order => _order;

        private int _index;

        public TurnContext turnContext { get; set; }

        public void Initialize(BattleContext battleContext, IEnumerable<UnitController> units)
        {
            _context = battleContext;

            foreach (var unit in units)
            {
                if (!unit.CanParticipateBattle())
                    continue;

                _order.Add(unit);
            }

            if (_order.Count == 0)
                throw new InvalidOperationException("No units registered to TurnManager.");

            _order.Sort((a, b) =>
                b.data._stat.Speed.CompareTo(a.data._stat.Speed));

            _index = 0;

            State = TurnState.None;
        }

        public void NextTurn()
        {
            Debug.Log("");
            _index = (_index + 1) % _order.Count;
        }

        public void CreateTurnContext(UnitController unit)
        {
            turnContext = new TurnContext();
            turnContext.caster = unit;
            turnContext.targets = new List<UnitController>();
        }

        public void SetTarget(UnitController target)
        {
            turnContext.targets.Clear();
            turnContext.targets.Add(target);
        }

        public void SetTargets(List<UnitController> targets)
        {
            turnContext.targets.Clear();
            foreach (var unit in targets)
            {
                turnContext.targets.Add(unit);
            }
        }

        public void SetSelectSkill(UnitSkill skill)
        {
            turnContext.selectSkill = skill;
        }
    }

}
