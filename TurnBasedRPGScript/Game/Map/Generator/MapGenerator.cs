using System.Collections.Generic;
using UnityEngine;

namespace S7.Game.Map
{
    public struct MapGeneratorConfig
    {
        public int totalLayers;
        public int minNodesPerLayer;
        public int maxNodesPerLayer;
        public int minConnectionsPerNode;
        public int maxConnectionsPerNode;
        public int randomSeed; // 시드 바꾸면 다른 맵 생성

        public static MapGeneratorConfig Default => new MapGeneratorConfig
        {
            totalLayers = 10,
            minNodesPerLayer = 2,
            maxNodesPerLayer = 4,
            minConnectionsPerNode = 1,
            maxConnectionsPerNode = 2,
            randomSeed = 0,
        };
    }

    public static class MapGenerator
    {
        public static MapData Generate(MapGeneratorConfig config)
        {
            if (config.randomSeed >= 0) Random.InitState(config.randomSeed);

            var mapData = new MapData();
            mapData.mapId = Random.Range(1000, 9999);
            mapData.totalLayers = config.totalLayers;

            int nodeIdCounter = 0;

            // Layer 0: Start
            var startNode = new MapNode(nodeIdCounter++, MAP_NODE_TYPE.START, 0, 0);
            startNode.normalizedPosition = new Vector2(0.5f, 0f);
            mapData.nodes.Add(startNode.nodeId, startNode);
            mapData.startNodeId = startNode.nodeId;
            mapData.currentNodeId = startNode.nodeId;
            startNode.isCurrentPosition = true;
            startNode.isVisited = true;

            var previousLayer = new List<MapNode> { startNode };

            // Layer 1 ~ N-2: 중간 층
            for (int layer = 1; layer < config.totalLayers - 1; layer++)
            {
                int nodeCount = Random.Range(config.minNodesPerLayer, config.maxNodesPerLayer + 1);
                var currentLayer = new List<MapNode>();

                for (int i = 0; i < nodeCount; i++)
                {
                    var node = new MapNode(nodeIdCounter++, MAP_NODE_TYPE.NORMAL, layer, i);
                    float xPos = (i + 1f) / (nodeCount + 1f);

                    // 약간의 랜덤 오프셋으로 자연스러운 배치
                    float xJitter = Random.Range(-0.05f, 0.05f);
                    xPos = Mathf.Clamp(xPos + xJitter, 0.1f, 0.9f);

                    float yPos = (float)layer / (config.totalLayers - 1);
                    node.normalizedPosition = new Vector2(xPos, yPos);

                    mapData.nodes.Add(node.nodeId, node);
                    currentLayer.Add(node);
                }

                ConnectLayers(previousLayer, currentLayer, mapData, config);
                previousLayer = currentLayer;
            }

            // 마지막 층: Boss
            int bossLayer = config.totalLayers - 1;
            var bossNode = new MapNode(nodeIdCounter++, MAP_NODE_TYPE.BOSS, bossLayer, 0);
            bossNode.normalizedPosition = new Vector2(0.5f, 1f);
            mapData.nodes.Add(bossNode.nodeId, bossNode);
            mapData.bossNodeId = bossNode.nodeId;

            ConnectLayers(previousLayer, new List<MapNode> { bossNode }, mapData, config);

            // 시작 노드 히스토리 기록
            mapData.history.RecordVisit(mapData.startNodeId);

            return mapData;
        }

        private static void ConnectLayers(
            List<MapNode> prevLayer,
            List<MapNode> nextLayer,
            MapData mapData,
            MapGeneratorConfig config)
        {
            var connectedPrev = new HashSet<int>();
            var connectedNext = new HashSet<int>();

            // 1차: 각 다음 층 노드에서 가장 가까운 이전 층 노드들과 연결
            foreach (var nextNode in nextLayer)
            {
                int connectionCount = Random.Range(
                    config.minConnectionsPerNode,
                    Mathf.Min(config.maxConnectionsPerNode + 1, prevLayer.Count + 1));

                var candidates = new List<MapNode>(prevLayer);
                candidates.Sort((a, b) =>
                {
                    float distA = Mathf.Abs(a.normalizedPosition.x - nextNode.normalizedPosition.x);
                    float distB = Mathf.Abs(b.normalizedPosition.x - nextNode.normalizedPosition.x);
                    return distA.CompareTo(distB);
                });

                for (int i = 0; i < connectionCount && i < candidates.Count; i++)
                {
                    CreateEdge(candidates[i], nextNode, mapData);
                    connectedPrev.Add(candidates[i].nodeId);
                    connectedNext.Add(nextNode.nodeId);
                }
            }

            // 2차: 연결되지 않은 이전 층 노드 → 가장 가까운 다음 층 노드와 연결
            foreach (var prevNode in prevLayer)
            {
                if (connectedPrev.Contains(prevNode.nodeId)) continue;

                MapNode closest = null;
                float minDist = float.MaxValue;

                foreach (var nextNode in nextLayer)
                {
                    float dist = Mathf.Abs(nextNode.normalizedPosition.x - prevNode.normalizedPosition.x);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = nextNode;
                    }
                }

                if (closest != null)
                    CreateEdge(prevNode, closest, mapData);
            }
        }

        private static void CreateEdge(MapNode from, MapNode to, MapData mapData)
        {
            // 중복 체크
            foreach (var edge in mapData.edges)
            {
                if (edge.Connects(from.nodeId, to.nodeId)) return;
            }

            var newEdge = new MapEdge(from.nodeId, to.nodeId, true);
            mapData.edges.Add(newEdge);

            from.AddConnection(to.nodeId);
            to.AddConnection(from.nodeId);
        }
    }
}
