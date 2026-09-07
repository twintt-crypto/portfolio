using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace S7
{
    public class PresentationSequence
    {
        private readonly List<IPresentationNode> _nodes = new();

        public void Add(IPresentationNode node)
        {
            _nodes.Add(node);
        }

        public async UniTask PlayAsync(
            PresentationContext ctx,
            CancellationToken token)
        {
            foreach (var node in _nodes)
            {
                token.ThrowIfCancellationRequested();

                await node.PlayAsync(ctx, token);
            }
        }
    }
}
