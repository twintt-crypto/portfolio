using UnityEngine;

namespace S7.Game.Field
{
	public class Mover : MonoBehaviour
	{
		[SerializeField] private float moveSpeed = 5f;
		[SerializeField] private float rotationSpeed = 720f;
		[SerializeField] private bool isRotate;

		private Rigidbody rb;
		private float _moveStepValue;

		public Vector3 Position => rb.position;
		public float MoveSpeed => moveSpeed;
		public float MoveStepValue => _moveStepValue;

		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
			_moveStepValue = moveSpeed * Time.fixedDeltaTime;
		}

		public void SetMoveSpeed(float moveSpeed)
		{
			this.moveSpeed = moveSpeed;
			_moveStepValue = moveSpeed * Time.fixedDeltaTime;
		}

		public void SetRotateSpeed(float rotationSpeed)
		{
			this.rotationSpeed = rotationSpeed;
		}

		public void SetPosition(Vector3 position) => rb.position = position;
		public void SetRotation(Quaternion rotation) => rb.MoveRotation(rotation);

		public void MoveDirFix(Vector3 moveDirection)
		{
			if (moveDirection == Vector3.zero) return;

			rb.MovePosition(rb.position + moveDirection * _moveStepValue);

			if(isRotate) RotateToDirection(moveDirection);
		}

		public void RotateToDirection(Vector3 direction)
		{
			if (direction.magnitude <= 0.0001f) return;

			Quaternion targetRotation = Quaternion.LookRotation(direction);
			float dot = Quaternion.Dot(rb.rotation, targetRotation);

			if (Mathf.Abs(dot) > 0.9999f) return;

			if (dot < 0f)
				targetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);

			Quaternion nextRot = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
			rb.MoveRotation(nextRot);
		}
	}
}
