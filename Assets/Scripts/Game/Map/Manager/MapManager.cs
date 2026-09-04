using System;
using System.Collections.Generic;
using UnityEngine;
using S7;

namespace S7.Game.Map
{
    public class MapManager : Singleton<MapManager>
    {
        private MapData _currentMap;
        public MapData CurrentMap => _currentMap;

        public event Action<MapData> OnMapChanged;
        public event Action<int> OnNodeChanged;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void GenerateNewMap(MapGeneratorConfig? config = null)
        {
            _currentMap = MapGenerator.Generate(config ?? MapGeneratorConfig.Default);
            OnMapChanged?.Invoke(_currentMap);

            Debug.Log($"[MapManager] Map Generated: {_currentMap.nodes.Count} nodes, {_currentMap.edges.Count} edges");
        }

        public MapNode GetCurrentNode()
        {
            if (_currentMap == null) return null;
            return _currentMap.GetNode(_currentMap.currentNodeId);
        }

        public bool CanMoveToNode(int targetNodeId)
        {
            if (_currentMap == null) return false;
            return _currentMap.CanMoveTo(targetNodeId);
        }

        public List<MapNode> GetAccessibleNodes()
        {
            if (_currentMap == null) return new List<MapNode>();
            return _currentMap.GetAccessibleNodes();
        }

        public void MoveToNode(int targetNodeId)
        {
            if (!CanMoveToNode(targetNodeId))
            {
                Debug.LogWarning($"[MapManager] Cannot move to node {targetNodeId}");
                return;
            }

            var currentNode = GetCurrentNode();
            var targetNode = _currentMap.GetNode(targetNodeId);
            if (targetNode == null) return;

            // 히스토리 기록
            _currentMap.history.RecordTravel(currentNode.nodeId, targetNodeId);

            // Edge 히스토리 표시
            foreach (var edge in _currentMap.edges)
            {
                if (edge.Connects(currentNode.nodeId, targetNodeId))
                    edge.isOnHistoryPath = true;
            }

            // 현재 위치 변경
            currentNode.isCurrentPosition = false;
            targetNode.isCurrentPosition = true;
            targetNode.isVisited = true;
            _currentMap.currentNodeId = targetNodeId;

            OnNodeChanged?.Invoke(targetNodeId);

            // 필드 씬 전환
            // if (targetNode.fieldId > 0)
            // {
            //     GameFlowManager.Instance.RequestMoveField(targetNode.fieldId);
            // }
            // else
            // {
            //     Debug.LogWarning($"[MapManager] Node {targetNodeId} has no fieldId assigned");
            // }
            // TODO: remove Temp 
            GameFlowManager.Instance.RequestMoveDayField(1);
        }

        public void SetDirectionType(MAP_DIRECTION_TYPE type)
        {
            if (_currentMap == null) return;
            _currentMap.directionType = type;
            OnMapChanged?.Invoke(_currentMap);
        }

        public void AssignFieldIds(Func<MapNode, int> assignFunc)
        {
            if (_currentMap == null) return;

            foreach (var node in _currentMap.nodes.Values)
            {
                node.fieldId = assignFunc(node);
            }
        }

        public void ResetMap()
        {
            if (_currentMap == null) return;

            _currentMap.history.Clear();

            foreach (var node in _currentMap.nodes.Values)
            {
                node.isVisited = false;
                node.isCurrentPosition = false;
            }

            foreach (var edge in _currentMap.edges)
            {
                edge.isOnHistoryPath = false;
            }

            var startNode = _currentMap.GetNode(_currentMap.startNodeId);
            if (startNode != null)
            {
                startNode.isCurrentPosition = true;
                startNode.isVisited = true;
                _currentMap.currentNodeId = _currentMap.startNodeId;
                _currentMap.history.RecordVisit(_currentMap.startNodeId);
            }

            OnMapChanged?.Invoke(_currentMap);
        }
    }
}
