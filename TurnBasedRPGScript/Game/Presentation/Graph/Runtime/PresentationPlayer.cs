using Cysharp.Threading.Tasks;
using S7;
using System.Threading;
using UnityEngine;

public static class PresentationPlayer
{
    public static async UniTask PlayAsync(
        PresentationGraphAsset graphAsset,
        PresentationContext ctx,
        CancellationToken token)
    {
        if (graphAsset == null)
        {
            Debug.LogWarning("PresentationPlayer: graphAsset is null");
            return;
        }

        PresentationRuntimeGraph runtimeGraph = PresentationRuntimeGraphBuilder.Build(graphAsset);

        if (runtimeGraph == null || runtimeGraph.startNode == null)
        {
            Debug.LogWarning("PresentationPlayer: runtimeGraph or startNode is null");
            return;
        }

        await GraphExecutor.Execute(runtimeGraph.startNode, ctx, token);
    }
}