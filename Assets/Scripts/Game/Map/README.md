# Map System (맵 시스템)

컬트 오브 더 램 스타일의 노드 그래프 맵 시스템.
프로시저럴 생성, bottom-up 층별 노드 배치, 히스토리 추적.

## 구조

```
Map/
├── MapEnumType.cs              # MAP_NODE_TYPE, MAP_DIRECTION_TYPE
├── Data/
│   ├── MapNode.cs              # 개별 노드 (타입, 층, 연결, 방문 상태)
│   ├── MapEdge.cs              # 노드 간 연결선
│   ├── MapHistory.cs           # 이동 경로 기록
│   └── MapData.cs              # 전체 맵 데이터 + 이동 가능 판단
├── Generator/
│   └── MapGenerator.cs         # 프로시저럴 맵 생성 알고리즘
└── Manager/
    └── MapManager.cs           # Singleton. 맵 상태 관리, 이동, 이벤트
```

UI: `Scripts/UI/Panel/UIPanelMap.cs` (UIBase 상속)

## 흐름

```
MapManager.GenerateNewMap()
  → MapGenerator.Generate(config)
    → Start 노드 (layer 0)
    → 중간 층 (layer 1~N-2, 각 2~4개 노드)
    → Boss 노드 (마지막 층)
    → 층간 x좌표 근접 기반 연결

UIPanelMap 열기
  → GameFlowManager.OpenMapPanel()
    → UIManager.OpenPanelAsync("UIPanelMap")

노드 선택
  → MapManager.MoveToNode(nodeId)
    → 히스토리 기록
    → GameFlowManager.RequestMoveDayField(fieldId)
```

## 양방향 / 단방향

현재 양방향 이동. 단방향 전환 시:
```csharp
MapManager.Instance.SetDirectionType(MAP_DIRECTION_TYPE.IRREVERSIBLE);
```
→ `GetAccessibleNodes()`에서 상위 층(layer가 큰 노드)으로만 이동 가능하도록 필터링됨.
