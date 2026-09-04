namespace S7.Game.Field.Enemy
{
    public enum ENEMY_AI_STATE
    {
        NONE   = 0,
        IDLE   = 1,
        ALERT  = 2,
        COMBAT = 3,
        RETURN = 4,
        DEATH  = 5,
        MAX    = 6,
    }

    public enum ENEMY_SOCIAL_TYPE
    {
        NONE = 0,
        INDIVIDUAL = 1,
        SOCIAL = 2
    }

    // 필드 접근 스타일
    public enum ENEMY_COMBAT_STYLE
    {
        NONE = 0,
        STAY = 1,
        CHASE = 2,
        KITE = 3,
        SUPPORT = 4,
    }

    // 접촉 전투 진입 조건
    public enum ENEMY_ENCOUNTER_TYPE
    {
        NONE = 0,
        PASSIVE = 1,
        AGGRESSIVE = 2
    }
}
