using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEventSystem
{
    #region EventParameter
    public class EventParameter
    {
        public string tag;
    }
    #endregion

    #region EventType
    public enum EventType
    {
        TurnStateChange = 1,
        BattleSelectTarget,
        UpdataCharacter,
        UpdataQuest,

        UpdateAp,

        Max
    }
    #endregion
}
