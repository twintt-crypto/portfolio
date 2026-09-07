using UnityEngine;

namespace S7.Game.Field
{
	public class UnitAction_Idle : UnitAction
	{
		public override void OnEnter()
		{
			// controller.AnimatorDriver?.PlayIdle();
		}

		public override void OnUpdate()
		{
			if (controller.MoveDirection != Vector3.zero)
			{
				controller.ChangeAction(ActionState.Move);
				return;
			}

			if (controller.RotateDirection != Vector3.zero)
			{
				controller.ChangeAction(ActionState.Alert);
				return;
			}
		}
	}
}
