using System.Collections.Generic;
using System.Linq;

namespace S7
{
    public partial class QuestManager : Singleton<QuestManager>
    {
        private Dictionary<int, Quest> _activeQuests = new();        
        private QuestConditionIndex _conditionIndex = new();

        private void Start()
        {
            DontDestroyOnLoad(this);
            GameEventBus.OnEvent += OnGameEvent;
        }

        public void AddQuest(Quest quest)
        {
            _activeQuests.Add(quest.questId, quest);
        }        
       

        public void CheckStepComplete(Quest quest)
        {
            var step = quest.CurrentStep;

            if (step.IsComplete)
            {
                quest.currentStep++;
            }
        }

        public Quest BuildQuest(int questId)
        {
            Quest quest = new Quest();
            quest.questId = questId;

            var steps = T_QuestStepData.Get(questId);

            foreach (var stepData in steps)
            {
                QuestStep step = new QuestStep();
                step.step = stepData.Step;

                var questConditionDatas = T_QuestConditionData.Get(stepData.ConditionGroupId);

                foreach (var condData in questConditionDatas)
                {
                    var condition = CreateCondition(condData);

                    if (condition != null)
                    {
                        step.conditions.Add(condition);
                        RegisterCondition(condition);
                    }                            
                }

                quest.steps.Add(step);
            }

            return quest;
        }

        private IQuestCondition CreateCondition(T_QuestConditionData data)
        {
            switch (data.ConditionType)
            {
                case QuestConditionType.KillMonster:
                    return new KillMonsterCondition(
                        data.TargetId,
                        data.RequiredCount);

                case QuestConditionType.TalkNpc:
                    return new TalkNpcCondition(
                        data.TargetId);

                case QuestConditionType.EnterArea:
                    return new EnterAreaCondition(
                        data.TargetId);

                case QuestConditionType.GetItem:
                    return new GetItemCondition(
                        data.TargetId,
                        data.RequiredCount);

                case QuestConditionType.ClearStage:
                    return new ClearStageCondition(
                        data.TargetId);                
            }

            return null;
        }

        private void RegisterCondition(IQuestCondition cond)
        {
            switch (cond)
            {
                case KillMonsterCondition m:
                    _conditionIndex.RegisterMonster(m.MonsterId, m);
                    break;

                case TalkNpcCondition n:
                    _conditionIndex.RegisterNpc(n.NpcId, n);
                    break;

                case EnterAreaCondition a:
                    _conditionIndex.RegisterArea(a.AreaId, a);
                    break;

                case GetItemCondition i:
                    _conditionIndex.RegisterItem(i.ItemId, i);
                    break;

                case ClearStageCondition s:
                    _conditionIndex.RegisterClearStage(s.StageId, s);
                    break;
            }
        }

        public void StartQuest(int questId)
        {
            if (_activeQuests.ContainsKey(questId))
                return;

            var quest = BuildQuest(questId);

            _activeQuests.Add(questId, quest);

            quest.Start();
        }


        private void OnGameEvent(GameEvent e)
        {
            foreach (var quest in _activeQuests)
            {
                quest.Value.OnEvent(e);
            }
        }

        public IEnumerable<Quest> GetQuests(QuestType type)
        {
            return _activeQuests.Values.Where(q => q.type == type);
        }
    }
}

