using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static void Call(this Action action)
    {
        if (action != null)
            action();
    }

    public static void Call<T>(this Action<T> action, T arg)
    {
        if (action != null)
            action(arg);
    }

    public static void Call<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
    {
        if (action != null)
            action(arg1, arg2);
    }

    public static void Call<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
        if (action != null)
            action(arg1, arg2, arg3);
    }

    public static TResult Call<TResult>(this Func<TResult> func, TResult result = default(TResult))
    {
        return func != null ? func() : result;
    }

    public static TResult Call<T, TResult>(this Func<T, TResult> func, T arg, TResult result = default(TResult))
    {
        return func != null ? func(arg) : result;
    }

    public static TResult Call<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2, TResult result = default(TResult))
    {
        return func != null ? func(arg1, arg2) : result;
    }

    public static TResult Call<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, TResult result = default(TResult))
    {
        return func != null ? func(arg1, arg2, arg3) : result;
    }

    public static bool IsNullOrEmpty(this string self)
    {
        return string.IsNullOrEmpty(self);
    }

    public static bool IsNullOrWhiteSpace(this string self)
    {
        return self == null || self.Trim() == "";
    }

    public static bool IsNullOrEmpty<T>(this IList<T> self)
    {
        return self == null || self.Count == 0;
    }

    public static T SafeGetComponent<T>(this GameObject self) where T : Component
    {
        T t = self.GetComponent<T>();
        if (t != null) return t;
        return self.AddComponent<T>();
    }

    public static T SafeGetComponent<T>(this Component self) where T : Component
    {
        T t = self.GetComponent<T>();
        if (t != null) return t;
        return self.gameObject.AddComponent<T>();
    }

    public static GameObject[] GetChildren(this GameObject self, bool includeInactive = false)
    {
        return self.GetComponentsInChildren<Transform>(includeInactive)
                   .Where(c => c != self.transform)
                   .Select(c => c.gameObject)
                   .ToArray();
    }

    public static GameObject[] GetChildren(this Component self, bool includeInactive = false)
    {
        return self.GetComponentsInChildren<Transform>(includeInactive)
                   .Where(c => c != self.transform)
                   .Select(c => c.gameObject)
                   .ToArray();
    }

    public static GameObject[] GetChildrenWithoutGrandchildren(this GameObject self)
    {
        var result = new List<GameObject>();
        foreach (Transform n in self.transform)
        {
            result.Add(n.gameObject);
        }
        return result.ToArray();
    }

    public static GameObject[] GetChildrenWithoutGrandchildren(this Component self)
    {
        var result = new List<GameObject>();
        foreach (Transform n in self.transform)
        {
            result.Add(n.gameObject);
        }
        return result.ToArray();
    }
    public static T[] GetChildrenWithoutGrandchildren<T>(this Component self) where T : Component
    {
        var result = new List<T>();
        foreach (Transform n in self.transform)
        {
            if (n.HasComponent<T>())
                result.Add(n.GetComponent<T>());
        }
        return result.ToArray();
    }


    public static T[] GetComponentsInChildrenWithoutSelf<T>(this GameObject self, bool includeInactive = false) where T : Component
    {
        return self.GetComponentsInChildren<T>(includeInactive)
                   .Where(c => self != c.gameObject)
                   .ToArray();
    }

    public static T[] GetComponentsInChildrenWithoutSelf<T>(this Component self, bool includeInactive = false) where T : Component
    {
        return self.GetComponentsInChildren<T>(includeInactive)
                   .Where(c => self.gameObject != c.gameObject)
                   .ToArray();
    }

    public static T GetComponentInParentEx<T>(this Component self) where T : Component
    {
        while (self.HasParent())
        {
            if (self.transform.parent.HasComponent<T>())
                return self.transform.parent.GetComponent<T>();
            self = self.transform.parent;
        }
        return null;
    }

    public static void RemoveComponent<T>(this GameObject self) where T : Component
    {
        GameObject.Destroy(self.GetComponent<T>());
    }

    public static void RemoveComponent<T>(this Component self) where T : Component
    {
        GameObject.Destroy(self.GetComponent<T>());
    }

    public static void RemoveComponentImmediate<T>(this GameObject self) where T : Component
    {
        GameObject.DestroyImmediate(self.GetComponent<T>());
    }

    public static void RemoveComponentImmediate<T>(this Component self) where T : Component
    {
        GameObject.DestroyImmediate(self.GetComponent<T>());
    }

    public static void RemoveComponents<T>(this GameObject self) where T : Component
    {
        foreach (var n in self.GetComponents<T>())
        {
            GameObject.Destroy(n);
        }
    }

    public static void RemoveComponents<T>(this Component self) where T : Component
    {
        foreach (var n in self.GetComponents<T>())
        {
            GameObject.Destroy(n);
        }
    }

    public static void RemoveComponentsImmediate<T>(this GameObject self) where T : Component
    {
        foreach (var n in self.GetComponents<T>())
        {
            GameObject.DestroyImmediate(n);
        }
    }

    public static void RemoveComponentsImmediate<T>(this Component self) where T : Component
    {
        foreach (var n in self.GetComponents<T>())
        {
            GameObject.DestroyImmediate(n);
        }
    }

    public static bool HasComponent<T>(this GameObject self) where T : Component
    {
        return self.GetComponent<T>() != null;
    }

    public static bool HasComponent<T>(this Component self) where T : Component
    {
        return self.GetComponent<T>() != null;
    }

    public static Transform Find(this GameObject self, string name)
    {
        return self.transform.Find(name);
    }

    public static Transform Find(this Component self, string name)
    {
        return self.transform.Find(name);
    }

    public static GameObject FindGameObject(this GameObject self, string name)
    {
        var result = self.transform.Find(name);
        return result != null ? result.gameObject : null;
    }

    public static GameObject FindGameObject(this Component self, string name)
    {
        var result = self.transform.Find(name);
        return result != null ? result.gameObject : null;
    }

    public static T FindComponent<T>(this GameObject self, string name) where T : Component
    {
        var t = self.transform.Find(name);
        if (t == null)
        {
            return null;
        }
        return t.GetComponent<T>();
    }

    public static T FindComponent<T>(this Component self, string name) where T : Component
    {
        var t = self.transform.Find(name);
        if (t == null)
        {
            return null;
        }
        return t.GetComponent<T>();
    }

    public static T GetComponentByName<T>(this GameObject self, string name = null, bool includeInactive = false) where T : Component
    {
        var children = self.GetComponentsInChildren(typeof(T), includeInactive);
        foreach (T c in children)
        {
            if (name == null || (c.name.ToLower() == name.ToLower()))
            {
                return c;
            }
        }
        return null;
    }

    public static T GetComponentByName<T>(this Component self, string name = null, bool includeInactive = false) where T : Component
    {
        var children = self.GetComponentsInChildren(typeof(T), includeInactive);
        foreach (T c in children)
        {
            if (name == null || (c.name.ToLower() == name.ToLower()))
            {
                return c;
            }
        }
        return null;
    }

    public static List<T> GetComponentsByName<T>(this GameObject self, string name = null, bool includeInactive = false) where T : Component
    {
        var listComponents = new List<T>();
        var children = self.GetComponentsInChildren(typeof(T), includeInactive);
        foreach (T c in children)
        {
            if (name == null || (c.name.ToLower() == name.ToLower()))
            {
                listComponents.Add(c);
            }
        }

        return listComponents;
    }

    public static List<T> GetComponentsByName<T>(this Component self, string name = null, bool includeInactive = false) where T : Component
    {
        var listComponents = new List<T>();
        var children = self.GetComponentsInChildren(typeof(T), includeInactive);
        foreach (T c in children)
        {
            if (name == null || (c.name.ToLower() == name.ToLower()))
            {
                listComponents.Add(c);
            }
        }

        return listComponents;
    }

    public static void SetParent(this GameObject self, Transform parent)
    {
        self.transform.SetParent(parent);
    }

    public static void SetParent(this GameObject self, GameObject parent)
    {
        self.transform.SetParent(parent.transform);
    }

    public static bool HasChild(this GameObject self)
    {
        return 0 < self.transform.childCount;
    }

    public static bool HasChild(this Component self)
    {
        return 0 < self.transform.childCount;
    }

    public static bool HasParent(this GameObject self)
    {
        return self.transform.parent != null;
    }

    public static bool HasParent(this Component self)
    {
        return self.transform.parent != null;
    }

    public static GameObject GetChild(this GameObject self, int index)
    {
        var t = self.transform.GetChild(index);
        return t != null ? t.gameObject : null;
    }

    public static GameObject GetChild(this Component self, int index)
    {
        var t = self.transform.GetChild(index);
        return t != null ? t.gameObject : null;
    }

    public static GameObject GetParent(this GameObject self)
    {
        var t = self.transform.parent;
        return t != null ? t.gameObject : null;
    }

    public static GameObject GetParent(this Component self)
    {
        var t = self.transform.parent;
        return t != null ? t.gameObject : null;
    }

    public static GameObject GetRoot(this GameObject self)
    {
        var root = self.transform.root;
        return root != null ? root.gameObject : null;
    }

    public static GameObject GetRoot(this Component self)
    {
        var root = self.transform.root;
        return root != null ? root.gameObject : null;
    }

    public static void SetLayer(this GameObject self, string layerName)
    {
        self.layer = LayerMask.NameToLayer(layerName);
    }

    public static void SetLayer(this Component self, string layerName)
    {
        self.gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    public static void SetLayerRecursively(this GameObject self, int layer)
    {
        self.layer = layer;

        foreach (Transform n in self.transform)
        {
            SetLayerRecursively(n.gameObject, layer);
        }
    }

    public static void SetLayerRecursively(this Component self, int layer)
    {
        self.gameObject.layer = layer;

        foreach (Transform n in self.transform)
        {
            SetLayerRecursively(n, layer);
        }
    }

    public static void SetLayerRecursively(this GameObject self, string layerName)
    {
        self.SetLayerRecursively(LayerMask.NameToLayer(layerName));
    }

    public static void SetLayerRecursively(this Component self, string layerName)
    {
        self.SetLayerRecursively(LayerMask.NameToLayer(layerName));
    }

    public static void SetActive(this Component self, bool value)
    {
        self.gameObject.SetActive(value);
    }

    public static float VectorToRad(this Vector2 thisVec)
    {
        return Mathf.Atan2(thisVec.y, thisVec.x);
    }

    public static Vector2 RadToVector(this float thisFloat)
    {
        return new Vector2(Mathf.Cos(thisFloat), Mathf.Sin(thisFloat));
    }

    public static float VectorToDeg(this Vector2 thisVec)
    {
        return Mathf.Atan2(thisVec.y, thisVec.x) * Mathf.Rad2Deg;
    }

    public static Vector2 DegToVector(this float thisFloat)
    {
        return new Vector2(Mathf.Cos(thisFloat * Mathf.Deg2Rad), Mathf.Sin(thisFloat * Mathf.Deg2Rad));
    }

    public static void Shuffle<T>(this List<T> listToShuffle, int numberOfTimesToShuffle = 1)
    {
        var newList = new List<T>();

        for (int i = 0; i < numberOfTimesToShuffle; i++)
        {
            while (listToShuffle.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, listToShuffle.Count);

                newList.Add(listToShuffle[index]);

                listToShuffle.RemoveAt(index);
            }

            listToShuffle.AddRange(newList);

            newList.Clear();
        }
    }

    public static void SetPosition(this Transform self, float x, float y, float z, Space relativeTo)
    {
        if (relativeTo == Space.World)
        {
            Vector3 position = self.position;
            position.Set(x, y, z);
            self.position = position;
        }
        else if (relativeTo == Space.Self)
        {
            Vector3 position = self.localPosition;
            position.Set(x, y, z);
            self.localPosition = position;
        }
    }

    public static void SetParentEx(this Transform tr, Transform parnet)
    {
        SetParentEx(tr, parnet, Vector3.zero);
    }

    public static void SetParentEx(this Transform tr, Transform parnet, Vector3 localPos)
    {
        SetParentEx(tr, parnet, localPos, Vector3.one);
    }

    public static void SetParentEx(this Transform tr, Transform parnet, Vector3 localPos, Vector3 localScale)
    {
        tr.parent = parnet;

        Transform tran = tr.transform;
        tran.localPosition = localPos;
        tran.localScale = localScale;
        tran.localRotation = Quaternion.identity;
    }

    public static IEnumerator AsIEnumerator(this Task task)
    {
        while (!task.IsCompleted)
            yield return null;

        if (task.IsFaulted)
            throw task.Exception;
    }

    static public void SetChildLayer(this Transform t, int layer)
    {
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            child.gameObject.layer = layer;
            SetChildLayer(child, layer);
        }
    }

    static public float EvaluateRatio(this AnimationCurve ac, float ratio)
    {
        return ac.Evaluate(ac.keys[ac.keys.Length - 1].time * ratio);
    }

    static public string Color(this string log, Color color)
    {
        return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color).ToString(), log);
    }

    static public string Color(this string log, string colorRGB)
    {
        return string.Format("<color=#{0}>{1}</color>", colorRGB.ToString(), log);
    }
}