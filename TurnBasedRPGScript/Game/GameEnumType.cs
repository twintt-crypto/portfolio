
public enum TurnState
{
    None,           
    StartBattle,
    PlayerTurn,
    SelectSkill,
    Attack,
    EnemyTurn,
    EnemySelectSkill,   
    EnemySelectTarget,  
    EnemyAttack,     
    EndTurn         
}

public enum BattleInputAction
{
    Attack,
    SkillAttack,
    SpecialAttack,
    UltimateAttack,
    Parry,
    Dodge,
}

public enum BattleResultType
{
    NONE    = 0,
    VICTORY = 1,
    DEFEAT  = 2,
    ESCAPE  = 3,
    MAX     = 4,
}