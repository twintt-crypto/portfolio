using Cysharp.Threading.Tasks;
using System.Threading;

namespace S7
{
    public class WaitAnimationEventNode : BasePresentationNode
    {
        private readonly string _eventName;

        public WaitAnimationEventNode(PresentationNodeData presentationNodeData) : base(presentationNodeData)
        {
            _eventName = presentationNodeData.param1;
        }

        public override async UniTask PlayAsync(PresentationContext ctx, CancellationToken token)
        {
            var receiver = ctx.caster.animationEventReceiver;

            var tcs = new UniTaskCompletionSource();

            void OnEvent()
            {
                tcs.TrySetResult();
            }

            receiver.Register(_eventName, OnEvent);

            try
            {
                await tcs.Task.AttachExternalCancellation(token);
            }
            finally
            {
                receiver.Unregister(_eventName, OnEvent);
            }
        }
    }
}
