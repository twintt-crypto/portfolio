using S7;

namespace S7
{
    public static class GameEvents
    {
        public static void KillMonster(int monsterId)
        {
            GameEventBus.Raise(new KillMonsterEvent
            {
                type = GameEventType.KillMonster,
                monsterId = monsterId
            });
        }

        public static void TalkNpc(int npcId)
        {
            GameEventBus.Raise(new TalkNpcEvent
            {
                type = GameEventType.TalkNpc,
                npcId = npcId
            });
        }

        public static void ClearStage(int stageId)
        {
            GameEventBus.Raise(new ClearStageEvent
            {
                type = GameEventType.TalkNpc,
                StageId = stageId
            });
        }

        public static void EnterAreaEvent(int areaId)
        {
            GameEventBus.Raise(new EnterAreaEvent
            {
                type = GameEventType.TalkNpc,
                areaId = areaId
            });
        }

        public static void GetItemEvent(int itemId)
        {
            GameEventBus.Raise(new GetItemEvent
            {
                type = GameEventType.TalkNpc,
                itemId = itemId
            });
        }
    }
}
