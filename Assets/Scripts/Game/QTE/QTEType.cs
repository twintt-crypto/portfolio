namespace Game.QTE
{
    public enum QTE_TYPE
    {
        NONE = 0,
        TAP   = 1,
        SWIPE = 2,
        MASH = 3,
        RELEASE = 4
    }

    public enum QTE_RESULT
    {
        PERFECT = 0,
        GOOD    = 1,
        MISS    = 2, // 입력했지만 타이밍 범위 밖
        FAIL    = 3, // 시간 내 입력 없음
    }
}
