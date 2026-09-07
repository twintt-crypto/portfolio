using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Events;

namespace GameEventSystem
{
	#region EventTarget
	public class IEventTargetComparer : IEqualityComparer<EventTarget>
	{
		public bool Equals(EventTarget x, EventTarget y)
		{
			var sIndex1 = string.IsNullOrEmpty(x.eventSubTarget.type) ? "-1" : x.eventSubTarget.index;
			var sIndex2 = string.IsNullOrEmpty(y.eventSubTarget.type) ? "-1" : y.eventSubTarget.index;

			return x.eventType == y.eventType &&
					x.eventSubTarget.gameObject == y.eventSubTarget.gameObject &&
					StringComparer.OrdinalIgnoreCase.Equals(x.eventSubTarget.type, y.eventSubTarget.type) && sIndex1 == sIndex2 &&
					StringComparer.OrdinalIgnoreCase.Equals(x.eventSubTarget.key, y.eventSubTarget.key);
		}

		public int GetHashCode(EventTarget obj)
		{
			var sIndex = string.IsNullOrEmpty(obj.eventSubTarget.type) ? "-1" : obj.eventSubTarget.index;

			return obj.eventType.GetHashCode()
					^ (null != obj.eventSubTarget.gameObject ? obj.eventSubTarget.gameObject.GetHashCode() : 0)
					^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.eventSubTarget.type ?? "")
					^ StringComparer.OrdinalIgnoreCase.GetHashCode(sIndex ?? "")
					^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.eventSubTarget.key ?? "");
		}
	}

	public struct EventTarget
	{
		public EventTarget(EventType eventType, GameObject gameObject_ = null, string type_ = "", string index_ = "-1", string key_ = "")
		{
			this.eventType = eventType;
			this.eventSubTarget = new EventSubTarget(gameObject_, type_, index_, key_);
		}

		public EventType eventType;
		public EventSubTarget eventSubTarget;
	}

	public struct EventSubTarget
	{
		public EventSubTarget(GameObject gameObject_ = null, string type_ = "", string index_ = "-1", string key_ = "")
		{
			this.gameObject = gameObject_;
			type = type_;
			index = index_;
			key = key_;
		}

		public GameObject gameObject;

		public string type;
		public string index;
		public string key;
	}
	#endregion

	#region EventManager
	public class EventManager
	{
		public static Dictionary<EventType, Dictionary<EventTarget, List<Delegate>>> eventListenerDictionary = new Dictionary<EventType, Dictionary<EventTarget, List<Delegate>>>();
		public static void AddEventReceiver(EventTarget target, Action<EventTarget> callback)
		{
			OnAddEventReceiver(target, callback);
		}

		public static void AddEventReceiver<T1>(EventTarget target, Action<EventTarget, T1> callback)
		{
			OnAddEventReceiver(target, callback);
		}

		public static void AddEventReceiver<T1, T2>(EventTarget target, Action<EventTarget, T1, T2> callback)
		{
			OnAddEventReceiver(target, callback);
		}

		public static void AddEventReceiver<T1, T2, T3>(EventTarget target, Action<EventTarget, T1, T2, T3> callback)
		{
			OnAddEventReceiver(target, callback);
		}

		public static void AddEventReceiver<T1, T2, T3, T4>(EventTarget target, Action<EventTarget, T1, T2, T3, T4> callback)
		{
			OnAddEventReceiver(target, callback);
		}

        public static void AddEventReceiver(EventTarget target, Action callback)
        {
            OnAddEventReceiver(target, callback);
        }

        public static void AddEventReceiver<T1>(EventTarget target, Action<T1> callback)
        {
            OnAddEventReceiver(target, callback);
        }

        public static void AddEventReceiver<T1, T2>(EventTarget target, Action<T1, T2> callback)
        {
            OnAddEventReceiver(target, callback);
        }

        public static void AddEventReceiver<T1, T2, T3>(EventTarget target, Action<T1, T2, T3> callback)
        {
            OnAddEventReceiver(target, callback);
        }

        public static void AddEventReceiver<T1, T2, T3, T4>(EventTarget target, Action<T1, T2, T3, T4> callback)
        {
            OnAddEventReceiver(target, callback);
        }

        public static void OnAddEventReceiver(EventTarget eventTarget, Delegate callback)
		{
			if (true == eventListenerDictionary.ContainsKey(eventTarget.eventType))
			{
				if (eventListenerDictionary[eventTarget.eventType].ContainsKey(eventTarget))
				{
					if (!eventListenerDictionary[eventTarget.eventType][eventTarget].Contains(callback))
						eventListenerDictionary[eventTarget.eventType][eventTarget].Add(callback);
				}
				else
				{
					List<Delegate> list = new List<Delegate>();
					list.Add(callback);
					eventListenerDictionary[eventTarget.eventType].Add(eventTarget, list);
				}
			}
			else
			{
				List<Delegate> list = new List<Delegate>();
				list.Add(callback);
				Dictionary<EventTarget, List<Delegate>> dic = new Dictionary<EventTarget, List<Delegate>>(new IEventTargetComparer());
				dic.Add(eventTarget, list);
				eventListenerDictionary.Add(eventTarget.eventType, dic);
			}
		}

		public static void RemoveEventReceiver(EventTarget target, Action<EventTarget> callback)
		{
			OnRemoveEventReceiver(target, callback);
		}    

        public static void RemoveEventReceiver<T1>(EventTarget target, Action<EventTarget, T1> callback)
		{
			OnRemoveEventReceiver(target, callback);
		}

		public static void RemoveEventReceiver<T1, T2>(EventTarget target, Action<EventTarget, T1, T2> callback)
		{
			OnRemoveEventReceiver(target, callback);
		}

		public static void RemoveEventReceiver<T1, T2, T3>(EventTarget target, Action<EventTarget, T1, T2, T3> callback)
		{
			OnRemoveEventReceiver(target, callback);
		}

		public static void RemoveEventReceiver<T1, T2, T3, T4>(EventTarget target, Action<EventTarget, T1, T2, T3, T4> callback)
		{
			OnRemoveEventReceiver(target, callback);
		}

        public static void RemoveEventReceiver(EventTarget target, Action callback)
        {
            OnRemoveEventReceiver(target, callback);
        }

        public static void RemoveEventReceiver<T1>(EventTarget target, Action<T1> callback)
        {
            OnRemoveEventReceiver(target, callback);
        }

        public static void RemoveEventReceiver<T1, T2>(EventTarget target, Action<T1, T2> callback)
        {
            OnRemoveEventReceiver(target, callback);
        }

        public static void RemoveEventReceiver<T1, T2, T3>(EventTarget target, Action<T1, T2, T3> callback)
        {
            OnRemoveEventReceiver(target, callback);
        }

        public static void RemoveEventReceiver<T1, T2, T3, T4>(EventTarget target, Action<T1, T2, T3, T4> callback)
        {
            OnRemoveEventReceiver(target, callback);
        }

        public static void OnRemoveEventReceiver(EventTarget target, Delegate callback)
		{
			if (eventListenerDictionary.ContainsKey(target.eventType) && eventListenerDictionary[target.eventType].ContainsKey(target))
			{
				eventListenerDictionary[target.eventType][target].Remove(callback);				
			}

			RemoveEventReceiver(target);
		}

		public static void RemoveEventReceiver(EventTarget target)
		{
			if( eventListenerDictionary.Count == 0 )
            {
				return;
            }

			if (true == eventListenerDictionary.ContainsKey(target.eventType)
				&& true == eventListenerDictionary[target.eventType].ContainsKey(target))
			{
				if(null == eventListenerDictionary[target.eventType][target] 
					|| eventListenerDictionary[target.eventType][target].Count < 1)
                {
					eventListenerDictionary[target.eventType].Remove(target);
				}                
			}			
		}

		public static void RemoveEventReceiver(EventType type)
		{
			if (eventListenerDictionary.ContainsKey(type) == true)
            {
				eventListenerDictionary.Remove(type);
			}
        }

        public static void BroadCasting(EventTarget eventTarget)
        {
            if (eventListenerDictionary.ContainsKey(eventTarget.eventType) == false)
            {                
                return;
            }

            foreach (var it in eventListenerDictionary[eventTarget.eventType])
            {
                if (eventTarget.eventType != it.Key.eventType)
                {
                    continue;
                }

                if (eventTarget.eventSubTarget.type != it.Key.eventSubTarget.type)
                {
                    if (false == string.IsNullOrEmpty(eventTarget.eventSubTarget.type) &&
                            false == string.IsNullOrEmpty(it.Key.eventSubTarget.type))
                    {
                        continue;
                    }
                }

                if (eventTarget.eventSubTarget.key != it.Key.eventSubTarget.key)
                {
                    continue;
                }

                foreach (var value in it.Value)
                {
                    Action<EventTarget> callback = value as Action<EventTarget>;
                    if (callback != null)
                    {
                        callback(eventTarget);
                    }
                    else
                    {
						Action callback2 = value as Action;
						if(callback2!= null)
                        {
							callback2();
						}
                        else
                        {
							Debug.LogError("callback is null");
						}
                    }
                }
            }
        }

        public static void BroadCasting<T>(EventTarget eventTarget, T param)
		{
			if (eventListenerDictionary.ContainsKey(eventTarget.eventType) == false)
			{
				return;
			}

			foreach (var it in eventListenerDictionary[eventTarget.eventType])
			{
				if (eventTarget.eventType != it.Key.eventType)
				{
					continue;
				}

				if (eventTarget.eventSubTarget.type != it.Key.eventSubTarget.type)
				{
					if (false == string.IsNullOrEmpty(eventTarget.eventSubTarget.type) &&
							false == string.IsNullOrEmpty(it.Key.eventSubTarget.type))
					{
						continue;
					}
				}

				if (eventTarget.eventSubTarget.key != it.Key.eventSubTarget.key)
				{
					continue;
				}

				foreach (var value in it.Value)
				{					
					Action<EventTarget, T> callback = value as Action<EventTarget, T>;
					if (callback != null)
					{
						callback(eventTarget, param);
					}					
					else
					{
                        Action<T> callback2 = value as Action<T>;
                        if (callback2 != null)
                        {
                            callback2(param);
                        }
                        else
                        {
							Debug.LogWarning("callback is null");
                        }
					}				
				}
			}
		}

		public static void BroadCasting<T1, T2>(EventTarget eventTarget, T1 param1, T2 param2)
		{
			if (eventListenerDictionary.ContainsKey(eventTarget.eventType) == false)
			{
				Debug.LogWarning("등록되지 않은 이벤트입니다.");
				return;
			}

			foreach (var it in eventListenerDictionary[eventTarget.eventType])
			{
				if (eventTarget.eventType != it.Key.eventType)
				{
					continue;
				}

				if (eventTarget.eventSubTarget.type != it.Key.eventSubTarget.type)
				{
					if (false == string.IsNullOrEmpty(eventTarget.eventSubTarget.type) &&
							false == string.IsNullOrEmpty(it.Key.eventSubTarget.type))
					{
						continue;
					}
				}

				if (eventTarget.eventSubTarget.key != it.Key.eventSubTarget.key)
				{
					continue;
				}

				foreach (var value in it.Value)
				{
					Action<EventTarget, T1, T2> callback = value as Action<EventTarget, T1, T2>;
					if (callback != null)
					{
						callback(eventTarget, param1, param2);
					}
					else
					{
                        Action<T1,T2> callback2 = value as Action<T1,T2>;
                        if (callback2 != null)
                        {
                            callback2(param1, param2);
                        }
                        else
                        {
							Debug.LogError("callback is null");
                        }
                    }
				}
			}
		}

		public static void BroadCasting<T1, T2, T3>(EventTarget eventTarget, T1 param1, T2 param2, T3 param3)
		{
			if (eventListenerDictionary.ContainsKey(eventTarget.eventType) == false)
			{
				Debug.LogWarning("등록되지 않은 이벤트입니다.");
				return;
			}

			foreach (var it in eventListenerDictionary[eventTarget.eventType])
			{
				if (eventTarget.eventType != it.Key.eventType)
				{
					continue;
				}

				if (eventTarget.eventSubTarget.type != it.Key.eventSubTarget.type)
				{
					if (false == string.IsNullOrEmpty(eventTarget.eventSubTarget.type) &&
							false == string.IsNullOrEmpty(it.Key.eventSubTarget.type))
					{
						continue;
					}
				}

				if (eventTarget.eventSubTarget.key != it.Key.eventSubTarget.key)
				{
					continue;
				}

				foreach (var value in it.Value)
				{
					Action<EventTarget, T1, T2, T3> callback = value as Action<EventTarget, T1,	T2, T3>;
					if (callback != null)
					{
						callback(eventTarget, param1, param2, param3);
					}
					else
					{
                        Action<T1, T2, T3> callback2 = value as Action<T1, T2, T3>;
                        if (callback2 != null)
                        {
                            callback2(param1, param2, param3);
                        }
                        else
                        {
							Debug.LogError("callback is null");
                        }
                    }
				}
			}
		}

		public static void BroadCasting<T1, T2, T3, T4>(EventTarget eventTarget, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			if (eventListenerDictionary.ContainsKey(eventTarget.eventType) == false)
			{
				Debug.LogWarning("등록되지 않은 이벤트입니다.");
				return;
			}

			foreach (var it in eventListenerDictionary[eventTarget.eventType])
			{
				if (eventTarget.eventType != it.Key.eventType)
				{
					continue;
				}

				if (eventTarget.eventSubTarget.type != it.Key.eventSubTarget.type)
				{
					if (false == string.IsNullOrEmpty(eventTarget.eventSubTarget.type) &&
							false == string.IsNullOrEmpty(it.Key.eventSubTarget.type))
					{
						continue;
					}
				}

				if (eventTarget.eventSubTarget.key != it.Key.eventSubTarget.key)
				{
					continue;
				}

				foreach (var value in it.Value)
				{
					Action<EventTarget, T1, T2, T3, T4> callback = value as Action<EventTarget, T1, T2, T3, T4>;
					if (callback != null)
					{
						callback(eventTarget, param1, param2, param3, param4);
					}
					else
					{
                        Action<T1, T2, T3, T4> callback2 = value as Action<T1, T2, T3, T4>;
                        if (callback2 != null)
                        {
                            callback2(param1, param2, param3, param4);
                        }
                        else
                        {
							Debug.LogError("callback is null");
                        }
                    }
				}
			}
		}

		public static int GetEventCount(GameEventSystem.EventType eventType)
        {

            if (eventListenerDictionary.ContainsKey(eventType) == false)
            {
				return 0;
            }

			return eventListenerDictionary[eventType].Count;
		}
	}
	#endregion
}

/*
등록
EventManager.AddEventReceiver<int>(new EventTarget(GameEventSystem.EventType.AppStart), test);

브로드
EventManager.BroadCasting<int>(new EventTarget(GameEventSystem.EventType.AppStart), 1);

콜백함수
public void test(EventTarget targer, int a)
{
    Debug.Log(a);
}
*/