using Cysharp.Threading.Tasks;
using System.Threading;

namespace S7
{
    public interface IPresentationNode
    {
        UniTask PlayAsync(
            PresentationContext context,
            CancellationToken token);
    }
}
