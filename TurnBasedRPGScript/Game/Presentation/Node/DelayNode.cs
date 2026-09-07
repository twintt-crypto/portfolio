using Cysharp.Threading.Tasks;
using System.Threading;

namespace S7
{
    public class DelayNode : BasePresentationNode
    {
        private readonly float _time;

        public DelayNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
            _time = float.Parse(_data.param1);
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            await UniTask.Delay(
            (int)(_time * 1000),
            DelayType.DeltaTime,
            PlayerLoopTiming.Update,
            token);
        }
    }
}
