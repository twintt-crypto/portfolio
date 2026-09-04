namespace S7.Game.Field
{
	public class UnitAction_Attack : UnitAction
	{
		private bool hasStarted;
		
		public override void OnEnter()
		{
			controller.AnimatorDriver?.PlayAttack();
		}

		public override void OnUpdate()
		{
			if (controller.AnimatorDriver == null) return;

			if (controller.AnimatorDriver.IsAttackFinished())
			{
				controller.ChangeAction(ActionState.Idle);
				return;
			}
		}

		public override void OnExit()
		{
			controller.AnimatorDriver?.StopAttack();
		}
	}
}
