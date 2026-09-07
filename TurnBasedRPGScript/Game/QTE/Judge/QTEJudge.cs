namespace Game.QTE
{
    public abstract class QTEJudge
    {
        public abstract void Feed();
        public abstract QTE_RESULT Judge();

        public bool IsComplete { get; protected set; } = false;
        
        public void ForceComplete() => IsComplete = true;
    }
}
