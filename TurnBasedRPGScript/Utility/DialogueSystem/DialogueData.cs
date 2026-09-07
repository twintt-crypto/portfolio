using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueChoice
    {
        public string labelText;
        public int nextId;  // 선택 후 이동할 DialogueEntry TID
    }

    public class DialogueEntry
    {
        public string speakerName;
        public string bodyText;
        public Sprite characterImage;         // null이면 이미지 패널 숨김
        public float typingSpeed = 0.04f;
        public DIALOGUE_TYPE type;
        public int nextId;                    // NORMAL: 다음 entry TID (-1이면 종료)
        public List<DialogueChoice> choices;  // QUESTION 타입일 때만 유효
    }

    public class DialogueSet
    {
        public int setId;
        public int startId;
        public Dictionary<int, DialogueEntry> entryDic;

        // T_DialogueScriptData → DialogueEntry 변환
        // speakerName, bodyText 등 프로젝트별 텍스트 룩업은 여기서 처리
        private static DialogueEntry DataConverter(T_DialogueScriptData data)
        {
            return new DialogueEntry
            {
                speakerName = data.Speaker_ID.ToString(), // TODO: 스피커 테이블 룩업으로 교체
                // bodyText    = StringManager.Get(data.Text_ID.ToString()),
                bodyText    = T_DialogueSciprtTextData.Get(data.Text_ID).Text_Key,
                typingSpeed = 0.04f,
                nextId      = data.Next_ID,
                type        = data.Type switch
                {
                    ScriptType.Normal   => DIALOGUE_TYPE.NORMAL,
                    ScriptType.Question => DIALOGUE_TYPE.QUESTION,
                    ScriptType.Choice   => DIALOGUE_TYPE.CHOICE,
                    _                    => DIALOGUE_TYPE.NONE,
                },
                choices = new List<DialogueChoice>(),
            };
        }

        // startId에서 도달 가능한 모든 entry를 BFS로 수집해 DialogueSet 생성
        public static DialogueSet GetRowById(int startId)
        {
            var entryDic = new Dictionary<int, DialogueEntry>();
            var visited  = new HashSet<int>();
            var queue    = new Queue<int>();
            queue.Enqueue(startId);

            while (queue.Count > 0)
            {
                int id = queue.Dequeue();
                if (id == -1 || visited.Contains(id)) continue;
                visited.Add(id);

                var data = T_DialogueScriptData.Get(id);
                if (data == null)
                {
                    Debug.LogWarning($"[DialogueSet] T_DialogueScriptData not found: {id}");
                    continue;
                }

                var entry = DataConverter(data);
                entryDic[id] = entry;

                switch (data.Type)
                {
                    case ScriptType.Normal:
                        queue.Enqueue(data.Next_ID);
                        break;

                    case ScriptType.Question:
                        // CHOICE 행들은 QUESTION 바로 다음 TID부터 연속으로 존재
                        // From_ID가 달라지거나 데이터가 없을 때까지 순차 스캔
                        int choiceId = id + 1;
                        while (true)
                        {
                            var choiceData = T_DialogueScriptData.Get(choiceId);
                            if (choiceData == null || choiceData.From_ID != id) break;

                            entry.choices.Add(new DialogueChoice
                            {
                                // labelText = StringManager.Get(choiceData.Text_ID.ToString()), 
                                labelText = T_DialogueSciprtTextData.Get(choiceData.Text_ID).Text_Key, // TODO: remove temp
                                nextId    = choiceData.Next_ID,
                            });
                            queue.Enqueue(choiceData.TID);     // CHOICE entry 자체도 dict에 추가
                            queue.Enqueue(choiceData.Next_ID); // 분기 이후 entry 탐색
                            choiceId++;
                        }
                        break;

                    case ScriptType.Choice:
                        queue.Enqueue(data.Next_ID);
                        break;
                }
            }

            return new DialogueSet
            {
                setId    = startId,
                startId  = startId,
                entryDic = entryDic,
            };
        }
    }
}
