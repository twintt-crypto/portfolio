
public enum GlobalValueType
{
    None        = 0,  // 없음

    DataVersion = 1,  // 데이터 버전
    Language    = 2,  // 언어
    RateBase    = 3,  // 기본배율
    ParryTime   = 4,  // 패링 타임

    Max         = 5,  // 최대
}

public enum LoginType
{
    None    = 0,  // 없음

    Guest   = 1,  // 게스트
    Goolgle = 2,  // 구글
    Apple   = 3,  // 애플

    Max     = 4,  // 최대
}

public enum ProductType
{
    None          = 0,  // 없음

    Consumable    = 1,  // 소비
    NonConsumable = 2,  // 비소비
    subscription  = 3,  // 구독

    Max           = 4,  // 최대
}

public enum AudioType
{
    None  = 0,  // 없음

    Bgm   = 1,  // BGM
    Sfx   = 2,  // SFX
    Voice = 3,  // VOICE

    Max   = 4,  // 최대
}

public enum SkillType
{
    None    = 0,  // 없음

    Active  = 1,  // 액티브
    Passive = 2,  // 패시브

    Max     = 3,  // 최대
}

public enum SkillActionType
{
    None   = 0,  // 없음

    Attack = 1,  // 일반공격
    Heal   = 2,  // 힐
    Summon = 3,  // 소환
    Buff   = 4,  // 버프
    QTE    = 5,  // QTE

    Max    = 6,  // 최대
}

public enum AttackType
{
    None       = 0,  // 없음

    Normal     = 1,  // 일반공격
    Projectile = 2,  // 발사체
    Ultimate   = 3,  // 궁극기

    Max        = 4,  // 최대
}

public enum TargetScope
{
    None     = 0,  // 없음

    Single   = 1,  // 단일
    Adjacent = 2,  // 인접
    All      = 3,  // 전체

    Max      = 4,  // 최대
}

public enum TargetType
{
    None     = 0,  // 없음

    Self     = 1,  // 나
    Ally     = 2,  // 적
    AllyAll  = 3,  // 아군 전체
    Enumy    = 4,  // 적
    EnemyAll = 5,  // 적전체

    Max      = 6,  // 최대
}

public enum BattleStageType
{
    None     = 0,  // 없음

    Normal   = 1,  // 일반
    Boss     = 2,  // 보스
    Tutorial = 3,  // 튜토리얼

    Max      = 4,  // 최대
}

public enum BattleClearCondition
{
    None         = 0,  // 없음

    ALL_CLEAR    = 1,  // 전부 처치
    TIME_ATTACK  = 2,  // 시간안에 처리
    TARGET_CLEAR = 3,  // 지정된 타겟 처리

    Max          = 4,  // 최대
}

public enum ResourceLoadType
{
    None   = 0,  // 없음

    Scene  = 1,  // 전투
    Prefab = 2,  // 필드

    Max    = 3,  // 최대
}

public enum BattleSide
{
    None  = 0,  // 없음

    Ally  = 1,  // 동맹
    Enemy = 2,  // 적

    Max   = 3,  // 최대
}

public enum UnitType
{
    None      = 0,  // 없음

    Character = 1,  // 캐릭터
    Monster   = 2,  // 몬스터
    Npc       = 3,  // NPC

    Max       = 4,  // 최대
}

public enum ActivationCondition
{
    None          = 0,  // 없음

    OnManualCast  = 1,  // 플레이어가 직접 스킬 버튼을 눌렀을 때 발동
    OnPreBattle   = 2,  // 전투에 진입하기 직전에 발동
    OnBattleStart = 3,  // 전투가 시작되자마자 1회 발동
    OnTurnStart   = 4,  // 자신의 턴이 시작될 때 자동 발동
    OnHit         = 5,  // 공격이 적중했을 때 발동
    OnDataged     = 6,  // 피해를 받았을 때 발동
    OnKill        = 7,  // 적을 처치했을 때 발동

    Max           = 8,  // 최대
}

public enum Faction
{
    None   = 0,     // 없음

    Player = 1001,  // 플레이어
    Enemy  = 2001,  // 적

    Max    = 2002,  // 최대
}

public enum ScriptType
{
    None     = 0,  // 없음

    Normal   = 1,  // 대사
    Question = 2,  // 질문
    Choice   = 3,  // 선택지

    Max      = 4,  // 최대
}

public enum MonstatType
{
    None   = 0,  // 없음

    Normal = 1,  // 일반 
    Elit   = 2,  // 정예
    Boss   = 3,  // 보스

    Max    = 4,  // 최대
}

public enum ParryReactionType
{
    None        = 0,  // 없음

    Stun        = 1,  // 스턴
    CounterStun = 2,  // 역스턴

    Max         = 3,  // 최대
}

public enum ClassType
{
    None    = 0,  // 없음

    Kendo   = 1,  // 검도부
    Archery = 2,  // 궁수부

    Max     = 3,  // 최대
}

public enum AttributeType
{
    None = 0,  // 없음

    Fire = 1,  // 불속성

    Max  = 2,  // 최대
}

public enum Rarity
{
    None = 0,  // 없음

    N    = 1,  // 
    R    = 2,  // 
    SR   = 3,  // 
    SSR  = 4,  // 

    Max  = 5,  // 최대
}

public enum UIDispalyType
{
    None = 0,  // 없음

    Null = 1,  // 보이지 않음

    Max  = 2,  // 최대
}

public enum QuestType
{
    None = 0,  // 없음

    Main = 1,  // 메인임무
    Sub  = 2,  // 서브임무

    Max  = 3,  // 최대
}

public enum QuestStepType
{
    None          = 0,  // 없음

    Dialogue      = 1,  // 대화
    WaitCondition = 2,  // 조건 대기
    PlayCutscene  = 3,  // 컷씬
    StartBattle   = 4,  // 전투 시작
    SpawnNpc      = 5,  // NPC 생성
    CompleteQuest = 6,  // 퀘스트 완료

    Max           = 7,  // 최대
}

public enum QuestConditionType
{
    None        = 0,  // 없음

    KillMonster = 1,  // 킬
    TalkNpc     = 2,  // 대화
    EnterArea   = 3,  // 집입
    GetItem     = 4,  // 아이템 획득
    ClearStage  = 5,  // 스테이지 클릴어

    Max         = 6,  // 최대
}

public enum QuestStatus
{
    None       = 0,  // 없음

    InProgress = 1,  // 진행중
    Completed  = 2,  // 완료함
    Rewarded   = 3,  // 보상받음

    Max        = 4,  // 최대
}

public enum QuestActionType
{
    None     = 0,  // 없음

    Dialogue = 1,  // 다이알로그
    Timeline = 2,  // 타임라인
    Battle   = 3,  // 전투
    SpawnNpc = 4,  // 스폰NPC
    Reward   = 5,  // 보상

    Max      = 6,  // 최대
}

public enum RewardType
{
    None        = 0,  // 없음

    Gold        = 1,  // 골드
    Exp         = 2,  // 경험치
    Item        = 3,  // 아이템
    Character   = 4,  // 캐릭터
    GachaTicket = 5,  // 가차티켓

    Max         = 6,  // 최대
}

public enum EffectType
{
    None            = 0,   // 없음

    Attack          = 1,   // 공격력을 일정 비율(%)만큼 증가시킨다.
    Defense         = 2,   // 방어력을 일정 비율(%)만큼 증가시킨다.
    Speed           = 3,   // 행동 속도를 일정 비율(%)만큼 증가시킨다.
    CritRate        = 4,   // 치명타 발생 확률을 증가시킨다.
    CritDamage      = 5,   // 치명타 피해량을 증가시킨다.
    HealOverTime    = 6,   // 지속 시간 동안 매 턴 체력을 회복한다.
    DamageOverTime  = 7,   // 지속 시간 동안 매 턴 피해를 입힌다.
    Shield          = 8,   // 일정량의 피해를 흡수하는 보호막을 생성한다.
    DamageIncrease  = 9,   // 대상이 가하는 피해량을 증가시킨다.
    DamageReduction = 10,  // 대상이 받는 피해량을 감소시킨다.
    LifeSteal       = 11,  // 공격 시 피해의 일부를 체력으로 회복한다.
    Stun            = 12,  // 대상이 일정 시간 동안 행동할 수 없게 만든다.
    Silence         = 13,  // 대상이 스킬을 사용할 수 없게 만든다.
    Taunt           = 14,  // 대상의 공격 목표를 강제로 자신에게 집중시킨다.
    Freeze          = 15,  // 대상이 일정 시간 동안 행동할 수 없게 하며 추가 피해를 받을 수 있다.

    Max             = 16,  // 최대
}

public enum EffectValueType
{
    None    = 0,  // 없음

    Flat    = 1,  // 고정값
    Percent = 2,  // 퍼센트

    Max     = 3,  // 최대
}

public enum BuffTickType
{
    None      = 0,  // 없음

    TurnStart = 1,  // 턴시작
    TurnEnd   = 2,  // 턴끝남
    OnHit     = 3,  // 피격당함
    OnAttack  = 4,  // 공격함

    Max       = 5,  // 최대
}

public enum AggroChangeType
{
    None = 0,  // 없음

    Add  = 1,  // 고정
    Mult = 2,  // 비율

    Max  = 3,  // 최대
}

public enum ReactiveType
{
    None  = 0,  // 없음

    Parry = 1,  // 페리
    Dodge = 2,  // 회피

    Max   = 3,  // 최대
}
