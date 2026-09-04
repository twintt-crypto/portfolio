using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationStateEventData
{
    public string stateName;
    public int stateHash;

    public List<AnimationEventData> events = new List<AnimationEventData>();
}