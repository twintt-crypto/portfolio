using Cysharp.Threading.Tasks;
using S7;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.AddressableAssets;

public class PresentationCore
{
    public async UniTask PlayAsync(
        string presentationGraph,
        PresentationContext context,
        CancellationToken token)    
    {
        var graphHandle = Addressables.LoadAssetAsync<PresentationGraphAsset>(presentationGraph);
        var graph = await graphHandle.ToUniTask(cancellationToken: token);
        
        TimelineAddressableProvider timelineProvider = null;
        try
        {
            token.ThrowIfCancellationRequested();

            if (graph == null)
            {
                Debug.LogWarning($"PresentationCore: graph load failed. key={presentationGraph}");
                return;
            }

            bool hasTimelineNode = false;
            if (graph.nodes != null)
            {
                for (int i = 0; i < graph.nodes.Count; i++)
                {
                    if (graph.nodes[i].nodeType == PresentationNodeType.PlayTimeline)
                    {
                        hasTimelineNode = true;
                        break;
                    }
                }
            }

            if (hasTimelineNode)
            {
                timelineProvider = new TimelineAddressableProvider();

                context.getTimelineAsync = timelineProvider.GetAsync;
                context.releaseTimeline = timelineProvider.ReleaseTimelineImpl;
            }

            PresentationRuntimeGraph runtimeGraph = PresentationRuntimeGraphBuilder.Build(graph);

            if (runtimeGraph == null)
            {
                Debug.LogWarning("PresentationCore: runtimeGraph is null");
                return;
            }

            if (runtimeGraph.startNode == null)
            {
                Debug.LogWarning("PresentationCore: startNode is null");
                return;
            }

            await GraphExecutor.Execute(runtimeGraph.startNode, context, token);
        }
        finally
        {
            timelineProvider?.ReleaseAll();
            Addressables.Release(graphHandle);
        }
    }
    
}