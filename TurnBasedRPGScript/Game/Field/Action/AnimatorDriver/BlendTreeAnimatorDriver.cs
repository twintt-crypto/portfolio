using UnityEngine;

namespace S7.Game.Field
{
    public class BlendTreeAnimatorDriver : IAnimatorDriver
    {
        private static readonly int MoveSpeedHash  = Animator.StringToHash("moveSpeed");
        private static readonly int AttackTriggerHash  = Animator.StringToHash("triggerAttack");

        private readonly Animator _animator;
        private bool _attackEntered;
        private int _savedHash;
        private float _savedTime;

        public BlendTreeAnimatorDriver(Animator animator) => _animator = animator;

        public void Reset() { }

        public void SetMoveSpeed(float speed) => _animator.SetFloat(MoveSpeedHash, speed);
        public void PlayIdle()                => SetMoveSpeed(0f);
        public void PlayAttack()
        {
            _attackEntered = false;
            _animator.SetTrigger(AttackTriggerHash);
        }
        public void StopAttack()
        {
            _attackEntered = false;
            _animator.ResetTrigger(AttackTriggerHash);
        }

        public void Play(string hash)
        {

        }
        
        public bool IsAttackFinished()
        {
            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsTag("Attack")) _attackEntered = true;

            return _attackEntered && !info.IsTag("Attack");
        }

        public void CaptureState()
        {
            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
            _savedHash = info.shortNameHash;
            _savedTime = info.normalizedTime;
        }

        public void RestoreState()
        {
            if (_savedHash == 0) return;
            _animator.Play(_savedHash, 0, _savedTime);
        }
    }
}
