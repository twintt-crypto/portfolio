using UnityEngine;

namespace S7.Game.Field
{
	public class UnitAction_Alert : UnitAction
	{
		public override void OnEnter()
		{
			controller.AnimatorDriver?.Play("AttackIdle");
		}
		
		public override void OnUpdate()
		{
			if (controller.RotateDirection == Vector3.zero)
			{
				controller.ChangeAction(ActionState.Idle);
				return;
			} 
			
			if (controller.MoveDirection != Vector3.zero)
			{
				controller.ChangeAction(ActionState.Move);
			}
		}
		
		public override void OnFixedUpdate()
		{
			controller.Mover.RotateToDirection(controller.RotateDirection);
		}
	}
}
