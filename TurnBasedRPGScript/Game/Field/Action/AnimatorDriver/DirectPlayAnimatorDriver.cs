using UnityEngine;

namespace S7.Game.Field
{
    public class DirectPlayAnimatorDriver : IAnimatorDriver
    {
        private static readonly int WalkHash   = Animator.StringToHash("Walk");
        private static readonly int JogHash    = Animator.StringToHash("Jog");
        private static readonly int SprintHash = Animator.StringToHash("Sprint");
        private static readonly int IdleHash   = Animator.StringToHash("Idle");
        private static readonly int AttackHash = Animator.StringToHash("Attack_1");

        private const float CrossFadeDuration = 0.1f;

        private readonly Animator _animator;
        private int _currentHash;
        private int _savedHash;
        private float _savedTime;

        public DirectPlayAnimatorDriver(Animator animator) => _animator = animator;

        public void Reset() => _currentHash = 0;

        public void SetMoveSpeed(float speed)
        {
            if (speed <= 1f) CrossFadeTo(WalkHash);
            else if (speed <= 10f) CrossFadeTo(JogHash);
            else CrossFadeTo(SprintHash);
        }
        public void PlayIdle()                => CrossFadeTo(IdleHash);
        public void PlayAttack()              => CrossFadeTo(AttackHash);
        public void StopAttack()              { }
        public void Play(string hash)         => CrossFadeTo(Animator.StringToHash(hash));

        public bool IsAttackFinished()
        {
            if (_animator.IsInTransition(0)) return false;
            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
            return info.shortNameHash == AttackHash && info.normalizedTime >= 1f;
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
            _currentHash = _savedHash;
        }

        private void CrossFadeTo(int hash)
        {
            if (_currentHash == hash) return;
            _currentHash = hash;
            _animator.CrossFadeInFixedTime(hash, CrossFadeDuration);
        }
    }
}
