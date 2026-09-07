using S7;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public static class PresentationRuntimeGraphBuilder
{    
    public static PresentationRuntimeGraph Build(PresentationGraphAsset graph)
    {
        PresentationRuntimeGraph runtimeGraph = new();

        if (graph == null)
        {
            Debug.LogWarning("PresentationRuntimeGraphBuilder: graph is null");
            return runtimeGraph;
        }

        if (graph.nodes == null || graph.nodes.Count == 0)
        {
            Debug.LogWarning("PresentationRuntimeGraphBuilder: graph.nodes is null or empty");
            return runtimeGraph;
        }

        // 1. 런타임 노드 생성
        foreach (var nodeData in graph.nodes)
        {
            IPresentationNode presentationNode = CreatePresentationNode(nodeData);
            if (presentationNode == null)
            {
                Debug.LogWarning($"PresentationRuntimeGraphBuilder: failed to create node. guid={nodeData.guid}, type={nodeData.nodeType}");
                continue;
            }

            RuntimeNode runtimeNode = new RuntimeNode
            {
                guid = nodeData.guid,
                node = presentationNode
            };

            if (presentationNode is BasePresentationNode baseNode)
            {
                baseNode.owner = runtimeNode;
            }

            runtimeGraph.nodeMap[runtimeNode.guid] = runtimeNode;

            if (nodeData.nodeType == PresentationNodeType.Start)
            {
                runtimeGraph.startNode = runtimeNode;
            }
        }

        // 2. 엣지 연결
        if (graph.edges != null)
        {
            foreach (var edge in graph.edges)
            {
                if (!runtimeGraph.nodeMap.TryGetValue(edge.outputNodeGuid, out var from))
                    continue;

                if (!runtimeGraph.nodeMap.TryGetValue(edge.inputNodeGuid, out var to))
                    continue;

                if (!from.nextNodes.Contains(to))
                    from.nextNodes.Add(to);

                if (!to.prevNodes.Contains(from))
                    to.prevNodes.Add(from);
            }
        }        

        // 3. 특수 노드 바인딩
        foreach (var runtimeNode in runtimeGraph.nodeMap.Values)
        {
            if (runtimeNode.node is ChoiceNode choiceNode)
            {
                BindChoiceNode(graph, runtimeNode, choiceNode);
            }
            else if (runtimeNode.node is ForkNode forkNode)
            {
                BindForkNode(graph, runtimeNode, forkNode);
            }
        }

        SetupJoinNodes(runtimeGraph);

        if (runtimeGraph.startNode == null)
        {
            Debug.LogWarning("PresentationRuntimeGraphBuilder: startNode not found");
        }

        return runtimeGraph;
    }

    public static IPresentationNode CreatePresentationNode(PresentationNodeData nodeData)
    {
        switch (nodeData.nodeType)
        {
            case PresentationNodeType.Start:
                return new EmptyNode(nodeData);

            case PresentationNodeType.Join:
                return new JoinNode(nodeData);

            case PresentationNodeType.Choice:
                return new ChoiceNode(nodeData);

            case PresentationNodeType.Fork:
                return new ForkNode(nodeData);

            case PresentationNodeType.PlayAnimation:
                return new PlayAnimationNode(nodeData);

            case PresentationNodeType.Delay:
                return new DelayNode(nodeData);

            case PresentationNodeType.Move:
                return new MoveNode(nodeData);

            case PresentationNodeType.WaitPlayAnimation:
                return new WaitPlayAnimationNode(nodeData);

            case PresentationNodeType.WaitAnimationEventNode:
                return new WaitAnimationEventNode(nodeData);

            case PresentationNodeType.SpawnArrowNode:
                return new SpawnArrowNode(nodeData);

            case PresentationNodeType.ShootArrowNode:
                return new ShootArrowNode(nodeData);

            case PresentationNodeType.OnHit:
                return new OnHitNode(nodeData);

            case PresentationNodeType.Dialogue:
                return new DialogueNode(nodeData);

            case PresentationNodeType.PlayTimeline:
                return new PlayTimelineNode(nodeData);

            case PresentationNodeType.Branch:
                return new BranchNode(nodeData);

            case PresentationNodeType.Castring:
                return new CastingNode(nodeData);

            case PresentationNodeType.FadeIn:
                return new FadeInNode(nodeData);

            case PresentationNodeType.FadeOut:
                return new FadeOutNode(nodeData);

            case PresentationNodeType.PlayQTE:
                return new PlayQTE(nodeData);

            case PresentationNodeType.End:
                return new EmptyNode(nodeData);

            case PresentationNodeType.RegisterOnHitNode:
                return new RegisterOnHitNode(nodeData);

            case PresentationNodeType.UnregisterOnHitNode:
                return new UnregisterOnHitNode(nodeData);
            
             case PresentationNodeType.ShowAlly:
                return new ShowAllyNode(nodeData);

            case PresentationNodeType.HideAlly:
                return new HideAllyNode(nodeData);

//             case PresentationNodeType.HideEnemy:

            case PresentationNodeType.LockAt:
                return new LookAtNode(nodeData);

        }

        return null;
    }

    private static void BindChoiceNode(
        PresentationGraphAsset graph,
        RuntimeNode runtimeNode,
        ChoiceNode choiceNode)
    {
        if (graph.edges == null || graph.edges.Count == 0)
            return;

        var orderedEdges = graph.edges
            .Where(x => x.outputNodeGuid == runtimeNode.guid)
            .OrderBy(x => ParseChoicePortIndex(x.outputPortName))
            .ThenBy(x => x.outputPortName)
            .ToList();

        choiceNode.options.Clear();

        foreach (var edge in orderedEdges)
        {
            var next = runtimeNode.nextNodes.FirstOrDefault(n => n.guid == edge.inputNodeGuid);
            if (next == null)
            {
                Debug.LogWarning($"BindChoiceNode: next node not found. inputGuid={edge.inputNodeGuid}");
                continue;
            }

            choiceNode.options.Add(new ChoiceOption
            {
                portName = string.IsNullOrEmpty(edge.outputPortName) ? string.Empty : edge.outputPortName,
                nextNode = next
            });
        }
    }

    private static void BindForkNode(
     PresentationGraphAsset graph,
     RuntimeNode runtimeNode,
     ForkNode forkNode)
    {
        if (graph == null)
            return;

        if (runtimeNode == null)
            return;

        if (forkNode == null)
            return;

        if (graph.edges == null || graph.edges.Count == 0)
            return;

        forkNode.children.Clear();

        var orderedEdges = graph.edges
            .Where(x => x.outputNodeGuid == runtimeNode.guid)
            .ToList();

        foreach (var edge in orderedEdges)
        {
            var next = runtimeNode.nextNodes.FirstOrDefault(n => n.guid == edge.inputNodeGuid);
            if (next == null)
            {
                Debug.LogWarning(
                    $"BindForkNode: next node not found. forkGuid={runtimeNode.guid}, inputGuid={edge.inputNodeGuid}");
                continue;
            }

            if (forkNode.children.Contains(next))
                continue;

            forkNode.children.Add(next);
        }
    }

    private static int ParseChoicePortIndex(string portName)
    {
        if (string.IsNullOrWhiteSpace(portName))
            return int.MaxValue;

        portName = portName.Trim();

        if (int.TryParse(portName, out int directIndex))
            return directIndex;

        string numberText = new string(portName.Where(char.IsDigit).ToArray());
        if (!string.IsNullOrEmpty(numberText) && int.TryParse(numberText, out int parsedIndex))
            return parsedIndex;

        return int.MaxValue;
    }

    private static void SetupJoinNodes(PresentationRuntimeGraph runtimeGraph)
    {
        foreach (var runtimeNode in runtimeGraph.nodeMap.Values)
        {
            if (runtimeNode.node is JoinNode joinNode)
            {
                joinNode.requiredCount = runtimeNode.prevNodes.Count;

#if UNITY_EDITOR
                UnityEngine.Debug.Log(
                    $"JoinNode Setup: guid={runtimeNode.guid}, requiredCount={joinNode.requiredCount}");
#endif
            }
        }
    }
}