using AmplifyShaderEditor;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace S7
{
    public static class GraphExecutor
    {
        public static async UniTask Execute(
       RuntimeNode node,
       PresentationContext ctx,
       CancellationToken token)
        {
            if (node == null || node.node == null)
                return;

            token.ThrowIfCancellationRequested();

            // 현재 노드 실행
            await node.node.PlayAsync(ctx, token);

            token.ThrowIfCancellationRequested();            

            if (node.nextNodes == null || node.nextNodes.Count == 0)
                return;

            // Fork는 내부에서 분기 처리
            if (node.node is ForkNode)
                return;

            if (node.node is JoinNode)
                return;

            if (node.node is BranchNode)
                return;

            // 다음 노드 하나
            if (node.nextNodes.Count == 1)
            {
                Debug.Log($"Join next: {node.nextNodes[0].node.GetType().Name}");
                await Execute(node.nextNodes[0], ctx, token);
                return;
            }

#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(
                $"GraphExecutor: multiple nextNodes but not Fork. guid={node.guid}");
#endif

            await Execute(node.nextNodes[0], ctx, token);
        }
    }
}
