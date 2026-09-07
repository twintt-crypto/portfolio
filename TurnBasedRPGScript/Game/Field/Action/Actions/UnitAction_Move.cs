using DG.Tweening;
using UnityEngine;

namespace S7.Game.Field
{
	public class UnitAction_Move : UnitAction
	{
		private const float DashSpeed = 5f;

		private Tweener _animTween;
		private Tweener _moveTween;
		private float _currentAnimSpeed;
		private float _currentMoveSpeed;
		private float _normalSpeed;
		private Vector3 _lastDirection;

		public override void OnEnter()
		{
			_animTween?.Kill();
			_moveTween?.Kill();

			_normalSpeed = controller.Mover.MoveSpeed;
			float tweenTime = 0.5f;
			float targetSpeed = controller.Mover.MoveSpeed;

			_currentAnimSpeed = 0f;
			_animTween = DOTween.To(() => _currentAnimSpeed, x =>
			{
				_currentAnimSpeed = x;
			}, targetSpeed, tweenTime).SetEase(Ease.OutQuad);

			_currentMoveSpeed = 0f;
			_moveTween = DOTween.To(() => _currentMoveSpeed, x =>
			{
				_currentMoveSpeed = x;
			}, targetSpeed, tweenTime).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Fixed);
		}

		public override void OnUpdate()
		{
			if (controller.MoveDirection == Vector3.zero)
			{
				controller.ChangeAction(ActionState.Idle);
				return;
			}

			float magnitude = controller.MoveDirection.magnitude;
			controller.AnimatorDriver?.SetMoveSpeed(_currentAnimSpeed * magnitude);
		}

		public override void OnFixedUpdate()
		{
			controller.Mover.MoveDirFix(controller.MoveDirection * (_currentMoveSpeed / controller.Mover.MoveSpeed));
		}

		public override void OnTrigger(ActionTrigger trigger)
		{
			if (trigger == ActionTrigger.Dash)
			{
				controller.Mover.SetMoveSpeed(DashSpeed);

				_animTween?.Kill();
				_moveTween?.Kill();

				float tweenTime = 0.3f;
				_animTween = DOTween.To(() => _currentAnimSpeed, x =>
				{
					_currentAnimSpeed = x;
				}, DashSpeed, tweenTime).SetEase(Ease.OutQuad);

				_moveTween = DOTween.To(() => _currentMoveSpeed, x =>
				{
					_currentMoveSpeed = x;
				}, DashSpeed, tweenTime).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Fixed);
			}
		}

		public override void OnExit()
		{
			controller.Mover.SetMoveSpeed(_normalSpeed);
			_lastDirection = controller.MoveDirection;
			if (_lastDirection == Vector3.zero) _lastDirection = controller.transform.forward;

			_animTween?.Kill();

			float tweenTime = 0.25f;
			
			_animTween = DOTween.To(() => _currentAnimSpeed, x =>
			{
				_currentAnimSpeed = x;
				controller.AnimatorDriver?.SetMoveSpeed(x);
			}, 0f, tweenTime).SetEase(Ease.OutQuad);

			_moveTween?.Kill();
			_currentMoveSpeed = controller.Mover.MoveSpeed;
			_moveTween = DOTween.To(() => _currentMoveSpeed, x =>
			{
				_currentMoveSpeed = x;
				controller.Mover.MoveDirFix(_lastDirection.normalized * (x / controller.Mover.MoveSpeed));
			}, 0f, tweenTime).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Fixed);
		}
	}
}
