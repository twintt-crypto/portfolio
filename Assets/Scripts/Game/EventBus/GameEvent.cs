
namespace S7
{
    public enum GameEventType
    {
        KillMonster,
        TalkNpc,
        EnterArea,
        GetItem,
        BattleClear,
        ClearStage
    }

    public class GameEvent
    {
        public GameEventType type;                
    }

    public class ClearStageEvent : GameEvent
    {
        public int StageId;
    }

    public class EnterAreaEvent : GameEvent
    {
        public int areaId;
    }

    public class GetItemEvent : GameEvent
    {
        public int itemId;
        public int count;
    }

    public class KillMonsterEvent : GameEvent
    {
        public int monsterId;
        public int count;
    }

    public class TalkNpcEvent : GameEvent
    {
        public int npcId;
    }
}

