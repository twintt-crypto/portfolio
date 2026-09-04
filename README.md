Unity로 개발한 **필드 탐험 + 턴제 전투 RPG 클라이언트**입니다.

이 저장소는 포트폴리오 검토를 위한 소스 코드 공개본입니다. 상용 에셋과 프로젝트 데이터는 제외했으며, 제가 중점적으로 구현한 **전투 시스템, 캐릭터 툴, 연출 그래프, 연출 뷰어** 코드를 중심으로 구성했습니다.

## 핵심 요약

> **기본 물리 공격까지 모든 공격을 스킬로 통합하고, 전투 시스템과 콘텐츠 제작 도구를 하나의 파이프라인으로 설계했습니다.**
>
> 캐릭터 애니메이션 세팅 → 연출 그래프 조립 → 전용 뷰어 검증 → 실제 전투 실행까지 이어지는 작업 흐름을 구현했습니다.

| 핵심 구현 | 해결한 문제 | 대표 코드 |
| --- | --- | --- |
| 턴제 전투 | 속도 기반 턴, 입력/AI 행동, 스킬·버프·연출과 Cinemachine 카메라를 비동기로 통합 | `BattleManager`, `TurnManager`, `BattleCameraManager` |
| 오브젝트 풀링 | Addressables 생성 비용을 사전 적재·키별 재사용·자동 반납으로 분산 | `ObjectPoolManager`, `ObjectPool` |
| 캐릭터 툴 | 애니메이션 재생·스크러빙, 이벤트·이펙트 배치, 설정 저장을 한 창에서 처리 | `CharacterToolWindow` |
| 연출 그래프 | 스킬 연출을 노드 데이터로 제작하고 순차·분기·병렬로 실행 | `PresentationGraphWindow`, `GraphExecutor` |
| 연출 뷰어 | 실제 전투 진입 없이 캐릭터·몬스터·스킬 연출을 반복 검증 | `SkillPreviewWindow`, `SkillPreviewRunner` |

## 1. 턴제 전투 시스템

전투 루프가 입력, 계산, 애니메이션, 카메라에 강하게 결합되지 않도록 **턴 진행·스킬 판정·연출 실행·화면 표현의 책임을 분리**했습니다. 각 시스템은 명확한 컨텍스트와 이벤트로 연결되며, 새로운 스킬과 연출을 기존 전투 루프의 큰 수정 없이 추가할 수 있습니다.

### 핵심 설계 원칙 — 모든 공격을 스킬로 통합

이 전투 시스템에서 가장 중요하게 생각한 부분은 **기본 물리 공격까지 포함한 모든 공격 행동을 `UnitSkill`로 표현하는 것**입니다.

일반 공격만 별도의 하드코딩된 데미지 함수를 호출하고 액티브 스킬은 별도 시스템을 타는 구조를 피했습니다. 플레이어가 `Attack`을 선택하면 캐릭터의 `AttackSkillId`로 `UnitSkill`을 조회하며, 이후 과정은 스킬 공격·특수기·궁극기와 동일하게 `SkillManager.UseSkill()`로 합류합니다.

```text
일반 공격 ── AttackSkillId ─────┐
스킬 공격 ── SkillAttackSkillId ─┤
특수기 ───── SpecialSkill ───────┼→ UnitSkill → SkillManager
궁극기 ───── UltimateSkillId ────┘              ↓
                                  Executor → SkillResult
                                          ↓
                                  Presentation Graph
```

`T_SkillData` 하나에서 다음 요소를 데이터로 조합합니다.

- `SkillType`: 스킬의 역할 구분
- `ActionType`: 공격·회복·버프·소환 등 실행 로직 선택
- `AttackType`: 일반·투사체·궁극기 등 공격 방식 선택
- `TargetType` / `TargetScope`: 대상과 범위 결정
- `StatRate` / `BreakRate` / `SplashEffectRate`: 수치 계산 정보
- `ActivationCondition`: 발동 조건
- `PresentationGraph`: 실행할 연출 그래프
- `ProjectileId` / `EffectId`: 투사체와 부가 효과 연결

이 구조를 선택한 이유는 다음과 같습니다.

- **일관된 실행 경로**: 기본 공격과 액티브 스킬이 같은 타깃 선정, 결과 계산, 연출, 피격 처리 과정을 사용합니다.
- **데이터 중심 밸런싱**: 물리 공격도 스킬 데이터이므로 계수, 대상, 부가 효과와 연출을 코드 수정 없이 교체할 수 있습니다.
- **콘텐츠 확장성**: 평타를 연속 공격, 투사체, 범위 공격 또는 상태 효과가 포함된 공격으로 바꿔도 전투 루프를 수정할 필요가 없습니다.
- **연출 재사용**: 모든 공격이 `PresentationGraph`를 가지므로 기본 공격도 캐릭터별 애니메이션·카메라·이펙트 시퀀스를 독립적으로 구성할 수 있습니다.
- **AI와 플레이어의 통합**: 적의 랜덤 스킬 선택과 플레이어의 버튼 입력이 최종적으로 동일한 `UnitSkill` 실행 파이프라인을 사용합니다.

즉, 전투 행동을 코드의 예외 분기로 추가하는 대신 **데이터 + 실행기 + 연출 그래프의 조합으로 생산하는 구조**를 목표로 했습니다.
### 전투 초기화

`BattleManager.ReadyBattle()`에서 전투에 필요한 시스템을 순서대로 구성합니다.

1. `BattleStage`가 씬의 Anchor 정보를 초기화합니다.
2. `BattleUnitManager`가 `BattleContext`의 아군과 적군을 런타임 유닛으로 생성합니다.
3. `TurnManager`가 전투 참여 가능 유닛을 선별하고 **Speed 스탯 내림차순**으로 행동 순서를 만듭니다.
4. `SkillManager`가 스킬 Executor와 전체 유닛 컨텍스트를 구성합니다.
5. `BattleCameraManager`가 Cinemachine 전투 카메라를 초기 구도로 전환합니다.

### 턴 상태 머신

전투 흐름을 다음 `TurnState`로 명시적으로 표현했습니다.

```text
StartBattle
  ├─ PlayerTurn → SelectSkill → Attack
  └─ EnemyTurn → EnemySelectSkill → EnemySelectTarget → EnemyAttack
                                                        ↓
                                                     EndTurn
```

상태가 바뀔 때마다 `TurnStateChange` 이벤트와 현재 `TurnContext`를 함께 발행합니다. 전투 UI와 입력 계층은 전투 루프를 직접 참조하지 않고 상태 이벤트를 기준으로 활성화할 수 있습니다.

각 턴에는 시전자, 대상 목록, 선택 스킬을 담은 `TurnContext`를 새로 생성합니다. 사망했거나 행동할 수 없는 유닛은 건너뛰고, 행동이 끝나면 인덱스를 순환시켜 다음 유닛으로 진행합니다.

### 플레이어와 적 턴 실행

```text
행동 유닛 선택
  → Buff OnTurnStart
  → 플레이어 입력 대기 / 적 스킬·타깃 결정
  → 타깃 표시 및 Cinemachine 구도 전환
  → 스킬 결과 계산
  → Presentation Graph 재생
  → 애니메이션 Hit Event 시점에 결과 반영
  → 공격자 원위치 복귀
  → Buff OnTurnEnd
  → 다음 턴
```

플레이어 턴은 선택 상태에서 스킬 입력을 비동기로 기다리고, 적 턴은 스킬과 대상을 결정한 뒤 동일한 `SkillManager.UseSkill()` 경로로 합류합니다. 아군과 적군이 같은 실행 파이프라인을 사용하므로 효과 계산과 연출 처리의 중복을 줄였습니다.

### 스킬 계산과 연출의 분리

`SkillManager`는 먼저 스킬 데이터를 Executor에 전달해 `SkillResult`를 생성한 뒤, 결과와 유닛 View를 `PresentationContext`로 변환하여 연출 그래프를 실행합니다.

- `AttackExecutor`: 공격 결과 계산
- `HealExecutor`: 회복 처리
- `BuffExecutor`: 상태 효과 처리
- `SummonExecutor`: 소환 처리
- `ProcessSkillEffect`: 즉시/지속 효과 적용 시점 분리

연출 그래프는 `PresentationContext.onHit` 콜백을 통해 애니메이션이나 투사체의 실제 피격 프레임을 `SkillManager`에 전달합니다. `hitIndex`에 해당하는 계산 결과를 조회해 피격 피드백을 재생하며, 효과 반영 지점을 연출 타이밍과 연결할 수 있도록 구성했습니다.

투사체 스킬은 실행 전에 필요한 프리팹을 오브젝트 풀에 미리 적재하여 연출 도중의 생성 비용과 로딩 지연을 줄이도록 구성했습니다.

### Addressables 기반 오브젝트 풀링

전투 중 반복적으로 생성되는 투사체와 이펙트의 Instantiate/Destroy 비용 및 순간 로딩을 줄이기 위해 **Addressables와 연동되는 오브젝트 풀을 직접 구현**했습니다.

```text
ResourceManager.NewAsync(key, usePooling: true)
  ├─ 대기 객체 있음 → Queue에서 Dequeue → Parent/Position 초기화 → 활성화
  └─ 대기 객체 없음 → Addressables.InstantiateAsync
                         └─ 기본 수량을 백그라운드 Preload Queue에 등록

사용 완료
  → ResourceManager.Free(gameObject)
  → 이름을 Key로 Pool 탐색
  → 비활성화 + 전용 Container 이동 + Queue에 반납
```

#### 주요 구현

- Addressables 주소를 Key로 사용하는 풀별 `Queue<GameObject>` 관리
- `PreLoadAsync(key, count)`를 통한 필요한 수량의 명시적 사전 생성
- 최초 요청 시 객체를 즉시 반환하고 나머지 기본 수량은 비동기 로딩 큐에서 보충
- 프레임마다 로딩 요청을 처리하는 UniTask 기반 백그라운드 Preload Loop
- 풀에서 꺼낼 때 Parent, 활성 상태, Local Position 초기화
- `DontDestroyOnLoad` 관리자와 비활성 전용 Container를 통한 대기 객체 관리
- 씬 초기화와 관리자 종료 시 Pool 일괄 정리
- `ResourceManager`에서 풀링 사용 여부와 관계없이 동일한 생성·반납 API 제공

#### 전투 시스템 적용

`SkillManager`는 투사체 데이터가 있는 스킬을 실행하기 전에 해당 프리팹 5개를 미리 적재합니다. 연출 중 투사체가 필요할 때 Addressables 로딩을 기다리지 않고 즉시 사용할 수 있도록 하기 위함입니다.

- `Projectile`: 타깃 충돌 및 Hit 콜백 실행 후 Pool 반환
- `FieldArrow`: 필드 투사체 사용 완료 후 Pool 반환
- `EffectAutoRelease`: 지정 시간이 지나거나 모든 ParticleSystem 재생이 끝나면 자동 반환

특히 `EffectAutoRelease`는 고정 시간 방식과 ParticleSystem 생존 상태 검사 방식을 모두 지원해 이펙트마다 별도의 제거 코드를 작성하지 않아도 됩니다. 이를 통해 전투 로직은 생성과 사용에만 집중하고, 반복 객체의 수명 관리는 공통 계층에서 처리하도록 분리했습니다.

**주요 코드**

- `Assets/Scripts/Utility/ObjectPool/ObjectPoolManager.cs`
- `Assets/Scripts/Utility/ObjectPool/ObjectPool.cs`
- `Assets/Scripts/Manager/ResourceManager.cs`
- `Assets/Scripts/Game/EffectAutoRelease.cs`
- `Assets/Scripts/Game/Projectile/Projectile.cs`
### 확장 가능한 전투 구조

- 스킬 효과: `ISkillActionExecutor` — 공격, 회복, 버프, 소환
- 공격 방식: `IAttackStrategy` — 일반, 투사체, 궁극기
- 상태 효과: `IBuff` — 능력치 변화, 보호막, 도발, 침묵, 기절, 지속 피해·회복

각 기능을 인터페이스와 Factory/Registry 구조로 분리했습니다. 새 스킬 동작은 Executor, 새 공격 연출 방식은 Strategy, 새 상태 효과는 Buff 구현체를 추가하는 방식으로 확장할 수 있습니다.

### Cinemachine 전투 카메라

전투 카메라는 Unity Cinemachine 3의 `CinemachineCamera`와 `CinemachineRotationComposer`를 사용해 구현했습니다. 단순 고정 카메라가 아니라 **턴 상태와 공격 단계에 따라 Follow·LookAt 대상을 런타임에 교체하는 전투 연출 시스템**입니다.

| 카메라 타입 | 용도 |
| --- | --- |
| `EnemyWide` | 전투 시작과 적 진영 전체를 보여주는 기본 구도 |
| `EnemySingle` | 선택한 대상 또는 행동 유닛을 강조하는 단일 구도 |
| `FllowEnemySingle` | 공격자를 따라가면서 타깃을 바라보는 공격 구도 |
| `TargetAlly` | 아군 대상 연출을 위한 구도 |

`BattleCameraManager`는 카메라 타입과 Virtual Camera를 Dictionary로 매핑하고 현재 카메라만 활성화합니다. 상황에 따라 다음 값을 동적으로 설정합니다.

- `Follow`: 공격자 또는 행동 유닛의 Transform/TargetPoint
- `LookAt`: 선택한 적 또는 공격 대상의 Transform/TargetPoint
- `Damping`: 선택 화면과 공격 연출에 맞춘 추적 반응 속도

타깃 선택 시 `EnemySingle`의 LookAt을 갱신하고, 공격 직전에는 `FllowEnemySingle`로 전환해 시전자를 추적하며 대상을 바라보게 합니다. 카메라 교체 사이에 한 프레임을 양보해 활성 상태와 Follow/LookAt 갱신 순서를 보장하고, 스킬 연출 종료 후 공격 카메라를 비활성화합니다.

이를 통해 전투 로직은 “어떤 구도가 필요한지”만 요청하고, Cinemachine의 실제 추적 대상과 Composer 설정은 카메라 매니저가 전담하도록 분리했습니다.

### 비동기 실행과 종료 처리

UniTask와 `CancellationToken`으로 입력 대기, 카메라 전환, 캐릭터 이동, 애니메이션, 연출 그래프를 하나의 비동기 흐름으로 연결했습니다. 전투 종료 시 토큰을 취소하여 진행 중인 턴과 연출이 다음 씬까지 남지 않도록 제어합니다.

**주요 코드**

- `Assets/Scripts/Game/Battle/Domain/Battle/BattleManager.cs`
- `Assets/Scripts/Game/Battle/Domain/Battle/BattleManager.BattleLoop.cs`
- `Assets/Scripts/Game/Battle/Domain/Battle/BattleCameraManager.cs`
- `Assets/Scripts/Game/Battle/Domain/Turn`
- `Assets/Scripts/Game/Battle/Domain/Skill`
- `Assets/Scripts/Game/Battle/Domain/Buff`
## 2. 캐릭터 툴

`Tools/S7/Character Tool`은 캐릭터 프리팹과 애니메이션을 게임에 적용하기 전에 **재생, 이벤트 편집, 이펙트 배치, 데이터 저장을 한 화면에서 처리**하기 위해 제작한 Unity Editor 도구입니다.

### 제작 배경

애니메이션 이벤트와 이펙트 타이밍을 맞추려면 Animator, 프리팹, Hierarchy, Inspector를 반복해서 오가야 했습니다. 이를 줄이기 위해 전용 Preview Scene과 타임라인 UI를 만들고, 편집 결과가 런타임에서 사용하는 `CharacterAnimationSet`에 바로 저장되도록 연결했습니다.

### 애니메이션 프리뷰

- 툴 실행 시 `ScenePreview`를 자동으로 열어 일정한 테스트 환경 구성
- 캐릭터 프리팹을 임시 인스턴스로 생성하여 원본 에셋과 씬 오염 방지
- Animator Controller의 State Machine과 하위 State Machine을 순회해 상태 목록 자동 수집
- 선택한 State의 AnimationClip과 재생 길이 자동 탐색
- Play/Pause/Reset과 타임 슬라이더를 이용한 임의 시점 스크러빙
- `EditorApplication.update`를 사용해 Play Mode가 아닌 상태에서도 애니메이션 진행

### 이벤트 타임라인 편집

Animation State별 이벤트를 타임라인 마커로 표시하며, 현재 프레임에서 이벤트를 추가하거나 마커를 드래그해 시간을 조절할 수 있습니다.

각 이벤트에는 다음 정보를 저장합니다.

- 이벤트 시간과 `AnimationEventType`
- 문자열·실수·정수·불리언 파라미터
- 이펙트 프리팹과 Addressables 주소
- 캐릭터 Socket 이름
- 위치·회전 Offset
- 이펙트 지속 시간

`SpawnEffect` 이벤트를 선택하면 캐릭터 Hierarchy에서 수집한 Socket 목록을 드롭다운으로 제공합니다. 선택한 Socket에 이펙트를 생성하고 Offset을 적용해 실제 부착 위치를 즉시 확인할 수 있습니다. 파티클 시스템의 길이를 분석해 지속 시간을 자동 설정하며, 스크러빙 또는 재생 위치에 맞춰 에디터 이펙트를 갱신합니다.

### 데이터와 Addressables 연동

- State 이름과 Hash, 이벤트 목록을 `CharacterAnimationSet` ScriptableObject에 저장
- 변경 시 `SetDirty`, `SaveAssets`, `Refresh`로 에셋 영속화
- 선택한 이펙트가 Addressables에 없으면 엔트리를 생성하고 런타임 주소 저장
- 편집 모드와 Play Mode의 데이터를 분리해 실행 중 원본 데이터 변경 방지
- 창 종료 또는 캐릭터 교체 시 임시 캐릭터와 이펙트 정리

### Animator Override 자동 매핑

`Tools/Animator Override Auto Assign`은 캐릭터별 애니메이션 교체 작업을 자동화하는 별도 도구입니다.

1. 대상 `AnimatorOverrideController`, 애니메이션 폴더, 매핑 Preset을 선택합니다.
2. Base Controller의 모든 State Machine을 재귀 탐색해 원본 Clip을 수집합니다.
3. Preset에 정의된 정규식 패턴을 우선순위대로 적용합니다.
4. 결과를 `SUCCESS`, `MULTIPLE`, `NO_MATCH`, `NO_RULE`로 분류해 표시합니다.
5. 후보가 여러 개인 항목은 드롭다운에서 사람이 최종 Clip을 선택합니다.
6. 검토된 결과만 일괄 적용하고 에셋으로 저장합니다.

첫 패턴이 없을 때 후순위 패턴을 폴백으로 사용하며, 자동 매핑의 속도와 수동 검토의 안전성을 함께 확보했습니다.

**주요 코드**

- `Assets/Scripts/Tool/Character/Editor/CharacterToolWindow.cs`
- `Assets/Scripts/Tool/Character/Editor/AnimatorOverrideAutoAssignWindow.cs`
- `Assets/Scripts/Tool/Character/AnimatorClipMappingPreset.cs`
- `Assets/Scripts/Tool/Character/CharacterAnimationSet.cs`
- `Assets/Scripts/Tool/Character/AnimationStateEventData.cs`

## 3. 연출 그래프 에디터

`Tools/S7/Presentation Graph`는 스킬 연출을 코드에 하드코딩하지 않고 **노드를 조립해 제작하는 GraphView 기반 Editor Window**입니다. 편집용 그래프와 런타임 실행 객체를 분리하여 제작 편의성과 런타임 독립성을 함께 확보했습니다.

### 그래프 제작 기능

- New, Load, Save, Save As를 지원하는 Toolbar
- `PresentationNodeType`별 노드 생성 메뉴
- 노드 드래그, 연결, 삭제와 위치 저장
- 노드 타입에 맞춘 입력·출력 Port 동적 구성
- 노드별 제목과 3개의 범용 파라미터 편집
- Start Node 삭제·이동 방지
- 그래프 자동 정렬

그래프 저장 시 각 노드의 GUID, 타입, 위치, 파라미터를 기록하고, Edge에는 시작/도착 노드 GUID와 양쪽 Port 이름을 저장합니다. 로드할 때 GUID Map으로 노드를 복원하고 Port 이름을 기준으로 연결을 재구성하므로 Choice나 Fork처럼 출력이 여러 개인 노드도 정확히 복원할 수 있습니다.

### 자동 레이아웃

단순히 노드를 한 줄로 배치하지 않고 연결 관계를 분석해 다음 요소를 반영합니다.

- Start Node부터 진행 방향 계산
- 이전/다음 노드 관계 Map 구성
- 일반 흐름의 X축 배치
- Branch/Fork Lane의 Y축 간격 분리
- Fork에서 Join까지의 병렬 블록 탐색
- 합류 지점 이후 흐름 재정렬

복잡한 병렬 연출을 수정한 후 수동으로 노드를 다시 정렬하는 시간을 줄이는 기능입니다.

### 런타임 변환과 실행

```text
PresentationGraphWindow
  → PresentationGraphAsset
  → PresentationRuntimeGraphBuilder
  → RuntimeNode + IPresentationNode
  → GraphExecutor
```

`PresentationRuntimeGraphBuilder`가 직렬화된 노드 타입을 실제 `IPresentationNode` 구현체로 변환하고 Edge 정보를 런타임 연결로 구성합니다. `GraphExecutor`는 CancellationToken을 전달하면서 노드를 비동기로 실행합니다.

지원하는 주요 노드는 다음과 같습니다.

- 애니메이션 재생 및 애니메이션 이벤트 대기
- 캐스팅, 이동, 바라보기, 페이드
- 투사체 생성·발사와 피격 이벤트 등록/해제
- Timeline, 대화, QTE 실행
- 조건 분기와 Choice
- Fork/Join 기반 병렬 연출
- 아군 표시·숨김 등 전투 화면 제어

각 노드는 `PresentationContext`를 통해 시전자, 대상, 스킬 결과, 아군·적군 목록, Timeline 공급자와 Hit 콜백을 전달받습니다. 동일한 그래프를 실제 전투와 연출 뷰어 양쪽에서 실행할 수 있습니다.

**주요 코드**

- `Assets/Scripts/Game/Presentation/Graph/Editor/PresentationGraphWindow.cs`
- `Assets/Scripts/Game/Presentation/Graph/Editor/PresentationGraphView.cs`
- `Assets/Scripts/Game/Presentation/Graph/Editor/PresentationGraphNodeView.cs`
- `Assets/Scripts/Game/Presentation/Graph/Runtime`
- `Assets/Scripts/Game/Presentation/Node`

## 4. 스킬 연출 뷰어

`Tools/S7/Skill Preview`는 전체 게임 플로우나 실제 전투 씬에 진입하지 않고 **실제 데이터와 동일한 연출 실행기를 사용해 스킬을 빠르게 검증**하는 도구입니다.

### 자동 테스트 환경 구성

1. 수정 중인 씬의 저장 여부를 확인합니다.
2. 전용 `SceneSkillPreview` 씬을 열고 Play Mode로 진입합니다.
3. Addressables를 초기화하고 실제 `T_UnitData`를 로드합니다.
4. 데이터를 Character와 Monster로 분류해 선택 목록을 만듭니다.
5. 지정 폴더의 `PresentationGraphAsset`을 검색하고 이름순으로 정렬합니다.
6. 기본 캐릭터와 몬스터를 각각의 Anchor에 생성합니다.

툴 창 하나에서 Character, Monster, Graph를 선택하고 Play/Stop할 수 있으며, 새 그래프를 만든 뒤 `Refresh Graphs`로 즉시 목록을 갱신할 수 있습니다.

### 실제 런타임과 동일한 실행 경로

`SkillPreviewContextBuilder`가 프리뷰 유닛을 `PresentationContext`로 변환하고, 실제 전투와 동일한 `PresentationRuntimeGraphBuilder`와 `GraphExecutor`를 사용합니다. 뷰어 전용으로 연출 로직을 복제하지 않았기 때문에 프리뷰 결과와 런타임 결과의 차이를 줄였습니다.

그래프에 Timeline Node가 있을 때만 `TimelineAddressableProvider`를 생성해 필요한 Timeline을 동적으로 공급합니다. 모든 노드는 CancellationToken을 전달받으며 Stop 버튼으로 실행 중인 연출을 취소할 수 있습니다.

### 반복 검증과 리소스 정리

- 재생 중 중복 Play 방지
- 캐릭터 또는 몬스터 변경 시 기존 Addressables 모델 해제 후 재생성
- 연출 종료 후 캐릭터 위치와 회전 원상 복구
- 툴 Window 비활성화 또는 종료 시 실행 취소 및 프리뷰 인스턴스 해제
- 그래프 또는 유닛 데이터가 없는 경우 실행 전 검증과 경고 출력

이를 통해 연출 하나를 확인하기 위해 게임 시작, 패치, 필드 진입, 적 조우, 스킬 선택을 반복할 필요 없이 **제작 직후 선택–재생–수정 사이클을 짧게 반복**할 수 있습니다.

**주요 코드**

- `Assets/Scripts/Tool/SkillPreview/Editor/SkillPreviewWindow.cs`
- `Assets/Scripts/Tool/SkillPreview/SkillPreviewRunner.cs`
- `Assets/Scripts/Tool/SkillPreview/SkillPreviewContextBuilder.cs`
- `Assets/Scripts/Tool/SkillPreview/PreviewUnitController.cs`
## 사용 기술

| 구분 | 기술 |
| --- | --- |
| 엔진 | Unity 6 (`6000.3.21f1`), C# |
| 비동기 | UniTask, CancellationToken |
| 리소스 | Unity Addressables, Object Pooling |
| 에디터 확장 | EditorWindow, GraphView, AssetDatabase |
| 전투 연출 | Cinemachine, Timeline, DOTween, Animation Event |
| 구조 | State Machine, Strategy, Executor, Factory, Event Bus |
| UI | uGUI, TextMesh Pro |
| DI | VContainer |

## 코드 구성

```text
Assets/Scripts/
├─ Game/
│  ├─ Battle/Domain/              전투, 턴, 스킬, 버프
│  ├─ Presentation/
│  │  ├─ Graph/Editor/            연출 그래프 제작 UI
│  │  ├─ Graph/Runtime/           그래프 빌드와 실행
│  │  └─ Node/                    연출 노드 구현체
│  └─ Unit/                       유닛 데이터, 컨트롤러, 뷰
├─ Tool/
│  ├─ Character/                  캐릭터 세팅 및 애니메이션 도구
│  └─ SkillPreview/               스킬 연출 뷰어
├─ UI/Battle/                     전투 HUD와 턴 UI
└─ Manager/                       씬, Addressables, UI 관리
```

## 추가 구현 영역

핵심 포트폴리오 영역 외에도 다음 시스템을 구현했습니다.

- Additive Scene 기반 필드/전투 전환
- Addressables 패치 및 게임 데이터 로딩
- 필드 플레이어 조작과 적 AI Strategy
- 이벤트 기반 퀘스트 조건 처리
- 대화와 QTE 시스템
- UI 패널·팝업 및 오브젝트 풀

## 공개 범위

이 저장소에는 `Assets/Scripts`와 문서만 포함되어 있어 Unity 프로젝트를 그대로 실행할 수 없습니다. 모델, 애니메이션, 프리팹, 씬, 데이터 테이블 및 유료 에셋은 저작권과 프로젝트 보안을 위해 제외했습니다.