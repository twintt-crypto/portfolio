using GameEventSystem;
using System.Collections.Generic;
using S7;

public partial class Global : Singleton<Global>
{
    private Dictionary<int, QuestState> _quests = new();

    public void ApplyServerQuestSync(List<QuestState> serverStates)
    {
        _quests.Clear();

        foreach (var state in serverStates)
            _quests[state.QuestId] = state;
    }

    public void UpdateQuestProgress(int questId, int conditionId, int value)
    {
        if (!_quests.ContainsKey(questId))
            return;

        _quests[questId].ConditionProgress[conditionId] = value;

        // 여기서 UI 갱신
        EventManager.BroadCasting(new EventTarget(EventType.UpdataQuest), questId, conditionId, value);
    }

    public QuestState GetQuest(int questId)
    {
        return _quests.TryGetValue(questId, out var state) ? state : null;
    }

    
}
