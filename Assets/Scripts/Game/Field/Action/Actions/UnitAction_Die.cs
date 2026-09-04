namespace S7.Game.Field
{
	public class UnitAction_Die : UnitAction
	{
		public override void OnEnter()
		{
			controller.AnimatorDriver?.Play("Death");
		}
	}
}
