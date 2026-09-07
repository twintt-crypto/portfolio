namespace Gpm.LogViewer.Cheat
{
    using NUnit.Framework;
    using S7;
    using S7.Game.Map;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using static UnityEngine.Rendering.DebugUI;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class CommandCheat : MonoBehaviour
    {
#if ENABLE_LOG
        // Start is called before the first frame update
        void Start()
        {
            Function.Instance.AddCheatKeyCallback(CheatCallback);

            Function.Instance.AddCommand(this, "Battle", new object[] {1});
            Function.Instance.AddCommand(this, "TestBattle", new object[] {});

            Function.Instance.AddCommandWithParameters(this, "FieldCallById");
            Function.Instance.AddCommandWithParameters(this, "FieldCallByName");
            Function.Instance.AddCommand(this, "ReplayField");
            
            Function.Instance.AddCommand(this, "Map");
        }
        
        private void CheatCallback(string cheatKey)
        {
            Debug.Log("Call cheat key callback with : " + cheatKey);
            string[] cheat = cheatKey.Split(' ');
            if (cheat.Length == 0)
            {
                return;
            }

            switch (cheat[0])
            {                
                default:
                    {
                        Cheat(cheat);
                    }
                    break;
            }        
        }

        private void Cheat(string[] param)
        {            
        }

        public void Battle(int battleId)
        {
            Debug.Log($"Battle : {battleId}");
            GameFlowManager.Instance.RequestBattle();
        }

        public void TestBattle()
        {
            GameFlowManager.Instance.RequestBattle();
        }

        public void FieldCallById(int fieldId)
        {
            SaveLastField("ById", fieldId.ToString(), "");
            GameFlowManager.Instance.RequestMoveDayField(fieldId);
        }

        public void FieldCallByName(string bgName, string areaName)
        {
            SaveLastField("ByName", bgName, areaName);
            GameFlowManager.Instance.RequestMoveDayField(bgName, areaName);
        }

        public void ReplayField()
        {
            string type = EditorPrefs.GetString("Cheat_LastFieldType", "");
            string param1 = EditorPrefs.GetString("Cheat_LastFieldParam1", "");
            string param2 = EditorPrefs.GetString("Cheat_LastFieldParam2", "");

            if (string.IsNullOrEmpty(type))
            {
                Debug.LogWarning("ReplayField: 저장된 필드 정보가 없습니다.");
                return;
            }

            Debug.Log($"ReplayField: type={type}, param1={param1}, param2={param2}");

            if (type == "ById") GameFlowManager.Instance.RequestMoveDayField(int.Parse(param1));
            else if (type == "ByName") GameFlowManager.Instance.RequestMoveDayField(param1, param2);
        }

        private void SaveLastField(string type, string param1, string param2)
        {
            EditorPrefs.SetString("Cheat_LastFieldType", type);
            EditorPrefs.SetString("Cheat_LastFieldParam1", param1);
            EditorPrefs.SetString("Cheat_LastFieldParam2", param2);
        }

        public void Map()
        {
            if(MapManager.Instance.CurrentMap == null) MapManager.Instance.GenerateNewMap();
            UIManager.Instance.OpenPanelAsync("UIPanelMap");
        }
#endif
    }
}
