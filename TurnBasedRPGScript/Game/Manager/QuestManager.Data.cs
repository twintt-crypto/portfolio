using System.Collections.Generic;
using UnityEngine;

namespace S7
{
    public partial class QuestManager : Singleton<QuestManager>
    {
        //퀘스트 타입
        private Dictionary<QuestType, List<T_QuestData>> _questByType = new();
        // 시작 NPC
        private Dictionary<int, List<T_QuestData>> _questByNpc = new();

        public void Initialize()
        {
            foreach (var data in T_QuestData.GetAll())
            {                
                if (_questByType.ContainsKey(data.Type) == false)
                    _questByType[data.Type] = new List<T_QuestData>();

                _questByType[data.Type].Add(data);

                if (_questByNpc.ContainsKey(data.StartNpcId) == false)
                    _questByNpc[data.StartNpcId] = new List<T_QuestData>();

                _questByNpc[data.StartNpcId].Add(data);
            }
        }            
    }
}

