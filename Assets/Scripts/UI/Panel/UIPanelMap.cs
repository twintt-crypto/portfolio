using S7.Game.Map;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace S7
{
    public class UIPanelMap : UIBase
    {
        [Header("Container")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _mapContainer;

        [Header("Settings")]
        [SerializeField] private float _mapWidth = 800f;
        [SerializeField] private float _mapHeight = 1200f;
        [SerializeField] private float _nodeSize = 60f;
        [SerializeField] private float _lineThickness = 3f;

        [Header("Colors")]
        [SerializeField] private Color _colorNodeNormal = Color.white;
        [SerializeField] private Color _colorNodeVisited = new Color(0.5f, 0.5f, 0.5f, 1f);
        [SerializeField] private Color _colorNodeCurrent = new Color(0.2f, 0.8f, 0.2f, 1f);
        [SerializeField] private Color _colorNodeAccessible = new Color(1f, 0.9f, 0.3f, 1f);
        [SerializeField] private Color _colorLineNormal = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private Color _colorLineHistory = new Color(0f, 0.8f, 0.8f, 1f);

        [Header("UI")]
        [SerializeField] private Button _closeButton;

        private readonly Dictionary<int, Button> _nodeButtons = new Dictionary<int, Button>();
        private readonly Dictionary<int, Image> _nodeImages = new Dictionary<int, Image>();
        private readonly List<GameObject> _lineObjects = new List<GameObject>();

        protected override void Initialize()
        {
            base.Initialize();

            if (_closeButton != null)
                _closeButton.onClick.AddListener(OnClose);

            MapManager.Instance.OnMapChanged += OnMapChanged;
            MapManager.Instance.OnNodeChanged += OnNodeChanged;

            DrawMap();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (!MapManager.IsDestroy())
            {
                MapManager.Instance.OnMapChanged -= OnMapChanged;
                MapManager.Instance.OnNodeChanged -= OnNodeChanged;
            }
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        #region Draw

        private void DrawMap()
        {
            ClearMap();

            var mapData = MapManager.Instance.CurrentMap;
            if (mapData == null) return;

            // 컨테이너 크기 설정
            _mapContainer.sizeDelta = new Vector2(_mapWidth, _mapHeight);

            DrawEdges(mapData);
            DrawNodes(mapData);
            ScrollToCurrentNode();
        }

        private void DrawNodes(MapData mapData)
        {
            foreach (var node in mapData.nodes.Values)
            {
                var nodeObj = CreateNodeObject(node);
                var btn = nodeObj.GetComponent<Button>();
                var img = nodeObj.GetComponent<Image>();

                int nodeId = node.nodeId;
                btn.onClick.AddListener(() => OnNodeClicked(nodeId));

                _nodeButtons[nodeId] = btn;
                _nodeImages[nodeId] = img;

                UpdateNodeVisual(node);
            }
        }

        private GameObject CreateNodeObject(MapNode node)
        {
            var nodeObj = new GameObject($"Node_{node.nodeId}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            nodeObj.transform.SetParent(_mapContainer, false);

            var rect = nodeObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(
                node.normalizedPosition.x * _mapWidth,
                node.normalizedPosition.y * _mapHeight
            );
            rect.sizeDelta = new Vector2(_nodeSize, _nodeSize);

            // 원형 표시를 위해 기본 Unity sprite 사용
            var img = nodeObj.GetComponent<Image>();
            img.sprite = null; // 기본 사각형, 추후 원형 sprite 교체 가능
            img.color = _colorNodeNormal;

            // 텍스트 (노드 타입 표시)
            var textObj = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(nodeObj.transform, false);

            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = GetNodeLabel(node.nodeType);
            tmp.fontSize = 20;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.black;
            tmp.raycastTarget = false;

            return nodeObj;
        }

        private void DrawEdges(MapData mapData)
        {
            foreach (var edge in mapData.edges)
            {
                var fromNode = mapData.GetNode(edge.fromNodeId);
                var toNode = mapData.GetNode(edge.toNodeId);
                if (fromNode == null || toNode == null) continue;

                var lineObj = new GameObject($"Edge_{edge.fromNodeId}_{edge.toNodeId}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                lineObj.transform.SetParent(_mapContainer, false);
                lineObj.transform.SetAsFirstSibling();

                Vector2 fromPos = new Vector2(
                    fromNode.normalizedPosition.x * _mapWidth,
                    fromNode.normalizedPosition.y * _mapHeight
                );
                Vector2 toPos = new Vector2(
                    toNode.normalizedPosition.x * _mapWidth,
                    toNode.normalizedPosition.y * _mapHeight
                );

                Vector2 direction = toPos - fromPos;
                float distance = direction.magnitude;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                var lineRect = lineObj.GetComponent<RectTransform>();
                lineRect.anchorMin = Vector2.zero;
                lineRect.anchorMax = Vector2.zero;
                lineRect.pivot = new Vector2(0f, 0.5f);
                lineRect.anchoredPosition = fromPos;
                lineRect.sizeDelta = new Vector2(distance, _lineThickness);
                lineRect.localRotation = Quaternion.Euler(0, 0, angle);

                var lineImg = lineObj.GetComponent<Image>();
                lineImg.color = edge.isOnHistoryPath ? _colorLineHistory : _colorLineNormal;
                lineImg.raycastTarget = false;

                _lineObjects.Add(lineObj);
            }
        }

        #endregion

        #region Visual Update

        private void UpdateNodeVisual(MapNode node)
        {
            if (!_nodeImages.TryGetValue(node.nodeId, out var img)) return;
            if (!_nodeButtons.TryGetValue(node.nodeId, out var btn)) return;

            if (node.isCurrentPosition)
            {
                img.color = _colorNodeCurrent;
                btn.interactable = false;
            }
            else if (MapManager.Instance.CanMoveToNode(node.nodeId))
            {
                img.color = _colorNodeAccessible;
                btn.interactable = true;
            }
            else if (node.isVisited)
            {
                img.color = _colorNodeVisited;
                btn.interactable = false;
            }
            else
            {
                img.color = _colorNodeNormal;
                btn.interactable = false;
            }
        }

        private void UpdateAllNodeVisuals()
        {
            var mapData = MapManager.Instance.CurrentMap;
            if (mapData == null) return;

            foreach (var node in mapData.nodes.Values)
            {
                UpdateNodeVisual(node);
            }
        }

        private void RedrawEdges()
        {
            ClearLines();

            var mapData = MapManager.Instance.CurrentMap;
            if (mapData != null) DrawEdges(mapData);
        }

        #endregion

        #region Scroll

        private void ScrollToCurrentNode()
        {
            var currentNode = MapManager.Instance.GetCurrentNode();
            if (currentNode == null || _scrollRect == null) return;

            _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(currentNode.normalizedPosition.y);
        }

        #endregion

        #region Events

        private void OnNodeClicked(int nodeId)
        {
            if (!MapManager.Instance.CanMoveToNode(nodeId)) return;

            UIManager.Instance.ClosePanel("UIPanelMap");
            MapManager.Instance.MoveToNode(nodeId);
        }

        private void OnMapChanged(MapData mapData)
        {
            DrawMap();
        }

        private void OnNodeChanged(int nodeId)
        {
            UpdateAllNodeVisuals();
            RedrawEdges();
            ScrollToCurrentNode();
        }

        #endregion

        #region Cleanup

        private void ClearMap()
        {
            ClearNodes();
            ClearLines();
        }

        private void ClearNodes()
        {
            foreach (var btn in _nodeButtons.Values)
            {
                if (btn != null) Destroy(btn.gameObject);
            }
            _nodeButtons.Clear();
            _nodeImages.Clear();
        }

        private void ClearLines()
        {
            foreach (var line in _lineObjects)
            {
                if (line != null) Destroy(line);
            }
            _lineObjects.Clear();
        }

        #endregion

        #region Utility

        private static string GetNodeLabel(MAP_NODE_TYPE type)
        {
            switch (type)
            {
                case MAP_NODE_TYPE.START: return "S";
                case MAP_NODE_TYPE.NORMAL: return "N";
                case MAP_NODE_TYPE.ELITE: return "E";
                case MAP_NODE_TYPE.REST: return "R";
                case MAP_NODE_TYPE.SHOP: return "$";
                case MAP_NODE_TYPE.TREASURE: return "T";
                case MAP_NODE_TYPE.BOSS: return "B";
                default: return "?";
            }
        }

        #endregion
    }
}

