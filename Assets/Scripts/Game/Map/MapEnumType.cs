
public enum MAP_NODE_TYPE
{
    NONE     = 0,  // 없음

    START    = 1,  // 시작
    NORMAL   = 2,  // 일반
    ELITE    = 3,  // 엘리트
    REST     = 4,  // 휴식
    SHOP     = 5,  // 상점
    TREASURE = 6,  // 보물
    BOSS     = 7,  // 보스

    MAX      = 8,  // 최대
}

public enum MAP_DIRECTION_TYPE
{
    NONE           = 0,  // 없음

    BIDIRECTIONAL  = 1,  // 양방향
    IRREVERSIBLE   = 2,  // 단방향 (되돌아갈 수 없음)

    MAX            = 3,  // 최대
}
