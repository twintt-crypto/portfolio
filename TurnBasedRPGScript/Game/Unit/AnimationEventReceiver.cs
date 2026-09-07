using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    private Dictionary<string, Action> _events = new();    

    public void Register(string eventName, Action action)
    {
        if (_events.TryGetValue(eventName, out var exist))
            _events[eventName] = exist + action;
        else
            _events.Add(eventName, action);
    }

    public void Unregister(string eventName, Action action )
    {
        if (_events.TryGetValue(eventName, out var exist) == false)
        {
            return;
        }        
        
        exist -= action;

        if (exist == null)
            _events.Remove(eventName);
        else
            _events[eventName] = exist;    
    }

    public void OnAnimationEvent(string eventName)
    {
        if (_events.TryGetValue(eventName, out var action))
            action?.Invoke();
    }

    public Action<int> OnHitEvent;
    public Action<int> OnAttackEffectEvent;

    public void OnHit(int index)
    {
        OnHitEvent?.Invoke(index);
    }

    public void OnAttackEffect(int index)
    {
        OnAttackEffectEvent?.Invoke(index);
    }

    public void RegisterHit(Action<int> action)
    {
        OnHitEvent += action;
    }

    public void UnregisterHit(Action<int> action)
    {
        OnHitEvent -= action;
    }

    public void RegisterAttackEffect(Action<int> action)
    {
        OnAttackEffectEvent += action;
    }

    public void UnregisterAttackEffect(Action<int> action)
    {
        OnAttackEffectEvent -= action;
    }

    public void ClearEvent()
    {
        OnHitEvent = null;
        OnAttackEffectEvent = null;
    }


    private Dictionary<AnimationEventType, Action<AnimationEventData>> _animationEvents = new();

    public void Register(AnimationEventType eventType, Action<AnimationEventData> action)
    {
        if (_animationEvents.TryGetValue(eventType, out var exist))
            _animationEvents[eventType] = exist + action;
        else
            _animationEvents.Add(eventType, action);
    }

    public void Unregister(AnimationEventType eventType, Action<AnimationEventData> action = null)
    {
        if (!_animationEvents.TryGetValue(eventType, out var exist))
            return;

        // action 없으면 전체 제거
        if (action == null)
        {
            _animationEvents.Remove(eventType);
            return;
        }

        exist -= action;

        // 다 제거됐으면 key 삭제
        if (exist == null)
            _animationEvents.Remove(eventType);
        else
            _animationEvents[eventType] = exist;
    }

    public void OnAnimationEvent(AnimationEventType eventType, AnimationEventData eventData)
    {
        if (_animationEvents.TryGetValue(eventType, out var action))
            action?.Invoke(eventData);
    }

}