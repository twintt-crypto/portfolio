using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace S7.Game.Field
{
	public class UnitActionController : MonoBehaviour
	{
		private Dictionary<ActionState, UnitAction> _actions;
		private Dictionary<ActionState, Action> _onEnterEvents = new();
		private ActionState _currentState;

		public Mover Mover { get; private set; }
		public Animator Animator { get; private set; }
		public IAnimatorDriver AnimatorDriver { get; private set; }
		private UnitAction CurrentAction => _actions != null && _actions.ContainsKey(_currentState) ? _actions[_currentState] : null;

		private Vector3 _moveDirection;
		public Vector3 MoveDirection
		{
			get => _moveDirection;
			set
			{
				_moveDirection = value;
				RotateDirection = value;
			}
		}
		public Vector3 RotateDirection { get; set; }

		public void Initialize(Dictionary<ActionState, UnitAction> actions)
		{
			Mover = GetComponent<Mover>();
			Animator = GetComponentInChildren<Animator>();
			// AnimatorDriver = new DirectPlayAnimatorDriver(Animator);
			AnimatorDriver = new BlendTreeAnimatorDriver(Animator);

			_actions = actions;
			foreach (UnitAction action in actions.Values)
				action.Initialize(this);

			ChangeAction(ActionState.Idle);
		}

		public void ChangeAction(ActionState state)
		{
			if (_currentState == state) return;
			if (_actions == null || !_actions.ContainsKey(state)) return;

			UnitAction next = _actions[state];
			if (!next.CanChange()) return;
			
			if (_currentState != ActionState.NONE)
			{
				CurrentAction?.OnExit();
			}

			_currentState = state;

			if (_onEnterEvents.TryGetValue(state, out Action evt)) evt?.Invoke();
			next.OnEnter();
		}

		public void Trigger(ActionTrigger trigger) => CurrentAction?.OnTrigger(trigger);

		public void SnapTo(Vector3 position) => Mover.SetPosition(position);
		public void SnapRotation(Quaternion rotation) => Mover.SetRotation(rotation);

		public void SubscribeOnEnter(ActionState state, Action callback)
		{
			_onEnterEvents[state] = _onEnterEvents.GetValueOrDefault(state) + callback;
		}

		public void UnsubscribeOnEnter(ActionState state, Action callback)
		{
			_onEnterEvents[state] = _onEnterEvents.GetValueOrDefault(state) - callback;
		}

		public void ChangeAnimator(Animator animator)
		{
			CurrentAction?.OnExit();
			_currentState = ActionState.NONE;

			AnimatorDriver?.Reset();

			Animator = animator;
			AnimatorDriver = new BlendTreeAnimatorDriver(Animator);

			MoveDirection = Vector3.zero;
			RotateDirection = Vector3.zero;

			ChangeAction(ActionState.Idle);
		}

		public void Reset()
		{
			AnimatorDriver?.Reset();
			MoveDirection = Vector3.zero;
			RotateDirection = Vector3.zero;
			ChangeAction(ActionState.Idle);
		}

		public void CaptureAnimatorState()
		{
			AnimatorDriver?.CaptureState();
		}

		public void RestoreAnimatorState()
		{
			AnimatorDriver?.RestoreState();
		}
		
		private void Update()
		{
			CurrentAction?.OnUpdate();
		}

		private void FixedUpdate()
		{
			CurrentAction?.OnFixedUpdate();
		}
	}
}
