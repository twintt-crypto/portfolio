namespace S7.Game.Field
{
    public interface IAnimatorDriver
    {
        void Reset();
        void SetMoveSpeed(float speed);
        void PlayIdle();
        void PlayAttack();
        void StopAttack();
        bool IsAttackFinished();
        void Play(string hashString);
        void CaptureState();
        void RestoreState();
    }
}
