using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PresentationGraphView : GraphView
{
    public Action OnGraphChanged;

    public PresentationGraphAsset GraphAsset { get; private set; }

    public PresentationGraphView()
    {
        style.flexGrow = 1;

        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        graphViewChanged += OnGraphViewChanged;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {
        if (change.movedElements != null)
        {
            foreach (var element in change.movedElements)
            {
                if (element is PresentationGraphNodeView nodeView)
                {
                    nodeView.UpdateNodePosition();
                }
            }
        }

        OnGraphChanged?.Invoke();

        return change;
    }

    private PresentationGraphNodeView CreateEntryPoint()
    {
        var data = new PresentationNodeData
        {
            guid = Guid.NewGuid().ToString(),
            nodeType = PresentationNodeType.Start,
            title = "Start",
            position = new Vector2(100, 200)
        };

        var node = new PresentationGraphNodeView(data);

        node.capabilities &= ~Capabilities.Deletable;
        node.capabilities &= ~Capabilities.Movable;

        node.SetPosition(new Rect(data.position, new Vector2(200, 150)));

        return node;
    }

    public void NewGraph()
    {
        GraphAsset = null;

        ClearGraph();
        AddElement(CreateEntryPoint());
    }

    public void CreateNode(PresentationNodeType type, Vector2 position)
    {
        if (GraphAsset == null)
        {
            Debug.LogWarning("GraphAsset is null. 먼저 그래프 에셋을 선택하거나 생성하세요.");
            return;
        }

        if (GraphAsset.nodes == null)
            GraphAsset.nodes = new List<PresentationNodeData>();

        if (type == PresentationNodeType.Start &&
            GraphAsset.nodes.Exists(n => n.nodeType == PresentationNodeType.Start))
        {
            return;
        }

        if (type == PresentationNodeType.End &&
            GraphAsset.nodes.Exists(n => n.nodeType == PresentationNodeType.End))
        {
            return;
        }

        var data = new PresentationNodeData
        {
            guid = Guid.NewGuid().ToString(),
            nodeType = type,
            title = type.ToString(),
            position = position
        };

        GraphAsset.nodes.Add(data);

        var node = new PresentationGraphNodeView(data);
        node.SetPosition(new Rect(position, new Vector2(250, 200)));

        AddElement(node);

        OnGraphChanged?.Invoke();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(port =>
            port != startPort &&
            port.node != startPort.node &&
            port.direction != startPort.direction
        ).ToList();
    }

    public void ClearGraph()
    {
        var elements = graphElements.ToList();

        foreach (var element in elements)
        {
            RemoveElement(element);
        }
    }

    public void Load(PresentationGraphAsset asset)
    {
        GraphAsset = asset;

        ClearGraph();

        if (asset == null || asset.nodes == null || asset.nodes.Count == 0)
        {
            AddElement(CreateEntryPoint());
            return;
        }

        Dictionary<string, PresentationGraphNodeView> nodeMap = new();

        foreach (var nodeData in asset.nodes)
        {
            var nodeView = new PresentationGraphNodeView(nodeData);

            if (nodeData.nodeType == PresentationNodeType.Start)
            {
                nodeView.capabilities &= ~Capabilities.Deletable;
                nodeView.capabilities &= ~Capabilities.Movable;
            }

            nodeView.SetPosition(new Rect(nodeData.position, new Vector2(250, 200)));

            AddElement(nodeView);

            nodeMap[nodeData.guid] = nodeView;
        }

        foreach (var edgeData in asset.edges)
        {
            if (!nodeMap.TryGetValue(edgeData.outputNodeGuid, out var outNode))
                continue;

            if (!nodeMap.TryGetValue(edgeData.inputNodeGuid, out var inNode))
                continue;

            Port outputPort = null;
            Port inputPort = null;

            if (string.IsNullOrEmpty(edgeData.outputPortName))
                outputPort = outNode.OutputPorts.Count > 0 ? outNode.OutputPorts[0] : null;
            else
                outputPort = outNode.OutputPorts.Find(p => p.portName == edgeData.outputPortName);

            if (string.IsNullOrEmpty(edgeData.inputPortName))
                inputPort = inNode.InputPorts.Count > 0 ? inNode.InputPorts[0] : null;
            else
                inputPort = inNode.InputPorts.Find(p => p.portName == edgeData.inputPortName);

            if (outputPort == null || inputPort == null)
                continue;

            var edge = outputPort.ConnectTo(inputPort);
            AddElement(edge);
        }
    }

    public void Save(PresentationGraphAsset asset)
    {
        if (asset == null)
            return;

        asset.nodes.Clear();
        asset.edges.Clear();

        var nodeViews = nodes.ToList().OfType<PresentationGraphNodeView>();

        foreach (var nodeView in nodeViews)
        {
            nodeView.UpdateNodePosition();
            asset.nodes.Add(nodeView.Data);
        }

        foreach (var edge in edges.ToList())
        {
            if (edge.output.node is not PresentationGraphNodeView outNode)
                continue;

            if (edge.input.node is not PresentationGraphNodeView inNode)
                continue;

            asset.edges.Add(new PresentationEdgeData
            {
                outputNodeGuid = outNode.Data.guid,
                inputNodeGuid = inNode.Data.guid,
                outputPortName = edge.output.portName,
                inputPortName = edge.input.portName
            });
        }

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
    }

    public Vector2 GetNextNodePosition()
    {
        float maxX = 100;
        float y = 200;

        foreach (var node in nodes.ToList().OfType<PresentationGraphNodeView>())
        {
            var rect = node.GetPosition();

            if (rect.x > maxX)
            {
                maxX = rect.x;
                y = rect.y;
            }
        }

        return new Vector2(maxX + 300, y);
    }

    public void AutoLayout()
    {
        var nodeViews = nodes.ToList().OfType<PresentationGraphNodeView>().ToList();
        if (nodeViews.Count == 0)
            return;

        var startNode = nodeViews.FirstOrDefault(x => x.Data.nodeType == PresentationNodeType.Start);
        if (startNode == null)
            return;

        var nextMap = BuildNextMap(nodeViews);
        var prevMap = BuildPrevMap(nodeViews);

        const float startX = 100f;
        const float startY = 300f;
        const float xSpacing = 150f;
        const float ySpacing = 100f;
        const float branchSpacing = 80f;

        Dictionary<string, Vector2> posMap = new();

        Vector2 GetSize(PresentationGraphNodeView node)
        {
            var size = node.GetPosition().size;
            if (size == Vector2.zero)
                size = new Vector2(250, 150);
            return size;
        }

        void SetNodePosition(PresentationGraphNodeView node, float x, float y)
        {
            if (!posMap.ContainsKey(node.Data.guid))
                posMap[node.Data.guid] = new Vector2(x, y);
        }

        posMap[startNode.Data.guid] = new Vector2(startX, startY);

        Queue<PresentationGraphNodeView> queue = new();
        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (!posMap.TryGetValue(current.Data.guid, out var currentPos))
                continue;

            if (!nextMap.TryGetValue(current.Data.guid, out var children) || children.Count == 0)
                continue;

            var currentSize = GetSize(current);

            //  Fork 처리
            if (current.Data.nodeType == PresentationNodeType.Fork)
            {
                int count = children.Count;
                float centerOffset = (count - 1) * 0.5f;

                for (int i = 0; i < count; i++)
                {
                    var child = children[i];
                    var childSize = GetSize(child);

                    float childX = currentPos.x + currentSize.x + xSpacing;
                    float childY = currentPos.y + (i - centerOffset) * (childSize.y + ySpacing);

                    SetNodePosition(child, childX, childY);
                    queue.Enqueue(child);
                }

                continue;
            }

            switch (current.Data.nodeType)
            {
                case PresentationNodeType.Branch:
                    {
                        foreach (var child in children)
                        {
                            var childSize = GetSize(child);

                            float childX = currentPos.x + currentSize.x + xSpacing;
                            float childY = currentPos.y;

                            string port = GetOutputPortName(current, child);

                            if (port == "True")
                                childY = currentPos.y - (childSize.y + branchSpacing) * 0.5f;
                            else if (port == "False")
                                childY = currentPos.y + (childSize.y + branchSpacing) * 0.5f;

                            SetNodePosition(child, childX, childY);
                            queue.Enqueue(child);
                        }
                        break;
                    }

                case PresentationNodeType.Choice:
                    {
                        int count = children.Count;
                        float centerOffset = (count - 1) * 0.5f;

                        for (int i = 0; i < count; i++)
                        {
                            var child = children[i];
                            var childSize = GetSize(child);

                            float childX = currentPos.x + currentSize.x + xSpacing;
                            float childY = currentPos.y + (i - centerOffset) * (childSize.y + ySpacing);

                            SetNodePosition(child, childX, childY);
                            queue.Enqueue(child);
                        }
                        break;
                    }

                default:
                    {
                        foreach (var child in children)
                        {
                            float childX = currentPos.x + currentSize.x + xSpacing;
                            float childY = currentPos.y;

                            SetNodePosition(child, childX, childY);
                            queue.Enqueue(child);
                        }
                        break;
                    }
            }
        }

        //  Join 처리 ( 핵심: 가장 긴 가지 기준)
        foreach (var node in nodeViews)
        {
            if (node.Data.nodeType != PresentationNodeType.Join)
                continue;

            if (!prevMap.TryGetValue(node.Data.guid, out var prevs) || prevs.Count == 0)
                continue;

            float maxRightX = float.MinValue;
            PresentationGraphNodeView rightMostNode = null;

            foreach (var prev in prevs)
            {
                if (!posMap.TryGetValue(prev.Data.guid, out var prevPos))
                    continue;

                var size = GetSize(prev);
                float rightX = prevPos.x + size.x;

                if (rightX > maxRightX)
                {
                    maxRightX = rightX;
                    rightMostNode = prev;
                }
            }

            if (rightMostNode == null)
                continue;

            var rightPos = posMap[rightMostNode.Data.guid];
            var rightSize = GetSize(rightMostNode);
            var joinSize = GetSize(node);

            float joinX = rightPos.x + rightSize.x + xSpacing;
            float joinY = rightPos.y + (rightSize.y - joinSize.y) * 0.5f;

            posMap[node.Data.guid] = new Vector2(joinX, joinY);
        }

        //  적용
        foreach (var node in nodeViews)
        {
            if (!posMap.TryGetValue(node.Data.guid, out var pos))
                continue;

            var size = node.GetPosition().size;
            if (size == Vector2.zero)
                size = new Vector2(250, 150);

            node.SetPosition(new Rect(pos, size));
            node.UpdateNodePosition();
        }
    }

    private Dictionary<string, List<PresentationGraphNodeView>> BuildNextMap(List<PresentationGraphNodeView> nodeViews)
    {
        Dictionary<string, List<PresentationGraphNodeView>> nextMap = new();

        foreach (var edge in edges.ToList())
        {
            if (edge.output?.node is not PresentationGraphNodeView outNode)
                continue;

            if (edge.input?.node is not PresentationGraphNodeView inNode)
                continue;

            if (!nextMap.TryGetValue(outNode.Data.guid, out var list))
            {
                list = new List<PresentationGraphNodeView>();
                nextMap[outNode.Data.guid] = list;
            }

            if (!list.Contains(inNode))
                list.Add(inNode);
        }

        return nextMap;
    }

    private Dictionary<string, List<PresentationGraphNodeView>> BuildPrevMap(List<PresentationGraphNodeView> nodeViews)
    {
        Dictionary<string, List<PresentationGraphNodeView>> prevMap = new();

        foreach (var edge in edges.ToList())
        {
            if (edge.output?.node is not PresentationGraphNodeView outNode)
                continue;

            if (edge.input?.node is not PresentationGraphNodeView inNode)
                continue;

            if (!prevMap.TryGetValue(inNode.Data.guid, out var list))
            {
                list = new List<PresentationGraphNodeView>();
                prevMap[inNode.Data.guid] = list;
            }

            if (!list.Contains(outNode))
                list.Add(outNode);
        }

        return prevMap;
    }  

    private bool TryLayoutForkJoinBlock(
    PresentationGraphNodeView forkNode,
    Vector2 forkPos,
    Dictionary<string, List<PresentationGraphNodeView>> nextMap,
    Dictionary<string, List<PresentationGraphNodeView>> prevMap,
    Dictionary<string, Vector2> posMap,
    HashSet<string> positioned,
    float xSpacing,
    float ySpacing,
    out PresentationGraphNodeView joinNode)
    {
        joinNode = null;

        if (!nextMap.TryGetValue(forkNode.Data.guid, out var children) || children.Count == 0)
            return false;

        List<List<PresentationGraphNodeView>> lanes = new();
        PresentationGraphNodeView commonJoin = null;

        foreach (var child in children)
        {
            if (!TryCollectLaneToJoin(child, nextMap, out var lane, out var foundJoin))
                return false;

            if (foundJoin == null)
                return false;

            if (commonJoin == null)
                commonJoin = foundJoin;
            else if (commonJoin != foundJoin)
                return false;

            lanes.Add(lane);
        }

        if (commonJoin == null)
            return false;

        joinNode = commonJoin;

        int maxLaneLength = lanes.Count > 0 ? lanes.Max(x => x.Count) : 0;
        float laneStartX = forkPos.x + xSpacing;
        float joinX = laneStartX + (maxLaneLength * xSpacing);

        float centerOffset = (lanes.Count - 1) * 0.5f;

        for (int laneIndex = 0; laneIndex < lanes.Count; laneIndex++)
        {
            float laneY = forkPos.y + (laneIndex - centerOffset) * ySpacing;
            var lane = lanes[laneIndex];

            for (int nodeIndex = 0; nodeIndex < lane.Count; nodeIndex++)
            {
                var node = lane[nodeIndex];
                float x = laneStartX + (nodeIndex * xSpacing);
                float y = laneY;

                posMap[node.Data.guid] = new Vector2(x, y);
                positioned.Add(node.Data.guid);
            }

            if (lane.Count > 0)
            {
                var lastNode = lane[lane.Count - 1];
                posMap[lastNode.Data.guid] = new Vector2(joinX - xSpacing, laneY);
            }
        }

        posMap[commonJoin.Data.guid] = new Vector2(joinX, forkPos.y);
        positioned.Add(commonJoin.Data.guid);

        return true;
    }

    private string GetOutputPortName(
        PresentationGraphNodeView fromNode,
        PresentationGraphNodeView toNode)
    {
        foreach (var edge in edges.ToList())
        {
            if (edge.output?.node != fromNode)
                continue;

            if (edge.input?.node != toNode)
                continue;

            return edge.output.portName;
        }

        return string.Empty;
    }

    private bool TryCollectLaneToJoin(
    PresentationGraphNodeView startNode,
    Dictionary<string, List<PresentationGraphNodeView>> nextMap,
    out List<PresentationGraphNodeView> lane,
    out PresentationGraphNodeView joinNode)
    {
        lane = new List<PresentationGraphNodeView>();
        joinNode = null;

        var current = startNode;
        HashSet<string> visited = new();

        while (current != null)
        {
            if (!visited.Add(current.Data.guid))
                return false;

            if (current.Data.nodeType == PresentationNodeType.Join)
            {
                joinNode = current;
                return true;
            }

            lane.Add(current);

            if (!nextMap.TryGetValue(current.Data.guid, out var nexts) || nexts.Count == 0)
                return false;

            if (nexts.Count > 1)
                return false;

            current = nexts[0];
        }

        return false;
    }

    private void SetNodePosition(
    Dictionary<string, Vector2> posMap,
    PresentationGraphNodeView node,
    float x,
    float y)
    {
        posMap[node.Data.guid] = new Vector2(x, y);
    }
}