namespace S7
{
    public interface IStateSaveable
    {
        void CaptureState(StateSnapshot snapshot);
        void RestoreState(StateSnapshot snapshot);
    }
}
