using UnityEngine;
using System.Collections.Generic;

namespace S7.Game.Field
{
	public static class UnitActionFactory
	{
		public static Dictionary<ActionState, UnitAction> EnemyActions() => new()
		{
			{ ActionState.Idle,   new UnitAction_Idle() },
			{ ActionState.Move,   new UnitAction_Move() },
			{ ActionState.Attack, new UnitAction_Attack() },
			{ ActionState.Alert,  new UnitAction_Alert() },
			{ ActionState.Die,    new UnitAction_Die() },
		};

		public static Dictionary<ActionState, UnitAction> PlayerActions() => new()
		{
			{ ActionState.Idle,   new UnitAction_Idle() },
			{ ActionState.Move,   new UnitAction_Move() },
			{ ActionState.Attack, new UnitAction_Attack() },
			{ ActionState.Die,    new UnitAction_Die() },
		};
	}
}