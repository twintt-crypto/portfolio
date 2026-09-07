using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Character/AnimatorClipMappingPreset")]
public class AnimatorClipMappingPreset : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public string baseClipName;
        public string[] overridePatterns;
    }

    public List<Entry> entries = new();
}
