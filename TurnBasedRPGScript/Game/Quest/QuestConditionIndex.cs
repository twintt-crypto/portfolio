using S7;
using System.Collections.Generic;

namespace S7
{
    public class QuestConditionIndex
    {
        private readonly Dictionary<int, List<KillMonsterCondition>> _monsterIndex = new();
        private readonly Dictionary<int, List<TalkNpcCondition>> _npcIndex = new();
        private readonly Dictionary<int, List<EnterAreaCondition>> _areaIndex = new();
        private readonly Dictionary<int, List<GetItemCondition>> _itemIndex = new();
        private readonly Dictionary<int, List<ClearStageCondition>> _clearStageIndex = new();

        //--------------------------------------------------
        // Register
        //--------------------------------------------------

        public void RegisterMonster(int monsterId, KillMonsterCondition condition)
        {
            if (!_monsterIndex.TryGetValue(monsterId, out var list))
            {
                list = new List<KillMonsterCondition>();
                _monsterIndex[monsterId] = list;
            }

            list.Add(condition);
        }

        public void RegisterNpc(int npcId, TalkNpcCondition condition)
        {
            if (!_npcIndex.TryGetValue(npcId, out var list))
            {
                list = new List<TalkNpcCondition>();
                _npcIndex[npcId] = list;
            }

            list.Add(condition);
        }

        public void RegisterArea(int areaId, EnterAreaCondition condition)
        {
            if (!_areaIndex.TryGetValue(areaId, out var list))
            {
                list = new List<EnterAreaCondition>();
                _areaIndex[areaId] = list;
            }

            list.Add(condition);
        }

        public void RegisterItem(int itemId, GetItemCondition condition)
        {
            if (!_itemIndex.TryGetValue(itemId, out var list))
            {
                list = new List<GetItemCondition>();
                _itemIndex[itemId] = list;
            }

            list.Add(condition);
        }

        public void RegisterClearStage(int itemId, ClearStageCondition condition)
        {
            if (!_clearStageIndex.TryGetValue(itemId, out var list))
            {
                list = new List<ClearStageCondition>();
                _clearStageIndex[itemId] = list;
            }

            list.Add(condition);
        }
    }
}

