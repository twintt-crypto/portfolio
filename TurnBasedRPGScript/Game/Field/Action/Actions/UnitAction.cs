using UnityEngine;

namespace S7.Game.Field
{
	public abstract class UnitAction
	{
		protected UnitActionController controller;

		public virtual void Initialize(UnitActionController controller)
		{
			this.controller = controller;
		}

		public virtual void OnEnter() { }
		public virtual void OnUpdate() { }
		public virtual void OnFixedUpdate() { }
		public virtual void OnExit() { }
		public virtual void OnTrigger(ActionTrigger trigger) { }
		public virtual bool CanChange() => true;
	}
}
