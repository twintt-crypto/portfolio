# SEQ7 Client

Unity로 개발 중인 **필드 탐험 + 턴제 전투 RPG 클라이언트**입니다. 필드 이동과 상호작용, 적 조우, 턴 기반 스킬 전투, 퀘스트, 대화/QTE, 그래프 기반 전투 연출을 하나의 클라이언트 흐름으로 구성합니다.

> 현재 `ProjectSettings`의 제품명은 `Client`, 버전은 `0.1.0`입니다. 저장소에는 개발 중인 코드와 임시 테스트 로직이 포함되어 있습니다.

## 개발 환경

| 항목 | 값 |
| --- | --- |
| Unity | `6000.3.21f1` |
| 렌더 파이프라인 | Universal Render Pipeline `17.3.0` |
| 입력 | Unity Input System `1.20.0` (구/신 입력 동시 사용 설정) |
| 비동기 처리 | UniTask |
| DI | VContainer `1.17.0` |
| 에셋 로딩 | Unity Addressables |
| UI | uGUI, TextMesh Pro, UI Extensions, Soft Mask, UI Particle |
| 애니메이션 | DOTween / DOTween Pro |
| AI·연출 | Behavior Designer, Cinemachine, Timeline |

프로젝트에 포함된 주요 서드파티 에셋으로 Magica Cloth 2, Amplify Shader Editor, Voyager Toon, GPM, Umbra Soft Shadows 등이 있습니다. 정확한 UPM 의존성은 [`Packages/manifest.json`](Packages/manifest.json)을 확인하세요.

## 시작하기

1. Unity Hub에서 Unity `6000.3.21f1`을 설치합니다.
2. 이 디렉터리를 Unity 프로젝트로 엽니다.
3. Package Manager가 `Packages/manifest.json`의 Git 및 UPM 패키지를 복원할 때까지 기다립니다.
4. Addressables Groups 창에서 설정과 활성 프로필을 확인합니다.
5. `Assets/Scenes/SceneRoot.unity`를 열고 Play를 실행합니다.

에디터에서는 다른 게임 씬을 직접 실행해도 `SceneBase.FirstLoad()`가 먼저 `SceneRoot`를 로드합니다. 정상적인 초기화 순서를 검증하려면 항상 `SceneRoot`부터 실행하는 것을 권장합니다.

## 실행 흐름

```text
SceneRoot
  └─ Global / System 초기화
      └─ ScenePatch
          ├─ 로컬 언어 데이터 로드
          ├─ Addressables 다운로드 크기 확인
          ├─ GameData 및 콘텐츠 다운로드/로드
          └─ 로그인 버튼 → 필드 이동
              └─ SceneField
                  ├─ Field 배경/구역 씬 추가 로드
                  ├─ 탐색, 상호작용, 적 AI, 퀘스트
                  └─ 전투 요청
                      ├─ 전투 로직 씬 추가 로드
                      ├─ 전투 배경 씬 추가 로드
                      └─ 턴/스킬/버프/연출 실행
```

Build Settings에 등록된 기본 씬은 다음 순서입니다.

1. `Assets/Scenes/SceneRoot.unity`
2. `Assets/Scenes/ScenePatch.unity`
3. `Assets/Scenes/SceneField.unity`

필드의 배경·구역 및 전투 씬은 Addressables를 통해 Additive 방식으로 로드됩니다.

## 아키텍처

### 씬과 게임 상태

- `GameSceneManager`는 기본 씬 전환, 페이드, 로딩 UI, 씬별 리소스 등록을 담당합니다.
- `GameFlowManager`는 `Field`, `NightField`, `Story`, `Battle`, `UI`, `Loading` 상태를 관리합니다.
- `SceneBase`는 씬 초기화, 사전 로드, 시작 및 해제 생명주기의 공통 기반입니다.
- 필드에서는 VContainer의 `FieldLifeTimeScope`가 `FieldManager`, `SceneField`, 카메라 컨트롤러를 주입합니다.

### 필드

- `FieldManager`와 `PlayerManager`가 필드 및 플레이어 상태를 관리합니다.
- 플레이어 입력, 이동, 자동 조준, 충돌 기반 전투 동작을 분리했습니다.
- 적 AI는 `Alert`, `Chase`, `Kite`, `Return`, `Stay`, `Death` 등의 전략 객체로 행동을 전환합니다.
- `FieldPortal`, `InteractableObject`, 감지기 계층이 이동과 상호작용을 처리합니다.
- 배경 씬(`FieldBg/`)과 플레이 영역 씬(`FieldArea/`)을 분리하여 필요할 때 교체합니다.

### 전투

- `BattleManager`가 전투 생명주기와 루프를 조정합니다.
- `BattleUnitManager`는 아군/적군 생성 및 참조를 관리합니다.
- `TurnManager`는 현재 행동 유닛, 타깃, 스킬 선택과 턴 상태를 관리합니다.
- `SkillManager`는 공격, 회복, 버프, 소환 실행기로 스킬 효과를 분배합니다.
- 공격은 일반/투사체/궁극기/저항 전략으로 세분화되어 있습니다.
- `BuffManager`는 공격력, 치명타, 보호막, 도발, 침묵, 기절, 지속 피해·회복 등의 버프를 관리합니다.

### 연출, 대화, QTE

- `PresentationGraphAsset`에 직렬화한 노드 그래프를 런타임 그래프로 변환해 실행합니다.
- 애니메이션, 이동, 피격, 투사체, 분기, 병렬 실행, Timeline, 대화, QTE 노드를 지원합니다.
- Timeline과 연출 그래프는 Addressables 키로 로드하고 실행 종료 후 해제합니다.
- QTE는 입력 UI, 판정 로직, 표시 뷰를 분리하며 탭·스와이프·릴리스 유형을 제공합니다.

### 퀘스트와 이벤트

- `GameEventBus`의 이벤트를 활성 퀘스트가 구독합니다.
- 처치, NPC 대화, 지역 진입, 아이템 획득, 스테이지 클리어 조건을 지원합니다.
- 퀘스트 단계는 데이터 테이블에서 조건을 조립하고 조건별 인덱스를 만들어 이벤트를 처리합니다.

### 데이터와 리소스

- `Assets/Scripts/GameData`에는 시트별 생성 데이터 클래스와 `SheetName` 열거형이 있습니다.
- `GameData` 라벨의 `*_Client.bytes` 파일을 로드한 뒤 `GameData.Excel.LoadGameData()`로 역직렬화합니다.
- UI 패널/팝업, 유닛, 필드, 전투, 연출, 효과 등의 에셋은 Addressables 그룹으로 분리되어 있습니다.
- 오브젝트 풀은 반복 생성되는 런타임 오브젝트를 재사용하며 씬 초기화 시 정리됩니다.

## 주요 디렉터리

```text
Assets/
├─ Scenes/                         기본 실행 및 개발용 씬
├─ Scripts/
│  ├─ Scenes/                     씬 생명주기
│  ├─ Manager/                    씬, UI, Addressables, 시스템 매니저
│  ├─ Game/
│  │  ├─ Battle/                  턴, 스킬, 버프, 전투 데이터
│  │  ├─ Field/                   플레이어, 적 AI, 상호작용, 필드 전투
│  │  ├─ Manager/                 게임 상태, 유닛, 퀘스트
│  │  ├─ Presentation/            그래프 기반 연출 런타임/에디터
│  │  ├─ QTE/                     QTE 실행 및 판정
│  │  ├─ Quest/                   퀘스트 조건과 액션
│  │  └─ Unit/                    유닛 데이터, 제어, 뷰
│  ├─ GameData/                   생성된 테이블 모델
│  ├─ UI/                         패널, 팝업, 전투/필드/QTE UI
│  ├─ Tool/                       캐릭터 및 스킬 연출 제작 도구
│  └─ Utility/                    풀링, 저장, 입력, 대화, 공통 유틸리티
├─ AddressableAssetsData/         Addressables 설정과 그룹
├─ _RemoteData/                   원격 배포 대상 콘텐츠
└─ Art/, UI/, Resources/          아트 및 로컬 리소스
```

`Library`, `Temp`, `Logs`, `obj`와 IDE가 생성하는 `.csproj`/`.sln` 파일은 Unity가 재생성할 수 있는 로컬 산출물입니다.

## Addressables 작업

프로젝트는 `GameData`, `Sprite`, `Font`, `UI`, `Unit`, `Presentation`, `Fx`, `Event` 라벨을 패치 시점에 확인합니다. 그룹에는 `Field.Area`, `Field.Bg`, `Battle`, `Battle.Bg`, `UI.Panel`, `UI.Popup` 등이 포함됩니다.

콘텐츠를 추가할 때는 다음을 확인하세요.

- 코드에서 사용하는 주소와 Addressables 엔트리 주소가 정확히 일치하는지
- 패치 대상 에셋에 필요한 라벨이 지정되어 있는지
- 씬을 Addressable로 로드한다면 해당 씬이 올바른 그룹에 포함되어 있는지
- 데이터 파일 이름이 `GameData/T_<SheetName>_Client.Bytes` 규칙을 따르는지
- 빌드 전 Addressables 콘텐츠 빌드를 최신 상태로 갱신했는지

로컬 프로필은 기본적으로 Addressables의 BuildPath/RuntimePath를 사용하며, Remote 경로는 프로필 값에 따라 `ServerData/[BuildTarget]`에서 빌드하도록 설정되어 있습니다. 배포 환경에서는 활성 프로필과 `Remote.LoadPath`를 반드시 검증하세요.

## 빌드

1. `File > Build Profiles`에서 대상 플랫폼과 씬 목록을 확인합니다.
2. `Window > Asset Management > Addressables > Groups`에서 사용할 프로필을 선택합니다.
3. Addressables 콘텐츠를 빌드합니다.
4. Player 빌드를 생성합니다.
5. 실행 후 패치 화면에서 카탈로그 탐색, 다운로드, GameData 로드가 완료되는지 확인합니다.

Android 빌드에서는 알림 및 저장소 권한 확인 코드가 조건부로 실행됩니다. 실제 배포 전에 대상 Android 버전에 맞게 권한 정책과 manifest를 재검토해야 합니다.

## 에디터 도구

- `CharacterToolWindow`: 캐릭터 애니메이션/이벤트 설정 지원
- `AnimatorOverrideAutoAssignWindow`: Animator Override 자동 연결
- `FieldEnemySetupWindow`: 필드 적 오브젝트 구성 지원
- `PresentationGraphWindow`: 연출 노드 그래프 편집
- `SkillPreviewWindow`: 캐릭터/몬스터 스킬 연출 미리보기
- `PlayerSwitchWindow`: 에디터 플레이어 전환 지원

도구의 실제 메뉴 경로와 요구 프리팹은 각 Editor 스크립트의 `MenuItem` 및 직렬화 필드를 기준으로 확인하세요.

## 개발 시 주의사항

- `UIPanelPatch`는 현재 로그인 후 임시 파티를 만들고 필드 ID `8`로 이동합니다.
- `GameFlowManager.EnterBattle()`에는 디버깅용 `Time.timeScale = 0.1f` 코드가 남아 있습니다.
- `Global.InitializeCharacter()`, 인벤토리, 로비 이동, 컷신 호출 등 일부 기능은 아직 비어 있거나 구현 중입니다.
- `Global.AddCharacter()`의 현재 조건문은 신규 키를 추가하지 못하는 형태이므로 캐릭터 저장 기능을 연결하기 전에 확인이 필요합니다.
- 일부 런타임 스크립트가 `NUnit.Framework` 또는 에디터/툴 네임스페이스를 참조합니다. Player 빌드 오류가 발생하면 런타임 의존성을 우선 점검하세요.
- 프로젝트 전용 asmdef가 없어 대부분의 게임 코드가 `Assembly-CSharp`로 컴파일됩니다. 규모가 커질 경우 Runtime/Editor 모듈 분리를 고려할 수 있습니다.
- 자동화된 프로젝트 테스트는 현재 별도 테스트 어셈블리에서 확인되지 않습니다. 변경 후 최소한 패치, 필드 진입, 전투 진입/복귀 흐름을 수동 검증하세요.

## 기본 점검 시나리오

1. `SceneRoot`에서 Play한다.
2. 패치 진행률과 로컬 문자열이 정상적으로 표시되는지 확인한다.
3. 로그인 버튼으로 필드에 진입한다.
4. 플레이어 이동, 카메라, 포털 및 상호작용을 확인한다.
5. 적 조우 후 전투 씬과 배경이 로드되는지 확인한다.
6. 아군 스킬 선택, 적 턴, 버프 및 연출이 정상 실행되는지 확인한다.
7. 승리 또는 도주 후 기존 필드 상태로 복귀하는지 확인한다.

