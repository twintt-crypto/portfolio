# SEQ7 Client

Unity로 개발한 **필드 탐험 + 턴제 전투 RPG 클라이언트**입니다.

이 저장소는 포트폴리오 검토를 위한 소스 코드 공개본입니다. 상용 에셋과 프로젝트 데이터는 제외했으며, 제가 중점적으로 구현한 **전투 시스템, 캐릭터 툴, 연출 그래프, 연출 뷰어** 코드를 중심으로 구성했습니다.

## 핵심 요약

> **런타임 전투 시스템과 콘텐츠 제작 도구를 하나의 파이프라인으로 설계했습니다.**
>
> 캐릭터 애니메이션 세팅 → 연출 그래프 조립 → 전용 뷰어 검증 → 실제 전투 실행까지 이어지는 작업 흐름을 구현했습니다.

| 핵심 구현 | 해결한 문제 | 대표 코드 |
| --- | --- | --- |
| 턴제 전투 | 속도 기반 턴, 입력/AI 행동, 스킬·버프·연출과 Cinemachine 카메라를 비동기로 통합 | `BattleManager`, `TurnManager`, `BattleCameraManager` |
| 캐릭터 툴 | 애니메이션 재생·스크러빙, 이벤트·이펙트 배치, 설정 저장을 한 창에서 처리 | `CharacterToolWindow` |
| 연출 그래프 | 스킬 연출을 노드 데이터로 제작하고 순차·분기·병렬로 실행 | `PresentationGraphWindow`, `GraphExecutor` |
| 연출 뷰어 | 실제 전투 진입 없이 캐릭터·몬스터·스킬 연출을 반복 검증 | `SkillPreviewWindow`, `SkillPreviewRunner` |

## 1. 턴제 전투 시스템

전투 루프가 입력, 계산, 애니메이션, 카메라에 강하게 결합되지 않도록 **턴 진행·스킬 판정·연출 실행·화면 표현의 책임을 분리**했습니다. 각 시스템은 명확한 컨텍스트와 이벤트로 연결되며, 새로운 스킬과 연출을 기존 전투 루프의 큰 수정 없이 추가할 수 있습니다.

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

`Tools/S7/Character Tool`에서 캐릭터 제작에 반복적으로 필요한 작업을 한 흐름으로 처리하는 Unity Editor 도구입니다.

### 주요 기능

- 전용 Preview Scene 자동 진입
- 캐릭터 프리팹 생성과 즉시 미리보기
- Animator State 선택 및 Play/Pause/Reset
- 타임라인 스크러빙과 프레임 단위 애니메이션 확인
- 애니메이션 이벤트 추가·삭제·시간 이동
- 이벤트 시점의 이펙트 생성과 위치 확인
- `CharacterAnimationSet` 에셋 저장
- 대상 에셋의 Addressables 등록 지원
- AnimationClip 이름 규칙을 이용한 Animator Override 자동 매핑

런타임에서 결과를 확인한 뒤 프리팹과 애니메이션 파일을 반복 수정하던 작업을 줄이고, **캐릭터 세팅과 이벤트 타이밍 검증을 에디터 안에서 완료**하는 데 목적을 두었습니다.

**주요 코드**

- `Assets/Scripts/Tool/Character/Editor/CharacterToolWindow.cs`
- `Assets/Scripts/Tool/Character/Editor/AnimatorOverrideAutoAssignWindow.cs`
- `Assets/Scripts/Tool/Character/CharacterAnimationSet.cs`
- `Assets/Scripts/Tool/Character/AnimationStateEventData.cs`

## 3. 연출 그래프

스킬 연출 순서를 코드에 하드코딩하지 않고 `PresentationGraphAsset` 데이터로 제작하는 노드 기반 시스템입니다.

### 제작과 실행 분리

```text
PresentationGraphWindow
  → PresentationGraphAsset 저장
  → PresentationRuntimeGraphBuilder
  → RuntimeNode 구성
  → GraphExecutor 실행
  → IPresentationNode.PlayAsync()
```

GraphView 기반 Editor Window가 노드와 연결 정보를 저장하고, 런타임 빌더가 직렬화 데이터를 실행 객체로 변환합니다. 편집 데이터와 런타임 객체를 분리하여 Editor 의존성이 실제 전투 코드에 섞이지 않도록 구성했습니다.

### 지원 노드

- 애니메이션 재생 및 애니메이션 이벤트 대기
- 캐스팅, 이동, 바라보기, 페이드
- 투사체 생성·발사와 피격 이벤트 등록
- Timeline, 대화, QTE 실행
- 조건 분기와 Choice
- Fork/Join 기반 병렬 연출
- 아군 표시·숨김 등 전투 화면 제어

각 노드는 `IPresentationNode.PlayAsync()` 계약을 따르고, `PresentationContext`를 통해 시전자, 대상, 전투 매니저, Timeline 공급자 등 실행 데이터를 전달받습니다. Timeline과 그래프 에셋은 Addressables로 로드하며 실행 완료 후 해제합니다.

**주요 코드**

- `Assets/Scripts/Game/Presentation/Graph/Editor`
- `Assets/Scripts/Game/Presentation/Graph/Runtime`
- `Assets/Scripts/Game/Presentation/Node`
- `Assets/Scripts/Game/Presentation/PresentationCore.cs`

## 4. 연출 뷰어

`Tools/S7/Skill Preview`는 전체 게임 플로우나 실제 전투 씬에 진입하지 않고 스킬 연출을 빠르게 확인하기 위한 전용 도구입니다.

### 주요 기능

- 프리뷰 환경에서 캐릭터·몬스터 모델 생성
- 캐릭터/몬스터 및 스킬 데이터 선택
- `SkillPreviewContextBuilder`로 실제 실행 구조와 동일한 연출 컨텍스트 구성
- `SkillPreviewRunner`를 통한 연출 그래프 실행
- 캐릭터와 몬스터의 위치·방향을 포함한 반복 테스트
- 대상 교체와 종료 시 Addressables 인스턴스 해제

연출 하나를 확인하기 위해 게임 시작, 필드 진입, 적 조우, 스킬 선택을 반복할 필요 없이 **제작 직후 결과를 검증**할 수 있도록 했습니다.

**주요 코드**

- `Assets/Scripts/Tool/SkillPreview/Editor/SkillPreviewWindow.cs`
- `Assets/Scripts/Tool/SkillPreview/SkillPreviewRunner.cs`
- `Assets/Scripts/Tool/SkillPreview/SkillPreviewContextBuilder.cs`
- `Assets/Scripts/Tool/SkillPreview/PreviewUnitController.cs`

## 콘텐츠 제작 워크플로

```text
[캐릭터 툴]
애니메이션 확인 + 이벤트/이펙트 타이밍 편집
        ↓
[연출 그래프]
애니메이션 + 이동 + 피격 + 카메라 + Timeline + QTE 조립
        ↓
[연출 뷰어]
캐릭터/몬스터/스킬 선택 후 빠른 반복 검증
        ↓
[전투 시스템]
동일한 Presentation Graph를 실제 턴과 스킬 실행에 연결
```

도구별로 독립된 데모 기능을 만드는 데 그치지 않고, **제작 데이터가 실제 런타임 전투에서 그대로 사용되는 구조**로 연결한 것이 이 프로젝트의 핵심입니다.

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