# QTE System

## 설계 구조

QTE는 **입력 감지(UI)**와 **판정 로직(Judge)**을 완전히 분리한 구조다.

```
[UI Layer]  QTETapUI / QTESwipeUI / QTEReleaseInput
                 │  이벤트만 발생 (OnTap / OnSwipe / OnRelease)
                 │  판정 로직 없음
                 ▼
[Game Layer] QTEJudge ← Feed() 호출
                 │  IsComplete, Judge() 로 결과 반환
                 ▼
            QTERunner  (루프 + 결과 처리)
```

- UI 컴포넌트는 입력을 감지해 이벤트를 발생시킬 뿐, 판정 조건을 알지 못한다.
- Runner가 `AttachInput()`에서 입력 이벤트와 Judge를 람다로 연결한다.
- Judge는 순수 동기 로직. async 없음.

---

## QTE_TYPE별 구성

| 타입 | 입력 UI | Judge | 판정 기준 |
|---|---|---|---|
| TAP | `QTETapUI` | `QTETimingJudge` | 탭한 시점의 elapsed 타이밍 |
| SWIPE | `QTESwipeUI` | `QTETimingJudge` | 스와이프 시점의 elapsed + 방향 일치 여부 |
| RELEASE | `QTEReleaseInput` | `QTETimingJudge` | 손을 뗀 시점의 elapsed 타이밍 |
| MASH | `QTETapUI` | `QTECountJudge` | 제한 시간 내 탭 횟수 |

### SWIPE 방향 처리
- 올바른 방향 → `judge.Feed()` → 타이밍 판정
- 틀린 방향 → `judge.ForceComplete()` → `Judge()`에서 FAIL

---

## QTE_RESULT

| 결과 | 의미 |
|---|---|
| PERFECT | timingPoint 기준 ±perfectNegative/Positive 범위 내 입력 |
| GOOD | timingPoint 기준 ±goodNegative/Positive 범위 내 입력 |
| MISS | 입력했지만 타이밍 범위 밖 |
| FAIL | 시간 내 유효한 입력 없음 / 잘못된 방향 |

MASH는 타이밍 무관. `count >= mashThreshold` → PERFECT, `count >= mashGoodThreshold` → GOOD, 미달 → FAIL

---

## Runner 함수 구조

```
RunGroupAsync(configs, viewPrefab, onEachComplete, ct)
  └─ configs 수만큼 RunSingleAsync를 UniTask.WhenAll로 병렬 실행
       │
       ├─ delay 대기 (config.delay)
       ├─ viewPrefab Instantiate → position 설정 (0~1 normalized anchor)
       ├─ IQTEView.Setup(config)
       ├─ QTEJudgeFactory.Create(config) → QTEJudge
       ├─ AttachInput(viewObject, config, judge)
       │    └─ 타입에 따라 입력 컴포넌트 AddComponent + 람다로 judge.Feed() 연결
       ├─ while (elapsed < duration && !judge.IsComplete) 진행 루프
       ├─ judge.Judge() → QTE_RESULT
       ├─ onEachComplete?.Invoke(index, result)
       └─ view.ShowResultAsync(result) → Destroy
```

### AttachInput 역할
Runner 내부 함수. QTE 타입에 따라 입력 컴포넌트를 `AddComponent`로 부착하고,
해당 컴포넌트의 이벤트를 `judge.Feed()`에 람다로 연결한다.
View Prefab은 순수 시각 프리팹이므로 입력 컴포넌트를 미리 가지고 있지 않는다.

### QTEJudgeFactory 역할
`QTEConfig.type`을 보고 적절한 `QTEJudge` 서브클래스를 생성해 반환한다.
Runner는 타입을 신경 쓰지 않고 `QTEJudge` 인터페이스만 사용한다.

---

## 새 QTE 타입 추가 방법

### 1. `QTE_TYPE` enum에 값 추가
```csharp
// QTEType.cs
public enum QTE_TYPE { NONE = 0, TAP = 1, SWIPE = 2, MASH = 3, RELEASE = 4, NEW_TYPE = 5 }
```

### 2. 입력 UI 컴포넌트 작성 (`Scripts/UI/QTE/`)
```csharp
// UI/QTE/QTENewInput.cs
namespace UI.QTE
{
    public class QTENewInput : MonoBehaviour, IPointerDownHandler
    {
        public event Action OnSomething;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnSomething?.Invoke();
        }
    }
}
```
- `namespace UI.QTE` 사용
- 판정 로직 없이 이벤트만 발생시킬 것
- 기존 Unity EventSystem 인터페이스(`IPointerDownHandler` 등) 활용

### 3. Judge 작성 (기존 재사용 or 신규, `Scripts/Game/QTE/Judge/`)
- 타이밍 기반이면 `QTETimingJudge` 재사용 가능
- 새 판정 방식이 필요하면 `QTEJudge` 상속
```csharp
// Game/QTE/Judge/QTENewJudge.cs
namespace Game.QTE
{
    public class QTENewJudge : QTEJudge
    {
        private readonly QTEConfig _config;

        public QTENewJudge(QTEConfig config) => _config = config;

        public override void Feed()
        {
            if (IsComplete) return;
            IsComplete = true;
        }

        public override QTE_RESULT Judge()
        {
            // 판정 로직
        }
    }
}
```

### 4. `QTEJudgeFactory`에 케이스 추가
```csharp
QTE_TYPE.NEW_TYPE => new QTENewJudge(config),
```

### 5. `QTERunner.AttachInput()`에 케이스 추가
```csharp
case QTE_TYPE.NEW_TYPE:
    QTENewInput newInput = viewObject.AddComponent<QTENewInput>();
    newInput.OnSomething += judge.Feed;
    return;
```

### 6. `QTEConfig`에 전용 필드 필요 시 추가
```csharp
// QTEConfig.cs
// NEW_TYPE 전용
public int newTypeParam;
```

### 7. `QTEConfigDrawer`에 Inspector 표시 추가
```csharp
// Editor/QTEConfigDrawer.cs
case QTE_TYPE.NEW_TYPE:
    EditorGUI.PropertyField(rect, property.FindPropertyRelative("newTypeParam"));
    break;
```
