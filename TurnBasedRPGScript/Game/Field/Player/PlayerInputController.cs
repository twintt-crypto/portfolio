using UnityEngine;
using UnityEngine.InputSystem;

namespace S7.Game.Field
{
	public class PlayerInputController : MonoBehaviour
	{
		private Transform _cameraTransform;
		private InputAction moveAction;
		private InputAction attackAction;
		private InputAction dashAction;
		private UnitActionController _actionController;

		private void Awake()
		{
			moveAction = InputSystem.actions.FindAction("Move");
			attackAction = InputSystem.actions.FindAction("Attack");
			dashAction = InputSystem.actions.FindAction("Dash");

			if (_cameraTransform == null) _cameraTransform = Camera.main.transform;
		}
		
		public void Initialize(UnitActionController controller) => _actionController = controller;

		void Update()
		{
			if (_actionController == null) return;  
			
			var raw = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1);
			_actionController.MoveDirection = GetCameraRelativeDirection(raw);

			if (attackAction.WasPressedThisFrame()) _actionController.ChangeAction(ActionState.Attack);
			if (dashAction.WasPressedThisFrame()) _actionController.Trigger(ActionTrigger.Dash);
		}

		private Vector3 GetCameraRelativeDirection(Vector2 input)
		{
			if (input == Vector2.zero) return Vector3.zero;

			Vector3 camForward = _cameraTransform.forward;
			Vector3 camRight = _cameraTransform.right;

			camForward.y = 0;
			camRight.y = 0;
			camForward.Normalize();
			camRight.Normalize();

			float magnitude = input.magnitude;
			return (camForward * input.y + camRight * input.x).normalized * magnitude;
		}
	}
}
