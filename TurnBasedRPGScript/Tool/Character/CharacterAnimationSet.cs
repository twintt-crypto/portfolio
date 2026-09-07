using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/AnimationSet")]
public class CharacterAnimationSet : ScriptableObject
{
    public List<AnimationStateEventData> animations = new List<AnimationStateEventData>();

    private Dictionary<int, AnimationStateEventData> map;

    public AnimationStateEventData Get(int stateHash)
    {
        if (map == null)
        {
            BuildMap();
        }

        map.TryGetValue(stateHash, out var data);
        return data;
    }

    private void BuildMap()
    {
        map = new Dictionary<int, AnimationStateEventData>();

        foreach (var a in animations)
        {
            if (a == null)
                continue;

            // hash 항상 최신화
            a.stateHash = Animator.StringToHash(a.stateName);

            if (a.events != null)
                a.events.Sort((x, y) => x.time.CompareTo(y.time));

            map[a.stateHash] = a;
        }
    }

    // 중요: 외부 수정 후 반드시 호출
    public void Invalidate()
    {
        map = null;
    }
}