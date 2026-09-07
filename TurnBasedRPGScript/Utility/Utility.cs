using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utility
{
    public static int[] GetRandomList(int start, int end)
    {
        int[] list = Enumerable.Range(start, end).ToArray();
        int idx, old;
        for (int i = 0; i < end; i++)
        {
            idx = UnityEngine.Random.Range(start, end);
            old = list[i];
            list[i] = list[idx];
            list[idx] = old;
        }
        return list;
	}

    /// <summary>
    /// base에서 rate만 뽑은 rate리스트
    /// 두 리스트 카운트가 맞지않으면 null or 0;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="baseList"></param>
    /// <param name="rateList"></param>
    /// <returns></returns>
    public static T GetRateResult<T>(List<T> baseList, List<int> rateList)
    {
        if(baseList == null || rateList == null)
        {
            Debug.LogError("GetRateResult [baseList] or [rateList] is null. Please check this.");
            return default(T);
        }
        if(baseList.Count != rateList.Count)
        {
            Debug.LogError("GetRateResult [baseList] , [rateList] each count is wrong. Please check this.");
            return default(T);
        }

        int totalRate = 0;
        foreach (var val in rateList)
        {
            totalRate += val;
        }

        int randomRate = UnityEngine.Random.Range(0, totalRate);
        int check = 0;

        for (int i = 0; i < baseList.Count; i++)
        {
            check += rateList[i];

            if(randomRate < check)
            {
                return baseList[i];
            }
        }
        return default(T);
    }

    public static void ChangeLayersRecursively(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child, name);
        }
    }
    
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for(int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if(string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach(T componenet in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || componenet.name == name)
                    return componenet;
            }
        }

        return null;
    }

      

    static public void KillTweenByID(string stringID, bool complete = false)
    {
        if (true == string.IsNullOrEmpty(stringID))
        {
            return;
        }

        var listTween = DOTween.TweensById(stringID, true);
        if (null == listTween)
        {
            return;
        }

        foreach (var tween in listTween)
        {
            if (tween.stringId == stringID)
            {
                tween.Kill(complete);
            }
        }
    }

    static public void SetWorldToCanvas(Vector3 worldPosition, Camera mainCamera, Camera uiCamera, Transform trThis, RectTransform rectCanvas = null)
    {
        if (null == mainCamera)
        {
            return;
        }


        if (null == uiCamera)
        {
            return;
        }

        if (null == trThis)
        {
            return;
        }

        if (rectCanvas == null)
        {
            var goCanvas = GetComponentInRecursiveParent<Canvas>(trThis.gameObject).gameObject;
            goCanvas.TryGetComponent(out rectCanvas);
        }

        var screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);

        Vector2 uiLocalPosition;
        if (true == RectTransformUtility.ScreenPointToLocalPointInRectangle(rectCanvas, screenPoint, uiCamera,
            out uiLocalPosition))
        {
            trThis.localPosition = uiLocalPosition;
        }
    }

    static public Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
    {
        //Vector position (percentage from 0 to 1) considering camera size.
        //For example (0,0) is lower left, middle is (0.5,0.5)
        Vector2 temp = camera.WorldToViewportPoint(position);

        //Calculate position considering our percentage, using our canvas size
        //So if canvas size is (1100,500), and percentage is (0.5,0.5), _current value will be (550,250)
        temp.x *= canvas.sizeDelta.x;
        temp.y *= canvas.sizeDelta.y;

        //The result is ready, but, this result is correct if canvas recttransform pivot is 0,0 - left lower corner.
        //But in reality its middle (0.5,0.5) by default, so we remove the amount considering cavnas rectransform pivot.
        //We could multiply with constant 0.5, but we will actually read the value, so if custom rect transform is passed(with custom pivot) , 
        //returned value will still be correct.

        temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
        temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

        return temp;
    }

    static public T GetComponentInRecursiveParent<T>(GameObject goParent) where T : Component
    {
        while (null != goParent)
        {
            if (true == goParent.TryGetComponent(out T comp))
            {
                return comp;
            }
            else
            {
                goParent = goParent.transform?.parent?.gameObject;
            }
        }

        return null;
    }

    public static Type GetType(string TypeName)
    {
        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        var type = Type.GetType(TypeName);        
        return type;
    }    
}